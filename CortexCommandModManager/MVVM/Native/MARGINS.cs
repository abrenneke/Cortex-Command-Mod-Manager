using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CortexCommandModManager.MVVM.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        /// <summary>The left margin.</summary>
        public int cxLeftWidth;
        
        /// <summary>The right margin.</summary>
        public int cxRightWidth;

        /// <summary>The top margin.</summary>
        public int cyTopHeight;

        /// <summary>The bottom margin.</summary>
        public int cyBottomHeight;
    }
}
