using System.IO;
using System;

namespace CortexCommandModManager
{
    public static class ModManager
    {
        /// <summary>
        /// Checks if the disabled mods folder exists and creates it if it doesn't.
        /// </summary>
        [Obsolete("Shouldn't be static")]
        public static void CheckDisabledFolderExists()
        {
            if (!Directory.Exists(Grabber.Settings.Get().CCInstallDirectory + "\\" + Constants.DisabledModPath))
            {
                Directory.CreateDirectory(Grabber.Settings.Get().CCInstallDirectory + "\\" + Constants.DisabledModPath);
            }
        }
        /// <summary>
        /// Enables a mod and returns the new location of the mod
        /// </summary>
        /// <param name="mod">The mod to enable</param>
        /// <returns>The new location of the mod</returns>
        public static string EnableMod(Mod mod)
        {
            DirectoryInfo modCurrentFolder = new DirectoryInfo(mod.FullFolderPath);
            string myDirectory = modCurrentFolder.Name;

            string source = mod.FullFolderPath;
            string destination = Grabber.Settings.Get().CCInstallDirectory + "\\" + myDirectory;
            MoveMod(source, destination);
            InvalidateCache();
            return destination;
        }
        /// <summary>
        /// Disables a mod and returns the new location of the mod
        /// </summary>
        /// <param name="mod">The mod to disable</param>
        /// <returns>The new location of the mod</returns>
        public static string DisableMod(Mod mod)
        {
            DirectoryInfo modCurrentFolder = new DirectoryInfo(mod.FullFolderPath);
            string myDirectory = modCurrentFolder.Name;

            string source = mod.FullFolderPath;
            string destination = Grabber.Settings.Get().CCInstallDirectory + "\\" + Constants.DisabledModPath + "\\" + myDirectory;

            MoveMod(source, destination);
            InvalidateCache();
            return destination;
        }
        private static void MoveMod(string source, string destination)
        {
            Directory.Move(source, destination);
        }
        private static void InvalidateCache()
        {
            if (Grabber.ActivityItemCache != null)
            {
                Grabber.ActivityItemCache.InvalidateCache();
            }
        }

        public static void DeleteMod(Mod mod)
        {
            Directory.Delete(mod.FullFolderPath, true);
        }
    }
}
