using System;
using System.Collections.Generic;
using System.Text;

namespace CortexCommandModManager
{
    class IncorrectCCMMFileTypeException : Exception
    {
        public IncorrectCCMMFileTypeException() : base() { }
        public IncorrectCCMMFileTypeException(string message) : base(message) { }
        public IncorrectCCMMFileTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
