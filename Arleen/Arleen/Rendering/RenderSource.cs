namespace Arleen.Rendering
{
    /// <summary>
    /// Represents something that will be systematically rendered.
    /// </summary>
    public abstract class RenderSource
    {
        private bool _enabled;
        private bool _initialized;

        /// <summary>
        /// Creates a new instance of RenderSource.
        /// </summary>
        protected RenderSource()
        {
            _enabled = true;
        }

        /// <summary>
        /// Gets or set whatever this RenderSource will produce output.
        /// </summary>
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

        /// <summary>
        /// Gets whatever the initialization of this RenderSource has been executed.
        /// </summary>
        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        /// <summary>
        /// Request this render source to produce its output.
        /// </summary>
        /// <param name="renderInfo">Information about the context in which this operation is called.</param>
        public void Render(RenderInfo renderInfo)
        {
            if (_enabled)
            {
                if (!_initialized)
                {
                    OnInitilaize();
                    _initialized = true;
                }
                OnRender(renderInfo);
            }
        }

        /// <summary>
        /// Performs the initialization of the current RenderSource.
        /// </summary>
        protected virtual void OnInitilaize()
        {
            //Empty
        }

        /// <summary>
        /// Performs the output of the current RenderSource.
        /// </summary>
        /// <param name="renderInfo"></param>
        protected abstract void OnRender(RenderInfo renderInfo);
    }
}