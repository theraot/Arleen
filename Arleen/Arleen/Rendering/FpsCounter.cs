using System;

namespace Arleen.Rendering
{
    internal sealed class FpsCounter
    {
        private int _fpsCount;
        private int _fpsLast;
        private double time;

        public int Fps
        {
            get
            {
                return _fpsLast;
            }
        }

        public void OnRender(double elapsedSeconds)
        {
            _fpsCount++;
            time += elapsedSeconds;
            if (time >= 1)
            {
                _fpsLast = _fpsCount;
                _fpsCount = 0;
                time -= Math.Floor(time);
            }
        }
    }
}