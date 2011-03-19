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

        public void Enable()
        {
            foreach (Mod mod in this)
            {
                mod.Enable();
            }
            this.IsEnabled = true;
            PresetManager.UpdatePreset(this);
        }
        public void Disable()
        {
            foreach (Mod mod in this)
            {
                mod.Disable();
            }
            this.IsEnabled = false;
            PresetManager.UpdatePreset(this);
        }
        public void ToggleEnabled()
        {
            if (IsEnabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Preset;
            if (other == null) return false;

            return Name == other.Name;
        }
    }
}
