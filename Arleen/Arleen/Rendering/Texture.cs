using Arleen.Rendering.Sources;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Arleen.Rendering
{
    public class Texture
    {
        private readonly int _height;
        private readonly int _index;
        private readonly int _width;

        public Texture(Bitmap bitmap)
        {
            try
            {
                _index = RenderSourceHelper.LoadTexture(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), (int)TextureMinFilter.Nearest, (int)TextureMagFilter.Nearest);
                _width = bitmap.Width;
                _height = bitmap.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        public Texture(Bitmap bitmap, Rectangle rectangle)
        {
            try
            {
                _index = RenderSourceHelper.LoadTexture(bitmap, rectangle, (int)TextureMinFilter.Nearest, (int)TextureMagFilter.Nearest);
                _width = rectangle.Width;
                _height = rectangle.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        public Texture(Bitmap bitmap, int minFilter, int magFilter)
        {
            try
            {
                _index = RenderSourceHelper.LoadTexture(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), minFilter, magFilter);
                _width = bitmap.Width;
                _height = bitmap.Height;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid resource.");
            }
        }

        public Texture(Bitmap bitmap, Rectangle rectangle, int minFilter, int magFilter)
        {
            try
            {
                _index = RenderSourceHelper.LoadTexture(bitmap, rectangle, minFilter, magFilter);
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
    }
}