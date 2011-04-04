using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager.NewActivities
{
    public class ActivityVariable
    {
        public string Name { get; set; }

        public IDictionary<string, string> Properties { get; private set; }

        public ActivityVariable()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}
