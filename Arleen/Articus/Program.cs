using Arleen;
using Arleen.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace Articus
{
    /// <summary>
    /// The Program class contains the entry point
    /// </summary>
    public static class Program
    {
        private const string STR_SandboxName = "Sandbox";

        public static void Launch()
        {
            var realmComponents = ModuleLoader.Instance.GetComponents(typeof(Realm));
            Realm realm = null;
            foreach (var realmComponent in realmComponents)
            {
                var tmp = ModuleLoader.Instance.Load(realmComponent);
                Facade.Logbook.Trace(TraceEventType.Information, "Loaded: {0}", tmp.ToString());
                // TODO Visual Studio Complains here - BindingFailure - yet it works correctly
                realm = tmp as Realm;
                break;
            }
            if (realm == null)
            {
                Facade.Logbook.Trace(TraceEventType.Critical, "No realm found.");
            }
            else
            {
                Engine.Run(realm);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void SandboxInitialize(FacadeCore facadeCore)
        {
            #pragma warning disable 618 // This is intended internal use
            Facade.Set(facadeCore);
            #pragma warning restore 618
            Engine.Initialize(STR_SandboxName);
            Facade.Logbook.Trace(TraceEventType.Verbose, "■■■ ■■■ ■  ■ ■■  ■■  ■■■ ■ ■");
            Facade.Logbook.Trace(TraceEventType.Verbose, "■   ■ ■ ■■ ■ ■ ■ ■ ■ ■ ■ ■ ■");
            Facade.Logbook.Trace(TraceEventType.Verbose, "■■■ ■■■ ■■■■ ■ ■ ■■  ■ ■  ■ ");
            Facade.Logbook.Trace(TraceEventType.Verbose, "  ■ ■ ■ ■ ■■ ■ ■ ■ ■ ■ ■ ■ ■");
            Facade.Logbook.Trace(TraceEventType.Verbose, "■■■ ■ ■ ■  ■ ■■  ■■  ■■■ ■ ■");
        }

        private static void CreateSandbox()
        {
            Facade.Logbook.Trace(TraceEventType.Information, "Creating Sandbox.");

            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.AllFlags));
            permissionSet.AddPermission(new UIPermission(PermissionState.Unrestricted));
            permissionSet.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
            permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Articus.Folder));

            var appDomainSetup = new AppDomainSetup { ApplicationBase = Articus.Folder };
            var sandboxDomain = AppDomain.CreateDomain(STR_SandboxName, null, appDomainSetup, permissionSet);

            Facade.Logbook.Trace(TraceEventType.Information, "Program.CreateSandbox from {0} to {1}.", AppDomain.CurrentDomain.FriendlyName, sandboxDomain.FriendlyName);

            var parameters = new object[] {
                #pragma warning disable 618 // This is intended internal use
                FacadeCore.Initialize(STR_SandboxName, false)
                #pragma warning restore 618
            };

            sandboxDomain.DoCallBack(new CrossCaller(Articus.Location, "Articus.Program", "SandboxInitialize", parameters).LoadAndCall);

            ModuleLoader.Initialize(sandboxDomain);

            Launch();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            // Legendary Pokémon
            var exception = eventArgs.ExceptionObject as Exception;
            if (exception != null)
            {
                var current = exception;
                do
                {
                    Facade.Logbook.Trace
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
                } while (current != null);
                var extendedStackTrace = Environment.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Facade.Logbook.Trace(TraceEventType.Error, " == Extended StackTrace == \n\n{0}\n\n", string.Join("\r\n", extendedStackTrace, 4, extendedStackTrace.Length - 4));
            }
            else if (eventArgs.ExceptionObject != null)
            {
                Facade.Logbook.Trace
                (
                    TraceEventType.Critical,
                    "Help me..."
                );
                Facade.Logbook.Trace
                (
                    TraceEventType.Critical,
                    eventArgs.ExceptionObject.ToString()
                );
            }
            else
            {
                Facade.Logbook.Trace
                (
                    TraceEventType.Critical,
                    "It is all darkness..."
                );
            }
        }

        [SecuritySafeCritical]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length == 0)
            {
                Engine.Initialize("Default");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■ ■ ■   ■■■");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■ ■ ■   ■   ■ ■ ■ ■ ■    ■ ");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■ ■ ■■■ ■■■ ■■■ ■ ■ ■    ■ ");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■ ■ ■   ■   ■ ■ ■ ■ ■    ■ ");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■■  ■■■ ■   ■ ■ ■■■ ■■■  ■ ");
                ModuleLoader.LoadModules();
                CreateSandbox();
            }
            else if (args[0] == "discover")
            {
                var dll = args[1];
                Engine.Initialize("Discovery - " + Path.GetFileName(dll));
                Facade.Logbook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■■■ ■ ■ ■■■ ■■■ ■ ■");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■ ■  ■  ■   ■   ■ ■ ■ ■ ■   ■ ■ ■ ■");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■ ■  ■  ■■■ ■   ■ ■ ■ ■ ■■■ ■■■ ■ ■");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■ ■  ■    ■ ■   ■ ■ ■ ■ ■   ■■   ■ ");
                Facade.Logbook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■■■  ■  ■■■ ■ ■  ■ ");
                Facade.Logbook.Trace(TraceEventType.Information, "Target: {0}", dll);

                ModuleLoader.Initialize(AppDomain.CurrentDomain);
                var result = new List<Component>(ModuleLoader.Instance.Discover(dll));
                Facade.Logbook.Trace(TraceEventType.Information, "Components found: {0}", result.Count);
                Facade.Logbook.Trace(TraceEventType.Information, "Serializing...");
                var data = JsonConvert.SerializeObject(result);
                var file = args[1].Substring(0, args[1].Length - 3) + ModuleLoader.STR_Module_Extension;
                Facade.Logbook.Trace(TraceEventType.Information, "Attempting to store in: {0}", file);
                File.WriteAllText(file, data);
                Facade.Logbook.Trace(TraceEventType.Information, "Done.");
            }
        }
    }
}