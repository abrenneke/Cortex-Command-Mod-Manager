using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using CortexCommandModManager.Activities;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for ActivityItemLoaderWindow.xaml
    /// </summary>
    public partial class ActivityItemLoaderWindow : Window
    {
        public IEnumerable<ActivityItem> Activities { get; set; }

        private readonly ModScanner scanner;
        private readonly ModActivityItemsLoader activityItemsLoader;
        private BackgroundWorker loadingWorker;

        public ActivityItemLoaderWindow(ModScanner scanner, ModActivityItemsLoader activityItemsLoader)
        {
            this.scanner = scanner;
            this.activityItemsLoader = activityItemsLoader;

            InitializeComponent();

            if (Grabber.ActivityItemCache == null)
                Grabber.ActivityItemCache = new ActivityItemCache(scanner, ".modcache");

            loadingWorker = new BackgroundWorker();
            loadingWorker.DoWork += (o, e) => LoadActivityItems();
            loadingWorker.ProgressChanged += (e, o) => 
                {
                    if(o.UserState is Mod)
                        loadingLabel.Content = ((Mod)o.UserState).Folder;
                    if (o.UserState is string)
                        loadingLabel.Content = (string)o.UserState;
                }; 
            loadingWorker.RunWorkerCompleted += (o, e) => Close();
            loadingWorker.WorkerReportsProgress = true;
            loadingWorker.RunWorkerAsync();
       } 

        private void LoadActivityItems()
        {
            var enabledMods = scanner.GetEnabledMods();

            foreach (var mod in enabledMods)
            {
                if (!Grabber.ActivityItemCache.ModIsCached(mod))
                {
                    loadingWorker.ReportProgress(50, mod);
                    Grabber.ActivityItemCache.AddItems(activityItemsLoader.LoadMod(mod));
                }
            }

            Grabber.ActivityItemCache.EnabledMods = enabledMods;
            Activities = Grabber.ActivityItemCache.GetAll();
            Grabber.ActivityItemCache.SaveCache();
            Grabber.ActivityItemCache.ValidateCache();

            /*var CCPath = Grabber.Settings.Get().CCInstallDirectory;
            foreach (var activityItem in Activities)
            {
                loadingWorker.ReportProgress(75, "Image: " + activityItem.PresetName);
                Dispatcher.Invoke(new Action(() => activityItem.MakeBitmapImage(CCPath)));
            }*/
        }
    }
}
