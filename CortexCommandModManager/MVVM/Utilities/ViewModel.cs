using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace CortexCommandModManager.MVVM.Utilities
{
    public abstract class ViewModel : IViewModel
    {
        /// <summary>Event when a property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Calls the property changed event for the ViewModel, with the specified property name.</summary>
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>Calls the property changed event for the ViewModel with a property.</summary>
        public void OnPropertyChanged<T>(Expression<Func<object, T>> propertyAction)
        {
            var expression = (MemberExpression)propertyAction.Body;
            var propertyName = expression.Member.Name;
            OnPropertyChanged(propertyName);
        }

        protected static T ThrowIfNull<T>(T item) where T : class
        {
            if (item == null)
                throw new InvalidOperationException("Attempt to access property before it it set. Set property first.");
            return item;
        }
    }
}
