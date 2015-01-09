using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    /// <summary>
    /// Utility methods for ViewingVolume
    /// </summary>
    public static class ViewingVolumeHelper
    {
        /// <summary>
        /// Sets the viewing volume to the current graphic context.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume to set.</param>
        public static void Place(this ViewingVolume viewingVolume)
        {
            viewingVolume.UpdateProjectionMatrices();
            GL.MatrixMode(MatrixMode.Projection);
            var matrix = viewingVolume.ProjectionMatrix;
            GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        /// <summary>
        /// Sets an orthogonal projection viewing volume to the current graphic context.
        /// </summary>
        /// <param name="width">Width of the far viewing plane.</param>
        /// <param name="height">Height of the far viewing plane.</param>
        /// <param name="nearPlane">Distance to the near viewing plane.</param>
        /// <param name="farPlane">Distance to the far viewing plane.</param>
        public static void PlaceOthogonalProjection(float width, float height, float nearPlane, float farPlane)
        {
            GL.MatrixMode(MatrixMode.Projection);
            var proj = Matrix4.CreateOrthographicOffCenter(0, width, 0, height, nearPlane, farPlane);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        /// <summary>
        /// Sets an perspective projection viewing volume to the current graphic context.
        /// </summary>
        /// <param name="fieldOfViewRadians">Vertical field of view in radians.</param>
        /// <param name="aspectRatio">Relation of width over height of the viewing volume.</param>
        /// <param name="nearPlane">Distance to the near viewing plane.</param>
        /// <param name="farPlane">Distance to the far viewing plane.</param>
        public static void PlacePerspectiveProjection(float fieldOfViewRadians, float aspectRatio, float nearPlane, float farPlane)
        {
            GL.MatrixMode(MatrixMode.Projection);
            var proj = Matrix4.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspectRatio, nearPlane, farPlane);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}