﻿using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents a texture
    /// </summary>
    public sealed class Texture : IDisposable
    {
        private readonly int _height;
        private readonly int _width;
        private int _index;

        /// <summary>
        /// Creates a new instance of Texture.
        /// </summary>
        /// <param name="stream">The bitmap stream that will be loaded to video memory.</param>
        public Texture(Stream stream)
        {
            try
            {
                var bitmap = new Bitmap(stream);
                _index = LoadTexture(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), TextureMinFilter.Nearest, TextureMagFilter.Nearest);
                _width = bitmap.Width;
                _height = bitmap.Height;
            }
            catch (Exception exception)
            {
                Facade.Logbook.ReportException
                (
                    exception,
                    "trying to load texture",
                    false
                );
            }
        }

        /// <summary>
        /// Creates a new instance of Texture.
        /// </summary>
        /// <param name="bitmap">The bitmap that will be loaded to video memory.</param>
        /// <param name="rectangle">The rectangle of the bitmap to use.</param>
        public Texture(Bitmap bitmap, Rectangle rectangle)
        {
            try
            {
                _index = LoadTexture(bitmap, rectangle, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
                _width = rectangle.Width;
                _height = rectangle.Height;
            }
            catch (Exception exception)
            {
                Facade.Logbook.ReportException
                (
                    exception,
                    "trying to load texture",
                    false
                );
            }
        }

        /// <summary>
        /// Creates a new instance of Texture.
        /// </summary>
        /// <param name="bitmap">The bitmap that will be loaded to video memory.</param>
        /// <param name="minFilter">Mipmap Filter for texture reduction.</param>
        /// <param name="magFilter">Mipmap filter for texture magnification.</param>
        public Texture(Bitmap bitmap, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            try
            {
                _index = LoadTexture(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), minFilter, magFilter);
                _width = bitmap.Width;
                _height = bitmap.Height;
            }
            catch (Exception exception)
            {
                Facade.Logbook.ReportException
                (
                    exception,
                    "trying to load texture",
                    false
                );
            }
        }

        /// <summary>
        /// Creates a new instance of Texture.
        /// </summary>
        /// <param name="bitmap">The bitmap that will be loaded to video memory.</param>
        /// <param name="rectangle">The rectangle of the bitmap to use.</param>
        /// <param name="minFilter">Mipmap Filter for texture reduction.</param>
        /// <param name="magFilter">Mipmap filter for texture magnification.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Texture(Bitmap bitmap, Rectangle rectangle, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            try
            {
                _index = LoadTexture(bitmap, rectangle, minFilter, magFilter);
                _width = rectangle.Width;
                _height = rectangle.Height;
            }
            catch (Exception exception)
            {
                Facade.Logbook.ReportException
                (
                    exception,
                    "trying to load texture",
                    false
                );
            }
        }

        /// <summary>
        /// Gets the height of the loaded texture
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// Gets the width of the loaded texture
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// Unbinds the texture to the current graphic context.
        /// </summary>
        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Binds the texture to the current graphic context.
        /// </summary>
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _index);
        }

        /// <summary>
        /// Releases the texture.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteTexture(_index);
            _index = 0;
        }

        [SecuritySafeCritical]
        public void Update(Bitmap bitmap, Rectangle rectangle)
        {
            if (_index != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, _index);

                BitmapData bitmapData = null;

                try
                {
                    // We use lockbits, that's why we mark SecurityPermission
                    bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    {
                        GL.BindTexture(TextureTarget.Texture2D, _index);
                        GL.TexSubImage2D(TextureTarget.Texture2D, 0, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
                    }
                }
                finally
                {
                    if (bitmapData != null)
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }
            }
            else
            {
                throw new ObjectDisposedException("The texture has been disposed.");
            }
        }

        [SecuritySafeCritical]
        private static int LoadTexture(Bitmap bitmap, Rectangle rectangle, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            var texture = new int[1];

            GL.GenTextures(1, texture);
            GL.BindTexture(TextureTarget.Texture2D, texture[0]);

            BitmapData bitmapData = null;

            try
            {
                // We use lockbits, that's why we mark SecurityPermission
                bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                {
                    GL.BindTexture(TextureTarget.Texture2D, texture[0]);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, rectangle.Width, rectangle.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
                }
            }
            finally
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }

            return texture[0];
        }
    }
}