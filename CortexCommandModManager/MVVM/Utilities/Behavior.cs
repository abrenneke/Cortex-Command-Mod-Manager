using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace CortexCommandModManager.MVVM.Utilities
{
    public class Behavior : Freezable
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(Behavior),
                new FrameworkPropertyMetadata(CommandChanged));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(string), typeof(Behavior),
                new FrameworkPropertyMetadata(EventChanged));

        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(Behavior),
                new FrameworkPropertyMetadata(CommandParameterChanged));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private DependencyObject owner;
        public DependencyObject Owner
        {
            get { return owner; }
            set 
            { 
                owner = value; 
                ResetEventBindings(); 
            }
        }

        private Lazy<CommandBehavior> behavior = new Lazy<CommandBehavior>();
        public CommandBehavior InnerBehavior { get { return behavior.Value; } }

        public Behavior()
        {
        }

        private void ResetEventBindings()
        {
            if (Owner == null)
                return;

            if (InnerBehavior.Event != null && InnerBehavior.Owner != null)
                InnerBehavior.Dispose();

            InnerBehavior.BindEvent(Owner, Event);
        }

        public static void CommandParameterChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ((Behavior)element).InnerBehavior.CommandParameter = e.NewValue;
        }


        public static void EventChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ((Behavior)element).ResetEventBindings();
        }

        public static void CommandChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var behavior = element as Behavior;
            behavior.InnerBehavior.Command = behavior.Command;
        }

        protected override Freezable CreateInstanceCore()
        { 
            return new Behavior { Command = Command, CommandParameter = CommandParameter, Event = Event, Owner = Owner };
        }
    }
}
