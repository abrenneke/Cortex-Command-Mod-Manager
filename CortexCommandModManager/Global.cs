using System;
using System.Windows;
using CortexCommandModManager.MVVM;
using CortexCommandModManager.MVVM.WindowViewModel;
using CortexCommandModManager.MVVM.WindowViewModel.Menu;
using CortexCommandModManager.MVVM.WindowViewModel.ModsTab;
using CortexCommandModManager.Startup;
using XmlConfig.Core;
using CortexCommandModManager.MVVM.Utilities;
using System.Threading;
using CortexCommandModManager.MVVM.WindowViewModel.BrowseTab;

namespace CortexCommandModManager
{
    public class Global
    {
        public static Window MainWindow { get; private set; }

        private Window loadingWindow;
        private CCMMInitialization initialization;
        private IViewModel mainViewModel;

        private Application application;

        public Global(Application application)
        {
            this.application = application;
        }
       
        public void Start()
        {
            StartApplicationWithExceptionHandling();
        }

        private void StartApplicationWithExceptionHandling()
        {
            try
            {
                StartApplication();
            }
            catch (Exception ex)
            {
                try
                {
                    ErrorWindow.Create(ex);
                }
                catch (Exception) { }

                //Log?

                End();
            }
        }

        private void StartApplication()
        {
            InitializePrerequisites();
            ShowLoadingWindow();
            InitializeCCMM();
            InitializeViewModel();
            HideLoadingWindow();
            StartMainWindow();
        }

        private void InitializePrerequisites()
        {
            //The very first thing to do is catch any unhandled exceptions. These can be caused by UI events.
            AppDomain.CurrentDomain.UnhandledException += ErrorWindow.HandleUnhandledException;

            var resolver = new AssemblyResolver(new DefaultAssemblyConfiguration());
            AppDomain.CurrentDomain.AssemblyResolve += resolver.Resolve;

            var elevator = new ProgramElevator();
            if (elevator.ExecutingProgramRequiresElevation)
                elevator.Elevate();
        }

        private void ShowLoadingWindow()
        {
            loadingWindow = new LoadingWindow();
            loadingWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loadingWindow.Show();
        }

        private void HideLoadingWindow()
        {
            if (loadingWindow == null)
                return;

            loadingWindow.Close();
        }

        private void InitializeCCMM()
        {
            var ccmmInitializer = new CCMMInitializer();
            
            initialization = ccmmInitializer.Initialize();

            var elevator = new ProgramElevator();
            if (elevator.DependentFolderRequiresElevation(initialization.Settings.Get().CCInstallDirectory))
                elevator.Elevate();

            var cortexCommandInitializer = new CortexCommandInitializer(initialization);
            cortexCommandInitializer.Initialize();
        }

        private void InitializeViewModel()
        {
            var viewModel = new NewMainWindowViewModel(initialization)
            {
                MenuViewModel = new MenuViewModel(),
                ModsTabViewModel = new ModsTabViewModel(initialization),
                SettingsTabViewModel = new SettingsTabViewModel(initialization.GameSettingsManager, initialization.SkirmishSettingsManager),
                SkirmishesViewModel = new SkirmishesViewModel(),
                BrowseViewModel = new BrowseTabViewModel()
            };
            viewModel.Initialize();

            mainViewModel = viewModel;
        }

        private void StartMainWindow()
        {
            MainWindow = new NewMainWindow
            {
                DataContext = mainViewModel
            };

            MainWindow.Closed += (sender, e) => End();

            MainWindow.Show();
        }

        public void End()
        {
            application.Shutdown();
        }
    }
}
