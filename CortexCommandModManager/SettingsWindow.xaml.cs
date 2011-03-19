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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsObject settingsObject;
        public SettingsWindow(SettingsObject settingsObject)
        {
            InitializeComponent();
            this.settingsObject = settingsObject;

            installDirectoryBox.Text = settingsObject.CCInstallDirectory;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CortexCommand.IsInstalledTo(installDirectoryBox.Text))
            {
                MessageBox.Show("The directory you chose does not contain a valid Cortex Command installation. Please choose a folder where Cortex Command is installed.", 
                    "Notice", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Asterisk);
                return;
            }
            this.settingsObject.CCInstallDirectory = installDirectoryBox.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
