using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace CortexCommandModManager.MVVM.Utilities
{
    public class TextBoxAttached
    {
        public static DependencyProperty EnterCommandProperty =
            DependencyProperty.RegisterAttached("EnterCommand", typeof(ICommand), typeof(TextBoxAttached),
            new PropertyMetadata((ICommand)null, EnterCommandChanged));

        public static void SetEnterCommand(DependencyObject obj, ICommand command)
        {
            var textBox = obj as TextBox;

            textBox.SetValue(EnterCommandProperty, command);

            textBox.KeyDown += (o, keyArgs) =>
            {
                if (keyArgs.Key == Key.Enter)
                {
                    command.ExecuteIfCan(keyArgs);
                }
            };
        }

        public static ICommand GetEnterCommand(DependencyObject obj)
        {
            return obj.GetValue(EnterCommandProperty) as ICommand;
        }

        public static void EnterCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null)
                return;
            SetEnterCommand(sender, e.NewValue as ICommand);
        }
    }
}
