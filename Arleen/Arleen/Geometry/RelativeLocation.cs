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

        public override Quaterniond Orientation
        {
            get
            {
                if ((_mode & Mode.OrientationOnly) != Mode.None)
                {
                    var orientation = _anchor.Location.Orientation;
                    return orientation * base.Orientation;
                }
                else
                {
                    return base.Orientation;
                }
            }
            set
            {
                if ((_mode & Mode.OrientationOnly) != Mode.None)
                {
                    var orientation = _anchor.Location.Orientation;
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
                if ((_mode & Mode.PositionOnly) != Mode.None)
                {
                    var position = _anchor.Location.Position;
                    return position + base.Position;
                }
                else
                {
                    return base.Position;
                }
            }
            set
            {
                if ((_mode & Mode.PositionOnly) != Mode.None)
                {
                    var position = _anchor.Location.Position;
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
            if ((mode == Mode.None) == (_anchor == null))
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
            if ((mode == Mode.None) == (_anchor == null))
            {
                {
                    var value = Position;
                    if ((_mode & Mode.PositionOnly) != Mode.None)
                    {
                        var position = anchor.Location.Position;
                        base.Position = value - position;
                    }
                    else
                    {
                        base.Position = value;
                    }
                }
                {
                    var value = Orientation;
                    if ((_mode & Mode.OrientationOnly) != Mode.None)
                    {
                        var orientation = anchor.Location.Orientation;
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
    }
}