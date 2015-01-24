using Arleen.Game;
using Arleen.Geometry;
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
    public class Renderer : ILocable
    {
        [ThreadStatic]
        private static Renderer _current;

        private readonly List<RenderTarget> _renderTargets;
        private FpsCounter _fpsCounter;
        private double _lastTime;
        private Rectangle _realClipArea;
        private Realm _realm;
        private RenderInfo _renderInfo;
        private Thread _thread;

        /// <summary>
        /// Creates a new instance of Renderer.
        /// </summary>
        public Renderer()
        {
            _current = this;
            _renderTargets = new List<RenderTarget>();
        }

        /// <summary>
        /// Gets the Current renderer.
        /// </summary>
        /// <remarks>On a renderer thread, by default returns the Renderer that created the Thread.
        /// Otherwise returns the last created renderer on the Thread, if none, returns null.</remarks>
        public static Renderer Current
        {
            get
            {
                return _current;
            }
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
        /// Returns the location of the current Camera.
        /// </summary>
        public Location Location
        {
            get
            {
                return RenderInfo.Location;
            }
        }

        /// <summary>
        /// Returns the last used RenderInfo.
        /// </summary>
        public RenderInfo RenderInfo
        {
            get
            {
                return _renderInfo;
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
            _lastTime = _realm.TotalTime;

            _realm.Resize += RealmResize;

            _realm.Context.MakeCurrent(null);

            _thread = new Thread
                (
                    () =>
                    {
                        _current = this;
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

        private static void InitializeOpenGl()
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
            var elapsed = totalTime - _lastTime;
            _lastTime = _realm.TotalTime;

            _fpsCounter.OnRender(elapsed / 1000.0);

            foreach (var target in _renderTargets)
            {
                RenderTarget(target, elapsed);
            }
        }

        private void RenderTarget(RenderTarget target, double elapsed)
        {
            if (target.Enabled)
            {
                var targetClipArea = target.GetTargetClipArea(_realClipArea);
                var camera = target.Camera;

                _renderInfo = new RenderInfo
                {
                    Camera = camera,
                    TargetSize = targetClipArea.Size,
                    ElapsedMilliseconds = elapsed,
                    Fps = _fpsCounter.Fps
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