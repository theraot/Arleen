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
        private Realm _realm;
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

        public void Initialize(Realm realm)
        {
            _realm = realm;
            _last_time = _realm.TotalTime;

            _realm.Resize += RealmResize;

            _realm.Context.MakeCurrent(null);

            _thread = new Thread
                (
                    () =>
                    {
                        _realm.Context.MakeCurrent(_realm.WindowInfo);
                        InitializeOpenGl();
                        while (true)
                        {
                            Render();
                            try
                            {
                                if (_realm.IsExiting)
                                {
                                    break;
                                }
                            }
                            catch (ObjectDisposedException)
                            {
                                break;
                            }
                            try
                            {
                                _realm.SwapBuffers();
                            }
                            catch (NullReferenceException)
                            {
                                break;
                            }
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
            var totalTime = _realm.TotalTime;
            var elapsed = totalTime - _last_time;
            _last_time = _realm.TotalTime;

            _fpsCounter.OnRender(elapsed / 1000.0);

            foreach (var item in _renderTargets)
            {
                item.Render(_renderSources, _realClipArea, elapsed, _fpsCounter.Fps);
            }
        }

        private void RealmResize(object sender, EventArgs e)
        {
            _realClipArea.Width = _realm.Width;
            _realClipArea.Height = _realm.Height;
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