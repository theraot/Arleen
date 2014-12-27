using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Arleen
{
    /// <summary>
    /// Façade to save and load configuration.
    /// </summary>
    public static class Config
    {
        private const string STR_Extension = ".json";
        private const string STR_Folder = "configuration";
        private const string STR_ResourceName = "default.json";

        /// <summary>
        /// Retrieve the configuration for the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of the object to be populated with the configuration.</typeparam>
        /// <returns>A new instance of T</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Load<T>()
        {
            var assembly = Assembly.GetCallingAssembly();
            var json = GetJson(assembly);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Saves the configuration for the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of the object to be populated with the configuration.</typeparam>
        /// <param name="target">The object containing the configuration to be stored.</param>
        /// <returns>true if the configuration was stored, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Save<T>(T target)
        {
            var assembly = Assembly.GetCallingAssembly();
            var json = JsonConvert.SerializeObject(target);
            return SetJson(assembly, json);
        }

        private static IEnumerable<string> GetConfigurationStorageFolders()
        {
            var first = Program.Folder;
            yield return first;
            var second = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                         + Path.DirectorySeparatorChar
                         + Program.InternalName;
            if (first != second)
            {
                yield return second;
            }
        }

        private static string GetJson(Assembly assembly)
        {
            string json;

            foreach (var configurationStorageFolder in GetConfigurationStorageFolders())
            {
                if (TryReadJson(configurationStorageFolder, assembly, out json))
                {
                    return json;
                }
            }

            return TryReadDefaultJson(assembly, out json) ? json : "null";
        }

        private static bool SetJson(Assembly assembly, string json)
        {
            foreach (var configurationStorageFolder in GetConfigurationStorageFolders())
            {
                if (TryWriteJson(configurationStorageFolder, assembly, json))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryProcessResource(Assembly assembly, string resource, out string json)
        {
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    json = null;
                    return false;
                }
                var reader = new StreamReader(stream);
                json = reader.ReadToEnd();
                return true;
            }
        }

        private static bool TryReadDefaultJson(Assembly assembly, out string json)
        {
            var resources = assembly.GetManifestResourceNames();
            var selectedResources = new List<string>();

            foreach (var resource in resources)
            {
                if (resource.EndsWith(STR_ResourceName))
                {
                    selectedResources.Add(resource);
                }
            }

            selectedResources.Sort((left, right) => left.Length.CompareTo(right.Length));

            foreach (var resource in selectedResources)
            {
                if (TryProcessResource(assembly, resource, out json))
                {
                    return true;
                }
            }

            json = null;
            return false;
        }

        private static bool TryReadJson(string basepath, Assembly assembly, out string json)
        {
            var path = basepath + STR_Folder + Path.DirectorySeparatorChar + assembly.GetName().Name + STR_Extension;
            try
            {
                json = File.ReadAllText(path);
                return true;
            }
            catch (IOException)
            {
                json = null;
                return false;
            }
        }

        private static bool TryWriteJson(string basepath, Assembly assembly, string json)
        {
            var path = basepath + STR_Folder + Path.DirectorySeparatorChar + assembly.GetName().Name + STR_Extension;
            try
            {
                File.WriteAllText(path, json);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}