using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents the location where to render.
    /// </summary>
    public class RenderTarget
    {
        private readonly Camera _camera;
        private readonly RectangleF _virtualClipArea;
        private bool _enabled;

        /// <summary>
        /// Creates a new RenderTarget
        /// </summary>
        /// <param name="virtualClipArea">The rectangle in which to render, in screen = 1.</param>
        /// <param name="camera">The camera associated with this target.</param>
        public RenderTarget(RectangleF virtualClipArea, Camera camera)
        {
            _virtualClipArea = virtualClipArea;
            _camera = camera;
            _enabled = true;
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
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
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
        /// Requests the output of the given list of RenderSource.
        /// </summary>
        /// <param name="sources">The list of RenderSource to get output from.</param>
        /// <param name="realClipArea">The rectangle in which to render, in pixel = 1.</param>
        /// <param name="elapsedMilliseconds">The time since the last iteration of the Renderer.</param>
        /// <param name="fps">The number of frames that were rendered in the past second.</param>
        public void Render(IEnumerable<RenderSource> sources, Rectangle realClipArea, double elapsedMilliseconds, int fps)
        {
            if (_enabled)
            {
                var targetClipArea = ComputeClipArea(realClipArea, _virtualClipArea);
                _camera.ViewingVolume.Update(targetClipArea.Width, targetClipArea.Height);
                GL.Viewport(targetClipArea);
                GL.Scissor(targetClipArea.X, targetClipArea.Y, targetClipArea.Width, targetClipArea.Height);
                Camera.ViewingVolume.Place();

                var renderInfo = new RenderInfo
                {
                    Camera = Camera,
                    TargetSize = targetClipArea.Size,
                    ElapsedMilliseconds = elapsedMilliseconds,
                    Fps = fps
                };

                foreach (var item in sources)
                {
                    item.Render(renderInfo);
                }
            }
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