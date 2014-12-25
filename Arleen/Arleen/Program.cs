using System.Globalization;
using System.IO;

namespace Arleen
{
    public static class Program
    {
        private static string _directorySeparator;
        private static string _displayName;
        private static string _folder;

        /// <summary>
        /// Returns the display name for the Program.
        /// </summary>
        /// <remarks>The display name is the simple name of the assembly, that is "Arleen"</remarks>
        public static string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        /// <summary>
        /// Gets the path to the folder from where Arleen is loaded
        /// </summary>
        public static string Folder
        {
            get
            {
                return _folder;
            }
        }

        /// <summary>
        /// Get a string with the directory separator for the current system.
        /// </summary>
        /// <returns>The directory separator for the current system as a string.</returns>
        private static string GetDirectorySeparator()
        {
            // At this point we don't worry about threading
            return _directorySeparator ?? (_directorySeparator = System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Initializing fiels with metadata about the the assembly
        /// </summary>
        private static void Initialize()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var location = assembly.Location;
            _folder = Path.GetDirectoryName(location);
            if (_folder == null)
            {
                // Failed to get permision to query the file storage system.
            }
            else if (!_folder.EndsWith(GetDirectorySeparator()))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                _folder += System.IO.Path.DirectorySeparatorChar;
            }
            _displayName = assembly.GetName().Name;
        }

        /// <summary>
        /// Entry point
        /// </summary>
        private static void Main()
        {
            Initialize();
        }
    }
}