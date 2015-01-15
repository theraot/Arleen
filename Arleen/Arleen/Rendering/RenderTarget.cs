using System.Drawing;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents the location where to render.
    /// </summary>
    public class RenderTarget
    {
        private readonly Camera _camera;
        private readonly RenderSource _renderSource;
        private readonly RectangleF _virtualClipArea;

        /// <summary>
        /// Creates a new RenderTarget
        /// </summary>
        /// <param name="virtualClipArea">The rectangle in which to render, in screen = 1.</param>
        /// <param name="camera">The camera associated with this target.</param>
        /// <param name="renderSource">The render source that will be rendered to this target.</param>
        public RenderTarget(RectangleF virtualClipArea, Camera camera, RenderSource renderSource)
        {
            _virtualClipArea = virtualClipArea;
            _camera = camera;
            _renderSource = renderSource;
            Enabled = true;
        }

        /// <summary>
        /// Gets the camera associates with this RenderTarget.
        /// </summary>
        public Camera Camera
        {
            get
            {
                return _camera;
            }
        }

        /// <summary>
        /// Gets or sets whatever this RenderTargets receives output.
        /// </summary>
        public bool Enabled { get; private set; }

        public RenderSource RenderSource
        {
            get
            {
                return _renderSource;
            }
        }

        /// <summary>
        /// Gets the rectangle in which to render, in screen = 1.
        /// </summary>
        public RectangleF VirtualClipArea
        {
            get
            {
                return _virtualClipArea;
            }
        }

        /// <summary>
        /// Returns the computed target clip area of the RenderTarget.
        /// </summary>
        /// <param name="realClipArea">The clip area of the graphic context.</param>
        /// <returns>A rectangle representing the target clip area.</returns>
        internal Rectangle GetTargetClipArea(Rectangle realClipArea)
        {
            var targetClipArea = ComputeClipArea(realClipArea, _virtualClipArea);
            _camera.ViewingVolume.Update(targetClipArea.Width, targetClipArea.Height);
            return targetClipArea;
        }

        private static Rectangle ComputeClipArea(Rectangle realClipArea, RectangleF virtualClipArea)
        {
            var targetClipArea = new Rectangle
                (
                    (int)((virtualClipArea.X * realClipArea.Width) + realClipArea.X),
                    (int)((virtualClipArea.Y * realClipArea.Height) + realClipArea.Y),
                    (int)(virtualClipArea.Width * realClipArea.Width),
                    (int)(virtualClipArea.Height * realClipArea.Height)
                );
            return targetClipArea;
        }
    }
}