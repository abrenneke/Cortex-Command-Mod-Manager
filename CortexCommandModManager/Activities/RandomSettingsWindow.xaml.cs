using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CortexCommandModManager.Activities
{
    /// <summary>
    /// Interaction logic for RandomSettingsWindow.xaml
    /// </summary>
    public partial class RandomSettingsWindow
    {
        public event Action<RandomSettings, IEnumerable<Mod>> GenerateRandomSkirmishClicked;

        public RandomSettings Settings { get; set; }
        private IEnumerable<Mod> mods { get; set; }

        public RandomSettingsWindow(IEnumerable<Mod> mods) : this(mods, new RandomSettings()) { }

        public RandomSettingsWindow(IEnumerable<Mod> mods, RandomSettings defaultSettings)
        {
            InitializeComponent();

            Settings = defaultSettings;
            Settings.PropertyChanged += Settings_PropertyChanged;
            MainGrid.DataContext = Settings;

            this.mods = mods;

            modsToIncludeListBox.ItemsSource = mods;
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int value = 0;
            switch (e.PropertyName)
            {
                case "MaximumCraft":
                    value = (int)Math.Min(minCraftSlider.Value, Settings.MaximumCraft);
                    minCraftSlider.Value = value; minCraftTextBox.Text = value.ToString();
                    break;
                case "MaximumActorsPerCraft":
                    value = (int)Math.Min(minActorsSlider.Value, Settings.MaximumActorsPerCraft);
                    minActorsSlider.Value = value; minActorsTextBox.Text = value.ToString();
                    break;
                case "MaximumWeaponsPerActor":
                    value = (int)Math.Min(minWeaponsSlider.Value, Settings.MaximumWeaponsPerActor);
                    minWeaponsSlider.Value = value; minWeaponsTextBox.Text = value.ToString();
                    break;
            }
        }

        private void ModsToIncludeListBox_ContextMenu_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            modsToIncludeListBox.SelectAll();
        }

        private void ModsToIncludeListBox_ContextMenu_SelectNone_Click(object sender, RoutedEventArgs e)
        {
            modsToIncludeListBox.SelectedIndex = -1;
        }

        private void modsToIncludeListBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void generateSkirmishButton_Click(object sender, RoutedEventArgs e)
        {
            var modsList = new List<Mod>();
            foreach(Mod mod in modsToIncludeListBox.SelectedItems)
                modsList.Add(mod);
            if(GenerateRandomSkirmishClicked != null)
                GenerateRandomSkirmishClicked(Settings, modsList);
        }

        private void Window_ContextMenu_ResetToDefaults_Click(object sender, RoutedEventArgs e)
        {
            Settings = new RandomSettings();
            MainGrid.DataContext = Settings;
        }
    }
}
