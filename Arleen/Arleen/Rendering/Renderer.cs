using Arleen.Game;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Threading;

namespace Arleen.Rendering
{
    /// <summary>
    /// Manages the rendering process.
    /// </summary>
    public class Renderer
    {
        private readonly Scene _scene;
        private FpsCounter _fpsCounter;
        private GameWindow _gameWindow;
        private double _lastTime;
        private Realm _realm;
        private Size _surfaceSize;
        private Thread _thread;

        /// <summary>
        /// Creates a new instance of Renderer.
        /// </summary>
        public Renderer(Scene scene)
        {
            if (scene == null)
            {
                throw new ArgumentNullException("scene");
            }
            _scene = scene;
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

        public Scene Scene
        {
            get
            {
                return _scene;
            }
        }

        /// <summary>
        /// Starts the render process
        /// </summary>
        /// <param name="gameWindow">The game window to which to render.</param>
        /// <param name="realm">The realm to which this renderer belongs.</param>
        public void Initialize(GameWindow gameWindow, Realm realm)
        {
            _gameWindow = gameWindow;
            _realm = realm;
            _lastTime = _realm.TotalTime;

            _gameWindow.Resize += RealmResize;

            _gameWindow.Context.MakeCurrent(null);

            ThreadStart render = () =>
            {
                Logbook.Instance.Trace(System.Diagnostics.TraceEventType.Information, "Renderer Thread started with Id {0}.", Thread.CurrentThread.ManagedThreadId);
                try
                {
                    _gameWindow.Context.MakeCurrent(_gameWindow.WindowInfo);
                    InitializeOpenGl();
                    while (true)
                    {
                        Render();
                        try
                        {
                            if (_gameWindow.IsExiting)
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
                            _gameWindow.SwapBuffers();
                        }
                        catch (NullReferenceException)
                        {
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logbook.Instance.ReportException(exception, "running Renderer Thread", true);
                }
            };

            _thread = new Thread(render) {
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

        private static void Render(RenderTarget target, Size surfaceSize)
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

                target.RenderSource.Render();
            }
        }

        private void RealmResize(object sender, EventArgs e)
        {
            _surfaceSize.Width = _gameWindow.Width;
            _surfaceSize.Height = _gameWindow.Height;
        }

        private void Render()
        {
            var totalTime = _realm.TotalTime;
            var elapsedMiliseconds = totalTime - _lastTime;
            _lastTime = _realm.TotalTime;

            _fpsCounter.OnRender(elapsedMiliseconds / 1000.0);

            var renderTargets = _scene.RenderTargets;

            RenderInfo.Current = new RenderInfo {
                SurfaceSize = _surfaceSize,
                ElapsedMilliseconds = elapsedMiliseconds,
                Fps = _fpsCounter.Fps
            };

            foreach (var target in renderTargets)
            {
                Render(target, _surfaceSize);
            }
        }
    }
}