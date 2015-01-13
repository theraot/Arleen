using System;

namespace Arleen.Rendering
{
    /// <summary>
    /// Counts the number of frames per second
    /// </summary>
    internal sealed class FpsCounter
    {
        private int _fpsCount;
        private int _fpsLast;
        private double time;

        /// <summary>
        /// The last recorded number of frames per second.
        /// </summary>
        public int Fps
        {
            get
            {
                return _fpsLast;
            }
        }

        /// <summary>
        /// Adds a frame to the count, and verifies if a second has passed.
        /// </summary>
        /// <param name="elapsedSeconds">The fraction of seconds that has passed since the last call.</param>
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