using Arleen.Geometry;
using Arleen.Rendering.Sources;
using OpenTK.Graphics.OpenGL;
using System;
using System.Security.Permissions;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents something that will be systematically rendered.
    /// </summary>
    public abstract class RenderSource : MarshalByRefObject, IRenderable
    {
        private bool _enabled;
        private bool _initialized;

        /// <summary>
        /// Creates a new instance of RenderSource.
        /// </summary>
        protected RenderSource()
        {
            if (Engine.AppDomain != AppDomain.CurrentDomain)
            {
                throw new InvalidOperationException(string.Format("Invalid AppDomain {0} - should be {1}", AppDomain.CurrentDomain.FriendlyName, Engine.AppDomain.FriendlyName));
            }
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
        /// Request this render source to produce its output.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void Render()
        {
            if (_enabled)
            {
                if (!_initialized)
                {
                    OnInitilaize();
                    _initialized = true;
                }
                if (this is ICameraRelative)
                {
                    GL.LoadIdentity();
                }
                else
                {
                    RenderTarget.Current.Camera.Place(Location.Mode.All);
                }
                var locable = this as ILocable;
                if (locable != null)
                {
                    locable.Location.Apply(Location.Mode.All);
                }
                OnRender();
            }
        }

        /// <summary>
        /// Performs the initialization of the current RenderSource.
        /// </summary>
        protected virtual void OnInitilaize()
        {
            // Empty
        }

        /// <summary>
        /// Performs the output of the current RenderSource.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected abstract void OnRender();
    }
}