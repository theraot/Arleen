using OpenTK;
using System;

namespace Arleen.Geometry
{
    /// <summary>
    /// Helper methods for the Vector3 class
    /// </summary>
    public static class Vector3Helper
    {
        /// <summary>
        /// Creates a vector from length, bearing and elevation.
        /// </summary>
        /// <param name="length">The length of the vector.</param>
        /// <param name="bearing">The angle from the north over the horizontal plane.</param>
        /// <param name="elevation">The angle from the horizontal plane.</param>
        /// <returns>A new vector created with the given length, bearing and elevation.</returns>
        public static Vector3 CreateGeo(float length, float bearing, float elevation)
        {
            var cosElevationLength = (float)Math.Cos(elevation) * length;
            return new Vector3
                (
                    (float)Math.Sin(bearing) * cosElevationLength,
                    (float)Math.Sin(elevation) * length,
                    (float)Math.Cos(bearing) * cosElevationLength
                );
        }
    }
}