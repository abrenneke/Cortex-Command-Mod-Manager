using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CortexCommandModManager.Activities
{
    public class RandomSettings : INotifyPropertyChanged
    {
        public int MinimumCraft { get; set; }
        public int MaximumCraft { get; set; }

        public int MinimumActorsPerCraft { get; set; }
        public int MaximumActorsPerCraft { get; set; }

        public int MinimumWeaponsPerActor { get; set; }
        public int MaximumWeaponsPerActor { get; set; }

        public bool CrabsCanCarryWeapons { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public RandomSettings()
        {
            MinimumCraft = 10;
            MaximumCraft = 25;
            MinimumActorsPerCraft = 1;
            MaximumActorsPerCraft = 3;
            MinimumWeaponsPerActor = 1;
            MaximumWeaponsPerActor = 3;
            CrabsCanCarryWeapons = false;
        }
    }
}
