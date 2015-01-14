using Arleen.Rendering.Utility;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Arleen.Rendering.Sources
{
    public class TextRenderer : RenderSource
    {
        private bool _antialias;
        private TextDrawer _drawer;
        private Font _font;
        private string _text;

        public TextRenderer(Font font, bool antialias)
        {
            _text = string.Empty;
            _font = font;
            _antialias = antialias;

            MaxSize = null;
            Wrap = TextWrap.Truncate;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(string text, Font font, bool antialias)
        {
            _text = text;
            _font = font;
            _antialias = antialias;

            MaxSize = null;
            Wrap = TextWrap.Truncate;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(Font font, bool antialias, TextWrap wrap, Size maxSize)
        {
            _text = string.Empty;
            _font = font;
            _antialias = antialias;

            MaxSize = maxSize;
            Wrap = wrap;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(string text, Font font, bool antialias, TextWrap wrap, Size maxSize)
        {
            _text = text;
            _font = font;
            _antialias = antialias;

            MaxSize = maxSize;
            Wrap = wrap;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(Font font)
        {
            _text = string.Empty;
            _font = font;
            _antialias = false;

            MaxSize = null;
            Wrap = TextWrap.Truncate;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(string text, Font font)
        {
            _text = text;
            _font = font;
            _antialias = false;

            MaxSize = null;
            Wrap = TextWrap.Truncate;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(Font font, TextWrap wrap, Size maxSize)
        {
            _text = string.Empty;
            _font = font;
            _antialias = false;

            MaxSize = maxSize;
            Wrap = wrap;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public TextRenderer(string text, Font font, TextWrap wrap, Size maxSize)
        {
            _text = text;
            _font = font;
            _antialias = false;

            MaxSize = maxSize;
            Wrap = wrap;

            Color = Color.White;
            HorizontalTextAlign = TextAlign.Left;
            VerticalTextAlign = TextAlign.Top;
        }

        public bool Antialias
        {
            get
            {
                return _antialias;
            }
            set
            {
                _antialias = value;
                DestroyDrawer();
            }
        }

        public Color Color { get; set; }

        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                DestroyDrawer();
            }
        }

        public TextAlign HorizontalTextAlign { get; private set; }

        public Size? MaxSize { get; private set; }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                DestroyDrawer();
            }
        }

        public TextAlign VerticalTextAlign { get; set; }

        public TextWrap Wrap { get; private set; }

        public void DisableWrapping()
        {
            Wrap = TextWrap.Truncate;
            MaxSize = null;
            DestroyDrawer();
        }

        public void EnableWrapping(TextWrap wrap, Size maxSize)
        {
            Wrap = wrap;
            MaxSize = maxSize;
            DestroyDrawer();
        }

        protected override void OnInitilaize()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            CreateDrawer();
        }

        protected override void OnRender()
        {
            var targetSize = Renderer.RenderInfo.TargetSize;
            var drawer = CreateDrawer();
            GL.Disable(EnableCap.DepthTest);
            GL.LoadIdentity();
            ViewingVolumeHelper.PlaceOthogonalProjection(targetSize.Width, targetSize.Height, 0, 1);
            drawer.Draw(Color, new Rectangle(0, 0, targetSize.Width, targetSize.Height), HorizontalTextAlign, VerticalTextAlign);
            GL.Enable(EnableCap.DepthTest);
        }

        private TextDrawer CreateDrawer()
        {
            var drawer = _drawer;
            if (drawer == null)
            {
                drawer = _drawer = MaxSize.HasValue ? new TextDrawer(_text, _font, _antialias, Wrap, MaxSize.Value) : new TextDrawer(_text, _font, _antialias);
            }
            return drawer;
        }

        private void DestroyDrawer()
        {
            if (_drawer != null)
            {
                _drawer.Dispose();
                _drawer = null;
            }
        }
    }
}