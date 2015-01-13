using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Arleen.Rendering.Utility
{
    public class TextureDrawer
    {
        public static void DrawTexture(Texture texture, Color color, double left, double top)
        {
            if (texture != null)
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
    }
}