using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.ModsDatabase;
using System.Windows.Controls;
using CortexCommandModManager.MVVM.Utilities;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using System.Net;
using System.Timers;
using System.ComponentModel;
using System.Windows.Threading;

namespace CortexCommandModManager.MVVM.WindowViewModel.BrowseTab
{
    public class ModDatabaseModViewModel : ViewModel
    {
        public string Title { get { return mod.Title; } }
        public string ShortDescription { get { return mod.ShortDescription; } }

        private BitmapSource screenshot;
        public BitmapSource Screenshot 
        { 
            get 
            {
                if (screenshot == null)
                    LoadScreenshot();
                return screenshot;
            }
            private set
            {
                screenshot = value;
                OnPropertyChanged(x => Screenshot);
            }
        }

        private bool isDownloading;
        public bool IsDownloading { get { return isDownloading; } set { isDownloading = value; OnPropertyChanged(x => IsDownloading); } }

        private int modDownloadProgress;
        public int ModDownloadProgress { get { return modDownloadProgress; } set { modDownloadProgress = value; OnPropertyChanged("ModDownloadProgress"); } }

        private bool modHasBeenDownloaded;
        public bool ModHasBeenDownloaded { get { return modHasBeenDownloaded; } set { modHasBeenDownloaded = value; OnPropertyChanged(x => ModHasBeenDownloaded); } }

        public bool IsIndeterminate { get { return isIndeterminate; } set { isIndeterminate = value; OnPropertyChanged(x => IsIndeterminate); } }
        private bool isIndeterminate;

        public Command DownloadModCommand { get; private set; }

        public event Action<ModDatabaseModViewModel> ModInstalled;

        private readonly ModDatabaseMod mod;
        private readonly ModDatabase database;

        private readonly DownloadedModSaver saver;
        private readonly ModExtracter extractor;
        private readonly Timer timer;

        private bool loadingScreenshot;
        private bool cannotLoadScreenshot;

        public ModDatabaseModViewModel(ModDatabaseMod mod, ModDatabase database)
        {
            this.mod = mod;
            this.timer = new Timer();
            this.database = database;
            this.saver = new DownloadedModSaver();
            this.extractor = new ModExtracter();

            DownloadModCommand = new Command(DownloadMod, x => !IsDownloading);

            timer.AutoReset = false;
            timer.Elapsed += (o, e) =>
            {
                ModHasBeenDownloaded = false;
                timer.Stop();
            };
            timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
        }

        private void DownloadMod()
        {
            if (IsDownloading)
                return;

            ModDownloadProgress = 0;
            IsDownloading = true;
            database.DownloadModAsync(mod, DownloadModCallback, DownloadModProgress);
        }

        private void DownloadModCallback(byte[] data)
        {
            IsIndeterminate = true;

            var dispatcher = Dispatcher.CurrentDispatcher;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, e) => 
            {
                var file = saver.Save(this.mod, data);
                extractor.Unpack(file.FullName);

                IsIndeterminate = false;
                IsDownloading = false;
                ModHasBeenDownloaded = true;

                if (ModInstalled != null)
                {
                    dispatcher.Invoke(new Action(() => ModInstalled(this)), DispatcherPriority.DataBind);
                }

                timer.Start(); //This will cause the donwload completed to go away in 5 seconds.
            };

            worker.RunWorkerAsync();
        }

        private void DownloadModProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ModDownloadProgress = e.ProgressPercentage;
        }

        private void LoadScreenshot()
        {
            if (loadingScreenshot || cannotLoadScreenshot)
                return;

            loadingScreenshot = true;
            database.DownloadImageAsync(mod, LoadScreenshotCallback);
        }

        private void LoadScreenshotCallback(byte[] screenshotData)
        {
            loadingScreenshot = false;

            var image = new BitmapImage();

            image.BeginInit();

            var stream = new MemoryStream(screenshotData);
            image.StreamSource = stream;
            try
            {
                image.EndInit();
            }
            catch (Exception)
            {
                Screenshot = null;
                cannotLoadScreenshot = true;
                return;
            }

            Screenshot = image;
        }
    }
}
