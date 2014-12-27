using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Arleen
{
    /// <summary>
    /// Use ResourceLoader to read and write resources.
    /// </summary>
    internal class ResourceLoader
    {
        private readonly string _extension;
        private readonly string _folder;
        private readonly string _resourceName;

        /// <summary>
        /// Creates a new instance of ResourceLoader.
        /// </summary>
        /// <param name="extension">The extension for the resources.</param>
        /// <param name="folder">The name of the folder for the resources.</param>
        /// <param name="resourceName">The name of the internal resource used as default.</param>
        internal ResourceLoader(string extension, string folder, string resourceName)
        {
            _extension = extension;
            _folder = folder;
            _resourceName = resourceName;
        }

        /// <summary>
        /// Reads a resource as an stream.
        /// </summary>
        /// <param name="assembly">The assembly the resource is associated with.</param>
        /// <returns>A readable stream for the resource.</returns>
        public Stream Read(Assembly assembly)
        {
            Stream stream;

            foreach (var configurationStorageFolder in GetConfigurationStorageFolders())
            {
                if (TryReadStream(configurationStorageFolder, assembly, out stream))
                {
                    return stream;
                }
            }

            return TryReadDefaultJson(assembly, out stream) ? stream : null;
        }

        /// <summary>
        /// Writes a resource stream.
        /// </summary>
        /// <param name="assembly">The assembly the resource is associated with.</param>
        /// <param name="stream">A readable stream with the resource to be written.</param>
        /// <returns>true if the resource was written, false otherwise.</returns>
        public bool Write(Assembly assembly, Stream stream)
        {
            foreach (var configurationStorageFolder in GetConfigurationStorageFolders())
            {
                if (TryWriteStream(configurationStorageFolder, assembly, stream))
                {
                    return true;
                }
            }

            return false;
        }

        private static void CopyStream(Stream input, Stream output)
        {
            const int INT_Length = 4096;
            var buffer = new byte[4096];
            for (var index = input.Read(buffer, 0, INT_Length); index > 0; index = input.Read(buffer, 0, INT_Length))
            {
                output.Write(buffer, 0, index);
            }
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

        private static bool TryProcessResource(Assembly assembly, string resource, out Stream stream)
        {
            stream = assembly.GetManifestResourceStream(resource);
            return stream != null;
        }

        private bool TryReadDefaultJson(Assembly assembly, out Stream stream)
        {
            var resources = assembly.GetManifestResourceNames();
            var selectedResources = new List<string>();

            foreach (var resource in resources)
            {
                if (resource.EndsWith(_resourceName))
                {
                    selectedResources.Add(resource);
                }
            }

            selectedResources.Sort((left, right) => left.Length.CompareTo(right.Length));

            foreach (var resource in selectedResources)
            {
                if (TryProcessResource(assembly, resource, out stream))
                {
                    return true;
                }
            }

            stream = null;
            return false;
        }

        private bool TryReadStream(string basepath, Assembly assembly, out Stream stream)
        {
            var path = basepath + _folder + Path.DirectorySeparatorChar + assembly.GetName().Name + _extension;
            try
            {
                stream = File.OpenRead(path);
                return true;
            }
            catch (IOException)
            {
                stream = null;
                return false;
            }
        }

        private bool TryWriteStream(string basepath, Assembly assembly, Stream stream)
        {
            var path = basepath + _folder + Path.DirectorySeparatorChar + assembly.GetName().Name + _extension;
            try
            {
                using (var file = File.OpenWrite(path))
                {
                    CopyStream(stream, file);
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}