using System;
using System.Diagnostics;
using System.Threading;

namespace Arleen
{
    public static class Facade
    {
        private static FacadeCore _instance;

        public static AppDomain AppDomain
        {
            get
            {
                return Instance.AppDomain;
            }
        }

        public static Configuration Configuration
        {
            get
            {
                return Instance.Configuration;
            }
        }

        public static TextLocalization TextLocalization
        {
            get
            {
                return Instance.TextLocalization;
            }
        }

        public static Logbook Logbook
        {
            get
            {
                return Instance.Logbook;
            }
        }

        public static Resources Resources
        {
            get
            {
                return Instance.Resources;
            }
        }

        public static string SystemLanguage
        {
            get
            {
                return Instance.SystemLanguage;
            }
        }

        private static FacadeCore Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Initialization is not done");
                }
                return _instance;
            }
        }

        [Obsolete("Do not set the Facade instance - intended for internal use")]
        public static FacadeCore Set(FacadeCore instance)
        {
            // Allow to write only if _instance is null
            return Interlocked.CompareExchange(ref _instance, instance, null);
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

        private static Configuration GetConfiguration()
        {
            Configuration result = null;
            try
            {
                result = ResourcesInternal.LoadConfig<Configuration>();
            }
            catch (Exception exception)
            {
                Logbook.ReportException(exception, true);
            }
            if (result == null)
            {
                Logbook.Trace(TraceEventType.Critical, "No configuration lodaded.");
                result = new Configuration {
                    ForceDebugMode = Engine.DebugMode,
                };
            }
            if (string.IsNullOrEmpty(result.DisplayName))
            {
                result.DisplayName = Engine.InternalName;
            }
            if (string.IsNullOrEmpty(result.Language))
            {
                result.Language = SystemLanguage;
            }
            if (result.Resolution.IsEmpty)
            {
                result.Resolution = new System.Drawing.Size(800, 600); // VGA
            }
            return result;
        }
    }
}