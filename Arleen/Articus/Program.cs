using Arleen;
using System;
using System.Diagnostics;
using Engine = Arleen.Program;

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
            if (eventArgs.ExceptionObject is Exception)
            {
                var exception = eventArgs.ExceptionObject as Exception;
                Engine.LogBook.Trace
                    (
                        TraceEventType.Critical,
                        "And suddently something went wrong, really wrong...\n == Exception Report == \n{0}\n == Stacktrace == \n{1}",
                        exception.Message,
                        exception.StackTrace
                    );
            }
            else if (eventArgs.ExceptionObject != null)
            {
                Engine.LogBook.Trace
                    (
                        TraceEventType.Critical,
                        "Help me..."
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

        private static void Main()
        {
            Engine.Initialize();

            var _ = Resources.LoadTexts();

            // Salute
            Engine.LogBook.Trace(TraceEventType.Information, _["Hello, my name is {name}."].FormatWith(new { name = Engine.DisplayName }));

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (Engine.Configuration == null)
            {
                Engine.LogBook.Trace(TraceEventType.Critical, "There was no configuration lodaded... will not proceed.");
                return;
            }

            Engine.Run(new Arleen.Game.DefaultRealm());
        }
    }
}