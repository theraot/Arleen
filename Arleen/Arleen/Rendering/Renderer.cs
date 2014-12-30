using Arleen.Game;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Arleen.Rendering
{
    public class Renderer
    {
        private readonly List<RenderSource> _renderSources;
        private readonly List<RenderTarget> _renderTargets;
        private FpsCounter _fpsCounter;
        private double _last_time;
        private Rectangle _realClipArea;
        private Window _window;
        private Thread _thread;

        public Renderer(Rectangle clipArea)
        {
            _realClipArea = clipArea;
            _renderSources = new List<RenderSource>();
            _renderTargets = new List<RenderTarget>();
        }

        public IList<RenderSource> RenderSources
        {
            get
            {
                return _renderSources;
            }
        }

        public IList<RenderTarget> RenderTargets
        {
            get
            {
                return _renderTargets;
            }
        }

        public void Initialize(Window window)
        {
            _window = window;
            _last_time = _window.TotalTime;

            _window.Resize += window_Resize;

            _window.Context.MakeCurrent(null);

            _thread = new Thread
                (
                    () =>
                    {
                        _window.Context.MakeCurrent(_window.WindowInfo);
                        InitializeOpenGl();
                        while (true)
                        {
                            Render();
                            if (_window.IsExiting)
                            {
                                break;
                            }
                            _window.SwapBuffers();
                        }
                    }
                )
            {
                Name = "Renderer Thread"
            };
            _thread.Start();

            _fpsCounter = new FpsCounter();
        }

        private void InitializeOpenGl()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.Enable(EnableCap.Blend);

            // ---

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // ---

            GL.CullFace(CullFaceMode.Back);

            // ---

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
        }

        private void Render()
        {
            var totalTime = _window.TotalTime;
            var elapsed = _last_time - totalTime;
            _last_time = _window.TotalTime;

            _fpsCounter.OnRender(elapsed);

            foreach (var item in _renderTargets)
            {
                item.Render(_renderSources, _realClipArea, elapsed, _fpsCounter.Fps);
            }
        }

        private void window_Resize(object sender, EventArgs e)
        {
            _realClipArea.Width = _window.Width;
            _realClipArea.Height = _window.Height;
        }
    }
}