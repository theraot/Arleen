using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;

namespace Arleen.Rendering
{
    public sealed class Texture : IDisposable
    {
        private readonly int _height;
        private readonly int _index;
        private readonly int _width;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Texture(Bitmap bitmap)
        {
            try
            {
                _index = LoadTexture(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), (int)TextureMinFilter.Nearest, (int)TextureMagFilter.Nearest);
                _width = bitmap.Width;
                _height = bitmap.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Texture(Bitmap bitmap, Rectangle rectangle)
        {
            try
            {
                _index = LoadTexture(bitmap, rectangle, (int)TextureMinFilter.Nearest, (int)TextureMagFilter.Nearest);
                _width = rectangle.Width;
                _height = rectangle.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Texture(Bitmap bitmap, int minFilter, int magFilter)
        {
            try
            {
                _index = LoadTexture(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), minFilter, magFilter);
                _width = bitmap.Width;
                _height = bitmap.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Texture(Bitmap bitmap, Rectangle rectangle, int minFilter, int magFilter)
        {
            try
            {
                _index = LoadTexture(bitmap, rectangle, minFilter, magFilter);
                _width = rectangle.Width;
                _height = rectangle.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _index);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_index);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static int LoadTexture(Bitmap bitmap, Rectangle rectangle, int minFilter, int magFilter)
        {
            var texture = new int[1];

            GL.GenTextures(1, texture);
            GL.BindTexture(TextureTarget.Texture2D, texture[0]);

            BitmapData bitmapData = null;

            try
            {
                bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                {
                    GL.BindTexture(TextureTarget.Texture2D, texture[0]);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, rectangle.Width, rectangle.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, magFilter);
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