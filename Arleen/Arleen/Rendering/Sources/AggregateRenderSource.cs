using System;
using System.Collections.Generic;
using System.Linq;

namespace Arleen.Rendering.Sources
{
    public sealed class AggregateRenderSource : RenderSource, IDisposable
    {
        private readonly IList<IRenderable> _renderSources;

        public AggregateRenderSource()
        {
            _renderSources = new List<IRenderable>();
        }

        public AggregateRenderSource(bool enabled)
        {
            Enabled = enabled;
            _renderSources = new List<IRenderable>();
        }

        public AggregateRenderSource(IList<IRenderable> renderSources)
        {
            if (renderSources == null)
            {
                throw new ArgumentNullException("renderSources");
            }
            _renderSources = renderSources;
        }

        public AggregateRenderSource(bool enabled, IList<IRenderable> renderSources)
        {
            if (renderSources == null)
            {
                throw new ArgumentNullException("renderSources");
            }
            Enabled = enabled;
            _renderSources = renderSources;
        }

        public IList<IRenderable> RenderSources
        {
            get
            {
                return _renderSources;
            }
        }

        public void Dispose()
        {
            var sources = _renderSources;
            foreach (var source in sources.OfType<IDisposable>())
            {
                source.Dispose();
            }
            _renderSources.Clear();
        }

        protected override void OnRender()
        {
            foreach (var item in _renderSources)
            {
                item.Render();
            }
        }
    }
}