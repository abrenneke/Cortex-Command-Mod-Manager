using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager
{
    public class GameSettingsManager
    {
        private const string GameSettingsFileRelative = @"Base.rte\Settings.ini";
        private string GameSettingsFile { get { return Grabber.Settings.Get().CCInstallDirectory + "\\" + GameSettingsFileRelative; } }
        private IniSettingFile gameSettingsIniFile;

        private const string ResolutionXSetting = "ResolutionX";
        private const string ResolutionYSetting = "ResolutionY";
        private const string FullscreenSetting = "Fullscreen";

        public GameSettingsManager()
        {
            gameSettingsIniFile = new IniSettingFile(GameSettingsFile);
        }

        public int ResolutionX
        {
            get { return getNumeric(ResolutionXSetting); }
            set { gameSettingsIniFile.Set(ResolutionXSetting, value.ToString()); }
        }
        public int ResolutionY
        {
            get { return getNumeric(ResolutionYSetting); }
            set { gameSettingsIniFile.Set(ResolutionYSetting, value.ToString()); }
        }
        public bool Fullscreen
        {
            get { return gameSettingsIniFile.Get(FullscreenSetting) == "1"; }
            set { gameSettingsIniFile.Set(FullscreenSetting, value ? "1" : "0"); }
        }
        private int getNumeric(string setting)
        {
            try
            {
                return Int32.Parse(gameSettingsIniFile.Get(setting));
            }
            catch (FormatException ex)
            {
                throw new FormatException("The setting value was not numeric. Most likely the settings file has become corrupted.", ex);
            }
        }
    }
}
