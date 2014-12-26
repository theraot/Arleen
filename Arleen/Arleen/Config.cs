using Newtonsoft.Json;
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

        private static string GetJson(Assembly assembly)
        {
            var folder = Program.Folder;
            string json;
            if (TryReadJson(folder, assembly, out json))
            {
                return json;
            }
            var applicationDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
                                + System.IO.Path.DirectorySeparatorChar
                                + Program.InternalName;
            if (applicationDataFolder != folder)
            {
                if (TryReadJson(applicationDataFolder, assembly, out json))
                {
                    return json;
                }
            }
            if (TryReadDefaultJson(assembly, out json))
            {
                return json;
            }
            return "null";
        }

        private static bool SetJson(Assembly assembly, string json)
        {
            var folder = Program.Folder;
            if (TryWriteJson(folder, assembly, json))
            {
                return true;
            }
            var applicationDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
                                + System.IO.Path.DirectorySeparatorChar
                                + Program.InternalName;
            if (applicationDataFolder != folder)
            {
                if (TryWriteJson(applicationDataFolder, assembly, json))
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
            const string STR_ResourceName = "default.json";

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
            var path = basepath + "configuration" + System.IO.Path.DirectorySeparatorChar + assembly.GetName().Name + ".json";
            try
            {
                json = System.IO.File.ReadAllText(path);
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
            var path = basepath + "configuration" + System.IO.Path.DirectorySeparatorChar + assembly.GetName().Name + ".json";
            try
            {
                System.IO.File.WriteAllText(path, json);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}