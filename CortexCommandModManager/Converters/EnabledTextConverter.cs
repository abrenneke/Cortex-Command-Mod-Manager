using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Data;

namespace CortexCommandModManager
{
    [ValueConversion(typeof(bool),typeof(string))]
    class EnabledTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isEnabled = (bool)value;
            return (isEnabled) ? "Enabled" : "Disabled";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
