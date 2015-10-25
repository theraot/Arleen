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

                const float A = 0.0f;
                const float B = 1.0f;
                var data = new[] {
                    // VEXTEX   // TEXTURES
                    // 0
                    A, A, A,    A, A,
                    B, A, A,    B, A,
                    // 2
                    B, B, A,    B, B,
                    A, B, A,    A, B
                };
                var indexes = new byte[] {
                    0, 1, 2, 3
                };
                using (var mesh = new Mesh(Mesh.VextexInfo.Position | Mesh.VextexInfo.Texture, data, PrimitiveType.Quads, indexes))
                {
                    mesh.Render();
                }
            }
        }
    }
}