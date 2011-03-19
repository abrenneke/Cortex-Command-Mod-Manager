using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager
{
    public static class PreinstalledMods
    {
        private static readonly string[] PreinstalledModsList = new string[] 
        {
            "Base.rte",
            "Browncoats.rte",
            "Coalition.rte",
            "Dummy.rte",
            "Missions.rte",
            "Ronin.rte",
            "Tutorial.rte",
            "Undead.rte",
            "Whitebots.rte",
            "Wildlife.rte"
        };

        private static readonly string[] OtherClassifiedAsPreinstalled = new string[] 
        {
            EnhancedSkirmish.ActivitiesFolderName
        };

        /// <summary>Returns true if the mod is classified as a "preinstalled mod".</summary>
        public static bool IsPreinstalledMod(Mod mod)
        {
            var folderName = mod.Folder;

            return PreinstalledModsList.Contains(folderName) ||
                   OtherClassifiedAsPreinstalled.Contains(folderName);
        }
    }
}
