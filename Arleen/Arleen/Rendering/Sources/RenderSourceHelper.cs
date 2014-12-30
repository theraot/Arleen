using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace Arleen.Rendering.Sources
{
    internal static class RenderSourceHelper
    {
        public static int LoadTexture(Bitmap bitmap, Rectangle rectangle, int minFilter, int magFilter)
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