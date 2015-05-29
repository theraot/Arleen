using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public class Scene
    {
        private readonly List<RenderTarget> _renderTargets;

        public Scene()
        {
            _renderTargets = new List<Arleen.Rendering.RenderTarget>();
        }

        public List<RenderTarget> RenderTargets
        {
            get
            {
                return _renderTargets;
            }
        }

        internal void Render(Rectangle clipArea, double elapsed, int fps)
        {
            foreach (var target in _renderTargets)
            {
                RenderTarget(target, clipArea, elapsed, fps);
            }
        }

        private void RenderTarget(RenderTarget target, Rectangle clipArea, double elapsed, int fps)
        {
            if (target.Enabled)
            {
                var targetClipArea = target.GetTargetClipArea(clipArea);
                var camera = target.Camera;

                RenderInfo.Current = new RenderInfo
                {
                    Camera = camera,
                    TargetSize = targetClipArea.Size,
                    ElapsedMilliseconds = elapsed,
                    Fps = fps
                };

                camera.ViewingVolume.Update(targetClipArea.Width, targetClipArea.Height);
                camera.ViewingVolume.Place();

                GL.Viewport(targetClipArea);
                GL.Scissor(targetClipArea.X, targetClipArea.Y, targetClipArea.Width, targetClipArea.Height);

                target.Renderable.Render();
            }
        }
    }
}