using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.NewActivities;

namespace CortexCommandModManager.MVVM.WindowViewModel.ActivitiesTab
{
    public class ActivityTypeViewModel : ViewModel
    {
        public ParsedActivity Inner { get; private set; }

        public string Name { get { return Inner.Name; } }

        public ActivityTypeViewModel(ParsedActivity inner)
        {
            Inner = inner;
        }
    }
}
