using OpenTK;
using System;

namespace Arleen.Geometry
{
    public static class QuaterniondHelper
    {
        public static Quaterniond CreateFromEulerAngles(double bearing, double elevation, double roll)
        {
            var c1 = Math.Cos(bearing / 2.0);
            var s1 = Math.Sin(bearing / 2.0);
            var c2 = Math.Cos(roll / 2.0);
            var s2 = Math.Sin(roll / 2.0);
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

        public static void ToEulerAngles(Quaterniond quaterniond, out double bearing, out double elevation, out double roll)
        {
            var sqw = quaterniond.W * quaterniond.W;
            var sqy = quaterniond.Y * quaterniond.Y;
            var sqz = quaterniond.Z * quaterniond.Z;

            elevation = Math.PI + Math.Atan2(2.0 * quaterniond.X * quaterniond.W + 2.0 * quaterniond.Y * quaterniond.Z, 1 - 2.0 * (sqz + sqw));
            bearing = -Math.Asin(2.0 * (quaterniond.X * quaterniond.Z - quaterniond.W * quaterniond.Y));
            roll = Math.Atan2(2.0 * quaterniond.X * quaterniond.Y + 2.0 * quaterniond.Z * quaterniond.W, 1 - 2.0 * (sqy + sqz));
        }
    }
}