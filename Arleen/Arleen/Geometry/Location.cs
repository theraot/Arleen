using OpenTK;

namespace Arleen.Geometry
{
    public class Location
    {
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

        public Matrix4d GetMatrix(PlaceMode mode)
        {
            if (_invalidated)
            {
                UpdateModelMatrices();
            }
            return GetMatrixExtracted(mode);
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

        private Matrix4d GetMatrixExtracted(PlaceMode mode)
        {
            switch (mode)
            {
                case PlaceMode.Full:
                    return _matrix;

                case PlaceMode.PositionOnly:
                    return _matrixPosition;

                case PlaceMode.OrientationOnly:
                    return _matrixOrientation;

                default:
                    return Matrix4d.Identity;
            }
        }

        private void UpdateModelMatrices()
        {
            _matrixPosition = OpenTK.Matrix4d.CreateTranslation(_position);
            _matrixOrientation = OpenTK.Matrix4d.Rotate(_orientation);
            _matrix = _matrixOrientation * _matrixPosition;
            _invalidated = false;
        }
    }
}