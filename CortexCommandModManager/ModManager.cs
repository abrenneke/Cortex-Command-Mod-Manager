using System.IO;
using System;
using System.Threading;
using CortexCommandModManager.Startup;
using XmlConfig.Core;

namespace CortexCommandModManager
{
    public class ModManager
    {
        public const string DisabledModsFolderName = "_disabled_mods";

        private const string OldDisabledModsFolderName = "Disabled_Mods";

        public string DisabledModPath { get { return Path.Combine(settings.Get().CCInstallDirectory, DisabledModsFolderName); } }
        
        private string OldDisabledModPath { get { return Path.Combine(settings.Get().CCInstallDirectory, OldDisabledModsFolderName); } }

        private readonly Settings<SettingsObject> settings;

        public ModManager(Settings<SettingsObject> settings)
        {
            this.settings = settings;
        }

        /// <summary>Checks if the disabled mods folder exists and creates it if it doesn't.</summary>
        public void CheckDisabledFolderExists()
        {
            if (!Directory.Exists(DisabledModPath))
                Directory.CreateDirectory(DisabledModPath);
        }

        /// <summary>Renames the old disabled mod directory name to the new one, if needed.</summary>
        public void RenameDisabledFolderIfNeeded()
        {
            if (Directory.Exists(OldDisabledModPath))
                Directory.Move(OldDisabledModPath, DisabledModPath);
        }

        /// <summary>Enables a mod and returns the new location of the mod</summary>
        public void EnableMod(Mod mod)
        {
            var source = mod.FullFolderPath;
            var destination = Path.Combine(settings.Get().CCInstallDirectory, mod.Folder);

            Directory.Move(source, destination);

            InvalidateCache();

            mod.IsEnabled = true;
            mod.FullFolderPath = destination;
            mod.IconPath = ModScanner.FindModImagePath(destination);
        }

        /// <summary>Disables a mod and returns the new location of the mod</summary>
        public void DisableMod(Mod mod)
        {
            var source = mod.FullFolderPath;
            var destination = Path.Combine(DisabledModPath, mod.Folder);

            Directory.Move(source, destination);

            InvalidateCache();

            mod.IsEnabled = false;
            mod.FullFolderPath = destination;
            mod.IconPath = ModScanner.FindModImagePath(destination);
        }

        /// <summary>Toggles the enabled state of the mod.</summary>
        public void ToggleEnabled(Mod mod)
        {
            if (mod.IsEnabled)
                DisableMod(mod);
            else
                EnableMod(mod);
        }

        public bool ModIsEnabled(string modDirectoryPath)
        {
            return modDirectoryPath.Contains(DisabledModsFolderName) == false;
        }

        private static void InvalidateCache()
        {
            if (Grabber.ActivityItemCache != null)
            {
                Grabber.ActivityItemCache.InvalidateCache();
            }
        }

        public void DeleteMod(Mod mod)
        {
            DeleteDirectory(mod.FullFolderPath);
        }

        private static void DeleteDirectory(string directory)
        {
            var files = Directory.GetFiles(directory);
            var directories = Directory.GetDirectories(directory);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var subDirectory in directories)
            {
                DeleteDirectory(subDirectory);
            }

            try
            {
                Directory.Delete(directory, false);
            }
            catch (IOException)
            {
                Thread.Sleep(0);
                Directory.Delete(directory, false);
            }
        }
    }
}
