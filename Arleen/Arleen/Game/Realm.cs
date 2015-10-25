using Arleen.Rendering;
using System;
using System.Diagnostics;

namespace Arleen.Game
{
    public abstract class Realm : MarshalByRefObject
    {
        private readonly Stopwatch _time = new Stopwatch();

        public double TotalTime
        {
            get
            {
                return _time.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            }
        }

        internal void RestartTime()
        {
            _time.Stop();
            _time.Start();
        }

        protected internal virtual bool Closing()
        {
            return true;
        }

        protected internal virtual Scene Load()
        {
            return null;
        }

        protected internal virtual void Resize()
        {
            // Empty
        }

        protected internal virtual void Unload()
        {
            // Empty
        }

        protected internal virtual void UpdateFrame(RenderInfo renderInfo)
        {
            // Empty
            GC.KeepAlive(renderInfo);
        }
    }
}