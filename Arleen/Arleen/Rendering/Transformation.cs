using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public class Transformation : ICloneable
    {
        private OpenTK.Matrix4d _matrix = OpenTK.Matrix4d.Identity;

        public OpenTK.Matrix4d Matrix
        {
            get
            {
                return _matrix;
            }
        }

        public void Invert()
        {
            _matrix = Matrix4d.Invert(_matrix);
        }

        public void Place()
        {
            GL.LoadMatrix(ref _matrix);
        }

        public void Rotate(Quaterniond quaternion)
        {
            _matrix *= Matrix4d.Rotate(quaternion);
        }

        public void Scale(double scale)
        {
            _matrix *= Matrix4d.Scale(scale);
        }

        public void Scale(Vector3d scale)
        {
            _matrix *= Matrix4d.Scale(scale);
        }

        public void Scale(double x, double y, double z)
        {
            _matrix *= Matrix4d.Scale(x, y, z);
        }

        public void Translate(Vector3d vector)
        {
            _matrix *= Matrix4d.CreateTranslation(vector);
        }

        public void Translate(double x, double y, double z)
        {
            _matrix = Matrix4d.CreateTranslation(x, y, z);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        private Transformation Clone()
        {
            return new Transformation { _matrix = _matrix };
        }
    }
}