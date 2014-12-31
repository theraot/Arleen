using System;

namespace Arleen.Rendering.Sources
{
    public class CustomRenderer : RenderSource
    {
        private readonly Action _initialize;
        private readonly Action<RenderInfo> _render;

        public CustomRenderer(Action<RenderInfo> render, Action initialize)
        {
            _render = render ??
                (
                    _ =>
                    {
                        //Empty
                    }
                );
            _initialize = initialize ??
                (
                    () =>
                    {
                        //Empty
                    }
                );
        }

        public CustomRenderer(Action<RenderInfo> render)
        {
            _render = render ??
                (
                    _ =>
                    {
                        //Empty
                    }
                );
            _initialize =
                () =>
                {
                    //Empty
                };
        }

        protected override void OnInitilaize()
        {
            _initialize();
        }

        protected override void OnRender(RenderInfo renderInfo)
        {
            _render(renderInfo);
        }
    }
}