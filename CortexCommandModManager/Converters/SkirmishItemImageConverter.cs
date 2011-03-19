using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CortexCommandModManager.Converters
{
    [ValueConversion(typeof(string),typeof(BitmapImage))]
    class SkirmishItemImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string type = (string)value;

            switch (type)
            {
                case "ACDropShip":
                    return MakeImage("ACDropShip.png");
                case "ACrab":
                    return MakeImage("ACrab.png");
                case "ACRocket":
                    return MakeImage("ACRocket.png");
                case "AHuman":
                    return MakeImage("AHuman.png");
                case "HDFirearm":
                    return MakeImage("HDFirearm.png");
                case "HeldDevice":
                    return MakeImage("HeldDevice.png");
                case "TDExplosive":
                    return MakeImage("TDExplosive.png");
                default:
                    return null;
            }
        }
        private BitmapImage MakeImage(string file)
        {
            BitmapImage image = new BitmapImage(new Uri("WaveIcons/" + file, UriKind.Relative));
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
