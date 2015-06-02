﻿using Arleen.Geometry;

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
    }
}