using System;
using System.ComponentModel;
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
using System.IO;
using CortexCommandModManager.Activities;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for LoadActivityWindow.xaml
    /// </summary>
    public partial class LoadSaveActivitiesWindow : Window
    {
        private readonly string DefaultActivitiesDirectory;
        public delegate void ActivitySelectionChangedDelegate(Activity selectedActivity);
        public event ActivitySelectionChangedDelegate ActivitySelectionChanged;

        public delegate Activity RequestCurrentActivityDelegate();
        public RequestCurrentActivityDelegate RequestCurrentActivity;

        public delegate void ActivitySavedDelegate();
        public event ActivitySavedDelegate ActivitySaved;

        private BindingList<Activity> internalActivitiesList;

        public LoadSaveActivitiesWindow()
        {
            InitializeComponent();
            DefaultActivitiesDirectory = Grabber.ModManagerDirectory + "\\Activities";
            if (!Directory.Exists(DefaultActivitiesDirectory))
            {
                Directory.CreateDirectory(DefaultActivitiesDirectory);
            }
            internalActivitiesList = new BindingList<Activity>();
            activitiesList.ItemsSource = internalActivitiesList;

        }
        public void Reload()
        {
            ClearActivities();
            LoadActivities();
        }

        private void ClearActivities()
        {
            internalActivitiesList.Clear();
        }

        private void LoadActivities()
        {
            List<Activity> activities = new List<Activity>();
            string[] files = GetCCMMFilesInActivitesDirectory();
            foreach (string file in files)
            {
                Activity activity;
                try
                {
                    activity = ActivityLoader.Load(file);
                    activities.Add(activity);
                }
                catch (ActivityLoadException ex)
                {
                    MessageBox.Show("ERROR ( " + file + " ) " + ex.Message, "Error Loading Activity", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IncorrectCCMMFileTypeException) { }
            }
            foreach(Activity activity in activities)
            {
                internalActivitiesList.Add(activity);
            }
        }

        private string[] GetCCMMFilesInActivitesDirectory()
        {
            return Directory.GetFiles(DefaultActivitiesDirectory, "*" + Grabber.ActivityFileExtension);
        }

        private void activitiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ActivitySelectionChanged((Activity)activitiesList.SelectedItem);
            }
            catch (NullReferenceException)
            {
            }
        }

        private void btnSaveCurrent_Click(object sender, RoutedEventArgs e)
        {
            bool saved = SaveActivity(RequestCurrentActivity());
            if (saved)
            {
                Reload();
            }
        }

        private bool SaveActivity(Activity activity)
        {
            if (String.IsNullOrEmpty(activity.Name) || activity.Root == null)
            {
                return false;
            }
            if (AllowSaveOrOverwrite(activity))
            {
                ActivitySaver saver = new ActivitySaver(activity);
                saver.Save();
                if (ActivitySaved != null)
                {
                    ActivitySaved();
                }
                return true;
            }
            return false;
        }

        private bool AllowSaveOrOverwrite(Activity activity)
        {
            foreach (Activity checkActivity in internalActivitiesList)
            {
                if (ActivitySaver.EscapeName(checkActivity.Name) == ActivitySaver.EscapeName(activity.Name))
                {
                    bool result = AskForOverwrite();
                    return result;
                }
            }
            return true;
        }

        private bool AskForOverwrite()
        {
            MessageBoxResult result = MessageBox.Show("An activity with this name already exists. Overwrite?", "Overwrite", 
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No, MessageBoxOptions.None);
            return (result == MessageBoxResult.Yes) ? true : false;
        }

        internal void InvokeSave()
        {
            SaveActivity(RequestCurrentActivity());
        }

        private void menuDeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            TryDeleteSelectedActivity();
        }

        private void TryDeleteSelectedActivity()
        {
            Activity selectedActivity;
            try
            {
                selectedActivity = (Activity)activitiesList.SelectedItem;
            }
            catch (Exception) { return; }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this activity?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            DeleteActivity(selectedActivity);
            Reload();
        }

        private void DeleteActivity(Activity selectedActivity)
        {
            string possibleFullPath = Grabber.ModManagerDirectory + "\\Activities\\" + ActivitySaver.EscapeName(selectedActivity.Name) + Grabber.ActivityFileExtension;
            if (File.Exists(possibleFullPath))
            {
                File.Delete(possibleFullPath);
            }
        }

        private void btnNewActivity_Click(object sender, RoutedEventArgs e)
        {
            Activity newActivity = new Activity();
            newActivity.Name = ActivityLoader.GetHighestAvailableTemplateActivityString();
            newActivity.Root = new InventoryItem();
            newActivity.Root.CopyOf = "All Waves";
            try
            {
                ActivitySelectionChanged(newActivity);
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}
