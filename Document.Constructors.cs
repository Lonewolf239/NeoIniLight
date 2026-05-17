using System.Collections.Generic;
using NeoIniLight.Models;
using NeoIniLight.Providers;

namespace NeoIniLight
{
    public partial class NeoIniDocument
    {
        /// <summary>
        /// Creates a new <see cref="NeoIniDocument"/> for the specified file path,
        /// with optional configuration options.
        /// </summary>
        /// <param name="path">Path to the INI file.</param>
        /// <param name="options">
        /// Optional document configuration; if <c>null</c>, <see cref="NeoIniOptions.Default"/> is used.
        /// </param>
        /// <param name="autoLoad">
        /// If <c>true</c>, the configuration data is loaded synchronously from the file during construction.
        /// If <c>false</c>, you must call <see cref="Reload"/> or <see cref="ReloadAsync"/> explicitly.
        /// </param>
        public NeoIniDocument(string? path, NeoIniOptions? options, bool autoLoad = true)
        {
            Data = new Dictionary<string, Dictionary<string, string>>();
            FilePath = path;
            Provider = new NeoIniFileProvider(path);
            ApplyOptions(options);
            if (autoLoad) Load();
        }
    }
}
