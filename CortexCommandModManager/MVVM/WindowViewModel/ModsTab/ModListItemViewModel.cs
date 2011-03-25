using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using System.Windows.Input;
using System.Windows;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    /// <summary>View model decorator for a mod list item (preset or mod).</summary>
    public abstract class ModListItemViewModel : ViewModel
    {
        public abstract bool IsEnabled { get; }

        public ICommand ToggleEnabledCommand { get; set; }

        public ModListItemViewModel()
        {
            ToggleEnabledCommand = new Command(ToggleEnabled);
        }

        public void ToggleEnabled()
        {
            try
            {
                if (IsEnabled)
                    Disable();
                else
                    Enable();
            }
            catch (ModLockedException ex)
            {
                //TODO refactor in to MessageBoxManager
                MessageBox.Show(Global.MainWindow, ex.Message, "Mod Locked", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            }
        }

        public abstract void Enable();

        public abstract void Disable();
    }
}
