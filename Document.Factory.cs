using System.Threading;
using System.Threading.Tasks;
using NeoIniLight.Models;

namespace NeoIniLight
{
    public partial class NeoIniDocument
    {
        /// <summary>
        /// Asynchronously creates a new <see cref="NeoIniDocument"/> for the specified file path,
        /// with optional configuration options.
        /// </summary>
        /// <param name="path">Path to the INI file.</param>
        /// <param name="options">
        /// Optional document configuration; if <c>null</c>, <see cref="NeoIniOptions.Default"/> is used.
        /// </param>
        /// <param name="autoLoad">
        /// If <c>true</c>, the configuration data is loaded asynchronously from the file during creation.
        /// If <c>false</c>, you must call <see cref="ReloadAsync"/> explicitly on the returned document.
        /// </param>
        /// <param name="cancellationToken">Token used to cancel the asynchronous initialization.</param>
        /// <returns>
        /// A task that represents the asynchronous creation operation,
        /// containing the initialized <see cref="NeoIniDocument"/>.
        /// </returns>
        public static async Task<NeoIniDocument> CreateAsync(string? path, NeoIniOptions? options = null,
            bool autoLoad = true, CancellationToken cancellationToken = default)
        {
            NeoIniDocument document = new NeoIniDocument(path, options, false);
            if (autoLoad) await document.LoadAsync(cancellationToken).ConfigureAwait(false);
            return document;
        }
    }
}
