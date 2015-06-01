using Arleen.Game;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace Arleen
{
    /// <summary>
    /// The Engine class is the main façade of the library
    /// </summary>
    public static class Engine
    {
        private const int INT_NotInitialized = 0;
        private const int INT_Initializing = 1;
        private const int INT_Initialized = 2;

        private static RealmRunner _realmRunner;
        private static bool _debugMode;
        private static int _status;

        /// <summary>
        /// Gets the loaded configuration for the program.
        /// </summary>
        public static Configuration Configuration { get; private set; }

        /// <summary>
        /// Gets the currently configured langauge.
        /// </summary>
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

        /// <summary>
        /// Gets the LogBook used to write log entries.
        /// </summary>
        public static Logbook LogBook { get; private set; }

        /// <summary>
        /// Gets the text localization 
        /// </summary>
        public static TextLocalization TextLocalization { get; private set; }

        /// <summary>
        /// Changes the current Realm.
        /// </summary>
        /// <param name="realm">The new realm.</param>
        public static void ChangeRealm(Realm realm)
        {
            if (realm == null)
            {
                if (_realmRunner != null)
                {
                    _realmRunner.CurrentRealm = null;
                    _realmRunner.Dispose ();
                    _realmRunner = null;
                }
            }
            else
            {
                if (_realmRunner == null)
                {
                    _realmRunner = new RealmRunner
                    {
                        CurrentRealm = realm
                    };
                }
                else
                {
                    _realmRunner.CurrentRealm = realm;
                }
            }
        }

        /// <summary>
        /// Initialized the engine
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Initialize()
        {
            try
            {
                if (Interlocked.CompareExchange(ref _status, INT_Initializing, INT_NotInitialized) == INT_NotInitialized)
                {
                    InitializeExtracted();
                    Thread.VolatileWrite(ref _status, INT_Initialized);
                }
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

        /// <summary>
        /// Runs the specified realm.
        /// </summary>
        /// <param name="realm">The realm to run.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Run(Realm realm)
        {
            if (Thread.VolatileRead(ref _status) != INT_Initialized)
            {
                throw new InvalidOperationException("The Engine has not been initialized");
            }
            try
            {
                try
                {
                    ChangeRealm(realm);
                }
                catch (Exception exception)
                {
                    LogBook.ReportException(exception, true);
                }
                finally
                {
                    ChangeRealm(null);
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
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + Path.DirectorySeparatorChar
                + InternalName;
            Directory.CreateDirectory(folder);
            return folder;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static void InitializeExtracted()
        {
            // Note: this method is not thread safe.

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
                LogBook.ReportException(exception, "trying to access the Console", false);
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

            Configuration = ResourcesInternal.LoadConfig<Configuration>();
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

            LogBook.Trace(TraceEventType.Information, "Current Language: {0}", CurrentLanguage);

            // *********************************
            // Load localized texts
            // *********************************

            TextLocalization = ResourcesInternal.LoadTexts(CurrentLanguage);
        }

        private static void Panic()
        {
            const string STR_PanicMessage =
                "Consider yourself lucky that this has been good to you so far.\n" +
                "Alternatively, if this hasn't been good to you so far,\n" +
                "consider yourself lucky that it won't be troubling you much longer.";
            LogBook.Trace(TraceEventType.Critical, STR_PanicMessage);
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

                if (!ResourcesInternal.SaveConfig(Configuration))
                {
                    LogBook.Trace(TraceEventType.Error, "Failed to save configuration.");
                }

                // Exit
                LogBook.Trace(TraceEventType.Information, "Goodbye, see you soon.", DisplayName);

                if (_debugMode)
                {
					try
					{
						// Test for Console
						GC.KeepAlive(Console.WindowHeight);
						Console.WriteLine("[Press a key to exit]");
						Console.ReadKey();
					}
					catch (IOException exception)
					{
						GC.KeepAlive(exception);
						// Running without console
					}
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