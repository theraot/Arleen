using Arleen.Rendering;
using OpenTK;
using System;

namespace Arleen.Game
{
    public class Window : GameWindow
    {
        private readonly Renderer _renderer;

        public Window()
            : this(Program.Configuration)
        {
            // Empty
        }

        // ===
        // TODO: world
        // ===
        // ---
        // TODO: Localization
        // ---
        // TODO: Input
        // ---
        // TODO: Resource loading
        // ---
        // TODO: User Interface
        // ---
        // TODO: Audio
        // ---
        // TODO: Physics
        // ---
        private Window(Configuration configuration)
            : base(configuration.Resolution.Width, configuration.Resolution.Height)
        {
            VSync = configuration.VSync ? VSyncMode.On : VSyncMode.Off;
            Title = Program.DisplayName;
            _renderer = new Renderer();
        }

        protected override void OnLoad(EventArgs e)
        {
            // ---
            // TODO: Bind input
            // ---
            // ---
            // TODO: Load localization
            // ---

            _renderer.Initialize(this);

            // ---
            // TODO: Load resources
            // ---
            // TODO: Load world
            // ---
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            SwapBuffers();
        }
    }
}