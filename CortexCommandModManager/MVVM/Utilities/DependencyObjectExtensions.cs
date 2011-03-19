using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CortexCommandModManager.MVVM.Utilities
{
    public static class DependencyObjectExtensions
    {
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var parentObject = ElementHelper.GetParentObject(child);

            if (parentObject == null) 
                return null;

            T parent = parentObject as T;

            if (parent != null)
                return parent;

            return FindParent<T>(parentObject);
        }
    }
}
