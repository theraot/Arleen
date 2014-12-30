using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Arleen
{
    /// <summary>
    /// The Program class contains the entry point
    /// </summary>
    public static class Program
    {
        private static bool _debugMode;
        private static Logbook _logBook;

        /// <summary>
        /// Returns the display name for the Program.
        /// </summary>
        /// <remarks>By default this is equal to InternalName</remarks>
        public static string DisplayName { get; private set; }

        public static string CurrentLanguage { get; private set; }

        /// <summary>
        /// Gets the path to the folder from where Arleen is loaded
        /// </summary>
        public static string Folder { get; private set; }

        /// <summary>
        /// Returns the internal name for the Program.
        /// </summary>
        /// <remarks>The internal name is the simple name of the assembly, that is "Arleen"</remarks>
        public static string InternalName { get; private set; }

        /// <summary>
        /// Gets the loaded configuration for the program.
        /// </summary>
        internal static Configuration Configuration { get; private set; }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            // Legendary Pokémon
            if (eventArgs.ExceptionObject is Exception)
            {
                var exception = eventArgs.ExceptionObject as Exception;
                _logBook.Trace
                    (
                        TraceEventType.Critical,
                        "And suddently something went wrong, really wrong...\n == Exception Report == \n{0}\n == Stacktrace == \n{1}",
                        exception.Message,
                        exception.StackTrace
                    );
            }
            else if (eventArgs.ExceptionObject != null)
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

        private static string GetApplicationDataFolder()
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
                + System.IO.Path.DirectorySeparatorChar
                + InternalName;
            Directory.CreateDirectory(folder);
            return folder;
        }

        private static void Initialize()
        {
            // *********************************
            // Getting folder and display name
            // *********************************

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            InternalName = assembly.GetName().Name;
            DisplayName = InternalName;

            var location = assembly.Location;
            Folder = Path.GetDirectoryName(location);
            if (Folder == null)
            {
                // Failed to get permision to query the file storage system.
                // Fallback to ApplicationData
                Folder = GetApplicationDataFolder();
            }
            if (!Folder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                Folder += System.IO.Path.DirectorySeparatorChar;
            }

            // *********************************
            // Setting debug mode
            // *********************************

            _debugMode = false;
            SetDebugMode();

            if (_debugMode)
            {
                try
                {
                    Console.Clear();
                }
                catch (IOException)
                {
                    // Ignore.
                }
            }

            // *********************************
            // Creating the logbook
            // *********************************

            _logBook = Logbook.Initialize(_debugMode ? SourceLevels.All : SourceLevels.Information, true);

            try
            {
                var logStreamWriter = new StreamWriter(Folder + "log.txt") { AutoFlush = true };
                _logBook.AddListener(new TextWriterTraceListener(logStreamWriter));
            }
            catch (Exception exception)
            {
                _logBook.ReportException(exception, "trying to create the log file.", true);
                try
                {
                    Console.WriteLine("Unable to create log file.");
                    Console.WriteLine("== Exception Report ==");
                    Console.WriteLine(exception.Message);
                    Console.WriteLine("== Stacktrace ==");
                    Console.WriteLine(exception.StackTrace);
                }
                catch (IOException)
                {
                    // Ignore.
                }
            }

            try
            {
                // Test for Console
                GC.KeepAlive(Console.WindowHeight);
                _logBook.AddListener(new ConsoleTraceListener());
            }
            catch (Exception exception)
            {
                _logBook.ReportException(exception, "trying to access the Console.", false);
            }

            if (_debugMode)
            {
                _logBook.Trace(TraceEventType.Information, "[Running debug build]");
            }

            _logBook.Trace(TraceEventType.Information, "Internal name: {0}", assembly.FullName);

            // *********************************
            // Detecting Language
            // *********************************

            // We get the culture name via TextInfo because it always includes the region.
            // If we get the name of the culture directly it will only have the region if it is not the default one.

            CurrentLanguage = CultureInfo.CurrentCulture.TextInfo.CultureName;

            // *********************************
            // Reading main configuration
            // *********************************

            Configuration = Resources.LoadConfig<Configuration>();
            if (Configuration == null)
            {
                return;
            }
            if (Configuration.ForceDebugMode)
            {
                if (!_debugMode)
                {
                    _logBook.ChangeLevel(SourceLevels.All);
                    _logBook.Trace(TraceEventType.Information, "[Forced debug mode]");
                }
            }
            if (string.IsNullOrEmpty(Configuration.DisplayName))
            {
                Configuration.DisplayName = DisplayName;
            }
            else
            {
                DisplayName = Configuration.DisplayName;
            }
            if (!string.IsNullOrEmpty(Configuration.Language))
            {
                CurrentLanguage = Configuration.Language;
            }
        }

        private static void Main()
        {
            // Initialize
            try
            {
                Initialize();
            }
            catch (Exception exception)
            {
                if (_logBook != null)
                {
                    _logBook.ReportException(exception, "doing initialization", true);
                }
                else
                {
                    throw;
                }
            }

            // Salute
            var _ = Resources.LoadTexts();
            _logBook.Trace(TraceEventType.Information, _["Hello, my name is {name}."].FormatWith(new { name = DisplayName }));

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (Configuration == null)
            {
                _logBook.Trace(TraceEventType.Critical, "There was no configuration lodaded... will not proceed.");
            }
            else
            {
                try
                {
                    using (var window = new Game.Window())
                    {
                        window.Run(Configuration.MaxUpdateRate, Configuration.MaxFrameRate);
                    }

                    // Save configuration

                    if (!Resources.SaveConfig(Configuration))
                    {
                        _logBook.Trace(TraceEventType.Error, "Failed to save configuration.");
                    }

                    // Exit
                    _logBook.Trace(TraceEventType.Information, "Goodbye, see you soon.", DisplayName);

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
                    _logBook.ReportException(exception, true);
                    Panic();
                }
            }
        }

        private static void Panic()
        {
            const string STR_PanicMessage =
                "Consider yourself lucky that this has been good to you so far.\n" +
                "Alternatively, if this hasn't been good to you so far,\n" +
                "consider yourself lucky that it won't be troubling you much longer.";
            _logBook.Trace(TraceEventType.Critical, STR_PanicMessage);
            if (_debugMode)
            {
                Console.WriteLine("[Press a key to exit]");
                Console.ReadKey();
            }
        }

        [Conditional("DEBUG")]
        private static void SetDebugMode()
        {
            _debugMode = true;
        }
    }
}