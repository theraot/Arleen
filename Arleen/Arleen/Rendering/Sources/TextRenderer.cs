using Arleen.Geometry;
using Arleen.Rendering.Utility;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Security.Permissions;

namespace Arleen.Rendering.Sources
{
    public sealed class TextRenderer : RenderSource, IDisposable, ILocable, ICameraRelative
    {
        private readonly TextDrawer _drawer;

        public TextRenderer(Font font, bool antialias)
        {
            _drawer = new TextDrawer(string.Empty, font, antialias);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(string text, Font font, bool antialias)
        {
            _drawer = new TextDrawer(text, font, antialias);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(Font font, bool antialias, TextWrap wrap, Size maxSize)
        {
            _drawer = new TextDrawer(string.Empty, font, antialias);
            _drawer.EnableWrapping(wrap, maxSize);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(string text, Font font, bool antialias, TextWrap wrap, Size maxSize)
        {
            _drawer = new TextDrawer(text, font, antialias);
            _drawer.EnableWrapping(wrap, maxSize);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(Font font)
        {
            _drawer = new TextDrawer(string.Empty, font, false);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(string text, Font font)
        {
            _drawer = new TextDrawer(text, font, false);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(Font font, TextWrap wrap, Size maxSize)
        {
            _drawer = new TextDrawer(string.Empty, font, false);
            _drawer.EnableWrapping(wrap, maxSize);
            Color = Color.White;
            Location = new Location();
        }

        public TextRenderer(string text, Font font, TextWrap wrap, Size maxSize)
        {
            _drawer = new TextDrawer(text, font, false);
            _drawer.EnableWrapping(wrap, maxSize);
            Color = Color.White;
            Location = new Location();
        }

        public bool Antialias
        {
            get
            {
                return _drawer.Antialias;
            }
            set
            {
                _drawer.Antialias = value;
            }
        }

        public Color Color { get; set; }

        public Font Font
        {
            get
            {
                return _drawer.Font;
            }
            set
            {
                _drawer.Font = value;
            }
        }

        public TextAlign HorizontalTextAlign { get; set; }

        public Location Location { get; set; }

        public string Text
        {
            get
            {
                return _drawer.Text;
            }
            set
            {
                _drawer.Text = value;
            }
        }

        public TextAlign VerticalTextAlign { get; set; }

        public void DisableWrapping()
        {
            _drawer.DisableWrapping();
        }

        public void Dispose()
        {
            _drawer.Dispose();
        }

        public void EnableWrapping(TextWrap wrap, Size maxSize)
        {
            _drawer.EnableWrapping(wrap, maxSize);
        }

        protected override void OnInitilaize()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void OnRender()
        {
            var targetSize = Renderer.Current.RenderInfo.TargetSize;
            GL.Disable(EnableCap.DepthTest);
            ViewingVolumeHelper.PlaceOthogonalProjection(targetSize.Width, targetSize.Height, 0, 1);

            // ---

            var size = _drawer.GetSize();

            var offsetX = 0.0;
            var offsetY = 0.0;

            switch (HorizontalTextAlign)
            {
                case TextAlign.Center:
                    offsetX = (targetSize.Width - size.Width) / 2.0;
                    break;

                case TextAlign.Right:
                    offsetX = targetSize.Width - size.Width;
                    break;
            }
            switch (VerticalTextAlign)
            {
                case TextAlign.Center:
                    offsetY = (targetSize.Height - size.Height) / 2.0;
                    break;

                case TextAlign.Top:
                    offsetY = targetSize.Height - size.Height;
                    break;
            }

            GL.Translate(offsetX, offsetY, 0.0);

            // ---

            _drawer.Draw(Color);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}