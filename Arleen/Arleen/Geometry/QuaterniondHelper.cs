using OpenTK;
using System;

namespace Arleen.Geometry
{
    public static class QuaterniondHelper
    {
        public static Quaterniond CreateFromEulerAngles(double bearing, double elevation, double tilt)
        {
            var c1 = Math.Cos(bearing / 2.0);
            var s1 = Math.Sin(bearing / 2.0);
            var c2 = Math.Cos(tilt / 2.0);
            var s2 = Math.Sin(tilt / 2.0);
            var c3 = Math.Cos(-elevation / 2.0);
            var s3 = Math.Sin(-elevation / 2.0);
            var c1c2 = c1 * c2;
            var s1s2 = s1 * s2;
            return new Quaterniond
                (
                    (c1c2 * s3) + (s1s2 * c3),
                    (s1 * c2 * c3) + (c1 * s2 * s3),
                    (c1 * s2 * c3) - (s1 * c2 * s3),
                    (c1c2 * c3) - (s1s2 * s3)
                );
        }
    }
}