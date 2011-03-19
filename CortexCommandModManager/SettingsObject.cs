namespace CortexCommandModManager
{
    public class SettingsObject
    {
        #region Global Settings

        public string CCInstallDirectory { get; set; }
        public string LastActivityTitle { get; set; }
        public string LastActivityDescription { get; set; }

        private int windowHeight = 459;
        public int WindowWidth { get { return windowHeight; } set { windowHeight = value; } }

        private int windowWidth = 493;
        public int WindowHeight { get { return windowWidth; } set { windowWidth = value; } }

        public int RandomSettingsMinCraft { get; set; }
        public int RandomSettingsMaxCraft { get; set; }
        public int RandomSettingsMinActors { get; set; }
        public int RandomSettingsMaxActors { get; set; }
        public int RandomSettingsMinWeapons { get; set; }
        public int RandomSettingsMaxWeapons { get; set; }
        public bool RandomSettingsCrabsHoldWeapons { get; set; }
        public bool RandomSettingsInitialized { get; set; }

        #endregion
    }
}
