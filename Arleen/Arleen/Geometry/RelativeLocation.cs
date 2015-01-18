using OpenTK;
using System;

namespace Arleen.Geometry
{
    public class RelativeLocation : Location
    {
        private ILocable _anchor;
        private Mode _mode;

        public RelativeLocation()
        {
            _anchor = null;
            _mode = Mode.None;
        }

        public RelativeLocation(ILocable anchor, Mode mode)
        {
            SetAnchor(anchor, mode);
        }

        public override Quaterniond Orientation
        {
            get
            {
                ILocable anchor;
                Location location;
                if ((anchor = _anchor) != null && (location = anchor.Location) != null && (_mode & Mode.OrientationOnly) != Mode.None)
                {
                    var orientation = location.Orientation;
                    return orientation * base.Orientation;
                }
                return base.Orientation;
            }
            set
            {
                ILocable anchor;
                Location location;
                if ((anchor = _anchor) != null && (location = anchor.Location) != null && (_mode & Mode.OrientationOnly) != Mode.None)
                {
                    var orientation = location.Orientation;
                    orientation.Conjugate();
                    base.Orientation = value * orientation;
                }
                else
                {
                    base.Orientation = value;
                }
            }
        }

        public override Vector3d Position
        {
            get
            {
                ILocable anchor;
                Location location;
                if ((anchor = _anchor) != null && (location = anchor.Location) != null && (_mode & Mode.PositionOnly) != Mode.None)
                {
                    var position = location.Position;
                    return position + base.Position;
                }
                return base.Position;
            }
            set
            {
                ILocable anchor;
                Location location;
                if ((anchor = _anchor) != null && (location = anchor.Location) != null && (_mode & Mode.PositionOnly) != Mode.None)
                {
                    var position = location.Position;
                    base.Position = value - position;
                }
                else
                {
                    base.Position = value;
                }
            }
        }

        public Quaterniond RelativeOrientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                base.Orientation = value;
            }
        }

        public Vector3d RelativePosition
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
            }
        }

        public void SetAnchor(ILocable anchor, Mode mode)
        {
            if ((mode == Mode.None) == (anchor == null))
            {
                _anchor = anchor;
                _mode = mode;
            }
            else
            {
                throw new ArgumentException("Whenever anchor is null mode must be None, whenever mode is None anchor must be null.");
            }
        }

        public void TransferAnchor(ILocable anchor, Mode mode)
        {
            if ((mode == Mode.None) == (anchor == null))
            {
                Location location = null;
                var check = anchor != null && (location = anchor.Location) != null;
                {
                    var value = Position;
                    if (check && (_mode & Mode.PositionOnly) != Mode.None)
                    {
                        var position = location.Position;
                        base.Position = value - position;
                    }
                    else
                    {
                        base.Position = value;
                    }
                }
                {
                    var value = Orientation;
                    if (check && (_mode & Mode.OrientationOnly) != Mode.None)
                    {
                        var orientation = location.Orientation;
                        orientation.Conjugate();
                        base.Orientation = value * orientation;
                    }
                    else
                    {
                        base.Orientation = value;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Whenever anchor is null mode must be None, whenever mode is None anchor must be null.");
            }
        }

        internal override bool UpdateModelMatrices()
        {
            ILocable anchor;
            Location location;
            if ((anchor = _anchor) != null && (location = anchor.Location) != null)
            {
                if (location.UpdateModelMatrices())
                {
                    Invalidate();
                }
            }
            return base.UpdateModelMatrices();
        }
    }
}