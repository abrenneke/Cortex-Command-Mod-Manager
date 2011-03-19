using System.ComponentModel;
using System.Windows;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.MVVM.WindowViewModel;
using System.Windows.Interop;
using CortexCommandModManager.MVVM.Native;
using System;
using System.Windows.Media;

namespace CortexCommandModManager.MVVM
{
    public partial class NewMainWindow : Window
    {
        public NewMainWindowViewModel ViewModel { get { return DataContext as NewMainWindowViewModel; } }

        public NewMainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Event handler for when the main window is loaded.</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExtendGlass();
        }

        /// <summary>Event handler for when the main window is closing. Attached behaviors do not work for this.</summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.WindowClosingCommand.ExecuteIfCan();
        }

        private void ExtendGlass()
        {
            try
            {
                var pointer = new WindowInteropHelper(this).Handle;
                var hwndSource = HwndSource.FromHwnd(pointer);

                Background = Brushes.Transparent;
                hwndSource.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                var margins = new MARGINS
                {
                    cxLeftWidth = -1,
                    cxRightWidth = -1,
                    cyBottomHeight = -1,
                    cyTopHeight = -1
                };

                var result = NativeWindow.DwmExtendFrameIntoClientArea(hwndSource.Handle, ref margins);

                //if(result < 0)
                //Failed extending.
            }
            catch (Exception)
            {
                //Not vista or 7
                Background = Brushes.White;
            }
        }
    }
}
