using System.Collections.Generic;

namespace CortexCommandModManager
{
    public class Preset : List<Mod>, IModListItem
    {
        public string Name { get; private set; }
        public bool IsEnabled { get; set; }
        public string ModCount { get { return "(" + Count + ")"; } }

        public bool IsPreinstalled { get { return false; } }

        public Preset(string name, bool enabled)
        {
            Name = name;
            IsEnabled = enabled;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Preset;
            if (other == null) return false;

            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
