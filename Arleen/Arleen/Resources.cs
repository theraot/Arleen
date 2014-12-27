﻿using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Arleen
{
    /// <summary>
    /// Façade to save and load resources.
    /// </summary>
    public static class Resources
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
            Logbook.Instance.Trace
                (
                    TraceEventType.Information,
                    "Requested to read configuration for {0}",
                    assembly.GetName().Name
                );
            // Will try to read:
            // - ~\Config\AssemblyName.json
            // - %AppData%\InternalName\Config\AssemblyName.json
            // - Assembly!Namespace.Config.default.json
            using (var reader = new StreamReader(ResourceLoader.Read(assembly, ".json", new[] { "Config" }, "default.json"), Encoding.UTF8))
            {
                var str = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(str);
            }
        }

        /// <summary>
        /// Saves the configuration for the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of the object to be populated with the configuration.</typeparam>
        /// <param name="target">The object containing the configuration to be stored.</param>
        /// <returns>true if the configuration was stored, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool SaveConfig<T>(T target)
        {
            var assembly = Assembly.GetCallingAssembly();
            Logbook.Instance.Trace
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
                return ResourceLoader.Write(assembly, "Config", ".json", stream);
            }
        }
    }
}