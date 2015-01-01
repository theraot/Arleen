using Arleen.Rendering;
using Arleen.Rendering.Sources;
using Arleen.Rendering.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Arleen.Game
{
    public sealed class Window : GameWindow
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private Renderer _renderer;
        private readonly Stopwatch _time = new Stopwatch();

        public Window()
            : this(Program.Configuration)
        {
            // Empty
        }

        // ===
        // TODO: world
        // ===
        // ---
        // TODO: Input
        // ---
        // TODO: User Interface
        // ---
        // TODO: Audio
        // ---
        // TODO: Physics
        // ---
        private Window(Configuration configuration)
            : base(configuration.Resolution.Width, configuration.Resolution.Height)
        {
            Title = Program.DisplayName;
            _time.Start();
            Initialize();
        }

        private void Initialize()
        {
            var _camera = new Camera
                (
                new ViewingVolume.Perspective
                {
                    FieldOfView = 45,
                    FarPlane = FLT_FarPlane,
                    NearPlane = FLT_NearPlane,
                    AspectRatio = 1
                }
                );
            _renderer = new Renderer();
            _renderer.RenderSources.Add
                (
                    new BackgroundColorRenderSource(Color.LightSkyBlue, 1.0)
                );
            _renderer.RenderSources.Add
                (
                    new SkyboxRenderer(Resources.LoadBitmap("skybox.png"))
                );
            _renderer.RenderSources.Add
                (
                    new CustomRenderer
                        (
                        info =>
                        {
                            GL.Enable(EnableCap.Blend);
                            GL.BlendEquation(BlendEquationMode.FuncAdd);
                            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                            GL.Disable(EnableCap.DepthTest);
                            ViewingVolumeHelper.PlaceOthogonalProjection(info.ClipArea.Width, info.ClipArea.Height,
                                FLT_NearPlane, FLT_FarPlane);
                            Rendering.Utility.TextDrawer.Draw
                                (
                                    Program._["Hello, my name is {name}."].FormatWith(new { name = Program.DisplayName }) + "\n" +
                                    "FPS:" + info.Fps,
                                    new Font("Verdana", 12, FontStyle.Regular),
                                    true,
                                    Color.White,
                                    info.ClipArea,
                                    TextAlign.Left,
                                    TextAlign.Top
                                );
                            GL.LoadIdentity();
                            GL.Enable(EnableCap.DepthTest);
                        }
                        )
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                        new RectangleF(0, 0, 1.0f, 1.0f),
                        _camera
                        )
                );
        }

        public double TotalTime
        {
            get
            {
                return _time.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // ---
            // TODO: Bind input
            // ---

            _renderer.Initialize(this);

            // ---
            // TODO: Load world
            // ---
        }
    }
}