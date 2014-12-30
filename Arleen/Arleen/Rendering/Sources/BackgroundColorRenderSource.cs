using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Arleen.Rendering.Sources
{
    public class BackgroundColorRenderSource : RenderSource
    {
        private readonly Color _color;
        private readonly double _depth;

        public BackgroundColorRenderSource(Color color, double depth)
        {
            _color = color;
            _depth = depth;
        }

        protected override void OnInitilaize()
        {
            GL.ClearColor(_color);
            GL.ClearDepth(_depth);
        }

        protected override void OnRender(Rectangle clipArea, double time)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}