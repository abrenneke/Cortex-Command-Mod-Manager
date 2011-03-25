using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlConfig.Core;

namespace CortexCommandModManager
{
    public class ModScanner
    {
        private readonly Settings<SettingsObject> settings;
        private readonly ModManager modManager;

        public ModScanner(Settings<SettingsObject> settings, ModManager modManager)
        {
            this.settings = settings;
            this.modManager = modManager;
        }

        public IList<Mod> GetAllMods()
        {
            return GetEnabledMods().Concat(GetDisabledMods()).ToList();
        }

        public IList<Mod> GetEnabledMods()
        {
            return GetDirectMods(settings.Get().CCInstallDirectory);
        }

        public IList<Mod> GetDisabledMods()
        {
            return GetDirectMods(modManager.DisabledModPath);
        }

        private IList<Mod> GetDirectMods(string folder)
        {
            var directories = Directory.GetDirectories(folder, "*.rte", SearchOption.TopDirectoryOnly);
            return directories.Select(x => MakeModFromDirectory(x, folder)).ToList();
        }

        private Mod MakeModFromDirectory(string directory, string rootFolder)
        {
            var isEnabled = modManager.ModIsEnabled(directory);
            var folderName = Path.GetFileName(directory);
            var name = TryGetModName(directory) ?? folderName;

            var image = FindModImagePath(directory);
            if (image != null && !File.Exists(image))
                image = null;

            return Mod.MakeMod(directory, name, isEnabled, folderName, image);
        }

        /// <summary>Searches for a mod based on simply the folder name, e.g. browncoats.rte</summary>
        public Mod SearchForMod(string modDirectoryName)
        {
            string[] enabledDirectories = Directory.GetDirectories(Grabber.Settings.Get().CCInstallDirectory);
            bool found = false;
            bool enabled = false;
            string modDirectory = "";
            string fullDirectory = "";
            foreach (string directory in enabledDirectories)
            {
                if (directory.Contains(modDirectoryName))
                {
                    found = true;
                    enabled = true;
                    modDirectory = Path.GetFileName(directory);
                    fullDirectory = directory;
                    break;
                }
            }
            if (!found)
            {
                var disabledDirectories = Directory.GetDirectories(modManager.DisabledModPath);
                foreach (string directory in disabledDirectories)
                {
                    if (directory.Contains(modDirectoryName))
                    {
                        found = true;
                        enabled = false;
                        modDirectory = Path.GetFileName(directory);
                        fullDirectory = directory;
                        break;
                    }
                }
            }
            if (!found)
            {
                throw new FileNotFoundException("The mod " + modDirectoryName + " in the preset was not found in this intallation of Cortex Command");
            }
            string name = TryGetModName(fullDirectory) ?? Path.GetFileName(fullDirectory);
            string image = FindModImagePath(fullDirectory);
            if (image != null)
            {
                FileInfo info = new FileInfo(image);
                if (!info.Exists)
                {
                    image = null;
                }
            }
            Mod mod = Mod.MakeMod(fullDirectory, name, enabled, modDirectory, image);
            return mod;
        }

        private static string TryGetModSetting(string directory, string name)
        {
            string indexIniFile = directory + "\\Index.ini";
            if (File.Exists(indexIniFile))
            {
                try
                {
                    using (FileStream stream = new FileStream(indexIniFile, FileMode.Open))
                    {
                        StreamReader reader = new StreamReader(stream);
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            if (line.Contains(name))
                            {
                                string moduleName = line.Substring(line.IndexOf('=') + 1).Trim();
                                return moduleName;
                            }
                            line = reader.ReadLine();
                        }
                        return null;
                    }
                }
                catch (IOException)
                {
                    return null;
                }
            }
            return null;
        }
        private static string TryGetModName(string directory)
        {
            return TryGetModSetting(directory, "ModuleName");
        }

        public static string FindModImagePath(string directory)
        {
            string indexIniFile = directory + "\\Index.ini";
            if (!File.Exists(indexIniFile))
            {
                return null;
            }
            using (FileStream stream = new FileStream(indexIniFile, FileMode.Open))
            {
                StreamReader reader = new StreamReader(stream);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Contains("IconFile"))
                    {
                        string nextLine = reader.ReadLine();
                        if (nextLine.Contains("Path"))
                        {
                            string moduleName = nextLine.Substring(nextLine.IndexOf('=') + 1).Trim();
                            moduleName = moduleName.Substring(moduleName.IndexOf('\\') + 1);
                            moduleName = moduleName.Substring(moduleName.IndexOf('/') + 1);
                            moduleName = moduleName.Trim();
                            return directory + "\\" + moduleName;
                        }
                    }
                    line = reader.ReadLine();
                }
                return null;
            }
        }
    }
}
