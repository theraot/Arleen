using OpenTK;
using System;

namespace Arleen.Geometry
{
    /// <summary>
    /// Represents a viewing volume.
    /// </summary>
    /// <remarks>The viewing volume can be understood as the space of things that may be visible to the camera.</remarks>
    public abstract partial class ViewingVolume
    {
        private double _farPlane;
        private double _nearPlane;
        private OpenTK.Matrix4d _projectionMatrix = OpenTK.Matrix4d.Identity;

        /// <summary>
        /// Gets or sets the distance to the far plane of the viewing volume.
        /// </summary>
        public double FarPlane
        {
            get
            {
                return _farPlane;
            }
            set
            {
                _farPlane = value;
                if (Math.Abs(_farPlane - value) > 0.0f)
                {
                    _farPlane = value;
                    InvalidProjectionMatrix = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the distance to the near plane of the viewing volume.
        /// </summary>
        public double NearPlane
        {
            get
            {
                return _nearPlane;
            }
            set
            {
                _nearPlane = value;
                if (Math.Abs(_nearPlane - value) > 0.0f)
                {
                    _nearPlane = value;
                    InvalidProjectionMatrix = true;
                }
            }
        }

        /// <summary>
        /// Returns the last placed projection matrix for the current viewing volume.
        /// </summary>
        public OpenTK.Matrix4d ProjectionMatrix
        {
            get
            {
                return _projectionMatrix;
            }
        }

        private bool InvalidProjectionMatrix { get; set; }

        /// <summary>
        /// Updates the viewing volume to adjust to a resize of the viewport.
        /// </summary>
        /// <param name="width">The new width of the viewport.</param>
        /// <param name="height">The new height of the viewport.</param>
        public virtual void Update(int width, int height)
        {
            //Empty
        }

        /// <summary>
        /// Calculates the projection matrix for the current viewing volume.
        /// </summary>
        /// <returns>A projection matrix for the current viewing volume.</returns>
        protected abstract Matrix4d CalculateProjectionMatrix();

        internal void UpdateProjectionMatrices()
        {
            if (InvalidProjectionMatrix)
            {
                _projectionMatrix = CalculateProjectionMatrix();
            }
            InvalidProjectionMatrix = false;
        }
    }
}