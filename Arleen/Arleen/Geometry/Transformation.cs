﻿using OpenTK;
using System;

namespace Arleen.Geometry
{
    public sealed class Transformation : ICloneable
    {
        private OpenTK.Matrix4d _matrix = OpenTK.Matrix4d.Identity;

        public OpenTK.Matrix4d Matrix
        {
            get
            {
                return _matrix;
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public void Invert()
        {
            _matrix = Matrix4d.Invert(_matrix);
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

        private Transformation Clone()
        {
            return new Transformation { _matrix = _matrix };
        }
    }
}