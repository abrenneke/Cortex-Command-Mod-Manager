using System.Collections.Generic;

namespace CortexCommandModManager
{
    public class ModEntity
    {
        public string Name { get; set; }
    }
    public class ModBase
    {
        public string Name { get; set; }

        public string ModPathName { get; set; }

        public List<ModWeapon> Weapons { get; set; }

        public List<ModActor> Actors { get; set; }

        public List<ModShip> Ships { get; set; }
    }
    public class ModWeapon : ModEntity
    {
    }
    public class ModActor : ModEntity
    {
    }
    public class ModShip : ModEntity
    {
    }
}
