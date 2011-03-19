using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CortexCommandModManager
{
    public static class PresetManager
    {
        private static List<string> presetModsBuffer;
        private const string PresetsFolder = "\\Presets";
        private const string PresetFileExtension = ".ccmmp";

        public static IList<Preset> GetAllPresets()
        {
            CheckForPresetsFolder();
            return GetAllPresetsFromFolder();
        }

        public static void AddModToPreset(Mod mod, Preset preset)
        {
            if (!ModIsInAPreset(mod))
            {
                preset.Add(mod);
                if (mod.IsEnabled != preset.IsEnabled)
                {
                    mod.ToggleEnabled();
                }
                PresetManager.SavePreset(preset);
                LoadModsBuffer();
            }
        }


        private static void LoadModsBuffer()
        {
            var presets = GetAllPresets();
            List<string> modsList = new List<string>();
            foreach (Preset preset in presets)
            {
                foreach (Mod mod in preset)
                {
                    modsList.Add(mod.Folder);
                }
            }
            presetModsBuffer = modsList;
        }
        public static bool ModIsInAPreset(Mod mod)
        {
            if (presetModsBuffer == null)
            {
                LoadModsBuffer();
            }
            if (presetModsBuffer.Contains(mod.Folder))
            {
                return true;
            }
            return false;
        }

        public static Preset RenamePreset(Preset preset, string newname)
        {
            DeletePreset(preset);
            Preset newPreset = new Preset(newname, preset.IsEnabled);
            newPreset.AddRange(preset);
            CreatePresetFile(newPreset);
            LoadModsBuffer();
            return newPreset;
        }
        public static void RemoveModFromPreset(Mod mod, Preset preset)
        {
            for (int i = 0; i < (new List<Mod>(preset)).Count; i++)
            {
                if (preset[i].Folder == mod.Folder)
                {
                    preset.RemoveAt(i);
                    break;
                }
            }
            SavePreset(preset);
        }

        public static void SavePreset(Preset preset)
        {
            if (PresetFileExists(preset))
            {
                SavePresetInternal(preset);
            }
            else
            {
                CreatePresetFile(preset);
            }
            LoadModsBuffer();
        }
        private static bool PresetFileExists(Preset preset)
        {
            return File.Exists(GetPresetFullFile(preset));
        }
        private static void CreatePresetFile(Preset preset)
        {
             StreamWriter writer = new StreamWriter(File.Create(GetPresetFullFile(preset)));
            string fileText = PresetToFileText(preset);
            writer.Write(fileText);
            writer.Close();
        }
        private static void SavePresetInternal(Preset preset)
        {
            DeletePreset(preset);
            CreatePresetFile(preset);
        }

        private static void DeletePreset(Preset preset)
        {
            File.Delete(GetPresetFullFile(preset));
        }
        private static string PresetToFileText(Preset preset)
        {
            string text;
            using (StringWriter stringWriter = new StringWriter())
            {
                stringWriter.WriteLine(preset.Name);
                stringWriter.WriteLine(preset.IsEnabled.ToString());
                foreach (Mod mod in preset)
                {
                    stringWriter.WriteLine(mod.Folder);
                }
                text = stringWriter.ToString();
            }
            return text;
        }
        private static string GetPresetFullFile(Preset preset)
        {
            return Grabber.ModManagerDirectory + PresetsFolder + "\\" + GetPresetFileName(preset);
        }
        private static string GetPresetFileName(Preset preset)
        {
            var charactersReplaced = preset.Name.Replace(' ', '-').Replace('.', '-');
            var symbolsReplaced = Regex.Replace(charactersReplaced, "[^a-zA-Z0-9-]", "");
            return symbolsReplaced + PresetFileExtension;
        }
        private static List<Preset> GetAllPresetsFromFolder()
        {
            string[] files = Directory.GetFiles(Grabber.ModManagerDirectory + PresetsFolder);
            List<Preset> presetList = new List<Preset>();
            foreach (string file in files)
            {
                Preset preset = GetPresetFromFile(file);
                presetList.Add(preset);
            }
            return presetList;
        }

        private static Preset GetPresetFromFile(string file)
        {
            Preset preset;
            using (StreamReader reader = new StreamReader(file))
            {
                string name = reader.ReadLine();
                string enabledString = reader.ReadLine();
                bool enabled = Boolean.Parse(enabledString);
                preset = new Preset(name, enabled);
                string line = reader.ReadLine();
                while (line != null)
                {
                    try
                    {
                        preset.Add(GetModDetailsFromName(line));
                    }
                    catch (FileNotFoundException) { }
                    line = reader.ReadLine();
                }
            }
            return preset;
        }

        private static Mod GetModDetailsFromName(string name)
        {
            return ModScanner.SearchForMod(name);
        }

        private static void CheckForPresetsFolder()
        {
            if (!Directory.Exists(Grabber.ModManagerDirectory + PresetsFolder))
            {
                Directory.CreateDirectory(Grabber.ModManagerDirectory + PresetsFolder);
            }
        }

        public static void DisbandPreset(Preset preset)
        {
            DeletePreset(preset);
            LoadModsBuffer();
        }

        public static void UpdatePreset(Preset preset)
        {
            SavePresetInternal(preset);
            LoadModsBuffer();
        }
    }
}
