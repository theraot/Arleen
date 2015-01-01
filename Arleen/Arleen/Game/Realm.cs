using OpenTK;
using System;
using System.Diagnostics;

namespace Arleen.Game
{
    public class Realm : GameWindow
    {
        private readonly Stopwatch _time = new Stopwatch();

        public Realm()
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
        private Realm(Configuration configuration)
            : base(configuration.Resolution.Width, configuration.Resolution.Height)
        {
            Title = Program.DisplayName;
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