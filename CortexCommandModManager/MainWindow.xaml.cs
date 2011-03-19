using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using XmlConfig.Core;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using CortexCommandModManager.Activities;
using CortexCommandModManager.MVVM.Utilities;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members
        private ModScanner modScanner;
        private Settings<SettingsObject> settings;
        private SkirmishSettingsManager skirmishSettingsManager;
        //private GameSettings gameSettings;
        private SkirmishWavesScanner skirmishWavesScanner;
        private CollectionViewSource modListSource;
        private List<IModListItem> modListItemInternal;
        private Inventory skirmishWavesInventoryInternal;
        private EnhancedSkirmish enhancedSkirmish;
        private LoadSaveActivitiesWindow loadSaveActivitiesWindow;
        private bool CurrentActivitySaved = true;
        private GameSettingsManager gameSettingsManager;
        #endregion

        #region Delegates
        public delegate void Action();
        #endregion

        #region First Start
        /// <summary>
        /// Attempts to get the CC install directory from the registry. Returns null if not found.
        /// </summary>
        private string TryGetCCPathFromRegistry()
        {
            object value = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Data Realms", "Cortex Command", null);
            return (string)value;
        }
        /// <summary>
        /// Asks for the install directory for Cortex Command, returns when it is found. Will keep asking until a valid directory is chosen.
        /// </summary>
        /// <returns></returns>
        private string AskForInstallDirectory()
        {
            throw new NotImplementedException();   
        }
        
        /// <summary>
        /// Throws a new IncorrectInstallationException with a message.
        /// </summary>
        public void IncorrectInstallation()
        {
            
        }
        #endregion

        #region Init
        /// <summary>
        /// Main initializer for CCMM.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                ElevateCCMMIfInProgramFiles();
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
                InitMembers();
                ElevateCCMMIfCCIsInProgramFiles();
                InitWindowSize();
                ModManager.CheckDisabledFolderExists();
                LoadEnhancedSkirmish();
                InitUIBoxes();
                LoadAllModListItems();
            }
            catch (IncorrectInstallationException ex)
            {
                MessageBox.Show(ex.Message, "Incorrect Installation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                UnhandledExceptionHandler(this, new UnhandledExceptionEventArgs(ex, true));
                this.Close();
            }
        }

        /// <summary>
        /// Elevates CCMM to administrator if CCMM is in Program Files, and the user is running Vista or Windows 7
        /// </summary>
        private void ElevateCCMMIfInProgramFiles()
        {
            if(isVistaOr7() && !isRunningAsAdmin() && CCMMIsInProgramFiles())
            {
                ElevateCCMM();
            }
        }
        /// <summary>
        /// Elevates CCMM to administrator is Cortex Command is in Program Files, and the user is running Vista or Windows 7
        /// </summary>
        private void ElevateCCMMIfCCIsInProgramFiles()
        {
            if (isVistaOr7() && !isRunningAsAdmin() && CCIsInProgramFiles())
            {
                ElevateCCMM();
            }
        }
        /// <summary>
        /// Returns true if Cortex Command is in Program Files.
        /// </summary>
        private bool CCIsInProgramFiles()
        {
            return Grabber.Settings.Get().CCInstallDirectory.Contains("Program Files");
        }
        /// <summary>
        /// Returns true if CCMM is in Program Files.
        /// </summary>
        private bool CCMMIsInProgramFiles()
        {
            return new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName.Contains("Program Files");
        }
        /// <summary>
        /// Elevates CCMM to administrator by killing the current process and starting a new CCMM with "runas"
        /// </summary>
        private void ElevateCCMM()
        {
            
        }
        /// <summary>
        /// Returns true if CCMM is currently running as an administrator.
        /// </summary>
        private bool isRunningAsAdmin()
        {
            return new System.Security.Principal.WindowsPrincipal(
                    System.Security.Principal.WindowsIdentity.GetCurrent()).
                        IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        /// <summary>
        /// Returns true if CCMM is running on a Vista or Windows 7 computer, mainly for UAC.
        /// </summary>
        private bool isVistaOr7()
        {
            return Environment.OSVersion.Version.Major == 6;
        }
        /// <summary>
        /// Initializes the window size from the settings object.
        /// </summary>
        private void InitWindowSize()
        {
            SettingsObject sets = Grabber.Settings.Get();
            this.Width = sets.WindowWidth;
            this.Height = sets.WindowHeight;
        }
        /// <summary>
        /// Functions as a bridge between a Dispatcher unhandled exception, and UnhandledExceptionHandler.
        /// </summary>
        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledExceptionHandler(sender, new UnhandledExceptionEventArgs(e.Exception, true));
        }
        /// <summary>
        /// Loads and enables Enhanced Skirmish based on the settings object value.
        /// </summary>
        private void LoadEnhancedSkirmish()
        {
            SettingsObject settings = Grabber.Settings.Get();
            enhancedSkirmish.RefreshCCDirectory();
            enhancedSkirmish.Initialize();
        }
        /// <summary>
        /// Initializes the members of the main CCMM window.
        /// </summary>
        public void InitMembers()
        {
            settings = new Settings<SettingsObject>("settings.xml");
            Grabber.SetSettings(settings);
            InitCortexCommandDirectory();

            modListSource = this.FindResource("modsListSource") as CollectionViewSource;
            //gameSettings = this.FindResource("skirmishBasicSettings") as GameSettings;

            enhancedSkirmish = new EnhancedSkirmish();
            skirmishSettingsManager = new SkirmishSettingsManager();
            gameSettingsManager = new GameSettingsManager();
            skirmishWavesScanner = new SkirmishWavesScanner();
            modScanner = new ModScanner();
        }
        /// <summary>
        /// Initizlizes the cortex command directory, asks if it doesn't exist.
        /// </summary>
        public void InitCortexCommandDirectory()
        {
            if (String.IsNullOrEmpty(Grabber.Settings.Get().CCInstallDirectory))
            {
                string installDirectory = TryGetCCPathFromRegistry();
                if (installDirectory == null)
                {
                    installDirectory = AskForInstallDirectory();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(
                        "A valid Cortex Command installation was found at \"" + installDirectory + "\". Would you like you use this?",
                        "Cortex Command Install Directory",
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes);
                    if (result == MessageBoxResult.No)
                    {
                        installDirectory = AskForInstallDirectory();
                    }
                }
                if (String.IsNullOrEmpty(installDirectory))
                {
                    IncorrectInstallation();
                }
                Grabber.Settings.Set(x => x.CCInstallDirectory = installDirectory);;
            }
        }
        /// <summary>
        /// Initizlizs the UI boxes fr the CCMM window. Skirmish settings boxes and tree view.
        /// </summary>
        private void InitUIBoxes()
        {
            //gameSettings.Player1Gold = skirmishSettingsManager.InitialP1Money;
            //gameSettings.Player2Gold = skirmishSettingsManager.InitialP2Money;
            //gameSettings.SpawnIntervalEasiest = skirmishSettingsManager.SpawnIntervalEasiest;
            //gameSettings.SpawnIntervalHardest = skirmishSettingsManager.SpawnIntervalHardest;
            //gameSettings.ResolutionX = gameSettingsManager.ResolutionX;
            //gameSettings.ResolutionY = gameSettingsManager.ResolutionY;
            //gameSettings.Fullscreen = gameSettingsManager.Fullscreen;
            actDescriptionBox.Text = Grabber.Settings.Get().LastActivityDescription;
            actTitleBox.Text = Grabber.Settings.Get().LastActivityTitle;
            InitWavesTreeView();
        }
        /// <summary>
        /// Initializes the tree view for waves.
        /// </summary>
        private void InitWavesTreeView()
        {
            skirmishWavesInventoryInternal = null;
            skirmishWavesTreeView.ItemsSource = null;
            Inventory waves = skirmishWavesScanner.GetAllWaves();

            InventoryItem newItem = new InventoryItem();
            newItem.CopyOf = "All Waves";
            newItem.SubItems = waves;

            Inventory mainInventory = new Inventory();
            mainInventory.Add(newItem);

            skirmishWavesInventoryInternal = mainInventory;
            RefreshTreeView();
        }
        #endregion

        #region Mods Tab
        /// <summary>
        /// Disables all mod list items in the mods/presets list.
        /// </summary>
        private void DisableAllModListItems()
        {
            foreach (IModListItem item in modsList.Items)
            {
                try
                {
                    //item.Disable();
                }
                catch (ModLockedException) { }
            }
            RefreshModsList();
        }
        /// <summary>
        /// Enables all items in the mod/preset list.
        /// </summary>
        private void EnableAllModListItems()
        {
            foreach (IModListItem item in modsList.Items)
            {
                //item.Enable();
            }
            RefreshModsList();
        }
        private void FilterModListItems()
        {
            ModListFilterer.ShowEnabled((bool)showEnabledMods.IsChecked);
            ModListFilterer.ShowDisabled((bool)showDisabledMods.IsChecked);
            modListSource.Filter += ModListFilterer.FilterHandler;
        }
        public void LoadAllModListItems()
        {
            Thread workerThread = new Thread(new ThreadStart(delegate()
            {
                while (Grabber.Settings.Get().CCInstallDirectory == String.Empty)
                {
                    Thread.Sleep(50);
                }
                IList<Mod> mods = modScanner.GetAllMods();
                IEnumerable<Preset> presets = PresetManager.GetAllPresets();
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => 
                {
                    ModListItemsLoaded(mods, presets);
                }));
            }));
            workerThread.IsBackground = true;
            workerThread.Start();
        }
        private void TrySwapModListItemState(IModListItem modListItem)
        {
            try
            {
                //modListItem.ToggleEnabled();
            }
            catch (ModLockedException ex)
            {
                MessageBox.Show(ex.Message, "Mod Locked", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            RefreshModsList();
        }
        private IModListItem GetModListItemFromListViewItem(object listViewItem)
        {
            ListViewItem item = listViewItem as ListViewItem;
            if (item == null)
                return null;
            IModListItem mod = modsList.ItemContainerGenerator.ItemFromContainer(item) as IModListItem;
            if (mod == null)
                return null;
            mod = modListItemInternal[modListItemInternal.IndexOf(mod)];
            return mod;
        }
        public void RefreshModsList()
        {
            modListSource.Source = null;
            SortModsList();
            modListSource.Source = modListItemInternal;
        }
        public void HardRefreshModsList()
        {
            LoadAllModListItems();
        }
        public void ModListItemsLoaded(IList<Mod> mods, IEnumerable<Preset> presets)
        {
            modListItemInternal = new List<IModListItem>();
            foreach (Mod mod in mods)
            {
                if(!PresetManager.ModIsInAPreset(mod))
                {
                    modListItemInternal.Add(mod);
                }
            }
            foreach (Preset preset in presets)
            {
                modListItemInternal.Add(preset);
            }
            foreach (IModListItem item in modListItemInternal)
            {
                if (item is Mod)
                {
                    ((Mod)item).LoadIcon();
                }
            }
            RefreshModsList();
            loadingLabel.Content = "Mods Loaded";
        }
        private void SortModsList()
        {
            if (modListItemInternal != null)
            {
                modListItemInternal.Sort(new ModItemComparer());
            }
        }
        private void ConfirmAndDeleteMod(Mod mod)
        {
            

            HardRefreshModsList();
        }
        private void ShowModItemContextMenu(object item)
        {
            ListViewItem listViewItem = item as ListViewItem;
            IModListItem modListItem = GetModListItemFromListViewItem(item);

            if (modListItem == null)
            {
                return;
            }

            ContextMenu menu = new ContextMenu();

            MenuItem enableDisableItem = new MenuItem();
            enableDisableItem.Header = (modListItem.IsEnabled) ? "Disable" : "Enable";
            enableDisableItem.Click += (o, e) => TrySwapModListItemState(modListItem);
            menu.Items.Add(enableDisableItem);

            if (modListItem is Mod)
            {
                menu.Items.Add(new Separator());

                Mod mod = (Mod)modListItem;

                var deleteModItem = new MenuItem();
                deleteModItem.Header = "Delete Mod";
                deleteModItem.Click += (o, e) => ConfirmAndDeleteMod(mod);
                menu.Items.Add(deleteModItem);

                menu.Items.Add(new Separator());
                
                MenuItem addToPresetItem = new MenuItem();
                addToPresetItem.Header = "Add To Preset";

                MenuItem newPresetItem = new MenuItem();
                newPresetItem.Header = "New Preset...";
                newPresetItem.Click += (o, e) => CreatePresetAndAddModTo(mod);

                addToPresetItem.Items.Add(newPresetItem);

                bool separatorAdded = false;
                foreach (Preset preset in PresetManager.GetAllPresets())
                {
                    Preset currentPreset = preset;
                    if (!separatorAdded)
                    {
                        addToPresetItem.Items.Add(new Separator());
                        separatorAdded = true;
                    }
                    MenuItem presetItem = new MenuItem();
                    presetItem.Header = preset.Name;
                    presetItem.Click += (o, e) => AddModToPreset(mod, currentPreset);
                    addToPresetItem.Items.Add(presetItem);
                }

                menu.Items.Add(addToPresetItem);
            }
            if (modListItem is Preset)
            {
                Preset preset = modListItem as Preset;

                menu.Items.Add(new Separator());

                MenuItem renamePresetItem = new MenuItem();
                renamePresetItem.Header = "Rename...";
                renamePresetItem.Click += (o, e) => RenamePreset(preset);

                menu.Items.Add(renamePresetItem);

                MenuItem disbandPresetItem = new MenuItem();
                disbandPresetItem.Header = "Disband";
                disbandPresetItem.Click += (o, e) => DisbandPreset(preset);

                menu.Items.Add(disbandPresetItem);
            }

            listViewItem.ContextMenu = menu;
        }
        private void DisbandPreset(Preset preset)
        {
            PresetManager.DisbandPreset(preset);
            HardRefreshModsList();
        }
        private void RenamePreset(Preset preset)
        {
            RenamePresetWindow window = new RenamePresetWindow(preset.Name);
            if ((bool)window.ShowDialog())
            {
                string presetName = window.PresetName;
                PresetManager.RenamePreset(preset, presetName);
            }
            HardRefreshModsList();
        }
        private void CreatePresetAndAddModTo(Mod mod)
        {
            if (mod.IsInOriginalInstallation())
            {
                MessageBox.Show("This mod cannot be added to a preset, as it is part of the core of Cortex Command.", "Notice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            AddPresetWindow window = new AddPresetWindow();
            if ((bool)window.ShowDialog())
            {
                string presetName = window.PresetName;
                Preset preset = new Preset(presetName, mod.IsEnabled);
                preset.Add(mod);
                PresetManager.SavePreset(preset);
            }
            HardRefreshModsList();
        }
        private void AddModToPreset(Mod mod, Preset preset)
        {
            if (mod.IsInOriginalInstallation())
            {
                MessageBox.Show("This mod cannot be added to a preset, as it is part of the core of Cortex Command.", "Notice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            PresetManager.AddModToPreset(mod, preset);
            HardRefreshModsList();
        }
        private void GeneratePresetModContextMenu(object sender)
        {
            ListViewItem senderItem = sender as ListViewItem;
            if (senderItem == null)
                return;

            Mod mod = senderItem.FindParent<ListView>().ItemContainerGenerator.ItemFromContainer(senderItem) as Mod;
            if (mod == null)
                return;
            Preset preset = GetModListItemFromListViewItem(senderItem.FindParent<ListViewItem>()) as Preset;
            if (preset == null)
                return;
            ContextMenu newMenu = new ContextMenu();
            MenuItem removeFromPresetItem = new MenuItem();
            removeFromPresetItem.Header = "Remove From Preset";
            removeFromPresetItem.Click += (o, e) => RemoveModFromPreset(mod, preset);
            newMenu.Items.Add(removeFromPresetItem);
            senderItem.ContextMenu = newMenu;
        }
        private void RemoveModFromPreset(Mod mod, Preset preset)
        {
            PresetManager.RemoveModFromPreset(mod, preset);
            HardRefreshModsList();
        }
        private void InstallModsFromDialog()
        {
            
        }
        #endregion

        #region Settings Tab
        private void SaveCurrentSkirmishSettings()
        {
            //skirmishSettingsManager.InitialP1Money = gameSettings.Player1Gold;
            //skirmishSettingsManager.InitialP2Money = gameSettings.Player2Gold;
            //skirmishSettingsManager.SpawnIntervalEasiest = gameSettings.SpawnIntervalEasiest;
            //skirmishSettingsManager.SpawnIntervalHardest = gameSettings.SpawnIntervalHardest;
        }
        private void SaveCurrentGameSettings()
        {
            //gameSettingsManager.ResolutionX = gameSettings.ResolutionX;
            //gameSettingsManager.ResolutionY = gameSettings.ResolutionY;
            //gameSettingsManager.Fullscreen = gameSettings.Fullscreen;
        }
        #endregion

        #region Skirmish Items Tab
        private void AddSkirmishItem()
        {
            if (skirmishWavesTreeView.SelectedItem == null)
            {
                return;
            }
            InventoryItem selectedItem = skirmishWavesTreeView.SelectedItem as InventoryItem;

            var addWindow = new ActivityItemWindow();
            addWindow.Owner = this;
            addWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            addWindow.Left = this.Left + this.Width - 30;
            addWindow.Top = this.Top + 50;
            addWindow.ShowDialog();

            if (addWindow.SelectedItem == null) return;

            InventoryItem newInvItem = new InventoryItem(addWindow.SelectedItem);
            newInvItem.ParentInventory = selectedItem.SubItems;
            selectedItem.SubItems.Add(newInvItem);
            MarkUnsaved();
            SaveSkirmishWaves();
            RefreshTreeView();
            TreeViewItem item = skirmishWavesTreeView.ItemContainerGenerator.ContainerFromItem(GetInventoryRootItem()) as TreeViewItem;
            item.IsExpanded = true;
        }

        private void MarkUnsaved()
        {
            this.CurrentActivitySaved = false;
        }
        private void RefreshTreeView()
        {
            skirmishWavesTreeView.ItemsSource = null;
            skirmishWavesTreeView.ItemsSource = skirmishWavesInventoryInternal;
            OpenMainSkirmishTreeViewItem();
        }
        private void RemoveSelectedSkirmishItem()
        {
            if (skirmishWavesTreeView.SelectedItem == null)
                return;
            InventoryItem selectedItem = skirmishWavesTreeView.SelectedItem as InventoryItem;
            RemoveInventoryItemFromCollection(selectedItem);
            MarkUnsaved();
            SaveSkirmishWaves();
            RefreshTreeView();
        }
        private void OpenMainSkirmishTreeViewItem()
        {
            TreeViewItem mainItem = skirmishWavesTreeView.ItemContainerGenerator.ContainerFromItem(GetInventoryRootItem()) as TreeViewItem;
            if (mainItem == null)
                return;
            mainItem.IsExpanded = true;
        }
        private Inventory GetInventoryRoot()
        {
            return skirmishWavesInventoryInternal[0].SubItems;
        }
        private InventoryItem GetInventoryRootItem()
        {
            return skirmishWavesInventoryInternal[0];
        }
        public void RemoveInventoryItemFromCollection(InventoryItem item)
        {
            RemoveInventoryItemFromCollectionRecursive(item, skirmishWavesInventoryInternal);
        }
        public void RemoveInventoryItemFromCollectionRecursive(InventoryItem item, Inventory collection)
        {
            Inventory toRemoveFrom = null; bool found = false;
            foreach (InventoryItem tempItem in collection)
            {
                if (tempItem.CompareTo(item) == 0)
                {
                    found = true;
                    toRemoveFrom = tempItem.ParentInventory;
                    if (toRemoveFrom == null)
                    {
                        toRemoveFrom = GetInventoryRoot();
                    }
                }
                else
                {
                    RemoveInventoryItemFromCollectionRecursive(item, tempItem.SubItems);
                }
            }
            if (found && toRemoveFrom != null)
            {
                int toRemoveFromCount = toRemoveFrom.Count;
                toRemoveFrom.Remove(item);
                if (toRemoveFromCount == toRemoveFrom.Count)
                {
                    toRemoveFrom.RemoveFirst(item);
                }
            }
        }
        private void SaveSkirmishWavesAndRefreshView()
        {
            SaveSkirmishWaves();
            InitWavesTreeView();
        }
        private void SaveSkirmishWaves()
        {
            skirmishWavesScanner.SetSkirmishWaves(GetInventoryRoot());
        }

        private void DuplicateSelectedSkirmishItem()
        {
            InventoryItem selectedItem = GetSelectedInventoryItem();
            if (selectedItem == null)
                return;
            InventoryItem parentOfSelected = selectedItem.FindParentFromRoot(GetInventoryRoot());
            if (parentOfSelected == null)
            {
                parentOfSelected = GetInventoryRootItem();
            }
            InventoryItem itemCopy = selectedItem.Copy();
            if (itemCopy.ParentInventory == null)
            {
                GetInventoryRoot().Add(itemCopy);
            }
            else
            {
                itemCopy.ParentInventory.Add(itemCopy);
            }
            MarkUnsaved();
            SaveSkirmishWaves();
            RefreshTreeView();
        }
        private InventoryItem GetSelectedInventoryItem()
        {
            return skirmishWavesTreeView.SelectedItem as InventoryItem;
        }
        private void SelectItemMouseClicked(object itemMouseClicked)
        {
            TreeViewItem senderItem = itemMouseClicked as TreeViewItem;
            if (itemMouseClicked == null)
                return;
            try
            {
                DependencyObject dObject = senderItem as DependencyObject;
                if (dObject == null)
                    return;

                System.Reflection.MethodInfo selectMethod =
                   typeof(TreeViewItem).GetMethod("Select",
                   System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                selectMethod.Invoke(dObject, new object[] { true });
            }
            catch { }
        }

        private void CreateActivitiesWindow()
        {
            loadSaveActivitiesWindow = new LoadSaveActivitiesWindow();
            loadSaveActivitiesWindow.Owner = this;
            loadSaveActivitiesWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            loadSaveActivitiesWindow.ActivitySelectionChanged += new LoadSaveActivitiesWindow.ActivitySelectionChangedDelegate(LoadActivityFromList);
            loadSaveActivitiesWindow.RequestCurrentActivity = GetCurrentActivity;
            loadSaveActivitiesWindow.ActivitySaved += new LoadSaveActivitiesWindow.ActivitySavedDelegate(MarkSaved);
        }
        private void ToggleActivitiesWindow()
        {
            if (loadSaveActivitiesWindow == null)
            {
                CreateActivitiesWindow();
            }
            if (loadSaveActivitiesWindow.IsVisible)
            {
                HideActivitiesWindowIfOpen();
            }
            else
            {
                ShowActivitiesWindowIfHidden();
            }
        }
        private void MoveActivitiesWindowIfOpen()
        {
            if (loadSaveActivitiesWindow == null || loadSaveActivitiesWindow.IsVisible == false)
            {
                return;
            }
            MoveActivitiesWindow();
        }
        private void MoveActivitiesWindow()
        {
            loadSaveActivitiesWindow.Left = this.Left + btnOpenActivitesWindow.TranslatePoint(new Point(0, 0), this).X + 50;
            loadSaveActivitiesWindow.Top = this.Top + btnOpenActivitesWindow.TranslatePoint(new Point(0, 0), this).Y - 200;
        }
        private void ShowActivitiesWindowIfHidden()
        {
            if (loadSaveActivitiesWindow != null)
            {
                MoveActivitiesWindow();
                ((TextBlock)btnOpenActivitesWindow.Content).Text = "«";
                loadSaveActivitiesWindow.Show();
                loadSaveActivitiesWindow.Reload();
            }
        }
        private void HideActivitiesWindowIfOpen()
        {
            if (loadSaveActivitiesWindow != null)
            {
                loadSaveActivitiesWindow.Hide();
                ((TextBlock)btnOpenActivitesWindow.Content).Text = "»";
            }
        }
        private void CloseActivitiesWindowIfOpen()
        {
            if (loadSaveActivitiesWindow != null)
            {
                loadSaveActivitiesWindow.Close();
            }
        }

        private void ToggleDescriptionBox()
        {
            if (waveDescriptionBox.Visibility == System.Windows.Visibility.Hidden)
            {
                waveDescriptionBox.Visibility = System.Windows.Visibility.Visible;
                btnShowDescription.Content = (object)"^";
            }
            else
            {
                waveDescriptionBox.Visibility = System.Windows.Visibility.Hidden;
                btnShowDescription.Content = (object)"˅";
            }
        }
        private void LoadActivityFromList(Activity activity)
        {
            bool cancel = PromptAndSaveIfActivityIsNotSaved();
            if (!cancel)
            {
                LoadNewActivity(activity);
            }
        }
        private bool PromptAndSaveIfActivityIsNotSaved()
        {
            if (!this.CurrentActivitySaved)
            {
                MessageBoxResult result = MessageBox.Show("The current activity is not saved. Would you like to save it before switching activities?", "Not Saved", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Cancel)
                {
                    return true;
                }
                if (result == MessageBoxResult.Yes)
                {
                    loadSaveActivitiesWindow.InvokeSave();
                }
            }
            return false;
        }
        private Activity GetCurrentActivity()
        {
            Activity activity = new Activity();
            activity.Name = actTitleBox.Text;
            activity.Description = actDescriptionBox.Text;
            activity.Root = GetInventoryRootItem();
            return activity;
        }
        private void LoadNewActivity(Activity activity)
        {
            actTitleBox.Text = activity.Name;
            actDescriptionBox.Text = activity.Description;
            skirmishWavesInventoryInternal = new Inventory();
            skirmishWavesInventoryInternal.Add(activity.Root);
            MarkSaved();
            SaveSkirmishWaves();
            RefreshTreeView();
        }
        private void MarkSaved()
        {
            this.CurrentActivitySaved = true;
        }
        private void UpdateLastActivityInformation()
        {
            Grabber.Settings.Set(x => { 
                x.LastActivityTitle = actTitleBox.Text;
                x.LastActivityDescription = actDescriptionBox.Text;
            });
        }
        #endregion

        #region Windows
        private void ShowAboutWindow()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
        private void OpenSettingsWindowAndRefreshData()
        {
            var settings = Grabber.Settings.Get();
            SettingsWindow settingsWindow = new SettingsWindow(settings);
            settingsWindow.ShowDialog();
            Grabber.Settings.Set(settings);
            LoadEnhancedSkirmish();
            InitUIBoxes();
            HardRefreshModsList();
        }
        private void UpdateWindowSizeVars()
        {
            Grabber.Settings.Set(x => { 
                x.WindowWidth = (int)Width;
                x.WindowHeight = (int)Height; 
            });
        }
        #endregion

        #region Misc Functions
        
        private void RunCortexCommand()
        {
            System.Diagnostics.Process gameProc = new System.Diagnostics.Process();
            string gameExe = GetGameExe();
            gameProc.StartInfo.FileName = gameExe;
            gameProc.StartInfo.WorkingDirectory = Grabber.Settings.Get().CCInstallDirectory;
            gameProc.Start();
        }
        private string GetGameExe()
        {
            return Grabber.Settings.Get().CCInstallDirectory + "\\" + "Cortex Command.exe";
        }
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            ErrorWindow error = new ErrorWindow(exception);
            if (this.IsVisible)
            {
                error.Owner = this;
                error.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            }
            else
            {
                error.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }
            error.ShowDialog();
            this.Close();
        }
        private void UpdateModsAndRunGameAsync()
        {
            Cursor = Cursors.AppStarting;
            BackgroundWorker worker = new BackgroundWorker();
            SaveCurrentSkirmishSettings();
            SaveSkirmishWavesAndRefreshView();
            worker.DoWork += (e, o) => 
            {
                RunCortexCommand();
            };
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += (o,e) => Dispatcher.Invoke(new Action(() => Cursor = Cursors.Arrow));
        }
        #endregion

        #region UI Events
        private void btnAddSkirmishItem_Click(object sender, RoutedEventArgs e)
        {
            AddSkirmishItem();
        }
        private void skirmishWavesTreeViewItem_Add_Click(object sender, RoutedEventArgs e)
        {
            AddSkirmishItem();
        }
        private void showEnabledMods_Click(object sender, RoutedEventArgs e)
        {
            FilterModListItems();
        }
        private void showDisabledMods_Click(object sender, RoutedEventArgs e)
        {
            FilterModListItems();
        }
        private void btnRemoveSkirmishItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedSkirmishItem();
        }
        private void skirmishWavesTreeViewItem_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            DuplicateSelectedSkirmishItem();
        }
        private void skirmishWavesTreeViewItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedSkirmishItem();
        }
        private void skirmishWavesTreeViewItem_PreviewMouseRightButtonDown(object itemMouseClicked, MouseEventArgs e)
        {
            SelectItemMouseClicked(itemMouseClicked);
        }
        private void skirmishWavesTreeViewItem_KeyDown(object sender, KeyEventArgs keyPressed)
        {
            if (keyPressed.Key == Key.Delete)
            {
                RemoveSelectedSkirmishItem();
            }
        }
        private void Menu_Help_About_Click(object sender, RoutedEventArgs e)
        {
            ShowAboutWindow();
        }
        private void btnSaveWaves_Click(object sender, RoutedEventArgs e)
        {
            SaveSkirmishWavesAndRefreshView();
        }
        private void modsList_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshModsList();
        }
        private void btnRunGame_Click(object sender, RoutedEventArgs e)
        {
            UpdateModsAndRunGameAsync();
            
        }

       
        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSaveSkirmishSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentSkirmishSettings();
            SaveCurrentGameSettings();
        }

        
        private void enableAllMods_Click(object sender, RoutedEventArgs e)
        {
            EnableAllModListItems();
        }
        private void disableAllMods_Click(object sender, RoutedEventArgs e)
        {
            DisableAllModListItems();
        }
        private void ModItem_DoubleClick(object sender, RoutedEventArgs args)
        {
            IModListItem item = GetModListItemFromListViewItem(sender);
            if (item == null)
                return;
            TrySwapModListItemState(item);
        }
        private void Menu_File_Settings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindowAndRefreshData();
        }
        private void btnShowDescription_Click(object sender, RoutedEventArgs e)
        {
            ToggleDescriptionBox();
        }
        private void btnOpenActivitesWindow_Click(object sender, RoutedEventArgs e)
        {
            ToggleActivitiesWindow();
        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            MoveActivitiesWindowIfOpen();
        }
        private void btnRefreshModsList_Click(object sender, RoutedEventArgs e)
        {
            HardRefreshModsList();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MoveActivitiesWindowIfOpen();
        }
        private void actTitleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateLastActivityInformation();
        }
        private void actDescriptionBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateLastActivityInformation();
        }
        private void mainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideActivitiesWindowIfOpen();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CloseActivitiesWindowIfOpen();
            UpdateLastActivityInformation();
            UpdateWindowSizeVars();
        }
        private void ModItem_RightClick(object sender, MouseEventArgs e)
        {
            ShowModItemContextMenu(sender);
        }
        private void presetModContextMenu_Click(object sender, RoutedEventArgs e)
        {
            GeneratePresetModContextMenu(sender);
        }
        private void btnInstallMods_Click(object sender, RoutedEventArgs e)
        {
            InstallModsFromDialog();
        }
        #endregion

        private void skirmishWavesTreeViewItem_Random_Click(object sender, RoutedEventArgs e)
        {
            if (Grabber.ActivityItemCache == null || Grabber.ActivityItemCache.CacheIsValid == false)
            {
                new ActivityItemLoaderWindow(){Owner = this, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner}.ShowDialog();
            }

            var items = Grabber.ActivityItemCache.GetAll();
            var mods = items.Select(x => x.Mod).Distinct();

            var settings = Grabber.Settings.Get();
            var randomSettings = new RandomSettings();
            if(settings.RandomSettingsInitialized)
                randomSettings = new RandomSettings 
                {
                    MinimumCraft = settings.RandomSettingsMinCraft,
                    MaximumCraft = settings.RandomSettingsMaxCraft,
                    MinimumActorsPerCraft = settings.RandomSettingsMinActors,
                    MaximumActorsPerCraft = settings.RandomSettingsMaxActors,
                    MinimumWeaponsPerActor = settings.RandomSettingsMinWeapons,
                    MaximumWeaponsPerActor = settings.RandomSettingsMaxWeapons,
                    CrabsCanCarryWeapons = settings.RandomSettingsCrabsHoldWeapons
                };

            RandomSettingsWindow randomWindow = new RandomSettingsWindow(mods, randomSettings) { Owner = this, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner };
            randomWindow.GenerateRandomSkirmishClicked += (a, b) => {
                SaveRandomSettings(a);
                var mainItems = GenerateRandomSkirmish(a, b, items);
                Dispatcher.Invoke(new Action(() => PlaceRandomSkirmishInList(mainItems)));
            };

            randomWindow.Show();
        }

        private void SaveRandomSettings(RandomSettings randomSettings)
        {
            Grabber.Settings.Set(x => {
                x.RandomSettingsInitialized = true;
                x.RandomSettingsMinCraft = randomSettings.MinimumCraft;
                x.RandomSettingsMaxCraft = randomSettings.MaximumCraft;
                x.RandomSettingsMinActors = randomSettings.MinimumActorsPerCraft;
                x.RandomSettingsMaxActors = randomSettings.MaximumActorsPerCraft;
                x.RandomSettingsMinWeapons = randomSettings.MinimumWeaponsPerActor;
                x.RandomSettingsMaxWeapons = randomSettings.MaximumWeaponsPerActor;
                x.RandomSettingsCrabsHoldWeapons = randomSettings.CrabsCanCarryWeapons;
            });
        }

        private void PlaceRandomSkirmishInList(IEnumerable<InventoryItem> mainItems)
        {
            GetInventoryRootItem().SubItems.Clear();
            GetInventoryRootItem().SubItems.AddRange(mainItems);
            RefreshTreeView();
        }

        /// <summary>Generates a random skirmish based on the parameters, returning the list of craft to form the root.</summary>
        private IEnumerable<InventoryItem> GenerateRandomSkirmish(RandomSettings settings, IEnumerable<Mod> mods, IEnumerable<ActivityItem> items)
        {
            items = items.Where(x => mods.Contains(x.Mod));
            var craft = items.Where(x => x.Group == ActivityItemGroup.Craft).ToList();
            var actors = items.Where(x => x.Group == ActivityItemGroup.Actor).ToList();
            var weapons = items.Where(x => x.Group == ActivityItemGroup.Weapon).ToList();

            Random random = new Random();

            var allCraft = new List<InventoryItem>();

            var numCraft = random.Next(settings.MinimumCraft, settings.MaximumCraft + 1);

            for (var i = 0; i < numCraft; i++)
            {
                if (craft.Count == 0) break;
                var craftItem = new InventoryItem(craft[random.Next(0, craft.Count )]);
                allCraft.Add(craftItem);

                var actorsInCraft = random.Next(settings.MinimumActorsPerCraft, settings.MaximumActorsPerCraft + 1);
                for (var a = 0; a < actorsInCraft; a++)
                {
                    if (actors.Count == 0) break;
                    var actor = actors[random.Next(0, actors.Count)];
                    var actorItem = new InventoryItem(actor);
                    craftItem.SubItems.Add(actorItem);

                    var actorWeapons = actor.Type == ActivityItemType.ACrab && !settings.CrabsCanCarryWeapons ? 0 : 
                        random.Next(settings.MinimumWeaponsPerActor, settings.MaximumWeaponsPerActor + 1);

                    for (var w = 0; w < actorWeapons; w++)
                    {
                        if (weapons.Count == 0) break;
                        var weaponItem = new InventoryItem(weapons[random.Next(0, weapons.Count)]);
                        actorItem.SubItems.Add(weaponItem);
                    }
                }
            }

            return allCraft;
        }
    }
}