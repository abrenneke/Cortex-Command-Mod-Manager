using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace CortexCommandModManager.Converters
{
    [ValueConversion(typeof(string),typeof(string))]
    class ActivityDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string actDescription = (string)value;
            if (String.IsNullOrEmpty(actDescription))
            {
                return "No Description";
            }
            else return actDescription;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
