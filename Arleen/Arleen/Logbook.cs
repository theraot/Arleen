using System;
using System.Diagnostics;
using System.Security.Permissions;

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
        private static Logbook _instance;
        private readonly TraceSource _logSource;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private Logbook(SourceLevels level, bool allowDefaultListener)
        {
            var displayName = Engine.InternalName;
            // TODO: this fails if running from Visaul Studio
            _logSource = new TraceSource(displayName) {
                Switch = new SourceSwitch(displayName) {
                    Level = level
                }
            };
            if (!allowDefaultListener)
            {
                _logSource.Listeners.Clear();
            }
        }

        /// <summary>
        /// Gets the existing instance of Logbook.
        /// </summary>
        /// <remarks>This may be null during initialization. </remarks>
        public static Logbook Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Engine initialization is not done");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Adds a new listener to the Logbook.
        /// </summary>
        /// <param name="listener">The new listener to be added.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void AddListener(TraceListener listener)
        {
            _logSource.Listeners.Add(listener);
        }

        /// <summary>
        /// Reports the occurrence of an exception.
        /// </summary>
        /// <param name="exception">The exception to report.</param>
        /// <param name="situation">A description of the situation when the exception happened. (Describe what was being attempted)</param>
        /// <param name="severe">true to indicated this exception will reduce functionality or require extra steps for fixing, false to indicate the program is designed to recover from this.</param>
        /// <remarks>If severe is set to false, the stack trace will not be included.</remarks>
        public void ReportException(Exception exception, string situation, bool severe)
        {
            if (severe)
            {
                var extendedStackTrace = Environment.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Trace
                    (
                    TraceEventType.Error,
                    "\n\n{0} ocurred while {1}. \n\n == Exception Report == \n\n{2}\n\n == Source == \n\n{3}\n\n == AppDomain == \n\n{4}\n\n == Stacktrace == \n\n{5}\n\n == Extended Stacktrace == \n\n{6}\n",
                    exception.GetType().Name,
                    situation,
                    exception.Message,
                    exception.Source,
                    AppDomain.CurrentDomain.FriendlyName,
                    exception.StackTrace,
                    string.Join("\r\n", extendedStackTrace, 4, extendedStackTrace.Length - 4)
                );
            }
            else
            {
                Trace
                (
                    TraceEventType.Error,
                    "\n\n{0}: {1}\nOcurred while {2}.\n",
                    exception.GetType().Name,
                    exception.Message,
                    situation
                );
            }
        }

        /// <summary>
        /// Reports the occurrence of an exception.
        /// </summary>
        /// <param name="exception">The exception to report.</param>
        /// <param name="severe">true to indicated this exception will reduce functionality or require extra steps for fixing, false to indicate the program is designed to recover from this.</param>
        /// <remarks>If severe is set to false, the stack trace will not be included.</remarks>
        public void ReportException(Exception exception, bool severe)
        {
            if (severe)
            {
                var extendedStackTrace = Environment.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Trace
                    (
                    TraceEventType.Error,
                    "\n\n{0} ocurred. \n\n == Exception Report == \n\n{1}\n\n == Source == \n\n{2}\n\n == AppDomain == \n\n{3}\n\n == Stacktrace == \n\n{4}\n\n == Extended Stacktrace == \n\n{5}\n",
                    exception.GetType().Name,
                    exception.Message,
                    exception.Source,
                    AppDomain.CurrentDomain.FriendlyName,
                    exception.StackTrace,
                    string.Join("\r\n", extendedStackTrace, 4, extendedStackTrace.Length - 4)
                );
            }
            else
            {
                Trace
                    (
                    TraceEventType.Error,
                    "\n\n{0}: {1}\n",
                    exception.GetType().Name,
                    exception.Message
                );
            }
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
        internal static void Initialize(SourceLevels level, bool allowDefaultListener)
        {
            // This should be called during initialization.
            // Double initialization is posible if multiple threads attemps to create the logbook...
            // Since that should not happen, let's accept the garbage if somehow that comes to be.
            _instance = new Logbook(level, allowDefaultListener);
        }

        /// <summary>
        /// Changes the level for the messages that will be recorded.
        /// </summary>
        /// <param name="level"></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal void ChangeLevel(SourceLevels level)
        {
            _logSource.Switch.Level = level;
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