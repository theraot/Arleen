namespace Arleen
{
    // Instances of this class are created via Config.Load<Configuration>
    internal class Configuration
    {
        /// <summary>
        /// Sets or get whatever this program should work in debug mode.
        /// </summary>
        /// <remarks>Debug mode is not a replacement for a debug build.
        /// Running with debug mode should allow for detailed logs and other info in release builds.</remarks>
        public bool ForceDebugMode { get; set; }
    }
}