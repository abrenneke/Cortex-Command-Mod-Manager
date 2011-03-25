using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    /// <summary>A view model for a Mod.</summary>
    public class ModViewModel : ModListItemViewModel
    {
        /// <summary>Gets the mod that this view model is wrapping.</summary>
        public Mod Mod { get; private set; }

        public string Name { get { return Mod.Name; } }

        /// <summary>Gets the folder of the mod.</summary>
        public string Folder { get { return Mod.Folder; } }

        /// <summary>Gets an ImageSource for the icon of the mod.</summary>
        public ImageSource Icon { get { return Mod.BitmapSource; } }

        public override bool IsEnabled { get { return Mod.IsEnabled; } }

        /// <summary>Event for when this mod has been deleted.</summary>
        public event Action OnDeleted;
        /// <summary>Event for when this mod has been added to a preset.</summary>
        public event Action<Preset> OnAddedToPreset;

        public ICommand DeleteModCommand { get; set; }
        public ICommand AddModToPresetCommand { get; set; }
        public ICommand NewPresetForModCommand { get; set; }

        public IList<MenuItem> PresetsCanBeAddedTo { get { return GetPresetsCanBeAddedToMenuItems(); } }

        private readonly ModManager modManager;
        private readonly PresetManager presetManager;

        /// <summary>Creates a new view model wrapper for a mod.</summary>
        public ModViewModel(Mod mod, ModManager modManager, PresetManager presetManager) : base()
        {
            this.modManager = modManager;
            this.presetManager = presetManager;

            Mod = mod;

            DeleteModCommand = new Command(DeleteMod);
            AddModToPresetCommand = new Command<Preset>(AddToPreset);
            NewPresetForModCommand = new Command(AddToNewPreset);
        }

        private void DeleteMod()
        {
            var result = MessageBox.Show("Are you sure you want to delete this mod? This action cannot be reversed.", "Confirm Mod Deletion",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            modManager.DeleteMod(Mod);

            if (OnDeleted != null)
                OnDeleted();
        }

        private void AddToNewPreset()
        {
            if (Mod.IsInOriginalInstallation())
            {
                MessageBox.Show("This mod cannot be added to a preset, as it is part of the core of Cortex Command.", "Notice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var addPresetWindow = new AddPresetWindow();
            addPresetWindow.Owner = Global.MainWindow;
            addPresetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var result = (bool)addPresetWindow.ShowDialog();
            
            if (result)
            {
                var presetName = addPresetWindow.PresetName;
                var preset = new Preset(presetName, Mod.IsEnabled);
                preset.Add(Mod);
                presetManager.SavePreset(preset);

                if (OnAddedToPreset != null)
                    OnAddedToPreset(preset);
            }
        }

        private void AddToPreset(Preset preset)
        {
            if (Mod.IsInOriginalInstallation())
            {
                MessageBox.Show("This mod cannot be added to a preset, as it is part of the core of Cortex Command.", "Notice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            presetManager.AddModToPreset(Mod, preset);

            if (OnAddedToPreset != null)
                OnAddedToPreset(preset);
        }

        private IList<MenuItem> GetPresetsCanBeAddedToMenuItems()
        {
            var presets = presetManager.GetAllPresets();

            return presets.Select(x =>
            {
                return new MenuItem
                {
                    Header = x.Name,
                    Command = AddModToPresetCommand,
                    CommandParameter = x
                };
            }).ToList();
        }

        public override void Enable()
        {
            modManager.EnableMod(Mod);
            OnPropertyChanged(x => IsEnabled);
        }

        public override void Disable()
        {
            modManager.DisableMod(Mod);
            OnPropertyChanged(x => IsEnabled);
        }
    }
}
