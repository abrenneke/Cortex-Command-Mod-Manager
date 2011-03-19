using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using XmlConfig.Core;
using CortexCommandModManager.Startup;
using System.ComponentModel;
using System.Windows.Input;

namespace CortexCommandModManager.MVVM.WindowViewModel
{
    public class NewMainWindowViewModel : ViewModel, IInitializable
    {
        private const int DefaultWindowWidth = 500;
        private const int DefaultWindowHeight = 460;

        /// <summary>Gets or sets the view model for the top menu.</summary>
        public IViewModel MenuViewModel { get; set; }

        /// <summary>Gets or sets the view model for the Mods tab.</summary>
        public IViewModel ModsTabViewModel { get; set; }

        /// <summary>Gets or sets the view model for the Settings tab.</summary>
        public IViewModel SettingsTabViewModel { get; set; }

        /// <summary>Gets or sets the view model for the Skirmishes tab.</summary>
        public IViewModel SkirmishesViewModel { get; set; }


        /// <summary>Gets or sets the width of the window.</summary>
        public double WindowWidth { get { return windowWidth; } set { windowWidth = value; OnPropertyChanged(x => WindowWidth); } }
        private double windowWidth;

        /// <summary>Gets or sets the height of the window.</summary>
        public double WindowHeight { get { return windowHeight; } set { windowHeight = value; OnPropertyChanged(x => WindowHeight); } }
        private double windowHeight;

        private CCMMInitialization initialization;

        public ICommand WindowLoadedCommand { get; set; }
        public ICommand WindowClosingCommand { get; set; }

        /// <summary>Creates a new MainWindowViewModel, supply sub-view-models that are required.</summary>
        public NewMainWindowViewModel(CCMMInitialization initialization)
        {
            WindowClosingCommand = new Command(WindowClosing);

            this.initialization = initialization;

            windowWidth = DefaultWindowWidth;
            windowHeight = DefaultWindowHeight;

            SetDimensionsFromSettings(initialization.Settings.Get());
        }

        public void Initialize()
        {
            foreach (var viewModel in new[] { MenuViewModel, ModsTabViewModel, SettingsTabViewModel, SkirmishesViewModel })
            {
                var initializable = viewModel as IInitializable;
                if (initializable != null)
                    initializable.Initialize();
            }
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
