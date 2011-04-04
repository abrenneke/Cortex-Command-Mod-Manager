using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace CortexCommandModManager.MVVM.Controls
{
    public class Overlay : Border
    {
        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(Overlay), new PropertyMetadata(IsShownChanged));

        public bool IsShown { get { return (bool)GetValue(IsShownProperty); } set { SetValue(IsShownProperty, value); } }

        public Overlay()
        {
            SetValue(VisibilityProperty, Visibility.Collapsed);
            SetValue(BackgroundProperty, new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)));
        }

        private static void IsShownChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var isShown = (bool)e.NewValue;

            var overlay = (Overlay)obj;

            overlay.Visibility = isShown ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
