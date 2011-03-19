using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace CortexCommandModManager
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    class ItemEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isEnabled = (bool)value;
            if (isEnabled)
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(159, 227, 172), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(192, 255, 204), 0.5));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(159, 227, 172), 1));
                brush.StartPoint = new System.Windows.Point(0.5, 0);
                brush.EndPoint = new System.Windows.Point(0.5, 1);
                return brush;
            }
            else
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(161, 194, 240), 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(192, 219, 255), 0.5));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(161, 194, 240), 1));
                brush.StartPoint = new System.Windows.Point(0.5, 0);
                brush.EndPoint = new System.Windows.Point(0.5, 1);
                return brush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
