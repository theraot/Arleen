namespace Arleen.Rendering
{
    public sealed class Camera
    {
        private Location _location;
        private ViewingVolume _volume;

        public Camera(ViewingVolume volume)
        {
            _volume = volume;
            _location = new Location();
        }

        public Camera(ViewingVolume volume, Location location)
        {
            _volume = volume;
            _location = location;
        }

        public Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        public ViewingVolume ViewingVolume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
            }
        }

        public void Place(Location.PlaceMode mode)
        {
            _volume.Place();
            _location.Place(mode);
        }
    }
}