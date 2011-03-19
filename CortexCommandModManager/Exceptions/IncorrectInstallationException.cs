using System;
using System.Collections.Generic;

using System.Text;

namespace CortexCommandModManager
{
    class IncorrectInstallationException : Exception
    {
        public IncorrectInstallationException() : base() { }
        public IncorrectInstallationException(string message) : base(message) { }
    }
}
