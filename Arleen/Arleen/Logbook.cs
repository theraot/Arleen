﻿using System;
using System.Diagnostics;

namespace Arleen
{
    /// <summary>
    /// The main façade for message logging.
    /// </summary>
    /// <remarks>Logbook is a singleton.
    /// These are the reasons for this:
    /// A) the responsibility of creating Logbook belongs to Program
    /// B) Logbook should not be responsible of getting the configuration to create itself.
    /// C) There should be only one Logbook per AppDomain. </remarks>
    public class Logbook
    {
        private readonly TraceSource _logSource;

        private Logbook(SourceLevels level, bool allowDefaultListener, TraceListener[] listeners)
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

        /// <summary>
        /// Gets the existing instance of Logbook.
        /// </summary>
        /// <remarks>This may be null during initialization. </remarks>
        public static Logbook Instance { get; private set; }

        /// <summary>
        /// Adds a new listener to the Logbook.
        /// </summary>
        /// <param name="listener">The new listener to be added.</param>
        /// <returns>true if the listener was added, false otherwise.</returns>
        public bool AddListener(TraceListener listener)
        {
            if (Instance == null)
            {
                return false;
            }
            return Instance.AddListener(listener);
        }

        /// <summary>
        /// Writes a message to the logs.
        /// </summary>
        /// <param name="eventType">The relevance of the message.</param>
        /// <param name="format">A composite format string (Composite Formatting) that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public void Trace(TraceEventType eventType, string format, params object[] args)
        {
            _logSource.TraceEvent(eventType, 0, UtcNowIsoFormat() + " " + format, args);
        }

        /// <summary>
        /// Writes a message to the logs.
        /// </summary>
        /// <param name="eventType">The relevance of the message.</param>
        /// <param name="message">The message to write.</param>
        public void Trace(TraceEventType eventType, string message)
        {
            _logSource.TraceEvent(eventType, 0, UtcNowIsoFormat() + " " + message);
        }

        /// <summary>
        /// Creates a new instance of Logbook if no previous instance is available. Returns the existing (newly created or not) instance.
        /// </summary>
        /// <param name="level">The level for the messages that will be recorded.</param>
        /// <param name="allowDefaultListener">indicated whatever the default listener should be kept or not.</param>
        /// <param name="listeners">An array of TraceListener that will be used for the Logbook.</param>
        /// <returns>The working instance of Logbook.</returns>
        internal static Logbook Initialize(SourceLevels level, bool allowDefaultListener, TraceListener[] listeners)
        {
            // This should be called during initialization.
            // Double initialization is posible if multiple threads attemps to create the logbook...
            // Since that should not happen, let's accept the garbage if somehow that comes to be.
            if (Instance != null)
            {
                return Instance;
            }
            return Instance = new Logbook(level, allowDefaultListener, listeners);
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