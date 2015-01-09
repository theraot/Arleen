using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public static class TransformationHelper
    {
        public static void Place(this Transformation transformation)
        {
            var matrix = transformation.Matrix;
            GL.LoadMatrix(ref matrix);
        }
    }
}