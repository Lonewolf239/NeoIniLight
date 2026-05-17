using System;

namespace NeoIniLight.Models
{
    /// <summary>
    /// Represents the error that occurs when a string contains characters that are invalid or reserved in INI formatting.
    /// </summary>
    public class UnsupportedIniCharacterException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedIniCharacterException"/> class.
        /// </summary>
        public UnsupportedIniCharacterException(string chars) : base($"The string contains unsupported characters (such as {chars}).") { }
    }

    /// <summary>
    /// Represents the error that occurs when an empty or null string is provided to a configuration setting that requires a value.
    /// </summary>
    public class EmptyValueNotAllowedException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyValueNotAllowedException"/> class with the name of the parameter that caused the exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        public EmptyValueNotAllowedException(string paramName)
            : base("Empty or null strings are not allowed when AllowEmptyValues is disabled.", paramName) { }
    }
}
