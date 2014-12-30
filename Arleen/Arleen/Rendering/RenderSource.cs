using System.Drawing;

namespace Arleen.Rendering
{
    public abstract class RenderSource
    {
        private bool _enabled;
        private bool _initialized;

        protected RenderSource()
        {
            _enabled = true;
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public void Render(Camera camera, Rectangle clipArea, double time)
        {
            if (_enabled)
            {
                if (!_initialized)
                {
                    OnInitilaize();
                    _initialized = true;
                }
                OnRender(camera, clipArea, time);
            }
        }

        protected virtual void OnInitilaize()
        {
            //Empty
        }

        protected abstract void OnRender(Camera camera, Rectangle clipArea, double time);
    }
}