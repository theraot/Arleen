using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public abstract partial class ViewingVolume
    {
        private double _farPlane;
        private bool _invalidProjectionMatrix;
        private double _nearPlane;
        private OpenTK.Matrix4d _perspectiveMatrix = OpenTK.Matrix4d.Identity;

        public double FarPlane
        {
            get
            {
                return _farPlane;
            }
            set
            {
                _farPlane = value;
                if (Math.Abs(_farPlane - value) > 0.0f)
                {
                    _farPlane = value;
                    _invalidProjectionMatrix = true;
                }
            }
        }

        public double NearPlane
        {
            get
            {
                return _nearPlane;
            }
            set
            {
                _nearPlane = value;
                if (Math.Abs(_nearPlane - value) > 0.0f)
                {
                    _nearPlane = value;
                    _invalidProjectionMatrix = true;
                }
            }
        }

        public OpenTK.Matrix4d PerspectiveMatrix
        {
            get
            {
                return _perspectiveMatrix;
            }
        }

        protected bool InvalidProjectionMatrix
        {
            get
            {
                return _invalidProjectionMatrix;
            }
            set
            {
                _invalidProjectionMatrix = value;
            }
        }

        public static void PlaceOthogonalProjection(double width, double height, double nearPlane, double farPlane)
        {
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4d proj = Matrix4d.CreateOrthographicOffCenter(0, width, height, 0, nearPlane, farPlane);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public static void PlacePerspectiveProjection(double fieldOfViewRadians, double aspectRatio, double nearPlane, double farPlane)
        {
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4d proj = Matrix4d.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspectRatio, nearPlane, farPlane);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Place()
        {
            if (InvalidProjectionMatrix)
            {
                UpdateProjectionMatrices();
            }
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _perspectiveMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public virtual void Update(int width, int height)
        {
            //Empty
        }

        protected abstract Matrix4d OnUpdateProjectionMatrices();

        private void UpdateProjectionMatrices()
        {
            _perspectiveMatrix = OnUpdateProjectionMatrices();
            InvalidProjectionMatrix = false;
        }
    }
}