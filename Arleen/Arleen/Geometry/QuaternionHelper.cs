using OpenTK;
using System;

namespace Arleen.Geometry
{
    /// <summary>
    /// Auxiliary methods for managing <see cref="Quaternion"/>
    /// </summary>
    public static class QuaternionHelper
    {
        /// <summary>
        /// Creates a <see cref="Quaternion"/> from "euler" angles.
        /// </summary>
        /// <param name="bearing">The angle from the north over the horizontal plane.</param>
        /// <param name="elevation">The angle from the horizontal plane.</param>
        /// <param name="roll">The rotation over the viewing axis.</param>
        /// <returns>The constructer <see cref="Quaternion"/>.</returns>
        public static Quaternion CreateFromEulerAngles(float bearing, float elevation, float roll)
        {
            var c1 = (float)Math.Cos(bearing / 2.0f);
            var s1 = (float)Math.Sin(bearing / 2.0f);
            var c2 = (float)Math.Cos(roll / 2.0f);
            var s2 = (float)Math.Sin(roll / 2.0f);
            var c3 = (float)Math.Cos(-elevation / 2.0f);
            var s3 = (float)Math.Sin(-elevation / 2.0f);
            return new Quaternion
                (
                    (c1 * c2 * s3) + (s1 * s2 * c3),
                    (s1 * c2 * c3) + (c1 * s2 * s3),
                    (c1 * s2 * c3) - (s1 * c2 * s3),
                    (c1 * c2 * c3) - (s1 * s2 * s3)
                );
        }

        /// <summary>
        /// Extrapolates a <see cref="Quaternion"/> to another one.
        /// </summary>
        /// <param name="origin">The <see cref="Quaternion"/> to extrapolate from.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> to extrapolate to.</param>
        /// <param name="factor">The extrapolation factor. 0 = origin, 1 = origin + rotation.</param>
        /// <returns>A new <see cref="Quaternion"/> creating by extrapolation a rotation.</returns>
        public static Quaternion Extrapolate(Quaternion origin, Quaternion rotation, float factor)
        {
            Vector3 axis;
            float angle;
            rotation.ToAxisAngle(out axis, out angle);
            var fullLoop = (float)(2 * Math.PI / angle);
            var dt = factor % fullLoop;
            return origin * Quaternion.FromAxisAngle(axis, angle * dt);
        }

        /// <summary>
        /// Retrieves the "euler" angles from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The <see cref="Quaternion"/>.</param>
        /// <param name="bearing">The angle from the north over the horizontal plane.</param>
        /// <param name="elevation">The angle from the horizontal plane.</param>
        /// <param name="roll">The rotation over the viewing axis.</param>
        public static void ToEulerAngles(Quaternion quaternion, out float bearing, out float elevation, out float roll)
        {
            var sqx = quaternion.X * quaternion.X;
            var sqy = quaternion.Y * quaternion.Y;
            var sqz = quaternion.Z * quaternion.Z;

            elevation = -(float)Math.Asin((2 * quaternion.Z * quaternion.Y) + (2 * quaternion.X * quaternion.W));
            bearing = (float)Math.Atan2((2 * quaternion.Y * quaternion.W) - (2 * quaternion.Z * quaternion.X), 1 - (2 * sqy) - (2 * sqx));
            roll = (float)Math.Atan2((2 * quaternion.Z * quaternion.W) - (2 * quaternion.Y * quaternion.X), 1 - (2 * sqz) - (2 * sqx));
        }
    }
}