using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    /// <summary>
    /// Helper methods for the Location class
    /// </summary>
    public static class LocationHelper
    {
        /// <summary>
        /// Applies a location, extending any prior location, to the current graphic context.
        /// </summary>
        /// <param name="location">The location to apply.</param>
        /// <param name="mode">The components of the location to use.</param>
        public static void Place(this Location location, Location.Mode mode)
        {
            var matrix = location.GetMatrix(mode);
            GL.LoadMatrix(ref matrix);
        }

        /// <summary>
        /// Places a location, replacing any prior location, to the current graphic context
        /// </summary>
        /// <param name="location">The location to place.</param>
        /// <param name="mode">The component of the location to use.</param>
        public static void Apply(this Location location, Location.Mode mode)
        {
            var matrix = location.GetMatrix(mode);
            GL.MultMatrix(ref matrix);
        }
    }
}