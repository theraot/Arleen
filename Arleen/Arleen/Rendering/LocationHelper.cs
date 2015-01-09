using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public static class LocationHelper
    {
        public static void Place(this Location location, Location.PlaceMode mode)
        {
            var matrix = location.GetMatrix(mode);
            GL.LoadMatrix(ref matrix);
        }

        public static void Apply(this Location location, Location.PlaceMode mode)
        {
            var matrix = location.GetMatrix(mode);
            GL.MultMatrix(ref matrix);
        }
    }
}