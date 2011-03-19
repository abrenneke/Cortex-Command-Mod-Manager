using System.Windows;
using System.Windows.Controls;

namespace CortexCommandModManager
{
    public class ModListTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is IModListItem)
            {
                IModListItem listItem = item as IModListItem;

                if (listItem is Mod)
                {
                    return element.FindResource("modItemTemplate") as DataTemplate;
                }
                else if(listItem is Preset)
                {
                    return element.FindResource("presetItemTemplate") as DataTemplate;
                }
            }
            return null;
        }
    }
}
