using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CortexCommandModManager.MVVM.WindowViewModel
{
    /// <summary>Static methods for getting program-resource (in the Resources folder)</summary>
    public static class Resource
    {
        /// <summary>
        /// Gets an image from the resources folder (using a pack URI)
        /// </summary>
        /// <param name="name">The name of the image, or directory + name relative to the resources folder.</param>
        /// <returns>An image suitable for data binding.</returns>
        public static Image GetImage(string name)
        {
            var image = new Image
            {
                Width = 16,
                Height = 16
            };

            var icon = new BitmapImage();
            icon.BeginInit();
            icon.UriSource = new Uri("pack://application:,,,/CortexCommandModManager;component/Resources/" + name);
            icon.EndInit();

            image.Source = icon;

            return image;
        }
    }
}
