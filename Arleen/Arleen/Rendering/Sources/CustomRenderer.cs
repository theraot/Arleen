using Arleen.Geometry;
using System;
using System.Security.Permissions;

namespace Arleen.Rendering.Sources
{
    public class CustomRenderer : RenderSource, ILocable
    {
        private readonly Action _initialize;
        private readonly Location _location;
        private readonly Action _render;

        public CustomRenderer(Action render, Action initialize)
        {
            _render = render ?? NoOp;
            _initialize = initialize ?? NoOp;
            _location = new Location();
        }

        public CustomRenderer(Action render)
        {
            _render = render ?? NoOp;
            _initialize = NoOp;
            _location = new Location();
        }

        public Location Location
        {
            get
            {
                return _location;
            }
        }

        protected override void OnInitilaize()
        {
            _initialize();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void OnRender()
        {
            _render();
        }

        private static void NoOp()
        {
            // Empty
        }
    }
}