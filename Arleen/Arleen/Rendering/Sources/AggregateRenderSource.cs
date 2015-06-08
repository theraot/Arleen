using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Arleen.Rendering.Sources
{
    public sealed class AggregateRenderSource : RenderSource, IDisposable
    {
        private readonly IList<RenderSource> _renderSources;

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public AggregateRenderSource()
        {
            _renderSources = new List<RenderSource>();
        }

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public AggregateRenderSource(bool enabled)
        {
            Enabled = enabled;
            _renderSources = new List<RenderSource>();
        }

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public AggregateRenderSource(IList<RenderSource> renderSources)
        {
            if (renderSources == null)
            {
                throw new ArgumentNullException("renderSources");
            }
            _renderSources = renderSources;
        }

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
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

        public static AggregateRenderSource Create()
        {
            return Facade.Create<AggregateRenderSource>();
        }

        public static AggregateRenderSource Create(bool enabled)
        {
            return Facade.Create<AggregateRenderSource>(enabled);
        }

        public static AggregateRenderSource Create(IList<RenderSource> renderSources)
        {
            return Facade.Create<AggregateRenderSource>(renderSources);
        }

        public static AggregateRenderSource Create(bool enabled, IList<RenderSource> renderSources)
        {
            return Facade.Create<AggregateRenderSource>(enabled, renderSources);
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