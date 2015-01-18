using OpenTK;
using System;

namespace Arleen.Geometry
{
    /// <summary>
    /// Auxiliary methods for managing Quaterniond
    /// </summary>
    public static class QuaterniondHelper
    {
        /// <summary>
        /// Creates a Quaterniond from "euler" angles.
        /// </summary>
        /// <param name="bearing">The angle from the north over the horizontal plane.</param>
        /// <param name="elevation">The angle from the horizontal plane.</param>
        /// <param name="roll">The rotation over the viewing axis.</param>
        /// <returns>The constructer Quaterniond.</returns>
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

        /// <summary>
        /// Extrapolates a Quaterniond to another one.
        /// </summary>
        /// <param name="origin">The Quaterniond to extrapolate from.</param>
        /// <param name="rotation">The Quaterniond to extrapolate to.</param>
        /// <param name="factor">The extrapolation factor. 0 = origin, 1 = origin + rotation.</param>
        /// <returns>A new Quaterniond creating by extrapolation a rotation.</returns>
        public static Quaterniond Extrapolate(Quaterniond origin, Quaterniond rotation, double factor)
        {
            Vector3d axis;
            double angle;
            rotation.ToAxisAngle(out axis, out angle);
            var full_loop = 2 * Math.PI / angle;
            var dt = factor % full_loop;
            return origin * Quaterniond.FromAxisAngle(axis, angle * dt);
        }

        /// <summary>
        /// Retrieves the "euler" angles from a Quaterniond.
        /// </summary>
        /// <param name="quaterniond">The Quaterniond.</param>
        /// <param name="bearing">The angle from the north over the horizontal plane.</param>
        /// <param name="elevation">The angle from the horizontal plane.</param>
        /// <param name="roll">The rotation over the viewing axis.</param>
        public static void ToEulerAngles(Quaterniond quaterniond, out double bearing, out double elevation, out double roll)
        {
            var sqx = quaterniond.X * quaterniond.X;
            var sqy = quaterniond.Y * quaterniond.Y;
            var sqz = quaterniond.Z * quaterniond.Z;

            elevation = -Math.Asin((2 * quaterniond.Z * quaterniond.Y) + (2 * quaterniond.X * quaterniond.W));
            bearing = Math.Atan2((2 * quaterniond.Y * quaterniond.W) - (2 * quaterniond.Z * quaterniond.X), 1 - (2 * sqy) - (2 * sqx));
            roll = Math.Atan2((2 * quaterniond.Z * quaterniond.W) - (2 * quaterniond.Y * quaterniond.X), 1 - (2 * sqz) - (2 * sqx));
        }
    }
}