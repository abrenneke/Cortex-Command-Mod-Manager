using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.NewActivities;
using System.Collections.Generic;

namespace CortexCommandModManager.MVVM.WindowViewModel.ActivitiesTab
{
    public class ActivitiesTabViewModel : ViewModel, IInitializable
    {
        public bool IsAddingActivity { get { return isAddingActivity; } set { isAddingActivity = value; OnPropertyChanged(x => IsAddingActivity); } }
        private bool isAddingActivity;

        public ICommand AddActivityCommand { get; private set; }
        public ICommand ConfirmNewActivityCommand { get; private set; }
        public ICommand CancelNewActivityCommand { get; private set; }

        public ObservableCollection<ActivityViewModel> Activities { get { return activities; } }
        private readonly ObservableCollection<ActivityViewModel> activities;

        public ObservableCollection<ActivityTypeViewModel> ActivityTypes { get { return activityTypes; } }
        private readonly ObservableCollection<ActivityTypeViewModel> activityTypes;

        private readonly SavedActivitiesManager savedActivities;
        private readonly ActivityLoader activityLoader;


        public ActivityTypeViewModel NewActivityType { get { return newActivityType; } set { newActivityType = value; OnPropertyChanged(x => NewActivityType); } }
        private ActivityTypeViewModel newActivityType;

        public string NewActivityName { get { return newActivityName; } set { newActivityName = value; OnPropertyChanged(x => NewActivityName); } }
        private string newActivityName;

        public ActivitiesTabViewModel()
        {
            activities = new ObservableCollection<ActivityViewModel>();
            activityTypes = new ObservableCollection<ActivityTypeViewModel>();

            savedActivities = new SavedActivitiesManager(Grabber.ModManagerDirectory);
            activityLoader = new ActivityLoader(Grabber.ModManagerDirectory);

            AddActivityCommand = new Command(x => { IsAddingActivity = true; });
            ConfirmNewActivityCommand = new Command(ConfirmNewActivity, CanConfirmNewActivity);
            CancelNewActivityCommand = new Command(CancelNewActivity);
        }

        public void Initialize()
        {
            savedActivities.Initialize();

            var activities = savedActivities.LoadAll();
            foreach (var activity in activities)
            {
                Activities.Add(new ActivityViewModel(activity));
            }

            var activityTypes = activityLoader.LoadAll();
            foreach (var type in activityTypes)
            {
                ActivityTypes.Add(new ActivityTypeViewModel(type));
            }

            NewActivityType = ActivityTypes.FirstOrDefault();
            NewActivityName = String.Empty;
        }

        private void ConfirmNewActivity()
        {
            var newActivity = new Activity { Name = NewActivityName, ActivityName = NewActivityType.Name };
            Activities.Add(new ActivityViewModel(newActivity));

            savedActivities.SaveAll(GetActivitiesRaw());

            NewActivityType = ActivityTypes.FirstOrDefault();
            NewActivityName = String.Empty;
            IsAddingActivity = false;
        }

        private bool CanConfirmNewActivity()
        {
            return NewActivityType != null && 
                   !string.IsNullOrWhiteSpace(NewActivityName) && 
                   !Activities.Any(x => x.Name.Equals(NewActivityName, StringComparison.OrdinalIgnoreCase));
        }

        private void CancelNewActivity()
        {
            NewActivityType = ActivityTypes.FirstOrDefault();
            NewActivityName = String.Empty;
            IsAddingActivity = false;
        }

        private IList<Activity> GetActivitiesRaw()
        {
            return Activities.Select(x => x.Activity).ToList();
        }
    }
}
