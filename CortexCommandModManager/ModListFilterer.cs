using System;
using System.Windows.Data;

namespace CortexCommandModManager
{
    static class ModListFilterer
    {
        public static FilterEventHandler FilterHandler = Filter;
        private static bool showEnabled;
        private static bool showDisabled;

        static public void ShowEnabled(bool show)
        {
            showEnabled = show;
        }
        static public void ShowDisabled(bool show)
        {
            showDisabled = show;
        }

        static public void Filter(object sender, FilterEventArgs args)
        {
            IModListItem mod = args.Item as IModListItem;
            if (mod == null) throw new NotImplementedException();
            args.Accepted = mod.IsEnabled ? showEnabled : showDisabled;
        }
    }
}
