using System.Drawing;

namespace Arleen.Rendering
{
    public struct RenderInfo
    {
        public Rectangle ClipArea { get; set; }

        public double ElapsedMilliseconds { get; set; }

        public int Fps { get; set; }
    }
}