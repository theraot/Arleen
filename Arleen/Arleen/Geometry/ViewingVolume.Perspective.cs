using OpenTK;
using System;

namespace Arleen.Geometry
{
    public abstract partial class ViewingVolume
    {
        /// <summary>
        /// Represents a perspective viewing volume.
        /// </summary>
        public sealed class Perspective : ViewingVolume
        {
            private double _aspectRatio;
            private double _fieldOfView;

            /// <summary>
            /// Gets or sets the relation of width over height of the viewing volume.
            /// </summary>
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

            /// <summary>
            /// Gets or sets the vertical field of view in degrees.
            /// </summary>
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

            /// <summary>
            /// Returns a string representation of the viewing volume.
            /// </summary>
            /// <returns>A string representing the viewing volume.</returns>
            public override string ToString()
            {
                return string.Format("Camera: fieldOfView {0}, aspectRatio {1}, zNear {2}, zFar {3}, {4}", _fieldOfView, _aspectRatio, NearPlane, FarPlane, base.ToString());
            }

            /// <summary>
            /// Updates the viewing volume to adjust to a resize of the viewport.
            /// </summary>
            /// <param name="width">The new width of the viewport.</param>
            /// <param name="height">The new height of the viewport.</param>
            public override void Update(int width, int height)
            {
                _aspectRatio = width / (double)height;
            }

            /// <summary>
            /// Calculates the projection matrix for the current viewing volume.
            /// </summary>
            /// <returns>A projection matrix for the current viewing volume.</returns>
            protected override Matrix4d CalculateProjectionMatrix()
            {
                return Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fieldOfView), _aspectRatio, NearPlane, FarPlane);
            }
        }
    }
}