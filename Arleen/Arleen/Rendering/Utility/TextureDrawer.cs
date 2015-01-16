using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Arleen.Rendering.Utility
{
    public static class TextureDrawer
    {
        public static void DrawTexture(Texture texture, Color color)
        {
            if (texture != null)
            {
                GL.Enable(EnableCap.Texture2D);
                texture.Bind();

                GL.Color4(color);

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(0, 0, 0);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(texture.Width, 0, 0);

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(texture.Width, texture.Height, 0);

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(0, texture.Height, 0);
                }
                GL.End();
            }
        }
    }
}