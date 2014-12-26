using OpenTK;
using System;

namespace Arleen.Game
{
    public class Window : GameWindow
    {
        // ---
        // TODO: Camera Projection
        // ---
        // TODO: Renderer
        // ---
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

        public Window()
            : this(Program.Configuration)
        {
            // Empty
        }

        private Window(Configuration configuration)
            : base(configuration.Resolution.Width, configuration.Resolution.Height)
        {
            VSync = configuration.VSync ? VSyncMode.On : VSyncMode.Off;
            Title = Program.DisplayName;
        }

        protected override void OnLoad(EventArgs e)
        {
            // ---
            // TODO: Bind input
            // ---
            // ---
            // TODO: Load localization
            // ---
            // TODO: Initialize renderer
            // ---
            // TODO: Load resources
            // ---
            // TODO: Load world
            // ---
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // TODO: Call renderer
        }

        protected override void OnResize(EventArgs e)
        {
            // TODO: Notify renderer
        }
    }
}