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
        public bool CacheIsValid { get; private set; }

        private IList<Mod> enabledMods;
        public IList<Mod> EnabledMods 
        { 
            get 
            { 
                if (enabledMods == null) 
                    enabledMods = scanner.GetEnabledMods(); 
                return enabledMods; 
            } 
            set { enabledMods = value; } 
        }

        private readonly ModScanner scanner;
        private readonly string cacheFile;

        private IList<ActivityItem> cacheItems;
        
        public ActivityItemCache(ModScanner scanner, string cacheFile)
        {
            this.scanner = scanner;
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

        public void InvalidateCache()
        {
            CacheIsValid = false;
        }

        public void ValidateCache()
        {
            CacheIsValid = true;
        }

        private void CreateAndLoadCacheFile()
        {
            File.Create(cacheFile).Close();
            cacheItems = new List<ActivityItem>();
        }

        private void LoadCacheFile()
        {
            string jsonData;
            using(var reader = new StreamReader(cacheFile))
            {
                jsonData = reader.ReadToEnd();
            }
            var allActivities = JsonConvert.DeserializeObject<List<ActivityItem>>(jsonData);

            cacheItems = allActivities;
        }

        public IEnumerable<ActivityItem> GetAll()
        {
            if (cacheItems == null)
                throw new InvalidOperationException("An unexpected error occurred when loading cached items. Please try deleting your .mmitems file and load this again.");
            return cacheItems.Where(x => EnabledMods.Contains(x.Mod));
        }

        public bool ModIsCached(Mod mod)
        {
            return cacheItems.Any(x => x.Mod.Equals(mod));
        }

        public void AddItems(IEnumerable<ActivityItem> items)
        {
            cacheItems = cacheItems.Union(items).ToList();
        }

        public void SaveCache()
        {
            var serializedItems = JsonConvert.SerializeObject(cacheItems, Formatting.None, new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.All });

            using (var writer = new StreamWriter(new FileStream(cacheFile, FileMode.Create)))
            {
                writer.Write(serializedItems);
            }
        }
    }
}
