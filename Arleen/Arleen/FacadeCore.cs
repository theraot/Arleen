using System;
using System.Diagnostics;
using System.Globalization;

namespace Arleen
{
    public sealed class FacadeCore : MarshalByRefObject
    {
        private FacadeCore()
        {
            // Empty
        }

        public AppDomain AppDomain { get; private set; }

        public Configuration Configuration { get; private set; }

        public Logbook Logbook { get; private set; }

        public Resources Resources { get; private set; }

        public string SystemLanguage { get; private set; }

        public TextLocalization TextLocalization { get; private set; }

        [Obsolete("Do not create Facade instances - intended for internal use")]
        public static FacadeCore Initialize(string name, bool set)
        {
            var result = new FacadeCore();
            if (set)
            {
                var found = Facade.Set(result);
                if (found != null)
                {
                    return found;
                }
            }
            // Take the AppDomain first
            result.AppDomain = AppDomain.CurrentDomain;
            // Always create logbook first
            result.Logbook = Logbook.Create(Engine.DebugMode ? SourceLevels.All : SourceLevels.Information, true, name);
            // We get the culture name via TextInfo because it always includes the region.
            // If we get the name of the culture directly it will only have the region if it is not the default one.
            result.SystemLanguage = CultureInfo.CurrentCulture.TextInfo.CultureName;
            // The only reason Resources is singleton is to ensure it runs on the correct AppDomain.
            // We need to load Resources before we attempt to read configuration.
            result.Resources = new Resources();
            // Load configuration after the current language is known.
            result.Configuration = result.GetConfiguration();
            // Load text localization once we have configuration and resources loaded.
            result.TextLocalization = ResourcesInternal.LoadTexts(result.Configuration.Language);
            return result;
        }
        private Configuration GetConfiguration()
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
                    ForceDebugMode = Engine.DebugMode
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