using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Arleen
{
    /// <summary>
    /// Fa�ade to save and load resources.
    /// </summary>
    internal static class ResourcesInternal
    {
        /// <summary>
        /// Retrieve the configuration for the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of the object to be populated with the configuration.</typeparam>
        /// <returns>A new instance of T</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T LoadConfig<T>()
        {
            var assembly = Assembly.GetCallingAssembly();
            Facade.Logbook.Trace
                (
                TraceEventType.Information,
                "Requested to read configuration for {0}",
                assembly.GetName().Name
            );
            // Will try to read:
            // - ~\Config\AssemblyName.json
            // - %AppData%\InternalName\Config\AssemblyName.json
            // - Assembly!Namespace.Config.default.json
            using (var reader = new StreamReader(Facade.Resources.Read(assembly, ".json", new[] { "Config" }, "default.json"), Encoding.UTF8))
            {
                var str = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(str);
            }
        }

        /// <summary>
        /// Retrieve a LocalizedTexts with the localized texts for the calling assembly.
        /// </summary>
        /// <param name="language">The language for which to load the texts.</param>
        /// <returns>a new LocalizedTexts object for the calling assembly</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TextLocalization LoadTexts(string language)
        {
            var dictionary = GetLocalizedTexts(language);
            if (dictionary == null)
            {
                // Keep the lambda notation - it is tempting to try to simplify this line... don't.
                return (format, source) => format.FormatWith(source);
            }
            return (format, source) =>
            {
                string result;
                return dictionary.TryGetValue(format, out result) ? result.FormatWith(source) : format;
            };
        }

        /// <summary>
        /// Saves the configuration for the calling assembly.
        /// </summary>
        /// <param name="target">The object containing the configuration to be stored.</param>
        /// <returns>true if the configuration was stored, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool SaveConfig<T>(T target)
        {
            var assembly = Assembly.GetCallingAssembly();
            Facade.Logbook.Trace
            (
                TraceEventType.Information,
                "Requested to write configuration for {0}",
                assembly.GetName().Name
            );
            var str = JsonConvert.SerializeObject(target);
            // Will try to write:
            // - ~\Config\AssemblyName.json
            // - %AppData%\InternalName\Config\AssemblyName.json
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return Facade.Resources.Write(assembly, "Config", ".json", stream);
            }
        }

        private static Dictionary<string, string> GetLocalizedTexts(string language)
        {
            var assembly = Assembly.GetCallingAssembly();
            Facade.Logbook.Trace
                (
                TraceEventType.Information,
                "Requested to read localized texts for {0}",
                assembly.GetName().Name
            );
            // Will try to read:
            // - ~\Lang\langcode\AssemblyName.json
            // - %AppData%\InternalName\Lang\langcode\AssemblyName.json
            // - Assembly!Namespace.Lang.langcode.json

            var languageArray = language.Split('-');

            var prefixes = new List<string>();

            var composite = new StringBuilder();

            foreach (var sublanguage in languageArray)
            {
                if (composite.Length > 0)
                {
                    composite.Append("-");
                }
                composite.Append(sublanguage.Trim());
                prefixes.Add("Lang." + composite);
            }

            prefixes.Reverse();

            var stream = Facade.Resources.Read(assembly, ".json", prefixes.ToArray(), "json");
            if (stream == null)
            {
                Facade.Logbook.Trace
                    (
                    TraceEventType.Information,
                    "No localized texts for {0}",
                    assembly.GetName().Name
                );
                return null;
            }
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var str = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            }
        }
    }
}