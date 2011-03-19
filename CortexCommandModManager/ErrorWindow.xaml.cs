using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for Error.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private Exception exception;

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow(Exception exception)
        {
            InitializeComponent();
            this.exception = exception;
            errorMessage.Text = exception.Message;
            detailsText.Text = "EXCEPTION: \n" + exception.ToString();
            detailsText.Text += (exception.InnerException != null) ? "\n\nINNER EXCEPTION: \n" + exception.InnerException.ToString() : "\n\nNO INNER EXCEPTION";
        }

        public static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Create(exception);
        }

        public static void Create(Exception ex)
        {
            var window = new ErrorWindow(ex);
            if (Global.MainWindow != null)
            {
                window.Owner = Global.MainWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            window.ShowDialog();
            throw ex;
        }
    }
}
