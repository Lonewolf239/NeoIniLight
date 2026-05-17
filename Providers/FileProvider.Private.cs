using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoIniLight.Core;
using NeoIniLight.Models;

namespace NeoIniLight.Providers
{
    internal partial class NeoIniFileProvider
    {
        private static string[] SplitLines(string content) => content.Split(LineSeparators, StringSplitOptions.None);

        private byte[] BuildHeader()
        {
            byte[] header = new byte[HeaderSize];
            Array.Copy(FileSignature, 0, header, 0, FileSignature.Length);
            header[4] = FileVersion;
            header[5] = 0;
            header[6] = 0;
            header[7] = 0;
            header[8] = 0x0D;
            header[9] = 0x0A;
            return header;
        }

        private string[]? CheckBackup()
        {
            if (!File.Exists(BackupFilePath)) return null;
            return ReadFile(BackupFilePath, true);
        }

        private async Task<string[]?> CheckBackupAsync(CancellationToken ct)
        {
            if (!File.Exists(BackupFilePath)) return null;
            return await ReadFileAsync(BackupFilePath, true, ct).ConfigureAwait(false);
        }

        private string[]? ReadError(Exception ex, bool isBackup)
        {
            if (!isBackup)
            {
                var data = CheckBackup();
                if (!(data is null)) return data;
                RaiseError(this, new ProviderErrorEventArgs(ex));
            }
            return null;
        }

        private string[]? ReadFile() => ReadFile(FilePath, false);

        private string[]? ReadFile(string path, bool isBackup)
        {
            if (!File.Exists(path))
            {
                if (isBackup) return null;
                return CheckBackup();
            }
            try
            {
                byte[] fileBytes = NeoIniIO.ReadAllBytes(path);
                string content;
                int dataLength = fileBytes.Length - HeaderSize;
                if (dataLength <= 0) return null;
                content = Encoding.UTF8.GetString(fileBytes, HeaderSize, dataLength);
                return SplitLines(content);
            }
            catch (UnauthorizedAccessException ex) { return ReadError(ex, isBackup); }
            catch (IOException ex) { return ReadError(ex, isBackup); }
        }

        private async Task<string[]?> ReadFileAsync(CancellationToken ct) => await ReadFileAsync(FilePath, false, ct).ConfigureAwait(false);

        private async Task<string[]?> ReadFileAsync(string path, bool isBackup, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (!File.Exists(path))
            {
                if (isBackup) return null;
                return await CheckBackupAsync(ct).ConfigureAwait(false);
            }
            try
            {
                byte[] fileBytes = await NeoIniIO.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
                ct.ThrowIfCancellationRequested();
                string content;
                int dataLength = fileBytes.Length - HeaderSize;
                if (dataLength <= 0) return null;
                ct.ThrowIfCancellationRequested();
                content = Encoding.UTF8.GetString(fileBytes, HeaderSize, dataLength);
                return SplitLines(content);
            }
            catch (UnauthorizedAccessException ex) { return ReadError(ex, isBackup); }
            catch (IOException ex) { return ReadError(ex, isBackup); }
        }
    }
}
