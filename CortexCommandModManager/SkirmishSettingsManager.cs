using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CortexCommandModManager
{
    public class SkirmishSettingsManager
    {
        private const string SkirmishSettingsFileRelative = @"Missions.rte\Skirmish Activities.ini";
        private string skirmishSettingsFile;

        private const string Team1Funds = "FundsOfTeam1";
        private const string Team2Funds = "FundsOfTeam2";

        private const string SpawnIntervalEasiestSetting = "SpawnIntervalEasiest";
        private const string SpawnIntervalHardestSetting = "SpawnIntervalHardest";

        private IniSettingFile settingFile;

        public SkirmishSettingsManager()
        {
            RefreshSettingsLocation();
        }

        private void RefreshSettingsLocation()
        {
            var ccDirectory = Grabber.Settings.Get().CCInstallDirectory;
            var activityFile = string.Format("{0}\\{1}\\{2}", ccDirectory, EnhancedSkirmish.ActivitiesFolderName, EnhancedSkirmish.ActivitiesFileName);
            settingFile = new IniSettingFile(activityFile);
        }

        public int InitialP1Money
        {
            get { return getNumeric(Team1Funds); }
            set { setSetting(Team1Funds, value.ToString()); }
        }
        public int InitialP2Money
        {
            get { return getNumeric(Team2Funds); }
            set { setSetting(Team2Funds, value.ToString()); }
        }
        public int SpawnIntervalEasiest
        {
            get { return getNumeric(SpawnIntervalEasiestSetting); }
            set { setSetting(SpawnIntervalEasiestSetting, value.ToString()); }
        }
        public int SpawnIntervalHardest
        {
            get { return getNumeric(SpawnIntervalHardestSetting); }
            set { setSetting(SpawnIntervalHardestSetting, value.ToString()); }
        }

        private int getNumeric(string setting)
        {
            try
            {
                return Int32.Parse(getSetting(setting));
            }
            catch (FormatException ex)
            {
                throw new FormatException("The setting value was not numeric. Most likely the settings file has become corrupted.", ex);
            }
        }
        private string getSetting(string setting)
        {
            RefreshSettingsLocation();
            return settingFile.Get(setting);
        }
        private void setSetting(string setting, string value)
        {
            RefreshSettingsLocation();
            settingFile.Set(setting, value);
        }
    }
}
