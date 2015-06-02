using Arleen;
using Arleen.Game;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Security.Policy;

namespace Articus
{
    /// <summary>
    /// The Program class contains the entry point
    /// </summary>
    public class Program : MarshalByRefObject
    {
        private static int _initialized;

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            // Legendary Pokémon
            var exception = eventArgs.ExceptionObject as Exception;
            if (exception != null)
            {
                var current = exception;
                do
                {
                    Engine.LogBook.Trace
                    (
                        TraceEventType.Critical,
                        "And suddently something went wrong, really wrong...\n\n{0} ocurred. \n\n == Exception Report == \n\n{1}\n\n == Source == \n\n{2}\n\n == AppDomain == \n\n{3}\n\n == Stacktrace == \n\n{4}\n",
                        current.GetType().Name,
                        current.Message,
                        current.Source,
                        AppDomain.CurrentDomain.FriendlyName,
                        current.StackTrace
                    );
                    current = current.InnerException;
                }
                while (current != null);
            }
            else if (eventArgs.ExceptionObject != null)
            {
                Engine.LogBook.Trace
                (
                    TraceEventType.Critical,
                    "Help me..."
                );
                Engine.LogBook.Trace
                (
                    TraceEventType.Critical,
                    eventArgs.ExceptionObject.ToString()
                );
            }
            else
            {
                Engine.LogBook.Trace
                (
                    TraceEventType.Critical,
                    "It is all darkness..."
                );
            }
        }

        static void CreateSandbox ()
        {
            var path = new Uri (Assembly.GetExecutingAssembly ().CodeBase).LocalPath;
            var folder = System.IO.Path.GetDirectoryName (path);

            //---

            Engine.LogBook.Trace (TraceEventType.Information, "Migrating to Sandbox.");
            var permSet = new PermissionSet(PermissionState.Unrestricted);
            var adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = folder;
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet);
            var program = newDomain.CreateInstanceFromAndUnwrap (path, "Articus.Program") as Program;
            var resources = Resources.Instance;
            ModuleLoader.Initialize (newDomain);
            var moduleLoader = ModuleLoader.Instance;
            program.Launch (resources, moduleLoader);
            Engine.LogBook.Trace (TraceEventType.Information, "Sandbox Execution Completed.");
        }

        static void Initialize (string purpose)
        {
            if (Interlocked.CompareExchange (ref _initialized, 1, 0) == 0)
            {
                Engine.Initialize (purpose);
                if (Engine.Configuration == null)
                {
                    Engine.LogBook.Trace (TraceEventType.Critical, "There was no configuration lodaded... will not proceed.");
                }
            }
        }

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length == 0)
            {
                Initialize ("Default");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■ ■ ■   ■■■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■ ■   ■   ■ ■ ■ ■ ■    ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■ ■■■ ■■■ ■■■ ■ ■ ■    ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■ ■   ■   ■ ■ ■ ■ ■    ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■   ■ ■ ■■■ ■■■  ■ ");
                var _ = Engine.TextLocalization;
                Engine.LogBook.Trace
                (
                    TraceEventType.Information,
                    _("Hello, my name is {name}.", new { name = Engine.DisplayName })
                );
                CreateSandbox ();
            }
            else if (args[0] == "discover")
            {
                var dll = args [1];
                Initialize ("Discovery - " + System.IO.Path.GetFileName(dll));
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■■■ ■ ■ ■■■ ■■■ ■ ■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■  ■  ■   ■   ■ ■ ■ ■ ■   ■ ■ ■ ■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■  ■  ■■■ ■   ■ ■ ■ ■ ■■■ ■■■ ■ ■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■  ■    ■ ■   ■ ■ ■ ■ ■   ■■   ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■■■  ■  ■■■ ■ ■  ■ ");
                Engine.LogBook.Trace(TraceEventType.Information, "Target: {0}", dll);
                var result = new List<Component>(ModuleLoader.Instance.Discover (dll));
                Engine.LogBook.Trace(TraceEventType.Information, "Components found: {0}", result.Count);
                Engine.LogBook.Trace(TraceEventType.Information, "Serializing...");
                var data = JsonConvert.SerializeObject(result);
                var file = args[1].Substring(0, args[1].Length - 3) + ModuleLoader.STR_Module_Extension;
                Engine.LogBook.Trace(TraceEventType.Information, "Attempting to store in: {0}", file);
                System.IO.File.WriteAllText(file, data);
                Engine.LogBook.Trace(TraceEventType.Information, "Done.");
            }
        }

        public Program ()
        {
            if (AppDomain.CurrentDomain.FriendlyName == "Sandbox")
            {
                if (Thread.VolatileRead(ref _initialized) != 0)
                {
                    throw new InvalidOperationException ("Initialization already done.");
                }    
            }
            else
            {
                throw new InvalidOperationException ("Wrong AppDomain.");
            }
        }

        public void Launch(Resources resources, ModuleLoader moduleLoader)
        {
            Resources.Instance = resources;
            ModuleLoader.Instance = moduleLoader;
            Initialize ("Sandbox");
            Engine.LogBook.Trace (TraceEventType.Verbose, "■■■ ■■■ ■  ■ ■■  ■■  ■■■ ■ ■");
            Engine.LogBook.Trace (TraceEventType.Verbose, "■   ■ ■ ■■ ■ ■ ■ ■ ■ ■ ■ ■ ■");
            Engine.LogBook.Trace (TraceEventType.Verbose, "■■■ ■■■ ■■■■ ■ ■ ■■  ■ ■  ■ ");
            Engine.LogBook.Trace (TraceEventType.Verbose, "  ■ ■ ■ ■ ■■ ■ ■ ■ ■ ■ ■ ■ ■");
            Engine.LogBook.Trace (TraceEventType.Verbose, "■■■ ■ ■ ■  ■ ■■  ■■  ■■■ ■ ■");
            var realmComponents = ModuleLoader.Instance.GetComponents (typeof(Realm));
            Realm realm = null;
            foreach (var realmComponent in realmComponents)
            {
                var tmp = ModuleLoader.Instance.Load (realmComponent);
                Engine.LogBook.Trace (TraceEventType.Information, "Loaded: {0}", tmp.ToString ());
                realm = tmp as Realm;
                break;
            }
            if (realm == null)
            {
                Engine.LogBook.Trace (TraceEventType.Critical, "No realm found.");
            }
            else
            {
                Engine.Run (realm);
            }
        }
    }
}