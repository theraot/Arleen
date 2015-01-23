using OpenTK;
using System;
using System.Diagnostics;

namespace Arleen.Game
{
    public abstract class Realm : GameWindow
    {
        private readonly Stopwatch _time = new Stopwatch();

        protected Realm()
            : this(Program.Configuration)
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
        protected Realm(Configuration configuration)
            : base(configuration.Resolution.Width, configuration.Resolution.Height)
        {
            Title = configuration.DisplayName;
            _time.Start();
        }

        public double TotalTime
        {
            get
            {
                return _time.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            }
        }
    }
}