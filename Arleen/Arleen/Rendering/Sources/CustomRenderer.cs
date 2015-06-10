using Arleen.Geometry;
using System;
using System.ComponentModel;
using System.Security;

namespace Arleen.Rendering.Sources
{
    public class CustomRenderer : RenderSource, ILocable
    {
        private readonly Action _initialize;
        private readonly Location _location;
        private readonly Action _render;

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public CustomRenderer(Action render, Action initialize)
        {
            _render = render ?? NoOp;
            _initialize = initialize ?? NoOp;
            _location = new Location();
        }

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
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

        public static CustomRenderer Create(Action render, Action initialize)
        {
            // TODO test
            return Facade.Create<CustomRenderer>(render, initialize);
        }

        public static CustomRenderer Create(Action render)
        {
            // TODO test
            return Facade.Create<CustomRenderer>(render);
        }

        protected override void OnInitilaize()
        {
            _initialize();
        }

        [SecuritySafeCritical]
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