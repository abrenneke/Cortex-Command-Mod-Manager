using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Collections.ObjectModel;

namespace CortexCommandModManager.MVVM.Utilities
{
    /// <summary>A list of behaviors.</summary>
    public class Behaviors : FreezableCollection<Behavior>
    {
        /// <summary>Gets or sets the owner of the binding.</summary>
        public DependencyObject Owner { get; set; }

        protected override Freezable CreateInstanceCore()
        {
            return new Behaviors { Owner = Owner };
        }
    }
}
