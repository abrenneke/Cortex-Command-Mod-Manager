using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CortexCommandModManager.MVVM.Utilities
{
    public static class CommandExtensions
    {   
        /// <summary>Executes a command if it is able.</summary>
        public static void ExecuteIfCan(this ICommand command)
        {
            command.ExecuteIfCan(null);
        }

        /// <summary>Executes a command if it is able.</summary>
        public static void ExecuteIfCan(this ICommand command, object parameter)
        {
            if (command == null)
                return;

            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}
