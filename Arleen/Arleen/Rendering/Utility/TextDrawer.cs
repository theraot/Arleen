using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Security.Permissions;

namespace Arleen.Rendering.Utility
{
    public sealed class TextDrawer : IDisposable
    {
        private bool _antialias;
        private Font _font;
        private bool _invalidated;
        private Size? _maxSize;
        private Size? _size;
        private string _text;
        private Texture _texture;
        private TextWrap _wrap;

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

        public bool Antialias
        {
            get
            {
                return _antialias;
            }
            set
            {
                _antialias = value;
                _invalidated = true;
            }
        }

        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                _invalidated = true;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                _invalidated = true;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, bool antialias, TextWrap wrap, Size maxSize, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias, wrap, maxSize))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, bool antialias, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, TextWrap wrap, Size maxSize, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font, wrap, maxSize))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, Color color, Rectangle area, TextAlign horizontalTextAlign, TextAlign verticalTextAlign)
        {
            using (var textDrawer = new TextDrawer(text, font))
            {
                textDrawer.Draw(color, area, horizontalTextAlign, verticalTextAlign);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, bool antialias, TextWrap wrap, Size maxSize, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias, wrap, maxSize))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, bool antialias, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font, antialias))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, TextWrap wrap, Size maxSize, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font, wrap, maxSize))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Draw(string text, Font font, Color color, double left, double top)
        {
            using (var textDrawer = new TextDrawer(text, font))
            {
                textDrawer.Draw(color, left, top);
            }
        }

        public void DisableWrapping()
        {
            _wrap = TextWrap.Truncate;
            _maxSize = null;
            _invalidated = true;
        }

        public void Dispose()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void Draw(Color color, double left, double top)
        {
            // We use GetTexture, that's why we mark SecurityPermission
            Texture texture = GetTexture();
            TextureDrawer.DrawTexture(texture, color, new Location { Position = new Vector3d(left, top, -1) });
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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

        public void EnableWrapping(TextWrap wrap, Size maxSize)
        {
            _wrap = wrap;
            _maxSize = maxSize;
            _invalidated = true;
        }

        private Texture CreateTexture()
        {
            SizeF size = GetSize();
            var textureWidth = (int)Math.Ceiling(size.Width);
            var textureHeight = (int)Math.Ceiling(size.Height);
            return CreateTextureExtracted(textureWidth, textureHeight, size);
        }

        private Texture CreateTextureExtracted(int textureWidth, int textureHeight, SizeF size)
        {
            if (textureWidth > 0 && textureHeight > 0)
            {
                using (var bitmap = new Bitmap(textureWidth, textureHeight))
                {
                    DrawTextToBitmap(bitmap, size);
                    return new Texture(bitmap, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
                }
            }
            return null;
        }

        private void DrawTextToBitmap(Bitmap bitmap, SizeF size)
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

        private Size GetSize()
        {
            if (_invalidated || _size == null)
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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private Texture GetTexture()
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
                    if (_invalidated)
                    {
                        // We use UpdateTexture, that's why we mark SecurityPermission
                        UpdateTexture();
                        _invalidated = false;
                    }
                    return _texture;
                }
            }
            return null;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private void UpdateTexture()
        {
            SizeF size = GetSize();
            var textureWidth = (int)Math.Ceiling(size.Width);
            var textureHeight = (int)Math.Ceiling(size.Height);
            if (_texture == null)
            {
                _texture = CreateTextureExtracted(textureWidth, textureHeight, size);
            }
            else if (textureWidth > _texture.Width || textureHeight > _texture.Height)
            {
                _texture.Dispose();
                _texture = CreateTextureExtracted(textureWidth, textureHeight, size);
            }
            else
            {
                using (var bitmap = new Bitmap(_texture.Width, _texture.Height))
                {
                    DrawTextToBitmap(bitmap, size);
                    // We use update, that's why we mark SecurityPermission
                    _texture.Update(bitmap, new Rectangle(0, 0, _texture.Width, _texture.Height));
                }
            }
        }
    }
}