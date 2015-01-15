using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Arleen.Rendering.Utility
{
    public static class TextureDrawer
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

        public static void DrawTexture(Texture texture, Color color, Location location)
        {
            if (texture != null)
            {
                GL.Enable(EnableCap.Texture2D);
                texture.Bind();
                Vector3d x = Vector3d.Transform(new Vector3d(texture.Width, 0, 0), location.MatrixOrientation);
                Vector3d y = Vector3d.Transform(new Vector3d(0, texture.Height, 0), location.MatrixOrientation);

                GL.Color4(color);

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(location.Position);

                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(location.Position + x);

                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(location.Position + x + y);

                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(location.Position + y);
                }
                GL.End();
            }
        }
    }
}