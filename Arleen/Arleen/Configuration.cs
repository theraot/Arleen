namespace Arleen
{
    /// <summary>
    /// Represents the loaded configuration
    /// </summary>
    /// <remarks>Changing the values in Configuration does not reflect in the running program.
    /// Note: Instances of this class are created via Resources.LoadConfig</remarks>
    public class Configuration
    {
        /// <summary>
        /// Sets or gets the display name for this program.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Sets or gets whatever this program should work in debug mode.
        /// </summary>
        /// <remarks>Debug mode is not a replacement for a debug build.
        /// Running with debug mode should allow for detailed logs and other info in release builds.</remarks>
        public bool ForceDebugMode { get; set; }

        /// <summary>
        /// Sets or gets the default resolution.
        /// </summary>
        public System.Drawing.Size Resolution { get; set; }

        /// <summary>
        /// Sets or gets the language string to be used when loading localized resources.
        /// </summary>
        /// <remarks>The language string is dash separated list of language / script / region requirements.
        /// These requirements are used to decide what resource to load.</remarks>
        public string Language { get; set; }
    }
}