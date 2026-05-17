namespace NeoIniLight.Providers
{
    internal partial class NeoIniFileProvider
    {
        private const byte FileVersion = 1;
        private const int HeaderSize = 10;

        private static readonly byte[] FileSignature = { (byte)'N', (byte)'I', (byte)'N', (byte)'I' };
        private static readonly string[] LineSeparators = new[] { "\r\n", "\n", "\r" };

        private readonly string FilePath;

        private string TempFilePath => FilePath + ".tmp";
        private string BackupFilePath => FilePath + ".backup";
        internal bool UseBackup = true;
    }
}
