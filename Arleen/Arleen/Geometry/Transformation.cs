using OpenTK;
using System;

namespace Arleen.Geometry
{
    /// <summary>
    /// Represents a transformation of the model geometry
    /// </summary>
    [Serializable]
    public struct Transformation
    {
        /// <summary>
        /// Represents the identity transformation.
        /// </summary>
        public static Transformation Identity = new Transformation(OpenTK.Matrix4d.Identity);

        private readonly Matrix4d _matrix;

        /// <summary>
        /// Creates a new instance of Transformation.
        /// </summary>
        /// <param name="matrix">The transformation matrix.</param>
        public Transformation(Matrix4d matrix)
        {
            _matrix = matrix;
        }

        /// <summary>
        /// Gets the transformation matrix.
        /// </summary>
        public OpenTK.Matrix4d Matrix
        {
            get
            {
                return _matrix;
            }
        }

        /// <summary>
        /// Creates a transformation inverted from the current one.
        /// </summary>
        /// <returns>The inverted transformation.</returns>
        public Transformation Invert()
        {
            return new Transformation(Matrix4d.Invert(_matrix));
        }

        /// <summary>
        /// Creates a transformation rotated from the current one.
        /// </summary>
        /// <param name="quaternion">The rotation of the current transformation.</param>
        /// <returns>The rotated transformation.</returns>
        public Transformation Rotate(Quaterniond quaternion)
        {
            return new Transformation(_matrix * Matrix4d.Rotate(quaternion));
        }

        /// <summary>
        /// Creates a transformation scaled from the current one.
        /// </summary>
        /// <param name="scale">The scale of the new transformation.</param>
        /// <returns>The scaled transformation.</returns>
        public Transformation Scale(double scale)
        {
            return new Transformation(_matrix * Matrix4d.Scale(scale));
        }

        /// <summary>
        /// Creates a transformation scaled from the current one.
        /// </summary>
        /// <param name="scale">The scale of the new transformation.</param>
        /// <returns>The scaled transformation.</returns>
        public Transformation Scale(Vector3d scale)
        {
            return new Transformation(_matrix * Matrix4d.Scale(scale));
        }

        public Transformation Scale(double x, double y, double z)
        {
            return new Transformation(_matrix * Matrix4d.Scale(x, y, z));
        }

        /// <summary>
        /// Creates a transformation translated from the current one.
        /// </summary>
        /// <param name="vector">The translation of the new transformation.</param>
        /// <returns>The translated transformation.</returns>
        public Transformation Translate(Vector3d vector)
        {
            return new Transformation(_matrix * Matrix4d.CreateTranslation(vector));
        }

        /// <summary>
        /// Creates a transformation translated from the current one.
        /// </summary>
        /// <param name="x">The x coordinate of the translation of the new transformation.</param>
        /// <param name="y">The y coordinate of the translation of the new transformation.</param>
        /// <param name="z">The z coordinate of the translation of the new transformation.</param>
        /// <returns>The translated transformation.</returns>
        public Transformation Translate(double x, double y, double z)
        {
            return new Transformation(_matrix * Matrix4d.CreateTranslation(x, y, z));
        }
    }
}