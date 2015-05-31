using Arleen;
using Arleen.Game;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Articus
{
    /// <summary>
    /// The Program class contains the entry point
    /// </summary>
    public static class Program
    {
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
                        "And suddently something went wrong, really wrong...\n\n{0} ocurred. \n\n == Exception Report == \n{1}\n == Stacktrace == \n{2}",
                        current.GetType().Name,
                        current.Message,
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

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Engine.Initialize();
            if (Engine.Configuration == null)
            {
                Engine.LogBook.Trace(TraceEventType.Critical, "There was no configuration lodaded... will not proceed.");
            }

            // ---

            if (args.Length == 0)
            {
                var _ = Engine.TextLocalization;
                Engine.LogBook.Trace
                (
                    TraceEventType.Information,
                    _("Hello, my name is {name}.", new { name = Engine.DisplayName })
                );

                // ---

                var realmComponents = ModuleLoader.GetComponents(typeof(Realm));
                Realm realm = null;
                foreach (var realmComponent in realmComponents)
                {
                    var tmp = ModuleLoader.Load(realmComponent);
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
            else if (args[0] == "discover")
            {
                Engine.LogBook.Trace(TraceEventType.Information, "Target: {0}.", args[1]);
                var data = JsonConvert.SerializeObject(ModuleLoader.Discover(args[1]));
                Engine.LogBook.Trace(TraceEventType.Information, "Found: {0}.", data);
                var file = args[1].Substring(0, args[1].Length - 3) + ModuleLoader.STR_Module_Extension;
                Engine.LogBook.Trace(TraceEventType.Information, "Attempting to store in: {0}.", file);
                System.IO.File.WriteAllText(file, data);
                Engine.LogBook.Trace(TraceEventType.Information, "Done.");
            }
        }
    }
}