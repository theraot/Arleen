using Arleen.Geometry;
using System.Drawing;
using System;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents the information about the context of a render call.
    /// </summary>
    [Serializable]
    public struct RenderInfo
    {
        public static RenderInfo Current;

        /// <summary>
        /// The time since the last Renderer iteration.
        /// </summary>
        public double ElapsedMilliseconds { get; set; }

        /// <summary>
        /// The number of frames rendered in the past second.
        /// </summary>
        public int Fps { get; set; }

        /// <summary>
        /// The size of the are to which to render, in pixel = 1.
        /// </summary>
        public Size SurfaceSize { get; set; }
    }
}