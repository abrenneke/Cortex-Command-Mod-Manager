using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;

namespace CortexCommandModManager.MVVM.WindowViewModel.Menu
{
    public class MenuViewModel : ViewModel
    {
        public MenuIcons Icons { get; private set; }

        public MenuViewModel()
        {
            Icons = new MenuIcons();
        }
    }
}
