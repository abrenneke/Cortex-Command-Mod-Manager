using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CortexCommandModManager.MVVM.Utilities;
using System.Windows;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    /// <summary>A view model wrapper for a preset.</summary>
    public class PresetViewModel : ModListItemViewModel
    {
        private Preset preset;

        /// <summary>Gets or sets the preset that is wrapped by this view model.</summary>
        public Preset Preset { get { return preset; } set { preset = value; ModListItem = preset; } }

        /// <summary>Gets the list of mods in the preset.</summary>
        public IList<ModViewModel> Mods { get { return Preset.Select(x => new ModViewModel(x)).ToList(); } }

        /// <summary>Gets the count of mods in the preset.</summary>
        public string ModCount { get { return Preset.ModCount; } }

        public ICommand RenameCommand { get; set; }
        public ICommand DisbandCommand { get; set; }

        public event Action OnDisband;

        /// <summary>Creates a new preset view model.</summary>
        public PresetViewModel(Preset preset) : base(preset)
        {
            Preset = preset;

            RenameCommand = new Command(Rename);
            DisbandCommand = new Command(Disband);
        }

        public void AddMod(Mod mod)
        {
            if(!Preset.Contains(mod))
                Preset.Add(mod);
            OnPropertyChanged(x => Mods);
            OnPropertyChanged(x => ModCount);
        }

        public void Rename()
        {
            var renameWindow = new RenamePresetWindow(preset.Name);

            var result = (bool)renameWindow.ShowDialog();

            if (result)
            {
                string presetName = renameWindow.PresetName;
                Preset = PresetManager.RenamePreset(preset, presetName);
                OnPropertyChanged(x => Preset);
                OnPropertyChanged(x => Name);
            }
        }

        public void Disband()
        {
            PresetManager.DisbandPreset(preset);

            if (OnDisband != null)
                OnDisband();
        }
    }
}
