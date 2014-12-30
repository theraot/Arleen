using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public class Location
    {
        //private double _elevation;
        //private double _pan; // azimuth
        //private double _tilt;

        /*public double Elevation
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
        }*/

        /*public double Pan
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
        */

        /*public double Tilt
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
        }*/

        private bool _invalidated;
        private OpenTK.Matrix4d _matrix = OpenTK.Matrix4d.Identity;
        private OpenTK.Matrix4d _matrixOrientation = OpenTK.Matrix4d.Identity;
        private OpenTK.Matrix4d _matrixPosition = OpenTK.Matrix4d.Identity;
        private Quaterniond _orientation;
        private Vector3d _position;

        public Location()
        {
            _orientation = Quaterniond.Identity;
        }

        public enum PlaceMode
        {
            Full = 0,
            PositionOnly = 1,
            OrientationOnly = 2
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

        public Quaterniond Orientation
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

        public static void PlaceDefaultLocation()
        {
            GL.LoadIdentity();
        }

        public Vector3d Apply(Vector3d target, PlaceMode mode)
        {
            if (_invalidated)
            {
                UpdateModelMatrices();
                _invalidated = false;
            }
            return ApplyExtracted(target, mode);
        }

        public Matrix4d Apply(Matrix4d target, PlaceMode mode)
        {
            if (_invalidated)
            {
                UpdateModelMatrices();
            }
            return ApplyExtracted(target, mode);
        }

        public Vector3d Apply(Vector3d target, PlaceMode mode, bool reverse)
        {
            if (_invalidated)
            {
                UpdateModelMatrices();
            }
            if (reverse)
            {
                switch (mode)
                {
                    case PlaceMode.Full:
                        return Vector3d.Transform(target, Matrix4d.Transpose(_matrix));

                    case PlaceMode.PositionOnly:
                        return Vector3d.Transform(target, Matrix4d.Transpose(_matrixPosition));

                    case PlaceMode.OrientationOnly:
                        return Vector3d.Transform(target, Matrix4d.Transpose(_matrixOrientation));

                    default:
                        return target;
                }
            }
            else
            {
                return ApplyExtracted(target, mode);
            }
        }

        public Matrix4d Apply(Matrix4d target, PlaceMode mode, bool reverse)
        {
            if (_invalidated)
            {
                UpdateModelMatrices();
            }
            if (reverse)
            {
                switch (mode)
                {
                    case PlaceMode.Full:
                        return target * Matrix4d.Transpose(_matrix);

                    case PlaceMode.PositionOnly:
                        return target * Matrix4d.Transpose(_matrixPosition);

                    case PlaceMode.OrientationOnly:
                        return target * Matrix4d.Transpose(_matrixOrientation);

                    default:
                        return target;
                }
            }
            else
            {
                return ApplyExtracted(target, mode);
            }
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
            if (_invalidated)
            {
                UpdateModelMatrices();
            }
            return string.Format("Location: {0} - {1}", _position, _orientation);
        }

        private Vector3d ApplyExtracted(Vector3d target, PlaceMode mode)
        {
            switch (mode)
            {
                case PlaceMode.Full:
                    return Vector3d.Transform(target, _matrix);

                case PlaceMode.PositionOnly:
                    return Vector3d.Transform(target, _matrixPosition);

                case PlaceMode.OrientationOnly:
                    return Vector3d.Transform(target, _matrixOrientation);

                default:
                    return target;
            }
        }

        private Matrix4d ApplyExtracted(Matrix4d target, PlaceMode mode)
        {
            switch (mode)
            {
                case PlaceMode.Full:
                    return target * _matrix;

                case PlaceMode.PositionOnly:
                    return target * _matrixPosition;

                case PlaceMode.OrientationOnly:
                    return target * _matrixOrientation;

                default:
                    return target;
            }
        }

        private void UpdateModelMatrices()
        {
            _matrixPosition = OpenTK.Matrix4d.CreateTranslation(_position);
            _matrixOrientation = OpenTK.Matrix4d.Rotate(_orientation);
            /*_matrixOrientation =
                OpenTK.Matrix4d.CreateRotationY(MathHelper.DegreesToRadians(_pan)) *
                OpenTK.Matrix4d.CreateRotationX(MathHelper.DegreesToRadians(-_elevation)) *
                OpenTK.Matrix4d.CreateRotationY(MathHelper.DegreesToRadians(_tilt));*/
            _matrix = _matrixPosition * _matrixOrientation;
            _invalidated = false;
        }
    }
}