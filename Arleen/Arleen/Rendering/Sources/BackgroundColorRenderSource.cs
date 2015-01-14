using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Security.Permissions;

namespace Arleen.Rendering.Sources
{
    /// <summary>
    /// Clears the screen leaving an uniform color background.
    /// </summary>
    public class BackgroundColorRenderSource : RenderSource
    {
        private readonly Color _color;
        private readonly double _depth;

        /// <summary>
        /// Creates a new instance of BackgroundColorRenderSource.
        /// </summary>
        /// <param name="color">The background color to use.</param>
        /// <param name="depth">The depth value of the background.</param>
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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void OnRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}