using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using System.Collections.ObjectModel;
using CortexCommandModManager.Startup;
using System.ComponentModel;
using System.Windows.Input;
using CortexCommandModManager.Extensions;
using System.Windows.Data;
using MoreLinq;
using System.Windows;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    /// <summary>View model for the Mods tab.</summary>
    public class ModsTabViewModel : ViewModel, IInitializable
    {
        /// <summary>Gets the collection of mod list items to appear in the mods list.</summary>
        public CollectionViewSource Mods { get { return mods; } set { mods = value; OnPropertyChanged(x => Mods); } }
        private CollectionViewSource mods;

        /// <summary>Whether to show disabled mods. Note that this is not applied until UpdateFiltersCommand is executed.</summary>
        public bool ShowDisabledMods { get { return showDisabledMods; } set { showDisabledMods = value; OnPropertyChanged(x => ShowDisabledMods); } }
        private bool showDisabledMods;

        /// <summary>Whether to show enabled mods. Note that this is not applied until UpdateFiltersCommand is executed.</summary>
        public bool ShowEnabledMods { get { return showEnabledMods; } set { showEnabledMods = value; OnPropertyChanged(x => ShowEnabledMods); } }
        private bool showEnabledMods;

        public ICommand EnableAllModsCommand { get; set; }
        public ICommand DisableAllModsCommand { get; set; }
        public ICommand RefreshListCommand { get; set; }
        public ICommand InstallModCommand { get; set; }

        public ICommand UpdateFiltersCommand { get; set; }

        public ModsTabIcons Icons { get; private set; }

        private ObservableCollection<ModListItemViewModel> modItemsInternal;
       
        private readonly CCMMInitialization initialization;
        private readonly ModManager modManager;
        private readonly PresetManager presetManager;

        public ModsTabViewModel(CCMMInitialization initialization)
        {
            EnableAllModsCommand = new Command(EnableAllMods);
            DisableAllModsCommand = new Command(DisableAllMods);
            RefreshListCommand = new Command(LoadAllMods);
            InstallModCommand = new Command(InstallModsFromDialog);

            UpdateFiltersCommand = new Command(RefreshView);

            Icons = new ModsTabIcons();

            ShowEnabledMods = true;
            ShowDisabledMods = true;

            InitModsCollectionSource();

            this.initialization = initialization;
            this.modManager = initialization.ModManager;
            this.presetManager = initialization.PresetManager;
        }

        private void InitModsCollectionSource()
        {
            mods = new CollectionViewSource();
            mods.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            mods.Filter += (sender, e) =>
            {
                e.Accepted = false;

                var modItem = e.Item as ModListItemViewModel;

                if (showEnabledMods && modItem.IsEnabled)
                    e.Accepted = true;

                if (showDisabledMods && modItem.IsEnabled == false)
                    e.Accepted = true;

                var modViewModel = modItem as ModViewModel;
                if (modViewModel != null && PreinstalledMods.IsPreinstalledMod(modViewModel.Mod))
                    e.Accepted = false;
            };
        }

        public void Initialize()
        {
            LoadAllMods();
        }

        private void RefreshView()
        {
            mods.View.Refresh();
        }

        private void InstallModsFromDialog()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Title = "Select the packed mod(s) for installing.";
            var result = dialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK)
                return;

            var files = dialog.FileNames;

            var extractor = new ModExtracter();
            var extracted = extractor.Unpack(files);

            LoadAllMods();

            if (extracted.Count > 0)
            {
                MessageBox.Show("Extracted mods: " + String.Join(", ", extracted.ToArray()));
            }
            else
            {
                MessageBox.Show("No mods extracted (either the archive did not contain any mods, or they are already installed.");
            }
        }

        /// <summary>Enables all enableable mods.</summary>
        private void EnableAllMods()
        {
            ChangeAllState(x => x.Enable());
        }

        /// <summary>Disables all disableable mods.</summary>
        public void DisableAllMods()
        {
            ChangeAllState(x => x.Disable());
        }

        private void ChangeAllState(Action<ModListItemViewModel> action)
        {
            foreach (var mod in modItemsInternal)
            {
                try
                {
                    action(mod);
                }
                catch (ModLockedException) { }
            }
        }

        private void LoadAllMods()
        {
            var mods = initialization.ModScanner.GetAllMods();
            var presets = presetManager.GetAllPresets();
            ModListItemsLoaded(mods, presets);
        }

        private void ModListItemsLoaded(IList<Mod> mods, IList<Preset> presets)
        {
            var modItems = new List<IModListItem>(mods.Count + presets.Count);

            var validMods = mods.Where(x => presetManager.ModIsInAPreset(x) == false);
            validMods.Each(x => x.LoadIcon());

            modItems.AddRange(validMods);
            modItems.AddRange(presets);

            var viewModels = modItems
                .OrderBy(x => x.Name)
                .Select(MakeViewModel)
                .ToList();

            AddViewModelsFilteringRefresh(viewModels);

            modItemsInternal = new ObservableCollection<ModListItemViewModel>(viewModels);

            BindModsList();
        }

        private void AddViewModelsFilteringRefresh(IList<ModListItemViewModel> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                viewModel.PropertyChanged += (sender, e) => 
                {
                    if (e.PropertyName == "IsEnabled" && (!showDisabledMods || !showEnabledMods))
                        RefreshView();
                };
            }
        }

        private void BindModsList()
        {
            Mods.Source = modItemsInternal;
        }

        private void OnModDeleted(ModViewModel viewModel)
        {
            modItemsInternal.Remove(viewModel);
        }

        private void OnModAddedToPreset(ModViewModel modViewModel, Preset preset)
        {
            var presetViewModel = modItemsInternal.OfType<PresetViewModel>().SingleOrDefault(x => x.Preset.Equals(preset));

            if (presetViewModel == null)
            {
                var newModel = MakePresetViewModel(preset);
                modItemsInternal.Add(newModel);
                presetViewModel = newModel;
            }

            presetViewModel.AddMod(modViewModel.Mod);
            modItemsInternal.Remove(modViewModel);
        }

        private void OnPresetDisband(PresetViewModel viewModel)
        {
            foreach (var mod in viewModel.Mods)
            {
                var newViewModel = MakeModViewModel(mod.Mod);
                newViewModel.Mod.LoadIcon();
                modItemsInternal.Add(newViewModel);
            }
            modItemsInternal.Remove(viewModel);
        }

        private ModListItemViewModel MakeViewModel(IModListItem modItem)
        {
            var mod = modItem as Mod;
            if (mod != null)
                return MakeModViewModel(mod);

            var preset = modItem as Preset;
            if (preset != null)
                return MakePresetViewModel(preset);

            throw new NotSupportedException("Unknown IModListItem {0}".With(modItem.GetType().Name));
        }

        private ModViewModel MakeModViewModel(Mod mod)
        {
            var viewModel = new ModViewModel(mod, initialization.ModManager, initialization.PresetManager);
            viewModel.OnDeleted += () => OnModDeleted(viewModel);
            viewModel.OnAddedToPreset += x => OnModAddedToPreset(viewModel, x);
            return viewModel;
        }

        private PresetViewModel MakePresetViewModel(Preset preset)
        {
            var viewModel = new PresetViewModel(preset, initialization.ModManager, initialization.PresetManager);
            viewModel.OnDisband += () => OnPresetDisband(viewModel);
            return viewModel;
        }
    }
}
