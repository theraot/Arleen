using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public abstract partial class ViewingVolume
    {
        private double _farPlane;
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
                    InvalidProjectionMatrix = true;
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
                    InvalidProjectionMatrix = true;
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

        protected bool InvalidProjectionMatrix { get; set; }

        public void Place()
        {
            UpdateProjectionMatrices();
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
            if (InvalidProjectionMatrix)
            {
                _perspectiveMatrix = OnUpdateProjectionMatrices();
            }
            InvalidProjectionMatrix = false;
        }
    }
}