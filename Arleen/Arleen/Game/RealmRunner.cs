using System.ComponentModel;
using OpenTK;
using System;
using Arleen.Rendering;

namespace Arleen.Game
{
    public sealed class RealmRunner : GameWindow
    {
        private Realm _currentRealm;

        private Renderer _renderer;

        public RealmRunner()
            : this(Engine.Configuration)
        {
            // Empty
        }

        // ===
        // TODO: world
        // ===
        // ---
        // TODO: Input
        // ---
        // TODO: User Interface
        // ---
        // TODO: Audio
        // ---
        // TODO: Physics
        // ---
        public RealmRunner(Configuration configuration)
            : base(configuration.Resolution.Width, configuration.Resolution.Height)
        {
            Title = configuration.DisplayName;
            Logbook.Instance.Trace (System.Diagnostics.TraceEventType.Information, "New RealmRunner created in {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public Realm CurrentRealm
        {
            get
            {
                return _currentRealm;
            }
            set
            {
                if (_currentRealm != value)
                {
                    if (_currentRealm != null)
                    {
                        _renderer = null;
                        //_renderer.Ter TODO
                    }
                    _currentRealm = value;
                    if (_currentRealm != null)
                    {
                        Run ();
                    }
                }
            }
        }
        public Renderer Renderer
        {
            get
            {
                return _renderer;
            }
        }

        protected override void OnLoad (EventArgs e)
        {
            var realm = _currentRealm;
            if (realm != null)
            {
                var scene = realm.Load ();
                realm.RestartTime ();
                if (scene != null)
                {
                    _renderer = new Renderer (scene);
                    _renderer.Initialize (this, _currentRealm);
                }
            }
            base.OnLoad (e);
        }

        protected override void OnClosing (CancelEventArgs e)
        {
            var realm = _currentRealm;
            if (realm != null)
            {
                e.Cancel = !realm.Closing ();
            }
            base.OnClosing (e);
        }

        protected override void OnResize (EventArgs e)
        {
            var realm = _currentRealm;
            if (realm != null)
            {
                realm.Resize ();
            }
            base.OnResize (e);
        }

        protected override void OnUnload (EventArgs e)
        {
            var realm = _currentRealm;
            if (realm != null)
            {
                realm.Unload ();
            }
            base.OnUnload (e);
        }

        protected override void OnUpdateFrame (FrameEventArgs e)
        {
            var realm = _currentRealm;
            if (realm != null)
            {
                realm.UpdateFrame (RenderInfo.Current);
            }
            base.OnUpdateFrame (e);
        }
    }
}