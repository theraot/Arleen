using System;
using System.ComponentModel;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Security;

namespace Arleen.Rendering.Sources
{
    /// <summary>
    /// Clears the screen leaving an uniform color background.
    /// </summary>
    public class BackgroundColorRenderSource : RenderSource
    {
        private readonly Color _color;
        private readonly double _depth;

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public BackgroundColorRenderSource(Color color, double depth)
        {
            _color = color;
            _depth = depth;
        }

        public static BackgroundColorRenderSource Create(Color color, double depth)
        {
            return Facade.Create<BackgroundColorRenderSource>(color, depth);
        }

        protected override void OnInitilaize()
        {
            GL.ClearColor(_color);
            GL.ClearDepth(_depth);
        }

        [SecuritySafeCritical]
        protected override void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}