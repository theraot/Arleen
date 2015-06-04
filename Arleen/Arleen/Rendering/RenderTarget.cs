using System;
using System.Collections.Generic;
using System.Drawing;
using Arleen.Rendering.Sources;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents the location where to render.
    /// </summary>
    public class RenderTarget : MarshalByRefObject
    {
        private readonly Camera _camera;

        private readonly IRenderable _renderable;

        private readonly RectangleF _virtualClipArea;

        private Rectangle _clipArea;

        /// <summary>
        /// Creates a new RenderTarget
        /// </summary>
        /// <param name="virtualClipArea">The rectangle in which to render, in screen = 1.</param>
        /// <param name="camera">The camera associated with this target.</param>
        /// <param name="renderable">The render source that will be rendered to this target.</param>
        public RenderTarget(RectangleF virtualClipArea, Camera camera, IRenderable renderable)
        {
            _virtualClipArea = virtualClipArea;
            _camera = camera;
            _renderable = renderable;
            Enabled = true;
        }

        /// <summary>
        /// Creates a new RenderTarget
        /// </summary>
        /// <param name="virtualClipArea">The rectangle in which to render, in screen = 1.</param>
        /// <param name="camera">The camera associated with this target.</param>
        /// <param name="renderable">The render source that will be rendered to this target.</param>
        public RenderTarget(RectangleF virtualClipArea, Camera camera, params IRenderable[] renderable)
        {
            _virtualClipArea = virtualClipArea;
            _camera = camera;
            _renderable = Engine.Create<AggregateRenderSource>((IList<IRenderable>)renderable);
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

        public Rectangle ClipArea
        {
            get
            {
                return _clipArea;
            }
        }

        /// <summary>
        /// Gets or sets whatever this RenderTargets receives output.
        /// </summary>
        public bool Enabled { get; private set; }

        public IRenderable Renderable
        {
            get
            {
                return _renderable;
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

        internal static RenderTarget Current { get; set; }

        /// <summary>
        /// Returns the computed target clip area of the RenderTarget.
        /// </summary>
        /// <param name="surfaceSize">The area of the graphic context.</param>
        /// <returns>A rectangle representing the target clip area.</returns>
        internal Rectangle SetSurfaceSize(Size surfaceSize)
        {
            _clipArea = ComputeClipArea(surfaceSize, _virtualClipArea);
            _camera.ViewingVolume.Update(_clipArea.Width, _clipArea.Height);
            return _clipArea;
        }

        private static Rectangle ComputeClipArea(Size surfaceSize, RectangleF virtualClipArea)
        {
            var targetClipArea = new Rectangle(
                                     (int)(virtualClipArea.X * surfaceSize.Width),
                                     (int)(virtualClipArea.Y * surfaceSize.Height),
                                     (int)(virtualClipArea.Width * surfaceSize.Width),
                                     (int)(virtualClipArea.Height * surfaceSize.Height)
                                 );
            return targetClipArea;
        }
    }
}