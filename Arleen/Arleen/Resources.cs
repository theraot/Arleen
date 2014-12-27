using Newtonsoft.Json;
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
        private static readonly ResourceLoader _loader;

        static Resources()
        {
            _loader = new ResourceLoader(".json", new[] { "Resources" }, "default.json");
        }

        /// <summary>
        /// Retrieve the configuration for the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of the object to be populated with the configuration.</typeparam>
        /// <returns>A new instance of T</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T LoadConfig<T>()
        {
            var assembly = Assembly.GetCallingAssembly();
            using (var reader = new StreamReader(_loader.Read(assembly), Encoding.UTF8))
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
            var str = JsonConvert.SerializeObject(target);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return _loader.Write(assembly, stream);
            }
        }
    }
}