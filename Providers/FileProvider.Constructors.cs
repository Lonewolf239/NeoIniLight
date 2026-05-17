using System;

namespace NeoIniLight.Providers
{
    internal partial class NeoIniFileProvider
    {
        internal NeoIniFileProvider(string? filePath)
        {
            if (filePath is null) throw new ArgumentNullException("File path cannot be null.");
            FilePath = filePath;
        }
    }
}
