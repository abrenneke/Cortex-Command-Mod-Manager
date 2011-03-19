namespace CortexCommandModManager
{
    static class Constants
    {
        public const string DisabledModPath = "Disabled_Mods";
        private static readonly string[] originalInstallationMods = new[] {"Base.rte","Browncoats.rte","Coalition.rte","Dummy.rte",
                                              "Missions.rte","Ronin.rte","Tutorial.rte","Undead.rte",
                                              "Whitebots.rte","Wildlife.rte"};
        public static string[] OriginalInstallationMods
        {
            get { return originalInstallationMods; }
        } 
    }
}
