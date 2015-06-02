using System;
using System.Collections.Generic;

namespace Arleen.Rendering
{
    public class Scene : MarshalByRefObject
    {
        private readonly List<RenderTarget> _renderTargets;

        public Scene()
        {
            _renderTargets = new List<RenderTarget>();
        }

        public List<RenderTarget> RenderTargets
        {
            get
            {
                return _renderTargets;
            }
        }
    }
}