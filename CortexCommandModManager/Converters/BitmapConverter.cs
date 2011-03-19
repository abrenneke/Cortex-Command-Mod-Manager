using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Data;


namespace CortexCommandModManager
{
    [ValueConversion(typeof(System.Drawing.Bitmap),typeof(System.Windows.Media.Imaging.BitmapSource))]
    class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)value;
         	throw new NotImplementedException();
        }

        public object  ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
         	throw new NotImplementedException();
        }
    }
}
