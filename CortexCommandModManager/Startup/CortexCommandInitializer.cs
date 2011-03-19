using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager.Startup
{
    public class CortexCommandInitializer
    {
        private CCMMInitialization initialization;

        public CortexCommandInitializer(CCMMInitialization initialization)
        {
            this.initialization = initialization;
        }

        public void Initialize()
        {
            ModManager.CheckDisabledFolderExists();
            InitEnhancedSkirmish();
        }

        private void InitEnhancedSkirmish()
        {
            initialization.EnhancedSkirmish.RefreshCCDirectory();
            initialization.EnhancedSkirmish.Initialize();
        }
    }
}
