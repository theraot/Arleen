using System;
using System.Diagnostics;

namespace Arleen
{
    public class Logbook
    {
        private readonly TraceSource _logSource;

        internal Logbook(SourceLevels level, bool allowDefaultListener, TraceListener[] listeners)
        {
            var displayName = Program.DisplayName;
            _logSource = new TraceSource(displayName)
            {
                Switch = new SourceSwitch(displayName)
                {
                    Level = level
                }
            };
            if (!allowDefaultListener)
            {
                _logSource.Listeners.Clear();
            }
            _logSource.Listeners.AddRange(listeners);
        }

        public void Trace(TraceEventType eventType, string format, params object[] args)
        {
            _logSource.TraceEvent(eventType, 0, UtcNowIsoFormat() + " " + format, args);
        }

        public void Trace(TraceEventType eventType, string message)
        {
            _logSource.TraceEvent(eventType, 0, UtcNowIsoFormat() + " " + message);
        }

        private static string UtcNowIsoFormat()
        {
            // UtcTime to miliseconds presition.
            // using Z to denote Zero offset.
            // No, that's not the time zone of zulu people.
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff'Z'");
        }
    }
}