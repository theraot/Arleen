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
            _renderTargets = new List<RenderTarget>();
        }

        public List<RenderTarget> RenderTargets
        {
            get
            {
                return _renderTargets;
            }
        }

        internal void Render(Size surfaceSize, double elapsedMiliseconds, int fps)
        {
            RenderInfo.Current = new RenderInfo
            {
                SurfaceSize = surfaceSize,
                ElapsedMilliseconds = elapsedMiliseconds,
                Fps = fps
            };

            foreach (var target in _renderTargets)
            {
                Render(target, surfaceSize, elapsedMiliseconds, fps);
            }
        }

        private static void Render(RenderTarget target, Size surfaceSize, double elapsed, int fps)
        {
            if (target.Enabled)
            {
                var targetClipArea = target.SetSurfaceSize(surfaceSize);
                var camera = target.Camera;

                RenderTarget.Current = target;

                camera.ViewingVolume.Update(targetClipArea.Width, targetClipArea.Height);
                camera.ViewingVolume.Place();

                GL.Viewport(targetClipArea);
                GL.Scissor(targetClipArea.X, targetClipArea.Y, targetClipArea.Width, targetClipArea.Height);

                target.Renderable.Render();
            }
        }
    }
}