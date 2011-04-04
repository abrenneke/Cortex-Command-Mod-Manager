using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using XmlConfig.Core;
using CortexCommandModManager.Startup;
using System.ComponentModel;
using System.Windows.Input;
using CortexCommandModManager.MVVM.WindowViewModel.Menu;
using CortexCommandModManager.MVVM.WindowViewModel.ModsTab;
using CortexCommandModManager.MVVM.WindowViewModel.BrowseTab;
using System.Diagnostics;
using CortexCommandModManager.MVVM.WindowViewModel.ActivitiesTab;

namespace CortexCommandModManager.MVVM.WindowViewModel
{
    public class NewMainWindowViewModel : ViewModel, IInitializable
    {
        private const int DefaultWindowWidth = 500;
        private const int DefaultWindowHeight = 460;

        /// <summary>Gets or sets the view model for the top menu.</summary>
        public MenuViewModel MenuViewModel { get; set; }

        /// <summary>Gets or sets the view model for the Mods tab.</summary>
        public ModsTabViewModel ModsTabViewModel { get; set; }

        /// <summary>Gets or sets the view model for the Browse Mods tab.</summary>
        public BrowseTabViewModel BrowseViewModel 
        { 
            get { return browseViewModel; } 
            set 
            {
                browseViewModel = value;
                value.ModListRequiresRefresh += () => ModsTabViewModel.RefreshListCommand.ExecuteIfCan();
            } 
        }
        private BrowseTabViewModel browseViewModel;

        /// <summary>Gets the </summary>
        public ActivitiesTabViewModel ActivitiesViewModel { get; set; }

        /// <summary>Gets or sets the width of the window.</summary>
        public double WindowWidth { get { return windowWidth; } set { windowWidth = value; OnPropertyChanged(x => WindowWidth); } }
        private double windowWidth;

        /// <summary>Gets or sets the height of the window.</summary>
        public double WindowHeight { get { return windowHeight; } set { windowHeight = value; OnPropertyChanged(x => WindowHeight); } }
        private double windowHeight;

        public Cursor Cursor { get { return cursor; } set { cursor = value; OnPropertyChanged(x => Cursor); } }
        private Cursor cursor;

        private CCMMInitialization initialization;

        public ICommand WindowLoadedCommand { get; set; }
        public ICommand WindowClosingCommand { get; set; }

        public ICommand RunCCCommand { get; set; }

        /// <summary>Creates a new MainWindowViewModel, supply sub-view-models that are required.</summary>
        public NewMainWindowViewModel(CCMMInitialization initialization)
        {
            Cursor = Cursors.Arrow;
            RunCCCommand = new Command(RunCCAsync);
            WindowClosingCommand = new Command(WindowClosing);

            this.initialization = initialization;

            windowWidth = DefaultWindowWidth;
            windowHeight = DefaultWindowHeight;

            SetDimensionsFromSettings(initialization.Settings.Get());
        }

        public void Initialize()
        {
            foreach (var viewModel in new IViewModel[] { MenuViewModel, ModsTabViewModel, ActivitiesViewModel })
            {
                var initializable = viewModel as IInitializable;
                if (initializable != null)
                    initializable.Initialize();
            }
        }

        private void RunCCAsync()
        {
            Cursor = Cursors.AppStarting;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, e) => RunCC();
            worker.RunWorkerCompleted += (o, e) => Cursor = Cursors.Arrow;
            worker.RunWorkerAsync();
        }

        private void RunCC()
        {
            var gameExe = Grabber.Settings.Get().CCInstallDirectory + "\\" + "Cortex Command.exe";
            var gameProc = new Process
            {
                StartInfo = 
                {
                    FileName = gameExe,
                    WorkingDirectory = initialization.Settings.Get().CCInstallDirectory
                }
            };

            gameProc.Start();
        }

        private void WindowClosing()
        {
            UpdateLastActivityInformation();
            UpdateWindowSizeVars();
        }

        private void UpdateWindowSizeVars()
        {
            initialization.Settings.Set(x =>
            {
                x.WindowWidth = (int)WindowWidth;
                x.WindowHeight = (int)WindowHeight;
            });
        }

        private void UpdateLastActivityInformation()
        {
            //TODO
            /*initialization.Settings.Set(x =>
            {
                x.LastActivityTitle = actTitleBox.Text;
                x.LastActivityDescription = actDescriptionBox.Text;
            });*/
        }

        private void SetDimensionsFromSettings(SettingsObject settings)
        {
            WindowWidth = settings.WindowWidth;
            WindowHeight = settings.WindowHeight;
        }
    }
}
