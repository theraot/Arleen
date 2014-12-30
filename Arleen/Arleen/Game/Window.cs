﻿using Arleen.Rendering;
using Arleen.Rendering.Sources;
using OpenTK;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Arleen.Game
{
    public sealed class Window : GameWindow
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private readonly Renderer _renderer;
        private readonly Stopwatch _time = new Stopwatch();

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
            var _camera = new Camera
                (
                    new ViewingVolume.Perspective
                    {
                        FieldOfView = 45,
                        FarPlane = FLT_FarPlane,
                        NearPlane = FLT_NearPlane,
                        AspectRatio = 1
                    }
                );
            _renderer = new Renderer(new Rectangle(0, 0, Width, Height));
            /*_renderer.RenderSources.Add
                (
                    new BackgroundColorRenderSource(Color.LightSkyBlue, 1.0)
                );*/
            _renderer.RenderSources.Add
                (
                    new SkyboxRenderer()
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0, 0, 1.0f, 1.0f),
                            _camera
                        )
                );
            _time.Start();
        }

        public double TotalTime
        {
            get
            {
                return _time.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            }
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
    }
}