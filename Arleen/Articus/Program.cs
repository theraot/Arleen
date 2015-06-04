﻿using Arleen;
using Arleen.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace Articus
{
    /// <summary>
    /// The Program class contains the entry point
    /// </summary>
    public static class Program
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Launch()
        {
            var realmComponents = ModuleLoader.Instance.GetComponents(typeof(Realm));
            Realm realm = null;
            foreach (var realmComponent in realmComponents)
            {
                var tmp = ModuleLoader.Instance.Load(realmComponent);
                Engine.LogBook.Trace(TraceEventType.Information, "Loaded: {0}", tmp.ToString());
                realm = tmp as Realm;
                break;
            }
            if (realm == null)
            {
                Engine.LogBook.Trace(TraceEventType.Critical, "No realm found.");
            }
            else
            {
                Engine.Run(realm);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void SandboxInitialize(Resources resources, AppDomain appDomain)
        {
            Resources.Instance = resources;
            Initialize("Sandbox", appDomain);
            Engine.LogBook.Trace(TraceEventType.Verbose, "■■■ ■■■ ■  ■ ■■  ■■  ■■■ ■ ■");
            Engine.LogBook.Trace(TraceEventType.Verbose, "■   ■ ■ ■■ ■ ■ ■ ■ ■ ■ ■ ■ ■");
            Engine.LogBook.Trace(TraceEventType.Verbose, "■■■ ■■■ ■■■■ ■ ■ ■■  ■ ■  ■ ");
            Engine.LogBook.Trace(TraceEventType.Verbose, "  ■ ■ ■ ■ ■■ ■ ■ ■ ■ ■ ■ ■ ■");
            Engine.LogBook.Trace(TraceEventType.Verbose, "■■■ ■ ■ ■  ■ ■■  ■■  ■■■ ■ ■");
        }

        private static void CreateSandbox()
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var folder = Path.GetDirectoryName(path);
            // Let this method throw if folder is null
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                folder += Path.DirectorySeparatorChar;
            }

            //---

            Engine.LogBook.Trace(TraceEventType.Information, "Migrating to Sandbox.");

            var permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.AllFlags));
            permSet.AddPermission(new UIPermission(PermissionState.Unrestricted));
            permSet.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));

            var permission = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, folder);
            permission.AddPathList(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write | FileIOPermissionAccess.PathDiscovery, folder + "Sandbox.log");
            permSet.AddPermission(permission);

            var appDomainSetup = new AppDomainSetup { ApplicationBase = folder };
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, appDomainSetup, permSet);

            Logbook.Instance.Trace(TraceEventType.Information, "Program.CreateSandbox from {0} to {1}.", AppDomain.CurrentDomain.FriendlyName, newDomain.FriendlyName);

            var parameters = new object[] {
                Resources.Instance,
                AppDomain.CurrentDomain
            };

            newDomain.DoCallBack(new CrossCaller(path, "Articus.Program", "SandboxInitialize", parameters).LoadAndCall);

            ModuleLoader.Initialize(newDomain);

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
                } while (current != null);
                var extendedStackTrace = Environment.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Engine.LogBook.Trace(TraceEventType.Error, " == Extended StackTrace == \n\n{0}\n\n", string.Join("\r\n", extendedStackTrace, 4, extendedStackTrace.Length - 4));
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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static void Initialize(string purpose, AppDomain appDomain)
        {
            // This methos should run only once, no check is perfomed to ensure this
            Engine.Initialize(purpose, appDomain);
            if (Engine.Configuration == null)
            {
                Engine.LogBook.Trace(TraceEventType.Critical, "There was no configuration lodaded... will not proceed.");
            }
        }

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length == 0)
            {
                Initialize("Default", AppDomain.CurrentDomain);
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■ ■ ■   ■■■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■ ■   ■   ■ ■ ■ ■ ■    ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■ ■■■ ■■■ ■■■ ■ ■ ■    ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■ ■   ■   ■ ■ ■ ■ ■    ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■   ■ ■ ■■■ ■■■  ■ ");
                var _ = Engine.TextLocalization;
                Engine.LogBook.Trace
                (TraceEventType.Information, _("Hello, my name is {name}.", new { name = Engine.DisplayName }));
                ModuleLoader.LoadModules();
                CreateSandbox();
            }
            else if (args[0] == "discover")
            {
                var dll = args[1];
                Initialize("Discovery - " + Path.GetFileName(dll), AppDomain.CurrentDomain);
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■■■ ■ ■ ■■■ ■■■ ■ ■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■  ■  ■   ■   ■ ■ ■ ■ ■   ■ ■ ■ ■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■  ■  ■■■ ■   ■ ■ ■ ■ ■■■ ■■■ ■ ■");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■ ■  ■    ■ ■   ■ ■ ■ ■ ■   ■■   ■ ");
                Engine.LogBook.Trace(TraceEventType.Verbose, "■■  ■■■ ■■■ ■■■ ■■■  ■  ■■■ ■ ■  ■ ");
                Engine.LogBook.Trace(TraceEventType.Information, "Target: {0}", dll);

                ModuleLoader.Initialize(AppDomain.CurrentDomain);
                var result = new List<Component>(ModuleLoader.Instance.Discover(dll));
                Engine.LogBook.Trace(TraceEventType.Information, "Components found: {0}", result.Count);
                Engine.LogBook.Trace(TraceEventType.Information, "Serializing...");
                var data = JsonConvert.SerializeObject(result);
                var file = args[1].Substring(0, args[1].Length - 3) + ModuleLoader.STR_Module_Extension;
                Engine.LogBook.Trace(TraceEventType.Information, "Attempting to store in: {0}", file);
                File.WriteAllText(file, data);
                Engine.LogBook.Trace(TraceEventType.Information, "Done.");
            }
        }
    }
}