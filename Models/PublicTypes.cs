using System;
using System.Globalization;

namespace NeoIniLight.Models
{
#if NETSTANDARD2_0
    /// <summary>Represents a matched entry found during a search operation in the INI file.</summary>
    public class SearchResult
    {
        /// <summary>Gets the name of the section where the match was found.</summary>
        public string Section { get; }

        /// <summary>Gets the key of the matched entry.</summary>
        public string Key { get; }

        /// <summary>Gets the value of the matched entry.</summary>
        public string Value { get; }

        /// <summary>Initializes a new instance of the <see cref="SearchResult"/> class.</summary>
        /// <param name="section">The name of the section where the match was found.</param>
        /// <param name="key">The key of the matched entry.</param>
        /// <param name="value">The value of the matched entry.</param>
        public SearchResult(string section, string key, string value)
        {
            Section = section;
            Key = key;
            Value = value;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is SearchResult other && Section == other.Section && Key == other.Key && Value == other.Value;

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Section != null ? Section.GetHashCode() : 0);
                hash = hash * 23 + (Key != null ? Key.GetHashCode() : 0);
                hash = hash * 23 + (Value != null ? Value.GetHashCode() : 0);
                return hash;
            }
        }
    }
#else
    /// <summary>Represents a matched entry found during a search operation in the INI file.</summary>
    /// <param name="Section">The name of the section where the match was found.</param>
    /// <param name="Key">The key of the matched entry.</param>
    /// <param name="Value">The value of the matched entry.</param>
    public record SearchResult(string Section, string Key, string Value);
#endif

    /// <summary>
    /// Represents a key-value pair used for bulk operations on an INI document.
    /// This class encapsulates a section, key, and a value that will be stored as a string.
    /// </summary>
    /// <remarks>
    /// When constructing a <see cref="NeoIniValue"/>, if the provided value implements <see cref="IFormattable"/>,
    /// its string representation is obtained using the invariant culture to ensure consistent formatting.
    /// </remarks>
    public class NeoIniValue
    {
        /// <summary>Gets the name of the section where the key-value pair belongs.</summary>
        public string Section { get; }

        /// <summary>Gets the key name of the key-value pair.</summary>
        public string Key { get; }

        /// <summary>
        /// Gets the string representation of the value.
        /// This value is derived from the object passed to the constructor, formatted using invariant culture if applicable.
        /// </summary>
        public string Value { get; }

        /// <summary>Initializes a new instance of the <see cref="NeoIniValue"/> class.</summary>
        /// <param name="section">The name of the section for the key-value pair.</param>
        /// <param name="key">The key name.</param>
        /// <param name="value">
        /// The value to be stored. If the value implements <see cref="IFormattable"/>, it is converted to a string using
        /// <see cref="CultureInfo.InvariantCulture"/>; otherwise, <see cref="object.ToString"/> is used.
        /// If <paramref name="value"/> is <c>null</c>, an empty string is stored.
        /// </param>
        public NeoIniValue(string? section, string? key, object? value)
        {
            if (section is null || key is null) throw new ArgumentNullException();
            Section = section;
            Key = key;
            if (value is IFormattable formattable) Value = formattable.ToString(null, CultureInfo.InvariantCulture);
            else Value = value?.ToString() ?? string.Empty;
        }
    }
}
