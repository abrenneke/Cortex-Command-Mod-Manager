using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CortexCommandModManager.MVVM.Native
{
    /// <summary>Static methods for native window methods.</summary>
    public static class NativeWindow
    {
        /// <summary>Extends the glass in to the client area.</summary>
        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref MARGINS pMarInset);
    }
}
