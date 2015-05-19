using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Arleen
{
    /// <summary>
    /// Façade to save and load resources.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        /// Retrieves a bitmap from the resources for the calling assembly.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Bitmap LoadBitmap(string resourceName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var stream = ResourceLoader.Read(assembly, Path.DirectorySeparatorChar + resourceName, new[] { "Images" }, resourceName);
            return new Bitmap(stream);
        }
    }
}