using System;
using System.Collections.Generic;

namespace Arleen.Rendering.Sources
{
    public class AggregateRenderSource : RenderSource
    {
        private readonly IList<RenderSource> _renderSources;

        public AggregateRenderSource()
        {
            _renderSources = new List<RenderSource>();
        }

        public AggregateRenderSource(bool enabled)
        {
            Enabled = enabled;
            _renderSources = new List<RenderSource>();
        }

        public AggregateRenderSource(IList<RenderSource> renderSources)
        {
            if (renderSources == null)
            {
                throw new ArgumentNullException("renderSources");
            }
            _renderSources = renderSources;
        }

        public AggregateRenderSource(bool enabled, IList<RenderSource> renderSources)
        {
            if (renderSources == null)
            {
                throw new ArgumentNullException("renderSources");
            }
            Enabled = enabled;
            _renderSources = renderSources;
        }

        public IList<RenderSource> RenderSources
        {
            get
            {
                return _renderSources;
            }
        }

        private void Release()
        {
            var sources = _renderSources;
            foreach (var source in sources)
            {
                if (source is IDisposable)
                {
                    (source as IDisposable).Dispose();
                }
            }
            _renderSources.Clear();
        }

        protected override void OnRender(RenderInfo renderInfo)
        {
            foreach (var item in _renderSources)
            {
                item.Render(renderInfo);
            }
        }
    }
}