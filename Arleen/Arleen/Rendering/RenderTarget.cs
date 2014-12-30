using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

namespace Arleen.Rendering
{
    public class RenderTarget
    {
        private readonly Camera _camera;
        private readonly RectangleF _virtualClipArea;
        private bool _enabled;

        public RenderTarget(RectangleF virtualClipArea, Camera camera)
        {
            _virtualClipArea = virtualClipArea;
            _camera = camera;
            _enabled = true;
        }

        public Camera Camera
        {
            get
            {
                return _camera;
            }
        }

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

        public RectangleF VirtualClipArea
        {
            get
            {
                return _virtualClipArea;
            }
        }

        public void Render(IEnumerable<RenderSource> sources, Rectangle realClipArea, double time)
        {
            if (_enabled)
            {
                var targetClipArea = new Rectangle
                (
                    (int)(_virtualClipArea.X * realClipArea.Width + realClipArea.X),
                    (int)(_virtualClipArea.Y * realClipArea.Height + realClipArea.Y),
                    (int)(_virtualClipArea.Width * realClipArea.Width),
                    (int)(_virtualClipArea.Height * realClipArea.Height)
                );
                _camera.ViewingVolume.Update(targetClipArea.Width, targetClipArea.Height);
                GL.Viewport(realClipArea);
                GL.Scissor(targetClipArea.X, targetClipArea.Y, targetClipArea.Width, targetClipArea.Height);
                Camera.ViewingVolume.Place();
                foreach (var item in sources)
                {
                    item.Render(targetClipArea, time);
                }
            }
        }
    }
}