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
        private const int INT_Initialized = 2;
        private const int INT_Initializing = 1;
        private const int INT_NotInitialized = 0;
        private static RealmRunner _realmRunner;
        private static int _status;

        public static AppDomain AppDomain { get; private set; }

        /// <summary>
        /// Gets the loaded configuration for the program.
        /// </summary>
        public static Configuration Configuration { get; private set; }

        /// <summary>
        /// Gets the currently configured language.
        /// </summary>
        public static string CurrentLanguage { get; private set; }

        public static bool DebugMode { get; private set; }

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
                    _realmRunner.Dispose();
                    _realmRunner = null;
                }
            }
            else
            {
                if (_realmRunner == null || _realmRunner.IsClosed)
                {
                    _realmRunner = new RealmRunner {
                        CurrentRealm = realm
                    };
                }
                else
                {
                    _realmRunner.CurrentRealm = realm;
                }
            }
        }

        public static T Create<T>()
            where T : class
        {
            return AppDomain.CreateInstanceAndUnwrap
            (
                typeof(T).Assembly.FullName,
                typeof(T).FullName,
                false,
                System.Reflection.BindingFlags.Default,
                null,
                null,
                null,
                null,
                null
            ) as T;
        }

        public static T Create<T>(object param)
            where T : class
        {
            return AppDomain.CreateInstanceAndUnwrap
            (
                typeof(T).Assembly.FullName,
                typeof(T).FullName,
                false,
                System.Reflection.BindingFlags.Default,
                null,
                new[] { param },
                null,
                null,
                null
            ) as T;
        }

        public static T Create<T>(params object[] param)
            where T : class
        {
            return AppDomain.CreateInstanceAndUnwrap
            (
                typeof(T).Assembly.FullName,
                typeof(T).FullName,
                false,
                System.Reflection.BindingFlags.Default,
                null,
                param,
                null,
                null,
                null
            ) as T;
        }

        /// <summary>
        /// Initialized the engine
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Initialize(string purpose, AppDomain appDomain)
        {
            if (Interlocked.CompareExchange(ref _status, INT_Initializing, INT_NotInitialized) == INT_NotInitialized)
            {
                AppDomain = appDomain;
                InitializeExtracted(purpose);
                Thread.VolatileWrite(ref _status, INT_Initialized);
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
                    Logbook.Instance.ReportException(exception, true);
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
                Logbook.Instance.ReportException(exception, true);
                Panic();
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static void InitializeExtracted(string purpose)
        {
            // Note: this method is not thread safe.

            // *********************************
            // Getting folder and display name
            // *********************************

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            InternalName = assembly.GetName().Name;
            DisplayName = InternalName;

            var location = assembly.Location;
            Folder = Path.GetDirectoryName(location);
            // Let this method throw if Folder is null
            if (!Folder.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                Folder += Path.DirectorySeparatorChar;
            }

            // *********************************
            // Setting debug mode
            // *********************************

            DebugMode = false;
            SetDebugMode();

            // *********************************
            // Creating the logbook
            // *********************************

            Logbook.Initialize(DebugMode ? SourceLevels.All : SourceLevels.Information, true);

            try
            {
                var logFile = purpose + ".log";
                foreach (char character in Path.GetInvalidFileNameChars())
                {
                    logFile = logFile.Replace(character.ToString(), string.Empty);
                }
                var logStreamWriter = new StreamWriter(Folder + logFile) { AutoFlush = true };
                Logbook.Instance.AddListener(new TextWriterTraceListener(logStreamWriter));
            }
            catch (Exception exception)
            {
                Logbook.Instance.ReportException(exception, "trying to create the log file.", true);
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
                Logbook.Instance.AddListener(new ConsoleTraceListener());
            }
            catch (Exception exception)
            {
                Logbook.Instance.ReportException(exception, "trying to access the Console", false);
            }

            if (DebugMode)
            {
                Logbook.Instance.Trace(TraceEventType.Information, "[Running debug build]");
            }

            Logbook.Instance.Trace(TraceEventType.Information, "Internal name: {0}", assembly.FullName);

            // *********************************
            // Detecting Language
            // *********************************

            // We get the culture name via TextInfo because it always includes the region.
            // If we get the name of the culture directly it will only have the region if it is not the default one.

            CurrentLanguage = CultureInfo.CurrentCulture.TextInfo.CultureName;

            // *********************************
            // Reading main configuration
            // *********************************

            try
            {
                Configuration = ResourcesInternal.LoadConfig<Configuration>();
            }
            catch (Exception exception)
            {
                Logbook.Instance.ReportException(exception, true);
            }
            if (Configuration == null)
            {
                return;
            }
            if (Configuration.ForceDebugMode)
            {
                if (!DebugMode)
                {
                    Logbook.Instance.ChangeLevel(SourceLevels.All);
                    Logbook.Instance.Trace(TraceEventType.Information, "[Forced debug mode]");
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

            Logbook.Instance.Trace(TraceEventType.Information, "Current Language: {0}", CurrentLanguage);

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
            Logbook.Instance.Trace(TraceEventType.Critical, STR_PanicMessage);
        }

        [Conditional("DEBUG")]
        private static void SetDebugMode()
        {
            DebugMode = true;
        }

        private static void Terminate()
        {
            try
            {
                // Save configuration

                if (!ResourcesInternal.SaveConfig(Configuration))
                {
                    Logbook.Instance.Trace(TraceEventType.Error, "Failed to save configuration.");
                }

                // Exit
                Logbook.Instance.Trace(TraceEventType.Information, "Goodbye, see you soon.", DisplayName);

                if (DebugMode)
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
                Logbook.Instance.ReportException(exception, true);
            }
        }
    }
}