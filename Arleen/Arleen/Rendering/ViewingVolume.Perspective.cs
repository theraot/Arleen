using OpenTK;
using System;

namespace Arleen.Rendering
{
    public abstract partial class ViewingVolume
    {
        public sealed class Perspective : ViewingVolume
        {
            private double _aspectRatio;
            private double _fieldOfView;

            public double AspectRatio
            {
                get
                {
                    return _aspectRatio;
                }
                set
                {
                    if (Math.Abs(_aspectRatio - value) > 0.0f)
                    {
                        _aspectRatio = value;
                        InvalidProjectionMatrix = true;
                    }
                }
            }

            public double FieldOfView
            {
                get
                {
                    return _fieldOfView;
                }
                set
                {
                    if (Math.Abs(_fieldOfView - value) > 0.0f)
                    {
                        _fieldOfView = value;
                        InvalidProjectionMatrix = true;
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("Camera: fieldOfView {0}, aspectRatio {1}, zNear {2}, zFar {3}, {4}", _fieldOfView, _aspectRatio, NearPlane, FarPlane, base.ToString());
            }

            public override void Update(int width, int height)
            {
                _aspectRatio = width / (double)height;
            }

            protected override Matrix4d OnUpdateProjectionMatrices()
            {
                return Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fieldOfView), _aspectRatio, NearPlane, FarPlane);
            }
        }
    }
}