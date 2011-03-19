using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager
{
    public static class StringExtensions
    {
        /// <summary>Formats the string with the specified arguments using String.Format.</summary>
        public static string With(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>Returns true if the string is null or empty.</summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }
    }
}
