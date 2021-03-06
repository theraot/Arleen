﻿using OpenTK;
using System;

namespace Arleen.Geometry
{
    public class Location : MarshalByRefObject, ILocable
    {
        private int _computedVersion;
        private int _currentVersion;
        private Matrix4d _matrix = Matrix4d.Identity;
        private Matrix4d _matrixOrientation = Matrix4d.Identity;
        private Matrix4d _matrixPosition = Matrix4d.Identity;
        private Quaterniond _orientation;
        private Vector3d _position;

        public Location()
        {
            _orientation = Quaterniond.Identity;
        }

        [Flags]
        public enum Mode
        {
            None = 0,
            PositionOnly = 1,
            OrientationOnly = 2,
            All = 3
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        Location ILocable.Location
        {
            get
            {
                return this;
            }
        }

        public Matrix4d Matrix
        {
            get
            {
                UpdateModelMatrices();
                return _matrix;
            }
        }

        public Matrix4d MatrixOrientation
        {
            get
            {
                UpdateModelMatrices();
                return _matrixOrientation;
            }
        }

        public Matrix4d MatrixPosition
        {
            get
            {
                UpdateModelMatrices();
                return _matrixPosition;
            }
        }

        public virtual Quaterniond Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    _orientation.Normalize();
                    _currentVersion++;
                }
            }
        }

        public virtual Vector3d Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _currentVersion++;
                }
            }
        }

        public Vector3d Apply(Vector3d target, Mode mode)
        {
            UpdateModelMatrices();
            return ApplyExtracted(target, mode);
        }

        public Matrix4d Apply(Matrix4d target, Mode mode)
        {
            UpdateModelMatrices();
            return ApplyExtracted(target, mode);
        }

        public Vector3d Apply(Vector3d target, Mode mode, bool reverse)
        {
            UpdateModelMatrices();
            if (reverse)
            {
                switch (mode)
                {
                    case Mode.All:
                        return Vector3d.Transform(target, Matrix4d.Transpose(_matrix));

                    case Mode.PositionOnly:
                        return Vector3d.Transform(target, Matrix4d.Transpose(_matrixPosition));

                    case Mode.OrientationOnly:
                        return Vector3d.Transform(target, Matrix4d.Transpose(_matrixOrientation));

                    default:
                        return target;
                }
            }
            return ApplyExtracted(target, mode);
        }

        public Matrix4d Apply(Matrix4d target, Mode mode, bool reverse)
        {
            UpdateModelMatrices();
            if (reverse)
            {
                switch (mode)
                {
                    case Mode.All:
                        return target * Matrix4d.Transpose(_matrix);

                    case Mode.PositionOnly:
                        return target * Matrix4d.Transpose(_matrixPosition);

                    case Mode.OrientationOnly:
                        return target * Matrix4d.Transpose(_matrixOrientation);

                    default:
                        return target;
                }
            }
            return ApplyExtracted(target, mode);
        }

        public Matrix4d GetMatrix(Mode mode)
        {
            UpdateModelMatrices();
            return GetMatrixExtracted(mode);
        }

        public override string ToString()
        {
            UpdateModelMatrices();
            return string.Format("Location: {0} - {1}", Position, Orientation);
        }

        internal void Invalidate()
        {
            _currentVersion++;
        }

        internal virtual int UpdateModelMatrices()
        {
            var current = _currentVersion;
            if (current != _computedVersion)
            {
                _matrixPosition = Matrix4d.CreateTranslation(Position);
                _matrixOrientation = Matrix4d.Rotate(Orientation);
                _matrix = _matrixOrientation * _matrixPosition;
                _computedVersion = current;
            }
            return current;
        }

        private Vector3d ApplyExtracted(Vector3d target, Mode mode)
        {
            switch (mode)
            {
                case Mode.All:
                    return Vector3d.Transform(target, _matrix);

                case Mode.PositionOnly:
                    return Vector3d.Transform(target, _matrixPosition);

                case Mode.OrientationOnly:
                    return Vector3d.Transform(target, _matrixOrientation);

                default:
                    return target;
            }
        }

        private Matrix4d ApplyExtracted(Matrix4d target, Mode mode)
        {
            switch (mode)
            {
                case Mode.All:
                    return target * _matrix;

                case Mode.PositionOnly:
                    return target * _matrixPosition;

                case Mode.OrientationOnly:
                    return target * _matrixOrientation;

                default:
                    return target;
            }
        }

        private Matrix4d GetMatrixExtracted(Mode mode)
        {
            switch (mode)
            {
                case Mode.All:
                    return _matrix;

                case Mode.PositionOnly:
                    return _matrixPosition;

                case Mode.OrientationOnly:
                    return _matrixOrientation;

                default:
                    return Matrix4d.Identity;
            }
        }
    }
}