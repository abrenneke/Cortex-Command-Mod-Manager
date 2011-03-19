using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Windows.Input;

namespace CortexCommandModManager.MVVM.Utilities
{
    public class CommandBehavior : IDisposable
    {
        public string EventName { get; private set; }
        public EventInfo Event { get; private set; }
        public Delegate EventHandler { get; private set; }
        public DependencyObject Owner { get; private set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }

        private bool isDisposed;

        public void BindEvent(DependencyObject owner, string eventName)
        {
            EventName = eventName;
            Owner = owner;
            Event = Owner.GetType().GetEvent(EventName, BindingFlags.Public | BindingFlags.Instance);

            if (Event == null)
                throw new InvalidOperationException("Could not resolve event name {0}".With(EventName));

            EventHandler = EventHandlerGenerator.CreateDelegate(
                Event.EventHandlerType, typeof(CommandBehavior).GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance), this);

            Event.AddEventHandler(Owner, EventHandler);
        }

        public void Execute()
        {
            Command.ExecuteIfCan(CommandParameter);
        }

        /// <summary>Unregisters the event handler.</summary>
        public void Dispose()
        {
            if (isDisposed)
                return;

            Event.RemoveEventHandler(Owner, EventHandler);
            isDisposed = true;
        }
    }
}
