using OpenTK;
using System;

namespace Arleen.Geometry
{
    public static class Vector3Helper
    {
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