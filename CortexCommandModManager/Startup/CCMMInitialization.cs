using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig.Core;

namespace CortexCommandModManager.Startup
{
    public class CCMMInitialization
    {
        public Settings<SettingsObject> Settings { get; set; }

        public EnhancedSkirmish EnhancedSkirmish { get; set; }
        public SkirmishSettingsManager SkirmishSettingsManager { get; set; }
        public GameSettingsManager GameSettingsManager { get; set; }
        public ModScanner ModScanner { get; set; }

        public ModManager ModManager { get; set; }
        public PresetManager PresetManager { get; set; }
    }
}
