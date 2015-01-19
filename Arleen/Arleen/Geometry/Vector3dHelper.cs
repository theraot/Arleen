using OpenTK;
using System;

namespace Arleen.Geometry
{
    /// <summary>
    /// Helper methods for the Vector3d class
    /// </summary>
    public static class Vector3dHelper
    {
        /// <summary>
        /// Creates a vector from length, bearing and elevation.
        /// </summary>
        /// <param name="length">The length of the vector.</param>
        /// <param name="bearing">The angle from the north over the horizontal plane.</param>
        /// <param name="elevation">The angle from the horizontal plane.</param>
        /// <returns>A new vector created with the given length, bearing and elevation.</returns>
        public static Vector3d CreateGeo(double length, double bearing, double elevation)
        {
            var cosElevationLength = Math.Cos(elevation) * length;
            return new Vector3d
                (
                    Math.Sin(bearing) * cosElevationLength,
                    Math.Sin(elevation) * length,
                    Math.Cos(bearing) * cosElevationLength
                );
        }
    }
}