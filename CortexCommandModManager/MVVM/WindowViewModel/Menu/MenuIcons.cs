using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CortexCommandModManager.MVVM.Utilities;
using System.Windows.Media.Imaging;

namespace CortexCommandModManager.MVVM.WindowViewModel
{
    /// <summary>Properties to access the icons for the various menu items.</summary>
    public class MenuIcons : ViewModel
    {
        /// <summary>Gets the icon for the File/Settings menu item.</summary>
        public Image File_Settings { get { return Resource.GetImage("Wrench_Outline_Icon_clip_art_small.png"); } }

        /// <summary>Gets the icon for the File/Exit menu item.</summary>
        public Image File_Exit { get { return Resource.GetImage("xIcon.png"); } }

        /// <summary>Gets the icon for the Help/About menu item.</summary>
        public Image Help_About { get { return Resource.GetImage("2009-12-26_231049.ico"); } }
    }
}
