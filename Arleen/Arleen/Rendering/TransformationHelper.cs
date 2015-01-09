using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public static class TransformationHelper
    {
        public static void Apply(this Transformation transformation)
        {
            var matrix = transformation.Matrix;
            GL.MultMatrix(ref matrix);
        }

        public static void Place(this Transformation transformation)
        {
            var matrix = transformation.Matrix;
            GL.LoadMatrix(ref matrix);
        }
    }
}