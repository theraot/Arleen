using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public sealed class Camera : ILocable
    {
        public Camera(ViewingVolume volume)
        {
            ViewingVolume = volume;
            Location = new Location();
        }

        public Camera(ViewingVolume volume, Location location)
        {
            ViewingVolume = volume;
            Location = location;
        }

        public Location Location { get; set; }

        public ViewingVolume ViewingVolume { get; set; }

        public void Place(Location.Mode mode)
        {
            ViewingVolume.Place();
            var matrix = Location.GetMatrix(mode);
            matrix.Invert();
            GL.LoadMatrix(ref matrix);
        }

        public void PlaceLocation(Location.Mode mode)
        {
            var matrix = Location.GetMatrix(mode);
            matrix.Invert();
            GL.LoadMatrix(ref matrix);
        }
    }
}