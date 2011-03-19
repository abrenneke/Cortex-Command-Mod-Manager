using System;
using System.Collections.Generic;
using System.IO;

namespace CortexCommandModManager
{
    public class EnhancedSkirmish
    {
        /// <summary>The name of the mod manager missions "module".</summary>
        public const string ActivitiesModuleName = "Mod Manager Generated Missions";

        /// <summary>The name of the activities folder.</summary>
        public const string ActivitiesFolderName = "zzzzCustom_Activities.rte";

        /// <summary>The name of the activities file.</summary>
        public const string ActivitiesFileName = "Activities.ini";

        /// <summary>The name of the index file for the module.</summary>
        public const string IndexFileName = "Index.ini";

        private string IndexFileTemplate
        {
            get
            {
                return new IniFileBuilder()
                    .Write("DataModule").Indent()
                        .Write("ModuleName", ActivitiesModuleName)
                        .Write("IncludeFile", ActivitiesFolderName + "/" + ActivitiesFileName)
                    .ToString();
            }
        }

        private string SkirmishFileTemplate
        {
            get 
            {
                return new IniFileBuilder()
                    .Write("AddActivity", "GABaseDefense").Indent()
                        .Write("PresetName", "Custom Defense")
                        .Write("SceneName", "Ketanot Hills")
                        .Write("Description", "This is the custom Skirmish you made in the Cortex Command Mod Manager.")
                        .Write("TeamCount", "2")
                        .Write("PlayerCount", "1")
                        .Write("TeamOfPlayer1", "0")
                        .Write("TeamOfPlayer2", "1")
                        .Write("FundsOfTeam1", "5000")
                        .Write("FundsOfTeam2", "5000")
                        .Write("CPUTeam", "1")
                        .Write("Difficulty", "3")
                        .Write("SpawnIntervalEasiest", "20000")
                        .Write("SpawnIntervalHardest", "8000")
                        .WriteLines(3)
                    .ToString();
            }
        }

        private string fullActivitiesFolderPath;

        public static List<ModBase> LoadedMods { get; set; }

        public EnhancedSkirmish()
        {
            fullActivitiesFolderPath = Grabber.Settings.Get().CCInstallDirectory + "\\" + ActivitiesFolderName;
        }

        public void RefreshCCDirectory()
        {
            fullActivitiesFolderPath = Grabber.Settings.Get().CCInstallDirectory + "\\" + ActivitiesFolderName;
        }

        /// <summary>Initializes enhanced skirmish. Should be called once per startup.</summary>
        public void Initialize()
        {
            CreateActivitiesFolderAndFilesIfDoesNotExist();
        }

        private void CreateActivitiesFolderAndFilesIfDoesNotExist()
        {
            if (ActivitiesFolderMissing())
            {
                CreateActivitiesFolder();
                PopulateActivitiesFolderWithDefault();
            }
            if (IndexFileMissing())
            {
                CreateIndexFileWithDefaultValues();
            }
            if (ActivityFileMissing())
            {
                CreateActivitiesFileWithDefaultValues();
            }
        }

        /// <summary>Returns the path to the activities file used by CCMM.</summary>
        public string GetActivitiesFile()
        {
            return fullActivitiesFolderPath + "\\" + ActivitiesFileName;
        }

        /// <summary>Gets the path to the activities file relative to the cortex command directory.</summary>
        public string GetActivitiesFileRelative()
        {
            return ActivitiesFolderName + "\\" + ActivitiesFileName;
        }

        private bool ActivityFileMissing()
        {
            return !File.Exists(fullActivitiesFolderPath + "\\" + ActivitiesFileName);
        }

        private bool IndexFileMissing()
        {
            return !File.Exists(fullActivitiesFolderPath + "\\" + IndexFileName);
        }

        private bool ActivitiesFolderMissing()
        {
            return !Directory.Exists(fullActivitiesFolderPath);
        }

        private void PopulateActivitiesFolderWithDefault()
        {
            CreateIndexFileWithDefaultValues();
            CreateActivitiesFileWithDefaultValues();
        }

        private void CreateIndexFileWithDefaultValues()
        {
            string indexFilePath = fullActivitiesFolderPath + "\\" + IndexFileName;
            try
            {
                FileStream newFile = File.Create(indexFilePath);
                using (StreamWriter newFileWriter = new StreamWriter(newFile))
                {
                    newFileWriter.Write(IndexFileTemplate);
                }
            }
            catch (IOException ex)
            {
                throw new IOException("There was a problem creating the index file for the Mod Manager Generated Missions.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("There was a problem creating the index file for the Mod Manager Generated Missions because of a security error. Try running CCMM as administrator or moving Cortex Command out of a secure directory.", ex);
            }
        }
        private void CreateActivitiesFileWithDefaultValues()
        {
            string activitiesFilePath = fullActivitiesFolderPath + "\\" + ActivitiesFileName;
            try
            {
                FileStream newFile = File.Create(activitiesFilePath);
                using (StreamWriter newFileWriter = new StreamWriter(newFile))
                {
                    newFileWriter.Write(SkirmishFileTemplate);
                }
            }
            catch (IOException ex)
            {
                throw new IOException("There was a problem creating the activities file for the Mod Manager Generated Missions.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("There was a problem creating the activities file for the Mod Manager Generated Missions because of a security error. Try running CCMM as administrator or moving Cortex Command out of a secure directory.", ex);
            }
        }

        private void CreateActivitiesFolder()
        {
            try
            {
                Directory.CreateDirectory(fullActivitiesFolderPath);
            }
            catch (IOException ex)
            {
                throw new IOException("There was a problem creating the directory for activities.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("There was a problem creating the directory for activities due to a security error. Try running CCMM as administrator or moving it out of a secure directory.", ex);
            }
        }
    }
}
