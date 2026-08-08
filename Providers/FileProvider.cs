using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoIniLight.Core;
using NeoIniLight.Models;
using Data = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace NeoIniLight.Providers
{
    internal partial class NeoIniFileProvider
    {
        public event EventHandler<ProviderErrorEventArgs>? Error;

        internal void DeleteBackup() { if (File.Exists(BackupFilePath)) File.Delete(BackupFilePath); }

        internal void DeleteFile()
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
            if (File.Exists(TempFilePath)) File.Delete(TempFilePath);
        }

        public Data GetData()
        {
            var data = new Data();
            string? directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            if (!File.Exists(FilePath))
            {
                using var stream = File.Create(FilePath);
                return data;
            }
            string? currentSection = null;
            var lines = ReadFile();
            if (lines is null) return data;
            for (int i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim(' ', '\t', '\u00A0', '\u200B');
                if (string.IsNullOrEmpty(trimmed)) continue;
                if (NeoIniParser.IsSectionLine(trimmed))
                {
                    currentSection = NeoIniParser.HandleSectionLine(trimmed, data);
                    continue;
                }
#if NETSTANDARD2_0
                if (!(currentSection is null) && NeoIniParser.TryMatchKey(trimmed, out string? key, out string? value))
#else
                if (currentSection is not null && NeoIniParser.TryMatchKey(trimmed.AsSpan(), out string? key, out string? value))
#endif
                    NeoIniParser.HandleKeyValueLine(trimmed, currentSection, key, value, data);
            }
            return data;
        }

        public async Task<Data> GetDataAsync(CancellationToken ct = default)
        {
            var data = new Data();
            string? directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            if (!File.Exists(FilePath))
            {
                using var stream = File.Create(FilePath);
                return data;
            }
            var lines = await ReadFileAsync(ct).ConfigureAwait(false);
            if (lines is null) return data;
            string? currentSection = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim(' ', '\t', '\u00A0', '\u200B');
                if (string.IsNullOrEmpty(trimmed)) continue;
                if (NeoIniParser.IsSectionLine(trimmed))
                {
                    currentSection = NeoIniParser.HandleSectionLine(trimmed, data);
                    continue;
                }
#if NETSTANDARD2_0
                if (!(currentSection is null) && NeoIniParser.TryMatchKey(trimmed, out string? key, out string? value))
#else
                if (currentSection is not null && NeoIniParser.TryMatchKey(trimmed.AsSpan(), out string? key, out string? value))
#endif
                    NeoIniParser.HandleKeyValueLine(trimmed, currentSection, key, value, data);
            }
            return data;
        }

        public void Save(string content)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(content ?? string.Empty);
            try
            {
                byte[] header = BuildHeader();
                using var ms = new MemoryStream(plaintextBytes.Length);
                ms.Write(header, 0, header.Length);
                ms.Write(plaintextBytes, 0, plaintextBytes.Length);
                NeoIniIO.WriteBytes(TempFilePath, ms.ToArray());
                if (File.Exists(FilePath)) File.Replace(TempFilePath, FilePath, UseBackup ? BackupFilePath : null);
                else File.Move(TempFilePath, FilePath);
            }
            catch (UnauthorizedAccessException ex) { RaiseError(this, new ProviderErrorEventArgs(ex)); }
            catch (IOException ex) { RaiseError(this, new ProviderErrorEventArgs(ex)); }
        }

        public async Task SaveAsync(string content, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(content ?? string.Empty);
            try
            {
                byte[] header = BuildHeader();
                ct.ThrowIfCancellationRequested();

                using var ms = new MemoryStream(plaintextBytes.Length);
                ms.Write(header, 0, header.Length);
#if NETSTANDARD2_0
                    ms.Write(plaintextBytes, 0, plaintextBytes.Length);
#else
                await ms.WriteAsync(plaintextBytes, ct).ConfigureAwait(false);
#endif
                ct.ThrowIfCancellationRequested();
                await NeoIniIO.WriteBytesAsync(TempFilePath, ms.ToArray(), ct).ConfigureAwait(false);
                ct.ThrowIfCancellationRequested();
                if (File.Exists(FilePath)) File.Replace(TempFilePath, FilePath, UseBackup ? BackupFilePath : null);
                else File.Move(TempFilePath, FilePath);
            }
            catch (UnauthorizedAccessException ex) { RaiseError(this, new ProviderErrorEventArgs(ex)); }
            catch (IOException ex) { RaiseError(this, new ProviderErrorEventArgs(ex)); }
        }

        public void RaiseError(object? sender, ProviderErrorEventArgs e)
        {
            if (Error is null) throw e.Exception;
            else Error.Invoke(sender, e);
        }
    }
}
