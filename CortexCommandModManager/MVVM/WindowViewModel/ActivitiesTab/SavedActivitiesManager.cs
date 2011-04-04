using System.Collections.Generic;
using System.IO;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.NewActivities;
using Newtonsoft.Json;

namespace CortexCommandModManager.MVVM.WindowViewModel.ActivitiesTab
{
    public class SavedActivitiesManager : IInitializable
    {
        public const string SavedActivitiesFileName = "_savedActivities.ccmm";

        private string ActivitiesFile { get { return Path.Combine(baseFolder, SavedActivitiesFileName); } }

        private readonly string baseFolder;

        public SavedActivitiesManager(string baseFolder)
        {
            this.baseFolder = baseFolder;
        }

        public IList<Activity> LoadAll()
        {
            var activities = File.ReadAllText(ActivitiesFile);
            var converted = JsonConvert.DeserializeObject<IList<Activity>>(activities);
            if (converted == null)
                return new List<Activity>();
            return converted;
        }

        public void SaveAll(IList<Activity> activities)
        {
            using (var stream = File.OpenWrite(ActivitiesFile))
                SaveAll(activities, stream);
        }

        private void SaveAll(IList<Activity> activities, Stream stream)
        {
            var writer = new StreamWriter(stream);
            var serialized = JsonConvert.SerializeObject(activities);
            writer.Write(serialized);
            writer.Flush();
        }

        private void CheckForFile()
        {
            if (File.Exists(ActivitiesFile))
                return;
            
            using (var stream = File.Create(ActivitiesFile))
            {
                SaveAll(new List<Activity>(), stream);
            }
        }

        public void Initialize()
        {
            CheckForFile();
        }
    }
}
