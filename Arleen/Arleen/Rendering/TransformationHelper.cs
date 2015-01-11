using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    /// <summary>
    /// Helper methods for the Transformation class
    /// </summary>
    public static class TransformationHelper
    {
        /// <summary>
        /// Applies a transformation, extending any prior transformation, to the current graphic context.
        /// </summary>
        /// <param name="transformation">The transformation to apply.</param>
        public static void Apply(this Transformation transformation)
        {
            var matrix = transformation.Matrix;
            GL.MultMatrix(ref matrix);
        }

        /// <summary>
        /// Places a transformation, replacing any prior transformation, to the current graphic context
        /// </summary>
        /// <param name="transformation">The transformation to place.</param>
        public static void Place(this Transformation transformation)
        {
            var matrix = transformation.Matrix;
            GL.LoadMatrix(ref matrix);
        }
    }
}