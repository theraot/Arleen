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
                GL.Scale(texture.Width, texture.Height, 0);
                GL.Enable(EnableCap.Texture2D);
                texture.Bind();

                GL.Color4(color);

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(0, 0, 0);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(1, 0, 0);

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(1, 1, 0);

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(0, 1, 0);
                }
                GL.End();
            }
        }
    }
}