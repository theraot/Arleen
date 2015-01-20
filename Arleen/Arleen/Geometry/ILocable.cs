namespace Arleen.Geometry
{
    /// <summary>
    /// Represents an object that can located in a 3D space.
    /// </summary>
    public interface ILocable
    {
        /// <summary>
        /// Gets the current location.
        /// </summary>
        Location Location { get; }
    }
}