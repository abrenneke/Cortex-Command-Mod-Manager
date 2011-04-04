using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.NewActivities;

namespace CortexCommandModManager.MVVM.WindowViewModel.ActivitiesTab
{
    public class ActivityViewModel : ViewModel
    {
        public Activity Activity { get; private set; }

        public string Name { get { return Activity.Name; } set { Activity.Name = value; OnPropertyChanged(x => Name); } }

        public string ActivityName { get { return Activity.ActivityName; } set { Activity.ActivityName = value; OnPropertyChanged(x => ActivityName); } }

        public ActivityViewModel(Activity activity)
        {
            Activity = activity;
        }
    }
}
