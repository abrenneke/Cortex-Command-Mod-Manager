using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using CortexCommandModManager.MVVM.Utilities;

namespace CortexCommandModManager.MVVM.WindowViewModel.ModsTab
{
    public class ModListItemTemplateSelector : DataTemplateSelector
    {
        public const string ModItemKey = "ModItemTemplate";
        public const string PresetItemKey = "PresetItemTemplate";

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            var listItem = item as IViewModel;

            if (listItem == null || element == null)
                return null;

            if (listItem is ModViewModel)
                return GetTemplate(ModItemKey, element);
            if (listItem is PresetViewModel)
                return GetTemplate(PresetItemKey, element);

            return null;
        }

        private DataTemplate GetTemplate(string key, FrameworkElement element)
        {
            return element.FindResource(key) as DataTemplate;
        }
    }
}
