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
        /// <summary>Gets or sets the wrapped mod list item.</summary>
        public IModListItem ModListItem
        {
            get { return ThrowIfNull(modListItem); }
            set { modListItem = value; }
        }
        private IModListItem modListItem;

        public ICommand ToggleEnabledCommand { get; set; }

        public ModListItemViewModel() 
        {
            ToggleEnabledCommand = new Command(ToggleEnabled);
        }

        /// <summary>Creates a new view model decorator around a mod list item.</summary>
        public ModListItemViewModel(IModListItem inner) : this()
        {
            this.ModListItem = inner;
        }

        /// <summary>Gets the name of the mod item.</summary>
        public string Name { get { return ModListItem.Name; } }

        /// <summary>Gets or sets whether the mod item is enabled.</summary>
        public bool IsEnabled { get { return ModListItem.IsEnabled; } }

        /// <summary>Gets whether the mod item is preinstalled and cannot be modified.</summary>
        public bool IsPreinstalled { get { return modListItem.IsPreinstalled; } }

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

        public void Enable()
        {
            ModListItem.Enable();
            OnPropertyChanged(x => IsEnabled);
        }

        public void Disable()
        {
            modListItem.Disable();
            OnPropertyChanged(x => IsEnabled);
        }
    }
}
