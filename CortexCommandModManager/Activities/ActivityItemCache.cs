using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace CortexCommandModManager.Activities
{
    public class ActivityItemCache
    {
        private IList<ActivityItem> CacheItems;
        private string cacheFile;

        public bool CacheIsValid { get; set; }

        public void InvalidateCache()
        {
            CacheIsValid = false;
        }

        private IList<Mod> enabledMods;
        public IList<Mod> EnabledMods { get { if (enabledMods == null) enabledMods = (new ModScanner()).GetEnabledMods(); return enabledMods; } set { enabledMods = value; } }

        public ActivityItemCache(string cacheFile)
        {
            this.cacheFile = cacheFile;
            if (File.Exists(cacheFile))
            {
                LoadCacheFile();
            }
            else
            {
                CreateAndLoadCacheFile();
            }
        }

        private void CreateAndLoadCacheFile()
        {
            File.Create(cacheFile).Close();
            CacheItems = new List<ActivityItem>();
        }

        private void LoadCacheFile()
        {
            string jsonData;
            using(var reader = new StreamReader(cacheFile))
            {
                jsonData = reader.ReadToEnd();
            }
            var allActivities = JsonConvert.DeserializeObject<List<ActivityItem>>(jsonData);

            CacheItems = allActivities;
        }

        public IEnumerable<ActivityItem> GetAll()
        {
            if (CacheItems == null)
                throw new InvalidOperationException("An unexpected error occurred when loading cached items. Please try deleting your .mmitems file and load this again.");
            return CacheItems.Where(x => EnabledMods.Contains(x.Mod));
        }

        public bool ModIsCached(Mod mod)
        {
            return CacheItems.Any(x => x.Mod.Equals(mod));
        }

        public void AddItems(IEnumerable<ActivityItem> items)
        {
            CacheItems = CacheItems.Union(items).ToList();
        }

        public void SaveCache()
        {
            var serializedItems = JsonConvert.SerializeObject(CacheItems, Formatting.None, new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.All });

            using (var writer = new StreamWriter(new FileStream(cacheFile, FileMode.Create)))
            {
                writer.Write(serializedItems);
            }
        }

    }
}
