using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Arleen
{
    /// <summary>
    /// Use ResourceLoader to read and write resources.
    /// </summary>
    internal static class ResourceLoader
    {
        /// <summary>
        /// Reads a resource as an stream.
        /// </summary>
        /// <param name="assembly">The assembly the resource is associated with.</param>
        /// <param name="extension">The extension of the resource to load.</param>
        /// <param name="prefixes">A list of valid prefixes for the resource to load.</param>
        /// <param name="resourceName">The name of the resource to load.</param>
        /// <returns>A readable stream for the resource.</returns>
        public static Stream Read(Assembly assembly, string extension, string[] prefixes, string resourceName)
        {
            Stream stream;

            foreach (var folder in GetFolders(prefixes))
            {
                if (TryReadStream(folder, extension, assembly, out stream))
                {
                    return stream;
                }
            }

            return TryReadDefaultStream(assembly, prefixes, resourceName, out stream) ? stream : null;
        }

        /// <summary>
        /// Writes a resource stream.
        /// </summary>
        /// <param name="assembly">The assembly the resource is associated with.</param>
        /// <param name="prefix">The prefix where to store the resource.</param>
        /// <param name="resourceName">The name of the resource to write.</param>
        /// <param name="stream">A readable stream with the resource to be written.</param>
        /// <returns>true if the resource was written, false otherwise.</returns>
        public static bool Write(Assembly assembly, string prefix, string resourceName, Stream stream)
        {
            foreach (var folder in GetFolders(new[] { prefix }))
            {
                if (TryWriteStream(folder, resourceName, assembly, stream))
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

        private static IEnumerable<string> GetFolders(string[] prefixes)
        {
            var folders = new List<string> { Engine.Folder };
            var optional = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                         + Path.DirectorySeparatorChar
                         + Engine.InternalName
                         + Path.DirectorySeparatorChar;
            if (optional != Engine.Folder)
            {
                folders.Add(optional);
            }
            foreach (var folder in folders)
            {
                foreach (var prefix in prefixes)
                {
                    yield return folder + prefix.Replace('.', Path.DirectorySeparatorChar);
                }
            }
        }

        private static bool TryProcessResource(Assembly assembly, string resource, out Stream stream)
        {
            stream = assembly.GetManifestResourceStream(resource);
            return stream != null;
        }

        private static bool TryReadDefaultStream(Assembly assembly, IEnumerable<string> prefixes, string resourceName, out Stream stream)
        {
            var resources = assembly.GetManifestResourceNames();
            var selectedResources = new List<string>();

            foreach (var prefix in prefixes)
            {
                foreach (var resource in resources)
                {
                    if (resource.EndsWith(prefix + "." + resourceName))
                    {
                        selectedResources.Add(resource);
                    }
                }

                selectedResources.Sort((left, right) => left.Length.CompareTo(right.Length));

                foreach (var resource in selectedResources)
                {
                    if (TryProcessResource(assembly, resource, out stream))
                    {
                        Logbook.Instance.Trace
                        (
                            TraceEventType.Information,
                            " - Loaded internal resource {0}",
                            resource
                        );
                        return true;
                    }
                }

                selectedResources.Clear();
            }

            stream = null;
            return false;
        }

        private static bool TryReadStream(string basepath, string extension, Assembly assembly, out Stream stream)
        {
            var path = basepath + Path.DirectorySeparatorChar + assembly.GetName().Name + extension;
            try
            {
                Logbook.Instance.Trace
                (
                    TraceEventType.Information,
                    " - Attempting to read from {0}",
                    path
                );
                stream = File.OpenRead(path);
                Logbook.Instance.Trace
                (
                    TraceEventType.Information,
                    " - Succeed to read from {0}",
                    path
                );
                return true;
            }
            catch (IOException exception)
            {
                Logbook.Instance.ReportException(exception, "trying to read resource", false);
                stream = null;
                return false;
            }
        }

        private static bool TryWriteStream(string basepath, string resourceName, Assembly assembly, Stream stream)
        {
            var path = basepath + Path.DirectorySeparatorChar + assembly.GetName().Name + resourceName;
            try
            {
                Logbook.Instance.Trace
                (
                    TraceEventType.Information,
                    " - Attempting to write to {0}",
                    path
                );
                Directory.CreateDirectory(basepath);
                using (var file = File.Open(path, FileMode.Create))
                {
                    CopyStream(stream, file);
                }
                Logbook.Instance.Trace
                (
                    TraceEventType.Information,
                    " - Succeed to write to {0}",
                    path
                );
                return true;
            }
            catch (IOException exception)
            {
                Logbook.Instance.ReportException(exception, "trying to write resource", false);
                return false;
            }
        }
    }
}