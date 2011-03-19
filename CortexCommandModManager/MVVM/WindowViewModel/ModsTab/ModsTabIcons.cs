using System.Windows.Controls;
using CortexCommandModManager.MVVM.Utilities;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    /// <summary>Icons for the mod tab.</summary>
    public class ModsTabIcons : ViewModel
    {
        /// <summary>The "Enable All Mods" icon.</summary>
        public Image EnableAllMods { get { return Resource.GetImage("icon-enable-all.png"); } }

        /// <summary>The "Disable All Mods" icon.</summary>
        public Image DisableAllMods { get { return Resource.GetImage("icon-disable-all.png"); } }
    }
}
