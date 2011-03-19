using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CortexCommandModManager.Activities
{
    public class ModActivityItemsLoader
    {
        private ActivityFileReader reader = new ActivityFileReader();

        public event Action<Mod> OnLoadingMod;

        public IEnumerable<ActivityItem> LoadAllMods()
        {
            ModScanner scanner = new ModScanner();

            if (Grabber.ActivityItemCache == null)
            {
                LoadCache();
            }

            var mods = scanner.GetEnabledMods();
            foreach (var mod in mods)
            {
                if (OnLoadingMod != null) OnLoadingMod(mod);

                foreach(var item in LoadMod(mod))
                {
                    yield return item;
                }
            }
        }

        private void LoadCache()
        {
            Grabber.ActivityItemCache = new ActivityItemCache(Path.Combine(Grabber.ModManagerDirectory, ".modcache"));
        }

        public IEnumerable<ActivityItem> LoadMod(Mod mod)
        {
            foreach (var item in loadMod(mod.FullFolderPath))
            {
                item.Mod = mod;
                yield return item;
            }
        }
        private IEnumerable<ActivityItem> loadMod(string folder)
        {
            string file;
            try
            {
                file = Directory.GetFiles(folder, "*ndex.ini").Single();
            }
            catch (InvalidOperationException) { return new List<ActivityItem>(); }

            return reader.GetActivitiesFromFile(file).Where(x => x.Buyable);
        }
    }
}
