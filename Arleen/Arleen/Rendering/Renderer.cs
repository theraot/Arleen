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

        public Renderer()
        {
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

            _window.Resize += Window_Resize;

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
                            try
                            {
                                if (_window.IsExiting)
                                {
                                    break;
                                }
                            }
                            catch (ObjectDisposedException)
                            {
                                break;
                            }
                            _window.SwapBuffers();
                        }
                        Release();
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
            var elapsed = totalTime - _last_time;
            _last_time = _window.TotalTime;

            _fpsCounter.OnRender(elapsed / 1000.0);

            foreach (var item in _renderTargets)
            {
                item.Render(_renderSources, _realClipArea, elapsed, _fpsCounter.Fps);
            }
        }

        private void Window_Resize(object sender, EventArgs e)
        {
            _realClipArea.Width = _window.Width;
            _realClipArea.Height = _window.Height;
        }

        private void Release()
        {
            var sources = _renderSources;
            foreach (var source in sources)
            {
                if (source is IDisposable)
                {
                    (source as IDisposable).Dispose();
                }
            }
            _renderSources.Clear();
            _renderTargets.Clear();
        }
    }
}