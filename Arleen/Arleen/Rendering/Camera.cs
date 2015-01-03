using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Arleen.Rendering
{
    public sealed class Camera
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

        public static void PlaceDefaultLocation()
        {
            GL.LoadIdentity();
        }

        public void Place(Location.PlaceMode mode)
        {
            ViewingVolume.Place();
            Matrix4d matrix = Matrix4d.Identity;
            matrix = Location.Apply(matrix, mode);
            GL.LoadMatrix(ref matrix);
        }
    }
}