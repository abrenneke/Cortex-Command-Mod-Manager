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

        private readonly ModDatabaseMod mod;
        private readonly ModDatabase database;

        private bool loadingScreenshot;
        private bool cannotLoadScreenshot;

        public ModDatabaseModViewModel(ModDatabaseMod mod, ModDatabase database)
        {
            this.mod = mod;
            this.database = database;
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
