using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    /// <summary>
    /// Helper methods for the Location class
    /// </summary>
    public static class LocationHelper
    {
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

        public static Vector3d GetUnitX(this Location location)
        {
            return Vector3d.Transform(Vector3d.UnitX, location.MatrixOrientation);
        }

        public static Vector3d GetUnitY(this Location location)
        {
            return Vector3d.Transform(Vector3d.UnitY, location.MatrixOrientation);
        }

        public static Vector3d GetUnitZ(this Location location)
        {
            return Vector3d.Transform(Vector3d.UnitZ, location.MatrixOrientation);
        }

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
    }
}