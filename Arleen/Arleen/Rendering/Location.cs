using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public class Location
    {
        private OpenTK.Matrix4d _computedMatrix = OpenTK.Matrix4d.Identity;
        private double _elevation;
        private bool _invalidModelMatrix;
        private OpenTK.Matrix4d _orientationMatrix = OpenTK.Matrix4d.Identity;
        private double _pan; // azimuth
        private Vector3d _position;
        private OpenTK.Matrix4d _positionMatrix = OpenTK.Matrix4d.Identity;
        private double _tilt;

        public enum PlaceMode
        {
            Full = 0,
            PositionOnly = 1,
            OrientationOnly = 2
        }

        public OpenTK.Matrix4d ComputedMatrix
        {
            get
            {
                return _computedMatrix;
            }
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
                    _invalidModelMatrix = true;
                }
            }
        }

        public OpenTK.Matrix4d OrientationMatrix
        {
            get
            {
                return _orientationMatrix;
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
                    _invalidModelMatrix = true;
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
                    _invalidModelMatrix = true;
                }
            }
        }

        public OpenTK.Matrix4d PositionMatrix
        {
            get
            {
                return _positionMatrix;
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
                    _invalidModelMatrix = true;
                }
            }
        }

        public static void PlaceDefaultLocation()
        {
            GL.LoadIdentity();
        }

        public void Place(PlaceMode mode)
        {
            if (_invalidModelMatrix)
            {
                UpdateModelMatrices();
            }
            switch (mode)
            {
                case PlaceMode.Full:
                    {
                        GL.LoadMatrix(ref _computedMatrix);
                    }
                    break;

                case PlaceMode.PositionOnly:
                    {
                        GL.LoadMatrix(ref _positionMatrix);
                    }
                    break;

                case PlaceMode.OrientationOnly:
                    {
                        GL.LoadMatrix(ref _orientationMatrix);
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
            _positionMatrix = OpenTK.Matrix4d.CreateTranslation(_position);
            _orientationMatrix =
                OpenTK.Matrix4d.CreateRotationY(MathHelper.DegreesToRadians(_pan)) *
                OpenTK.Matrix4d.CreateRotationX(MathHelper.DegreesToRadians(-_elevation)) *
                OpenTK.Matrix4d.CreateRotationY(MathHelper.DegreesToRadians(_tilt));
            _computedMatrix = _positionMatrix * _orientationMatrix;
            _invalidModelMatrix = false;
        }
    }
}