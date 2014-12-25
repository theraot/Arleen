using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Arleen
{
    public static class Program
    {
        private static bool _debugMode;
        private static string _directorySeparator;
        private static string _displayName;
        private static string _folder;
        private static Logbook _logBook;

        /// <summary>
        /// Returns the display name for the Program.
        /// </summary>
        /// <remarks>The display name is the simple name of the assembly, that is "Arleen"</remarks>
        public static string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        /// <summary>
        /// Gets the path to the folder from where Arleen is loaded
        /// </summary>
        public static string Folder
        {
            get
            {
                return _folder;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Legendary Pokémon
            if (e.ExceptionObject is Exception)
            {
                var exception = e.ExceptionObject as Exception;
                _logBook.Trace
                    (
                        TraceEventType.Critical,
                        "And suddently something went wrong, really wrong...\n == Exception Report == \n{0}\n == Stacktrace == \n{1}",
                        exception.Message,
                        exception.StackTrace
                    );
            }
            else if (e.ExceptionObject != null)
            {
                _logBook.Trace
                    (
                        TraceEventType.Critical,
                        "Help me..."
                    );
            }
            else
            {
                _logBook.Trace
                    (
                        TraceEventType.Critical,
                        "It is all darkness..."
                    );
            }
        }

        /// <summary>
        /// Get a string with the directory separator for the current system.
        /// </summary>
        /// <returns>The directory separator for the current system as a string.</returns>
        private static string GetDirectorySeparator()
        {
            // At this point we don't worry about threading
            return _directorySeparator ?? (_directorySeparator = System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Initializing fiels with metadata about the the assembly
        /// </summary>
        private static void Initialize()
        {
            // *********************************
            // Getting folder and display name
            // *********************************
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var location = assembly.Location;
            _folder = Path.GetDirectoryName(location);
            if (_folder == null)
            {
                // Failed to get permision to query the file storage system.
            }
            else if (!_folder.EndsWith(GetDirectorySeparator()))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                _folder += System.IO.Path.DirectorySeparatorChar;
            }
            _displayName = assembly.GetName().Name;
            // *********************************
            // Setting debug mode
            // *********************************
            _debugMode = false;
            SetDebugMode();
            // *********************************
            // Creating the logbook
            // *********************************

            var logStreamWriter = new StreamWriter(_folder + "log.txt") {AutoFlush = true};

            _logBook = new Logbook
            (
                _debugMode ? SourceLevels.All : SourceLevels.Information,
                true,
                new TraceListener[]
                            {
                                new ConsoleTraceListener(),
                                new TextWriterTraceListener(logStreamWriter)
                            }
            );
        }

        /// <summary>
        /// Entry point
        /// </summary>
        private static void Main()
        {
            // Initialize
            Initialize();

            if (_debugMode)
            {
                Console.Clear();
            }

            // Salute
            _logBook.Trace(TraceEventType.Information, "Hello, my name is {0}.", _displayName);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // TODO: life, the universe and everything

                // Exit
                _logBook.Trace(TraceEventType.Information, "Goodbye, see you soon.", _displayName);

                if (_debugMode)
                {
                    Console.WriteLine("[Press a key to exit]");
                    Console.ReadKey();
                }
            }
            catch (Exception exception)
            {
                // Pokémon
                // Gotta catch'em all!
                ReportException(exception);
                Panic();
            }
        }

        private static void Panic()
        {
            _logBook.Trace
                    (
                        TraceEventType.Critical,
                        "Consider yourself lucky that this has been good to you so far.\n" +
                        "Alternatively, if this hasn't been good to you so far,\n" +
                        "consider yourself lucky that it won't be troubling you much longer."
                    );
            if (_debugMode)
            {
                Console.WriteLine("[Press a key to exit]");
                Console.ReadKey();
            }
        }

        private static void ReportException(Exception exception)
        {
            _logBook.Trace
                (
                    TraceEventType.Error,
                    "It has been a privilege, yet sometimes thing don't work as expected...\n == Exception Report == \n{0}\n == Stacktrace == \n{1}",
                    exception.Message,
                    exception.StackTrace
                );
        }

        /// <summary>
        /// Annotates that the program is running on debug mode - this method only exists on debug builds.
        /// </summary>
        [Conditional("DEBUG")]
        private static void SetDebugMode()
        {
            _debugMode = true;
        }
    }
}