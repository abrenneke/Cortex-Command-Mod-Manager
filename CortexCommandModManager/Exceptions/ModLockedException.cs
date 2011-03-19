using System;
using System.Collections.Generic;

using System.Text;

namespace CortexCommandModManager
{
    class ModLockedException : Exception
    {
        public ModLockedException() : base() { }
        public ModLockedException(string message) : base(message) { }
    }
}
