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
    /// Interaction logic for AddPresetWindow.xaml
    /// </summary>
    public partial class AddPresetWindow : Window
    {
        public string PresetName { get; set; }
        public AddPresetWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(presetNameBox.Text))
            {
                PresetName = presetNameBox.Text;
                DialogResult = true;
            }
        }

        private void presetNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PresetName = presetNameBox.Text;
                DialogResult = true;
            }
        }
    }
}
