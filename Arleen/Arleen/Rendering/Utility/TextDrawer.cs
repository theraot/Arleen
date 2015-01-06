using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Arleen.Rendering.Utility
{
    public sealed class TextDrawer : IDisposable
    {
        private readonly bool _antialias;
        private readonly Font _font;
        private readonly string _text;
        private readonly TextWrap _wrap;
        private Size? _maxSize;

        private Size? _size;
        private Texture _texture;

        public TextDrawer(string text, Font font, bool antialias)
        {
            _text = text;
            _font = font;
            _antialias = antialias;

            _maxSize = null;
            _wrap = TextWrap.Truncate;
        }

        public TextDrawer(string text, Font font, bool antialias, TextWrap wrap, Size maxSize)
        {
            _text = text;
            _font = font;
            _antialias = antialias;

            _maxSize = maxSize;
            _wrap = wrap;
        }

        public TextDrawer(string text, Font font)
        {
            _text = text;
            _font = font;
            _antialias = false;

            _maxSize = null;
            _wrap = TextWrap.Truncate;
        }

        public TextDrawer(string text, Font font, TextWrap wrap, Size maxSize)
        {
            _text = text;
            _font = font;
            _antialias = false;

            _maxSize = maxSize;
            _wrap = wrap;
        }

        public static void Draw(string text, Font font, bool antialias, TextWrap wrap, Size maxSize, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias, wrap, maxSize))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        public static void Draw(string text, Font font, bool antialias, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        public static void Draw(string text, Font font, TextWrap wrap, Size maxSize, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font, wrap, maxSize))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        public static void Draw(string text, Font font, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        public static void Draw(string text, Font font, bool antialias, TextWrap wrap, Size maxSize, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias, wrap, maxSize))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        public static void Draw(string text, Font font, bool antialias, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        public static void Draw(string text, Font font, TextWrap wrap, Size maxSize, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font, wrap, maxSize))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        public static void Draw(string text, Font font, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        public void Dispose()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }

        public void Draw(Color color, double left, double top)
        {
            Texture texture;
            if (TryGetTexture(out texture))
            {
                GL.Enable(EnableCap.Texture2D);
                texture.Bind();
                double minX = left;
                double minY = top;
                double maxX = left + texture.Width;
                double maxY = top + texture.Height;

                GL.Color4(color);

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(minX, minY, -1);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(maxX, minY, -1);

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(maxX, maxY, -1);

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(minX, maxY, -1);
                }
                GL.End();
            }
        }

        public void Draw(Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            var size = GetSize();

            double x = area.Location.X;
            double y = area.Location.Y;

            switch (horizontalTextAlign)
            {
                case TextAlign.Center:
                    x += (area.Size.Width - size.Width) / 2.0;
                    break;

                case TextAlign.Right:
                    x += area.Size.Width - size.Width;
                    break;
            }
            switch (verticalTextAlign)
            {
                case TextAlign.Center:
                    y += (area.Size.Height - area.Location.Y) / 2.0;
                    break;

                case TextAlign.Top:
                    y += area.Size.Height - size.Height;
                    break;
            }
            Draw(color, x, y);
        }

        private Size GetSize()
        {
            if (_size == null)
            {
                using (var bitmap = new Bitmap(1, 1))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        SizeF stringSize;
                        if (_maxSize == null)
                        {
                            stringSize = graphics.MeasureString(_text, _font, new PointF(0.0f, 0.0f), GetFormat());
                        }
                        else
                        {
                            var maxsize = _maxSize.Value;
                            stringSize = graphics.MeasureString
                                (
                                    _text,
                                    _font,
                                    new SizeF(maxsize.Width, maxsize.Height),
                                    GetFormat()
                                );
                        }
                        var size = new Size((int)Math.Ceiling(stringSize.Width), (int)Math.Ceiling(stringSize.Height));
                        _size = size;
                        return size;
                    }
                }
            }
            return _size.Value;
        }

        private bool TryGetTexture(out Texture texture)
        {
            if (_texture == null)
            {
                _texture = CreateTexture();
            }
            if (_texture != null)
            {
                if (_texture.Width == 0 || _texture.Height == 0)
                {
                    _texture.Dispose();
                    _texture = null;
                }
                else
                {
                    texture = _texture;
                    return true;
                }
            }
            texture = null;
            return false;
        }

        private Texture CreateTexture()
        {
            SizeF size = GetSize();
            var textureWidth = (int)Math.Ceiling(size.Width);
            var textureHeight = (int)Math.Ceiling(size.Height);
            if (textureWidth > 0 && textureHeight > 0)
            {
                using (var bitmap = new Bitmap(textureWidth, textureHeight))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        if (_antialias)
                        {
                            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        }
                        graphics.Clear(Color.Transparent);

                        StringFormat stringFormat = GetFormat();
                        graphics.DrawString(_text, _font, Brushes.White, new RectangleF(0f, 0f, size.Width, size.Height), stringFormat);
                    }

                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    return new Texture(bitmap, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the string format used for drawing or measuring text.
        /// </summary>
        private StringFormat GetFormat()
        {
            var stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
            switch (_wrap)
            {
                case TextWrap.Truncate:
                    stringFormat.Trimming = StringTrimming.None;
                    stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                    break;

                case TextWrap.Ellipsis:
                    stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                    stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                    break;

                case TextWrap.Wrap:
                    stringFormat.Trimming = StringTrimming.None;
                    break;
            }
            return stringFormat;
        }
    }
}