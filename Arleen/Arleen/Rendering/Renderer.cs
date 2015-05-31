using Arleen.Game;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Threading;
using OpenTK;

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
        private Size _surfaceSize;
        private Realm _realm;
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

            _thread = new Thread
                (
                    () =>
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
            _surfaceSize.Width = _gameWindow.Width;
            _surfaceSize.Height = _gameWindow.Height;
        }

        private void Render()
        {
            var totalTime = _realm.TotalTime;
            var elapsedMiliseconds = totalTime - _lastTime;
            _lastTime = _realm.TotalTime;

            _fpsCounter.OnRender(elapsedMiliseconds / 1000.0);

            _scene.Render(_surfaceSize, elapsedMiliseconds, _fpsCounter.Fps);
        }
    }
}