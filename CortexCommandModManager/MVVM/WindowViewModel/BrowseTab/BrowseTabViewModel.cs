using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.ModsDatabase;
using System.Windows;
using System.Collections.ObjectModel;

namespace CortexCommandModManager.MVVM.WindowViewModel.BrowseTab
{
    public class BrowseTabViewModel : ViewModel
    {
        public bool BrowseModsIsSelected { get { return browseModsIsSelected; } set { browseModsIsSelected = value; OnPropertyChanged(x => BrowseModsIsSelected); } }
        private bool browseModsIsSelected;

        public bool IsLoadingMods { get { return isLoadingMods; } set { isLoadingMods = value; OnPropertyChanged(x => IsLoadingMods); } }
        private bool isLoadingMods;

        public ObservableCollection<ModDatabaseModViewModel> Mods { get; private set; }

        public event Action ModListRequiresRefresh;

        private readonly ModDatabase modDatabase;

        private IList<ModDatabaseMod> mods;

        public BrowseTabViewModel()
        {
            Mods = new ObservableCollection<ModDatabaseModViewModel>();
            modDatabase = new ModDatabase();

            PropertyChanged += (o, e) => 
            { 
                if (e.PropertyName == "BrowseModsIsSelected" && BrowseModsIsSelected) 
                    LazyLoadMods(); 
            };
        }

        private void LazyLoadMods()
        {
            if (mods != null || IsLoadingMods)
                return;

            IsLoadingMods = true;
            modDatabase.GetAllModsAsync(ModsLoadedCallback);
        }

        private void ModsLoadedCallback(IList<ModDatabaseMod> mods)
        {
            IsLoadingMods = false;
            this.mods = mods;

            Mods.Clear();
            foreach (var mod in mods.Where(x => x != null))
            {
                var vm = new ModDatabaseModViewModel(mod, modDatabase);
                vm.ModInstalled += x => { if (ModListRequiresRefresh != null) ModListRequiresRefresh(); };
                Mods.Add(vm);
            }
        }
    }
}
