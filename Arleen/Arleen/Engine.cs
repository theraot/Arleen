using Arleen.Game;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
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

        public static bool DebugMode { get; private set; }

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

        /// <summary>
        /// Initialized the engine
        /// </summary>
        public static void Initialize(string name)
        {
            if (Interlocked.CompareExchange(ref _status, INT_Initializing, INT_NotInitialized) == INT_NotInitialized)
            {
                InitializeExtracted(name);
                Thread.VolatileWrite(ref _status, INT_Initialized);
            }
        }

        /// <summary>
        /// Runs the specified realm.
        /// </summary>
        /// <param name="realm">The realm to run.</param>
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
                    Facade.Logbook.ReportException(exception, true);
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
                Facade.Logbook.ReportException(exception, true);
                Panic();
            }
        }

        [SecuritySafeCritical]
        private static void InitializeExtracted(string name)
        {
            // Note: this method is not thread safe.

            // *********************************
            // Getting folder and display name
            // *********************************

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            InternalName = assembly.GetName().Name;

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
            // Creating the Facade
            // *********************************

            #pragma warning disable 618 // This is intended internal use
            FacadeCore.Initialize(name, true);
            #pragma warning restore 618

            if (Facade.Configuration.ForceDebugMode)
            {
                if (!DebugMode)
                {
                    Facade.Logbook.ChangeLevel(SourceLevels.All);
                    Facade.Logbook.Trace(TraceEventType.Information, "[Forced debug mode]");
                }
            }

            // *********************************
            // Reporting
            // *********************************

            if (DebugMode)
            {
                Facade.Logbook.Trace(TraceEventType.Information, "[Running debug build]");
            }

            Facade.Logbook.Trace(TraceEventType.Information, "Internal name: {0}", assembly.FullName);

            // *********************************
            // Reading main configuration
            // *********************************

            Facade.Logbook.Trace(TraceEventType.Information, "Current Language: {0}", Facade.Configuration.Language);
        }

        private static void Panic()
        {
            const string STR_PanicMessage =
                "Consider yourself lucky that this has been good to you so far.\n" +
                "Alternatively, if this hasn't been good to you so far,\n" +
                "consider yourself lucky that it won't be troubling you much longer.";
            Facade.Logbook.Trace(TraceEventType.Critical, STR_PanicMessage);
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

                if (!ResourcesInternal.SaveConfig(Facade.Configuration))
                {
                    Facade.Logbook.Trace(TraceEventType.Error, "Failed to save configuration.");
                }

                // Exit
                Facade.Logbook.Trace(TraceEventType.Information, "Goodbye, see you soon.");
            }
            catch (Exception exception)
            {
                // Pokémon
                // Gotta catch'em all!
                Facade.Logbook.ReportException(exception, true);
            }
        }
    }
}