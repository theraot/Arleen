using Arleen.Game;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Arleen.Rendering
{
    /// <summary>
    /// Manages the rendering process.
    /// </summary>
    public class Renderer
    {
        private readonly List<RenderTarget> _renderTargets;
        private FpsCounter _fpsCounter;
        private double _last_time;
        private Rectangle _realClipArea;
        private Realm _realm;
        private Thread _thread;

        /// <summary>
        /// Creates a new instance of Renderer.
        /// </summary>
        public Renderer()
        {
            _renderTargets = new List<RenderTarget>();
        }

        /// <summary>
        /// The last recorded number of frames per second.
        /// </summary>
        public int Fps
        {
            get
            {
                return _fpsCounter.Fps;
            }
        }

        /// <summary>
        /// The list of targets rendered by this instance.
        /// </summary>
        public IList<RenderTarget> RenderTargets
        {
            get
            {
                return _renderTargets;
            }
        }

        /// <summary>
        /// Starts the render process
        /// </summary>
        /// <param name="realm">The realm to which this renderer belongs.</param>
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

        private void RealmResize(object sender, EventArgs e)
        {
            _realClipArea.Width = _realm.Width;
            _realClipArea.Height = _realm.Height;
        }

        private void Release()
        {
            _renderTargets.Clear();
        }

        private void Render()
        {
            var totalTime = _realm.TotalTime;
            var elapsed = totalTime - _last_time;
            _last_time = _realm.TotalTime;

            _fpsCounter.OnRender(elapsed / 1000.0);

            foreach (var item in _renderTargets)
            {
                item.Render(_realClipArea, elapsed, _fpsCounter.Fps);
            }
        }
    }
}