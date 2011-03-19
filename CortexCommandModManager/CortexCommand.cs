using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager
{
    class CortexCommand
    {
        /// <summary>
        /// Returns true if the specified directory contains a valid Cortex Command installation.
        /// </summary>
        /// <param name="directory">The directory to check for a CC installation.</param>
        public static bool IsInstalledTo(string directory)
        {
            if (String.IsNullOrEmpty(directory))
                return false;
            var directoryInfo = new System.IO.DirectoryInfo(directory);
            if (!directoryInfo.Exists)
                return false;
            var files = directoryInfo.GetFiles();
            if (!files.Any(x => x.Name == "Cortex Command.exe"))
                return false;
            var directories = directoryInfo.GetDirectories();
            if (!directories.Any(x => x.Name == "Base.rte"))
                return false;
            return true;
        }
    }
}
