using OpenTK;
using System;

namespace Arleen.Geometry
{
    public static class QuaternionHelper
    {
        public static Quaternion CreateFromEulerAngles(float bearing, float elevation, float roll)
        {
            var c1 = (float)Math.Cos(bearing / 2.0f);
            var s1 = (float)Math.Sin(bearing / 2.0f);
            var c2 = (float)Math.Cos(roll / 2.0f);
            var s2 = (float)Math.Sin(roll / 2.0f);
            var c3 = (float)Math.Cos(-elevation / 2.0f);
            var s3 = (float)Math.Sin(-elevation / 2.0f);
            var c1c2 = c1 * c2;
            var s1s2 = s1 * s2;
            return new Quaternion
                (
                    (c1c2 * s3) + (s1s2 * c3),
                    (s1 * c2 * c3) + (c1 * s2 * s3),
                    (c1 * s2 * c3) - (s1 * c2 * s3),
                    (c1c2 * c3) - (s1s2 * s3)
                );
        }

        public static void ToEulerAngles(Quaternion quaterniond, out float bearing, out float elevation, out float roll)
        {
            var sqw = quaterniond.W * quaterniond.W;
            var sqy = quaterniond.Y * quaterniond.Y;
            var sqz = quaterniond.Z * quaterniond.Z;

            elevation = (float)Math.PI + (float)Math.Atan2(2.0f * quaterniond.X * quaterniond.W + 2.0f * quaterniond.Y * quaterniond.Z, 1 - 2.0f * (sqz + sqw));
            bearing = -(float)Math.Asin(2.0f * (quaterniond.X * quaterniond.Z - quaterniond.W * quaterniond.Y));
            roll = (float)Math.Atan2(2.0f * quaterniond.X * quaterniond.Y + 2.0f * quaterniond.Z * quaterniond.W, 1 - 2.0f * (sqy + sqz));
        }
    }
}