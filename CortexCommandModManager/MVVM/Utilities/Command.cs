using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CortexCommandModManager.MVVM.Utilities
{
    public class Command : ICommand
    {
        private Action action;
        private Action<object> objectAction;
        private Func<object, bool> canExecute;

        public Command(Action action)
        {
            this.action = action;
            this.canExecute = x => true;
        }

        public Command(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = x => canExecute();
        }

        public Command(Action action, Func<object, bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public Command(Action<object> action)
        {
            this.objectAction = action;
            this.canExecute = x => true;
        }

        public Command(Action<object> action, Func<object, bool> canExecute)
        {
            this.objectAction = action;
            this.canExecute = canExecute;
        }

        public virtual bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual void Execute(object parameter)
        {
            if (action != null)
                action();
            if (objectAction != null)
                objectAction(parameter);
        }
    }

    public class Command<T> : ICommand where T : class
    {
        private Action<T> action;
        private Func<T, bool> canExecute;

        public Command(Action<T> action)
        {
            this.action = action;
            this.canExecute = x => true;
        }

        public Command(Action<T> action, Func<T, bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter as T);
        }

        public void Execute(object parameter)
        {
            if (action != null)
                action(parameter as T);
        }
    }
}
