using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CortexCommandModManager
{
    public class ModScanner
    {
        /// <summary>
        /// All of the mods returned by GetAllMods, GetEnabledMods, or GetDisabledMods
        /// </summary>
        public List<Mod> Mods { get; private set; }

        public List<Mod> GetAllMods()
        {
            List<Mod> mods = GetEnabledMods();
            mods.AddRange(GetDisabledMods());
            Mods = mods;
            return mods;
        }
        public List<Mod> GetEnabledMods()
        {
            string mainGameFolder = Grabber.Settings.Get().CCInstallDirectory;
            List<Mod> modList = GetDirectMods(mainGameFolder);
            Mods = modList;
            return modList;
        }
        public List<Mod> GetDisabledMods()
        {
            string mainGameFolder = Grabber.Settings.Get().CCInstallDirectory;
            List<Mod> modList = GetDirectMods(mainGameFolder + "\\" + ModManager.DisabledModPath);
            Mods = modList;
            return modList;
        }
        private static List<Mod> GetDirectMods(string folder)
        {
            var directories = Directory.GetDirectories(folder, "*.rte", SearchOption.TopDirectoryOnly);
            return directories.Select(x => MakeModFromDirectory(x, folder)).ToList();
        }

        private static Mod MakeModFromDirectory(string directory, string rootFolder)
        {
            string modPath = directory;
            bool enabled = !(rootFolder.Contains(ModManager.DisabledModPath));
            string modDirectory = MakeModNameFromDirectory(directory);
            string name = TryGetModName(modPath) ?? MakeModNameFromDirectory(directory);
            string image = FindModImagePath(modPath);
            if (image != null)
            {
                FileInfo info = new FileInfo(image);
                if (!info.Exists)
                {
                    image = null;
                }
            }
            Mod mod = Mod.MakeMod(modPath, name, enabled, modDirectory, image);
            return mod;
        }
        /// <summary>
        /// Searches for a mod based on simply the folder name, e.g. browncoats.rte
        /// </summary>
        /// <param name="modDirectoryName"></param>
        /// <returns></returns>
        public static Mod SearchForMod(string modDirectoryName)
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
                    modDirectory = MakeModNameFromDirectory(directory);
                    fullDirectory = directory;
                    break;
                }
            }
            if (!found)
            {
                string[] disabledDirectories = Directory.GetDirectories(Grabber.Settings.Get().CCInstallDirectory + "\\" + ModManager.DisabledModPath);
                foreach (string directory in disabledDirectories)
                {
                    if (directory.Contains(modDirectoryName))
                    {
                        found = true;
                        enabled = false;
                        modDirectory = MakeModNameFromDirectory(directory);
                        fullDirectory = directory;
                        break;
                    }
                }
            }
            if (!found)
            {
                throw new FileNotFoundException("The mod " + modDirectoryName + " in the preset was not found in this intallation of Cortex Command");
            }
            string name = TryGetModName(fullDirectory) ?? MakeModNameFromDirectory(fullDirectory);
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

        private static string MakeModNameFromDirectory(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            return directoryInfo.Name;
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
