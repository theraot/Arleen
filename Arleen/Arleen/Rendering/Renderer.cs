using Arleen.Game;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Arleen.Rendering
{
    public class Renderer
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private readonly Camera _camera;
        private readonly LinkedList<RenderSource> _renderSources;
        private Rectangle _clipArea;

        public Renderer()
        {
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

        public LinkedList<RenderSource> RenderSources
        {
            get
            {
                return _renderSources;
            }
        }

        public void Initialize(Window window)
        {
            InitializeOpenGl();
            window.Resize += window_Resize;
            window.RenderFrame += window_RenderFrame;
        }

        private void window_RenderFrame(object sender, OpenTK.FrameEventArgs e)
        {
            GL.ClearColor(Color.BlanchedAlmond);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            foreach (var item in _renderSources)
            {
                item.Render(_camera, _clipArea, e.Time);
            }
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

        private void window_Resize(object sender, EventArgs e)
        {
            var window = (Window)sender;
            GL.Viewport(0, 0, window.Width, window.Height);
            _camera.ViewingVolume.Update(window.Width, window.Height);
            _clipArea.Width = window.Width;
            _clipArea.Height = window.Height;
        }
    }
}