using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AsyncLocks;
using NeoIniLight.Core;
using NeoIniLight.Models;
using NeoIniLight.Providers;

namespace NeoIniLight
{
    public partial class NeoIniDocument
    {
        private readonly NeoIniFileProvider Provider;
        private readonly string? FilePath;

        private Dictionary<string, Dictionary<string, string>> Data;
        private readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();

        private bool Disposed = false;
        private int DisposeState = 0;

        private int _AutoSaveInterval;
        private int IsSaving = 0;
        private int SaveIterationCounter = 0;

        private string? ExtractContent()
        {
            using (Lock.WriteLock())
            {
                string? content = SaveOnDispose ? NeoIniParser.GetContent(Data) : null;
                Data?.Clear();
                return content;
            }
        }

        /// <summary>Releases managed resources and saves changes to the file</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref DisposeState, 1, 0) != 0) return;
            if (disposing)
            {
                try
                {
                    if (ExtractContent() is string content)
                    {
                        Provider.Save(content);
                        Saved?.Invoke(this, EventArgs.Empty);
                    }
                }
                finally
                {
                    DataCleared?.Invoke(this, EventArgs.Empty);
                    Lock.Dispose();
                }
            }
            Disposed = true;
        }

#if !NETSTANDARD2_0
        /// <summary>Asynchronously releases managed resources and saves changes to the file</summary>
        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (Interlocked.CompareExchange(ref DisposeState, 1, 0) != 0) return;
            if (disposing)
            {
                try
                {
                    if (ExtractContent() is string content)
                    {
                        await Provider.SaveAsync(content, CancellationToken.None).ConfigureAwait(false);
                        Saved?.Invoke(this, EventArgs.Empty);
                    }
                }
                finally
                {
                    DataCleared?.Invoke(this, EventArgs.Empty);
                    Lock.Dispose();
                }
            }
            Disposed = true;
        }
#endif

        internal void Load() => Data = Provider.GetData();

        internal async Task LoadAsync(CancellationToken cancellationToken = default) =>
            Data = await Provider.GetDataAsync(ct: cancellationToken).ConfigureAwait(false);

        private void ApplyOptions(NeoIniOptions? options)
        {
            options ??= new NeoIniOptions();
            UseAutoSave = options.UseAutoSave;
            AutoSaveInterval = options.AutoSaveInterval;
            UseAutoBackup = options.UseAutoBackup;
            UseAutoAdd = options.UseAutoAdd;
            SaveOnDispose = options.SaveOnDispose;
            AllowEmptyValues = options.AllowEmptyValues;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(Disposed, nameof(NeoIniDocument));
#else
            if (Disposed) throw new ObjectDisposedException(nameof(NeoIniDocument));
#endif
        }

        private void ThrowIfEmpty(string? value, bool useAEVRule = true)
        {
            if (useAEVRule && AllowEmptyValues) return;
            if (string.IsNullOrEmpty(value)) throw new EmptyValueNotAllowedException(nameof(value));
        }

        private void ThrowIfContainsUnsupportedChars(string? value, bool isValue)
        {
            if (value is null) return;
#if NETSTANDARD2_0
            if (value.Contains(";") || value.Contains("=") || value.Contains("\""))
                throw new UnsupportedIniCharacterException("; = \"");
#else
            if (value.AsSpan().IndexOfAny(";=\"".AsSpan()) >= 0) throw new UnsupportedIniCharacterException("; = \"");
#endif
        }

        private bool ShouldAutoSave()
        {
            if (Interlocked.CompareExchange(ref IsSaving, 1, 0) != 0) return false;
            if (!UseAutoSave) { Interlocked.Exchange(ref IsSaving, 0); return false; }
            if (AutoSaveInterval == 0) return true;
            if (Interlocked.Increment(ref SaveIterationCounter) % AutoSaveInterval == 0) return true;
            Interlocked.Exchange(ref IsSaving, 0);
            return false;
        }

        private void DoAutoSave()
        {
            if (!ShouldAutoSave()) return;
            try
            {
                AutoSave?.Invoke(this, EventArgs.Empty);
                SaveFile();
            }
            finally { Interlocked.Exchange(ref IsSaving, 0); }
        }

        private async Task DoAutoSaveAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (!ShouldAutoSave()) return;
            try
            {
                AutoSave?.Invoke(this, EventArgs.Empty);
                await SaveFileAsync(ct).ConfigureAwait(false);
            }
            finally { Interlocked.Exchange(ref IsSaving, 0); }
        }

        private async Task ExecuteWithReadLockAsync(Action action, CancellationToken ct)
        {
            using (await Lock.ReadLockAsync(ct).ConfigureAwait(false))
            {
                ct.ThrowIfCancellationRequested();
                action();
            }
        }

        private async Task ExecuteWithWriteLockAsync(Action action, CancellationToken ct)
        {
            using (await Lock.WriteLockAsync(ct).ConfigureAwait(false))
            {
                ct.ThrowIfCancellationRequested();
                action();
            }
        }

        private void ValidateValue(string? value, bool isValue = false)
        {
            ThrowIfEmpty(value, isValue);
            ThrowIfContainsUnsupportedChars(value, isValue);
        }

        private void ValidateTwoValue(string? value1, string? value2)
        {
            ValidateValue(value1, false);
            ValidateValue(value2, false);
        }
    }
}
