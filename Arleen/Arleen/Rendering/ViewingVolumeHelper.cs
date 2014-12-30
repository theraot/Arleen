using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public static class ViewingVolumeHelper
    {
        public static void Place(this ViewingVolume viewingVolume)
        {
            GL.MatrixMode(MatrixMode.Projection);
            var viewingVolumePerspectiveMatrix = viewingVolume.PerspectiveMatrix;
            GL.LoadMatrix(ref viewingVolumePerspectiveMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public static void PlaceOthogonalProjection(float width, float height, float nearPlane, float farPlane)
        {
            GL.MatrixMode(MatrixMode.Projection);
            var proj = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, nearPlane, farPlane);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public static void PlacePerspectiveProjection(float fieldOfViewRadians, float aspectRatio, float nearPlane, float farPlane)
        {
            GL.MatrixMode(MatrixMode.Projection);
            var proj = Matrix4.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspectRatio, nearPlane, farPlane);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}