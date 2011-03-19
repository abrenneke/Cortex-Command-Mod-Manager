using System;
using System.Collections.Generic;
using System.Text;

namespace CortexCommandModManager
{
    class ActivityLoadException : Exception
    {
        public ActivityLoadException() : base() { }
        public ActivityLoadException(string message) : base(message) { }
        public ActivityLoadException(string message, Exception innerException) : base(message, innerException) { }
    }
}
