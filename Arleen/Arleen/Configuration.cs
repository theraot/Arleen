namespace Arleen
{
    /// <summary>
    /// Represents the loaded configuration
    /// </summary>
    /// <remarks>Changing the values in Configuration does not reflect in the running program.
    /// Note: Instances of this class are created via Config.Load<Configuration></remarks>
    internal class Configuration
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
        /// Sets or gets the maximum frame rate.
        /// </summary>
        public double MaxFrameRate { get; set; }

        /// <summary>
        /// Sets or gets the maximum update rate.
        /// </summary>
        public double MaxUpdateRate { get; set; }

        /// <summary>
        /// Sets or gets the default resolution.
        /// </summary>
        public System.Drawing.Size Resolution { get; set; }

        /// <summary>
        /// Sets or gets whatever to use vertical synchronization.
        /// </summary>
        public bool VSync { get; set; }
    }
}