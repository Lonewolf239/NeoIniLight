using System;

namespace NeoIniLight
{
    public partial class NeoIniDocument
    {
        /// <summary>
        /// Determines whether changes are automatically written to the disk after every modification.
        /// Default is <c>true</c>.
        /// </summary>
        public bool UseAutoSave { get; set; }

        /// <summary>
        /// Interval (in operations) between automatic saves when <see cref="UseAutoSave"/> is enabled.
        /// Default value is 0.
        /// </summary>
        public int AutoSaveInterval
        {
            get => _AutoSaveInterval;
            set
            {
                if (value < 0) throw new ArgumentException("Interval cannot be negative.");
                _AutoSaveInterval = value;
            }
        }

        /// <summary>
        /// Determines whether backup files (.backup) are created during save operations.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool UseAutoBackup
        {
            get => Provider.UseBackup;
            set => Provider.UseBackup = value;
        }

        /// <summary>
        /// Determines whether missing keys are automatically added to the file with a default value when requested via <see cref="GetValue{T}"/>. 
        /// Default is <c>true</c>.
        /// </summary>
        public bool UseAutoAdd { get; set; }

        /// <summary>
        /// Determines whether the configuration is automatically saved when the instance is disposed.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool SaveOnDispose { get; set; }

        /// <summary>
        /// Determines whether empty strings or null values are permitted for configuration keys.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool AllowEmptyValues { get; set; }
    }
}
