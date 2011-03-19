using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>Executes an action on each item in the collection.</summary>
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
