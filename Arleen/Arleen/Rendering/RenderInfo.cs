using System.Drawing;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents the information about the context of a render call.
    /// </summary>
    public struct RenderInfo
    {
        /// <summary>
        /// The camera to which to render.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// The rectangle to which to render, in pixel = 1.
        /// </summary>
        public Rectangle ClipArea { get; set; }

        /// <summary>
        /// The time since the last Renderer iteration.
        /// </summary>
        public double ElapsedMilliseconds { get; set; }

        /// <summary>
        /// The number of frames rendered in the past second.
        /// </summary>
        public int Fps { get; set; }
    }
}