using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CortexCommandModManager.MVVM.Utilities
{
    public class ElementHelper
    {
        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) 
                return null;

            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);

                if (parent != null) 
                    return parent;

                var frameworkContentElement = contentElement as FrameworkContentElement;

                return frameworkContentElement != null ? 
                    frameworkContentElement.Parent : null;
            }

            var frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                var parent = frameworkElement.Parent;

                if (parent != null) 
                    return parent;
            }

            return VisualTreeHelper.GetParent(child);
        }
    }
}
