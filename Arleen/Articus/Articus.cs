using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Articus
{
    public static class Articus
    {
        private static string _folder;
        private static string _location;

        public static string Folder
        {
            get
            {
                if (_folder == null)
                {
                    DiscoverPath();
                }
                return _folder;
            }
        }

        public static string Location
        {
            get
            {
                if (_location == null)
                {
                    DiscoverPath();
                }
                return _location;
            }
        }

        private static void DiscoverPath()
        {
            _location = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            _folder = Path.GetDirectoryName(_location);
            // Let this method throw if folder is null
            if (!_folder.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                // On Windows, if you run from the root directoy it will have a trailing directory separator but will not otherwise... so we addd it
                _folder += Path.DirectorySeparatorChar;
            }
        }
    }
}