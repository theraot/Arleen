using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public class Location
    {
        private double _elevation;
        private bool _invalidated;
        private OpenTK.Matrix4d _matrix = OpenTK.Matrix4d.Identity;
        private OpenTK.Matrix4d _matrixOrientation = OpenTK.Matrix4d.Identity;
        private OpenTK.Matrix4d _matrixPosition = OpenTK.Matrix4d.Identity;
        private double _pan; // azimuth
        private Vector3d _position;
        private double _tilt;

        public enum PlaceMode
        {
            Full = 0,
            PositionOnly = 1,
            OrientationOnly = 2
        }

        public double Elevation
        {
            get
            {
                return _elevation;
            }
            set
            {
                if (Math.Abs(_elevation - value) > 0.0f)
                {
                    _elevation = value;
                    _invalidated = true;
                }
            }
        }

        public OpenTK.Matrix4d Matrix
        {
            get
            {
                return _matrix;
            }
        }

        public OpenTK.Matrix4d MatrixOrientation
        {
            get
            {
                return _matrixOrientation;
            }
        }

        public OpenTK.Matrix4d MatrixPosition
        {
            get
            {
                return _matrixPosition;
            }
        }

        public double Pan
        {
            get
            {
                return _pan;
            }
            set
            {
                if (Math.Abs(_pan - value) > 0.0f)
                {
                    _pan = value;
                    _invalidated = true;
                }
            }
        }

        public Vector3d Position
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
                    _invalidated = true;
                }
            }
        }

        public double Tilt
        {
            get
            {
                return _tilt;
            }
            set
            {
                if (Math.Abs(_tilt - value) > 0.0f)
                {
                    _tilt = value;
                    _invalidated = true;
                }
            }
        }

        public static void PlaceDefaultLocation()
        {
            GL.LoadIdentity();
        }

        public void Place(PlaceMode mode)
        {
            if (_invalidated)
            {
                UpdateModelMatrices();
            }
            switch (mode)
            {
                case PlaceMode.Full:
                    {
                        GL.LoadMatrix(ref _matrix);
                    }
                    break;

                case PlaceMode.PositionOnly:
                    {
                        GL.LoadMatrix(ref _matrixPosition);
                    }
                    break;

                case PlaceMode.OrientationOnly:
                    {
                        GL.LoadMatrix(ref _matrixOrientation);
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("Location: {0} - {1}, {2}, {3}", _position, _pan, _elevation, _tilt);
        }

        private void UpdateModelMatrices()
        {
            //Use quaternions?
            _matrixPosition = OpenTK.Matrix4d.CreateTranslation(_position);
            _matrixOrientation =
                OpenTK.Matrix4d.CreateRotationY(MathHelper.DegreesToRadians(_pan)) *
                OpenTK.Matrix4d.CreateRotationX(MathHelper.DegreesToRadians(-_elevation)) *
                OpenTK.Matrix4d.CreateRotationY(MathHelper.DegreesToRadians(_tilt));
            _matrix = _matrixPosition * _matrixOrientation;
            _invalidated = false;
        }
    }
}