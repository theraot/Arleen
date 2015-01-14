using System;
using System.Security.Permissions;

namespace Arleen.Rendering.Sources
{
    public class CustomRenderer : RenderSource
    {
        private readonly Action _initialize;
        private readonly Action _render;

        public CustomRenderer(Action render, Action initialize)
        {
            _render = render ?? NoOp;
            _initialize = initialize ?? NoOp;
        }

        public CustomRenderer(Action render)
        {
            _render = render ?? NoOp;
            _initialize = NoOp;
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