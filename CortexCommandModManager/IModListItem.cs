using System;
using System.Collections.Generic;
using System.Text;

namespace CortexCommandModManager
{
    public interface IModListItem
    {
        /// <summary>Gets the name of the mod item.</summary>
        string Name { get; }

        /// <summary>Gets or sets whether the mod item is enabled.</summary>
        bool IsEnabled { get; }

        /// <summary>Gets whether the mod list item is preinstalled and cannot be removed.</summary>
        bool IsPreinstalled { get; }
    }
}
