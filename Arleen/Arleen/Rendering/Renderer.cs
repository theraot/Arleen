using Arleen.Game;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Arleen.Rendering
{
    public class Renderer
    {
        private readonly List<RenderSource> _renderSources;
        private readonly List<RenderTarget> _renderTargets;
        private Rectangle _realClipArea;

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
            InitializeOpenGl();
            window.Resize += window_Resize;
            window.RenderFrame += window_RenderFrame;
        }

        private void window_RenderFrame(object sender, OpenTK.FrameEventArgs e)
        {
            GL.ClearColor(Color.BlanchedAlmond);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            foreach (var item in _renderTargets)
            {
                item.Render(_renderSources, _realClipArea, e.Time);
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
            _realClipArea.Width = window.Width;
            _realClipArea.Height = window.Height;
        }
    }
}