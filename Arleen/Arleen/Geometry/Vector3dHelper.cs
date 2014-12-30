using OpenTK;
using System;

namespace Arleen.Geometry
{
    public static class Vector3dHelper
    {
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