using System;
using System.Collections.Generic;

namespace CortexCommandModManager
{
    public class ModItemComparer : IComparer<IModListItem>
    {
        public int Compare(IModListItem x, IModListItem y)
        {
            return StringComparer.CurrentCulture.Compare(x.Name, y.Name);
        }
    }
}
