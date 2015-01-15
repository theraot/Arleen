using Arleen.Geometry;

namespace Arleen.Rendering.Sources
{
    public interface ICameraRelative
    {
        Location.Mode CameraPlaceMode { get; }
    }
}