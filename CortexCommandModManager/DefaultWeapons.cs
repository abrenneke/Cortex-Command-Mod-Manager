
namespace CortexCommandModManager
{
    public class DefaultItems
    {
        private static readonly string[] factions =
            new[] { "Browncoats", "Coalition", "Dummy", "Ronin", "Undead" };

        public static string[] Factions
        {
            get { return factions; }
        } 


        private static readonly DefaultWeapons weapons = new DefaultWeapons();

        public static DefaultWeapons Weapons
        {
          get { return weapons; }  
        }
        private static readonly DefaultActors actors = new DefaultActors();

        public static DefaultActors Actors
        {
            get { return actors; }
        } 

        private static readonly DefaultShips ships = new DefaultShips();

        public static DefaultShips Ships
        {
            get { return ships; }
        } 


    }
    public abstract class DefaultType
    {
        public abstract string[] Coalition { get; }
        public abstract string[] Browncoats { get; }
        public abstract string[] Undead { get; }
        public abstract string[] Ronin { get; }
        public abstract string[] Dummy { get; }
    }
    public class DefaultWeapons : DefaultType
    {
        private readonly string[] coalition = new[] { "Coalition/Spike Launcher","Coalition/Auto Pistol","Coalition/Grenade","Coalition/Flak Cannon",
                                                "Coalition/Gatling Gun","Coalition/Auto Shot Pistol","Coalition/Heavy Sniper Rifle",
                                                "Coalition/Sniper Rifle","Coalition/Pulse Digger","Coalition/Assault Rifle", "Coalition/Cluster Grenade",
                                                "Coalition/Pistol", "Coalition/Concrete Sprayer", "Coalition/Auto Cannon", "Coalition/Revolver Cannon",
                                                "Coalition/Uber Cannon", "Coalition/Napalm Flamer", "Coalition/Flamer", "Coalition/Grenade Launcher",
                                                "Coalition/Rocket Launcher", "Coalition/Homing Missile Launcher", "Coalition/Mauler Shotgun",
                                                "Coalition/Compact Assault Rifle", "Coalition/Shotgun", "Coalition/Auto Shotgun","Coalition/Riot Shield",
                                                "Blue Bomb","Frag Grenade","Coalition/Standard Bomb","Coalition/Napalm Bomb", "Coalition/Incendiary Grenade"};

        private readonly string[] ronin = new[] {"Ronin/Bazooka","Ronin/Glock","Ronin/Riot Shield","Ronin/Medium Digger",
                                                "Ronin/M16","Ronin/Luger","Ronin/Pineapple Grenade","Ronin/Spaz12","Ronin/Peacemaker",
                                                "Ronin/Foam Sprayer", "Ronin/Shovel", "Ronin/RPC M17", "Ronin/M1600", "Ronin/TommyGun",
                                                "Ronin/Uzi", "Ronin/YAK47", "Ronin/YAK4700","Ronin/Desert Eagle", "Ronin/HAK 20", 
                                                "Ronin/Peacemaker", "Ronin/Lady Pistol", "Ronin/Sniper Rifle", "Ronin/Rifle Long",
                                                "Ronin/Pumpgun", "Ronin/Shortgun", "Ronin/Spaz12", "Ronin/Spaz1200", "Ronin/Stick Grenade",
                                                "Ronin/Stone","Ronin/Molotov Cocktail"};

        private readonly string[] dummy = new[] { "Dummy/Repeater","Dummy/Rail Pistol","Dummy/Turbo Digger","Dummy/Blaster",
                                                "Dummy/Impulse Grenade", "Dummy/Blaster","Dummy/Nailgun","Dummy/Annihiliator",
                                                "Dummy/Grenade Launcher","Dummy/Distuptor Grenade","Dummy/Sniper Rifle", "Dummy/Shielder",
                                                "Dummy/Destroyer Cannon", "Dummy/Nailer Cannon" };
        private readonly string[] browncoats = new string[] { };
        private readonly string[] undead = new[] { "Undead/Blunderbuss", "Undead/Blunderpop"};

        public override string[] Browncoats
        {
            get { return browncoats; }
        }
        public override string[] Undead
        {
            get { return undead; }
        }
        public override string[] Coalition
        {
            get { return coalition; }
        }
        public override string[] Ronin
        {
            get { return ronin; }
        }
        public override string[] Dummy
        {
            get { return dummy; }
        }
        
    }
    public class DefaultActors : DefaultType
    {
        private readonly string[] browncoats = new[] { "Browncoats/Browncoat Light", "Browncoats/Browncoat Heavy" };
        private readonly string[] coalition = new[] { "Coalition/Heavy Brain Robot", "Coalition/Soldier Light","Coalition/Soldier Heavy",
                                                             "Coalition/Drone","Coalition/Medic Drone"};
        private readonly string[] dummy = new[] { "Dummy/Dummy", "Dummy/Dreadnought", "Dummy/Small MG Turret" };
        private readonly string[] ronin = new[] { "Ronin/Ronin Soldier","Ronin/Dafred","Ronin/Mia","Ronin/Dimitri","Ronin/Brutus",
                                                         "Ronin/Sandra","Ronin/Gordon"};
        private readonly string[] undead = new[] { "Undead/Skeleton", "Undead/Zombie Medium", "Undead/Zombie Thin", "Undead/Zombie Fat" };
        public override string[] Browncoats
        {
            get { return browncoats; }
        }
        public override string[] Coalition
        {
            get { return coalition; }
        }
        public override string[] Dummy
        {
            get { return dummy; }
        }
        public override string[] Ronin
        {
            get { return ronin; }
        }
        public override string[] Undead
        {
            get { return undead; }
        } 

    }
    public class DefaultShips : DefaultType
    {
        private readonly string[] coalition = new[] { "Drop Ship MK1", "Rocket MK1", "Rocket MK2" };
        private readonly string[] dummy = new[] { "Dummy/Drop Ship", "Dummy/Rocklet", "Dummy/Drop Crate", "Dummy/Storage Crate" };
        private readonly string[] ronin = new string[] { };
        private readonly string[] undead = new string[] { };
        private readonly string[] browncoats = new string[] { };

        public override string[] Browncoats
        {
            get { return browncoats; }
        }
        public override string[] Undead
        {
            get { return undead; }
        } 
        public override string[] Ronin
        {
            get { return ronin; }
        } 
        public override string[] Coalition
        {
            get { return coalition; }
        }
        public override string[] Dummy
        {
            get { return dummy; }
        } 
    }
}
