using Arleen.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Arleen.Game
{
    public class Window : GameWindow
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private readonly Camera _camera;
        private readonly LinkedList<RenderSource> _renderSources;

        public Window()
            : this(Program.Configuration)
        {
            // Empty
        }

        // ---
        // TODO: Camera Projection
        // ---
        // TODO: Renderer
        // ---
        // ===
        // TODO: world
        // ===
        // ---
        // TODO: Localization
        // ---
        // TODO: Input
        // ---
        // TODO: Resource loading
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
            VSync = configuration.VSync ? VSyncMode.On : VSyncMode.Off;
            Title = Program.DisplayName;

            // ---

            InitializeOpenGl();

            // ---

            _renderSources = new LinkedList<RenderSource>();
            _camera = new Camera
                (
                    new ViewingVolume.Perspective
                    {
                        FieldOfView = 45,
                        FarPlane = FLT_FarPlane,
                        NearPlane = FLT_NearPlane
                    }
                );
        }

        protected override void OnLoad(EventArgs e)
        {
            // ---
            // TODO: Bind input
            // ---
            // ---
            // TODO: Load localization
            // ---
            // TODO: Initialize renderer
            // ---
            // TODO: Load resources
            // ---
            // TODO: Load world
            // ---
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            foreach (var item in _renderSources)
            {
                item.Render(_camera, e.Time);
            }

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.ViewingVolume.Update(Width, Height);
        }

        private void InitializeOpenGl()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend); // need for fonts

            //---

            GL.CullFace(CullFaceMode.Back);

            // ---
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // ---

            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
        }
    }
}