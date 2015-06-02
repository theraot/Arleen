using System.ComponentModel;
using OpenTK;
using System;
using Arleen.Rendering;
using System.Security;

namespace Arleen.Game
{
    public sealed class RealmRunner : GameWindow
    {
        private Realm _currentRealm;
        private bool _isClosed;

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
            Logbook.Instance.Trace(System.Diagnostics.TraceEventType.Information, "New RealmRunner created in {0}", AppDomain.CurrentDomain.FriendlyName);
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
                    }
                    _currentRealm = value;
                    if (_currentRealm != null)
                    {
                        Run();
                    }
                }
            }
        }

        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }
        public Renderer Renderer
        {
            get
            {
                return _renderer;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _isClosed = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                var realm = _currentRealm;
                if (realm != null)
                {
                    e.Cancel = !realm.Closing();
                }
                base.OnClosing(e);
            }
            catch (SecurityException exception)
            {
                Logbook.Instance.ReportException(exception, true);
                Close();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
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
            catch (SecurityException exception)
            {
                Logbook.Instance.ReportException(exception, true);
                Close ();
            }
        }
        protected override void OnResize (EventArgs e)
        {
            try
            {
                var realm = _currentRealm;
                if (realm != null)
                {
                    realm.Resize ();
                }
                base.OnResize (e);
            }
            catch (SecurityException exception)
            {
                Logbook.Instance.ReportException(exception, true);
                Close ();
            }
        }

        protected override void OnUnload (EventArgs e)
        {
            try
            {
                var realm = _currentRealm;
                if (realm != null)
                {
                    realm.Unload ();
                }
                base.OnUnload (e);
            }
            catch (SecurityException exception)
            {
                Logbook.Instance.ReportException(exception, true);
                Close ();
            }
        }

        protected override void OnUpdateFrame (FrameEventArgs e)
        {
            try
            {
                var realm = _currentRealm;
                if (realm != null)
                {
                    realm.UpdateFrame (RenderInfo.Current);
                }
                base.OnUpdateFrame (e);
            }
            catch (SecurityException exception)
            {
                Logbook.Instance.ReportException(exception, true);
                Close ();
            }
        }
    }
}