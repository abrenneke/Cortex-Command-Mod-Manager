using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CortexCommandModManager.MVVM.Utilities;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    /// <summary>A view model wrapper for a preset.</summary>
    public class PresetViewModel : ModListItemViewModel
    {
        /// <summary>Gets the list of mods in the preset.</summary>
        public IList<ModViewModel> Mods { get { return Preset.Select(x => new ModViewModel(x, modManager, presetManager)).ToList(); } }

        /// <summary>Gets the count of mods in the preset.</summary>
        public string ModCount { get { return Preset.ModCount; } }

        /// <summary>Gets the name of the preset.</summary>
        public string Name { get { return Preset.Name; } }

        /// <summary>Gets whether the preset is enabled.</summary>
        public override bool IsEnabled { get { return Preset.IsEnabled; } }
        
        /// <summary>Gets the preset that this view model is wrapping.</summary>
        public Preset Preset { get; private set; }

        /// <summary>Gets the command for renaming the preset.</summary>
        public ICommand RenameCommand { get; private set; }
        /// <summary>Gets the command for disbanding the preset.</summary>
        public ICommand DisbandCommand { get; set; }

        /// <summary>This event will be called when the preset has been disbanded and it should be removed from the list.</summary>
        public event Action OnDisband;

        private readonly ModManager modManager;
        private readonly PresetManager presetManager;

        /// <summary>Creates a new preset view model.</summary>
        public PresetViewModel(Preset preset, ModManager modManager, PresetManager presetManager)
        {
            this.modManager = modManager;
            this.presetManager = presetManager;

            Preset = preset;

            RenameCommand = new Command(Rename);
            DisbandCommand = new Command(Disband);
        }

        /// <summary>Adds a mod to the preset.</summary>
        public void AddMod(Mod mod)
        {
            if(!Preset.Contains(mod))
                Preset.Add(mod);

            OnPropertyChanged(x => Mods);
            OnPropertyChanged(x => ModCount);
        }

        /// <summary>Renames the preset by opening a rename window. Called from the RenameCommand.</summary>
        private void Rename()
        {
            var renameWindow = new RenamePresetWindow(Preset.Name);

            var result = (bool)renameWindow.ShowDialog();

            if (result)
            {
                string presetName = renameWindow.PresetName;
                Preset = presetManager.RenamePreset(Preset, presetName);

                OnPropertyChanged(x => Preset);
                OnPropertyChanged(x => Name);
            }
        }

        /// <summary>Disbands the preset. Called from the DisbandCommand. Will raise OnDisband.</summary>
        private void Disband()
        {
            presetManager.DisbandPreset(Preset);

            if (OnDisband != null)
                OnDisband();
        }

        public override void Enable()
        {
            presetManager.EnablePreset(Preset);
            OnPropertyChanged(x => IsEnabled);
        }

        public override void Disable()
        {
            presetManager.DisablePreset(Preset);
            OnPropertyChanged(x => IsEnabled);
        }
    }
}
