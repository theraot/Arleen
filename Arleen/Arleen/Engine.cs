using Arleen.Game;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;

namespace Arleen
{
    /// <summary>
    /// The Engine class is the main façade of the library
    /// </summary>
    public static class Engine
    {
        private static Realm _currentRealm;
        private static bool _debugMode;

        /// <summary>
        /// Gets the loaded configuration for the program.
        /// </summary>
        public static Configuration Configuration { get; private set; }

        public static string CurrentLanguage { get; private set; }

        /// <summary>
        /// Returns the display name for the Program.
        /// </summary>
        /// <remarks>By default this is equal to InternalName</remarks>
        public static string DisplayName { get; private set; }

        /// <summary>
        /// Gets the path to the folder from where Arleen is loaded
        /// </summary>
        public static string Folder { get; private set; }

        /// <summary>
        /// Returns the internal name for the Program.
        /// </summary>
        /// <remarks>The internal name is the simple name of the assembly, that is "Arleen"</remarks>
        public static string InternalName { get; private set; }

        public static Logbook LogBook { get; private set; }

        /// <summary>
        /// Changes the current Realm.
        /// </summary>
        /// <param name="realm">The new realm.</param>
        public static void ChangeRealm(Realm realm)
        {
            _currentRealm.Dispose();
            _currentRealm = realm;
            if (_currentRealm != null)
            {
                _currentRealm.Run();
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Initialize()
        {
            try
            {
                InitializeExtracted();
            }
            catch (Exception exception)
            {
                if (LogBook != null)
                {
                    LogBook.ReportException(exception, "doing initialization", true);
                }
                else
                {
                    throw;
                }
            }
        }

        public static void Run(Realm realm)
        {
            try
            {
                try
                {
                    _currentRealm = realm;
                    _currentRealm.Run();
                }
                finally
                {
                    if (_currentRealm != null)
                    {
                        _currentRealm.Dispose();
                    }
                    Terminate();
                }
            }
            catch (Exception exception)
            {
                // Pokémon
                // Gotta catch'em all!
                LogBook.ReportException(exception, true);
                Panic();
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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static void InitializeExtracted()
        {
            // *********************************
            // Getting folder and display name
            // *********************************

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            InternalName = assembly.GetName().Name;
            DisplayName = InternalName;

            var location = assembly.Location;
            Folder = Path.GetDirectoryName(location) ?? GetApplicationDataFolder();
            if (!Folder.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                Folder += Path.DirectorySeparatorChar;
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

            LogBook = Logbook.Initialize(_debugMode ? SourceLevels.All : SourceLevels.Information, true);

            try
            {
                var logStreamWriter = new StreamWriter(Folder + "log.txt") { AutoFlush = true };
                LogBook.AddListener(new TextWriterTraceListener(logStreamWriter));
            }
            catch (Exception exception)
            {
                LogBook.ReportException(exception, "trying to create the log file.", true);
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
                LogBook.AddListener(new ConsoleTraceListener());
            }
            catch (Exception exception)
            {
                LogBook.ReportException(exception, "trying to access the Console.", false);
            }

            if (_debugMode)
            {
                LogBook.Trace(TraceEventType.Information, "[Running debug build]");
            }

            LogBook.Trace(TraceEventType.Information, "Internal name: {0}", assembly.FullName);

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
                    LogBook.ChangeLevel(SourceLevels.All);
                    LogBook.Trace(TraceEventType.Information, "[Forced debug mode]");
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

        private static void Panic()
        {
            const string STR_PanicMessage =
                "Consider yourself lucky that this has been good to you so far.\n" +
                "Alternatively, if this hasn't been good to you so far,\n" +
                "consider yourself lucky that it won't be troubling you much longer.";
            LogBook.Trace(TraceEventType.Critical, STR_PanicMessage);
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

        private static void Terminate()
        {
            try
            {
                // Save configuration

                if (!Resources.SaveConfig(Configuration))
                {
                    LogBook.Trace(TraceEventType.Error, "Failed to save configuration.");
                }

                // Exit
                LogBook.Trace(TraceEventType.Information, "Goodbye, see you soon.", DisplayName);

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
                LogBook.ReportException(exception, true);
            }
        }
    }
}