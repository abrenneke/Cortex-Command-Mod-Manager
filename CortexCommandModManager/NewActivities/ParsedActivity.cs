using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager.NewActivities
{
    public class ParsedActivity
    {
        public string Name { get; set; }

        public IList<ActivityVariable> Variables { get; private set; }

        public string IniFormat { get; set; }

        public string LuaFileName { get; set; }

        public string FolderLocation { get; set; }

        public ParsedActivity()
        {
            Variables = new List<ActivityVariable>();
        }
    }
}
