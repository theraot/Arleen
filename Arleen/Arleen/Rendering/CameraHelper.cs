using Arleen.Geometry;

namespace Arleen.Rendering
{
    public static class CameraHelper
    {
        public static void Place(this Camera camera, Location.Mode mode)
        {
            camera.ViewingVolume.Place();
            camera.Location.PlaceInverted(mode);
        }
    }
}