using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CortexCommandModManager.NewActivities
{
    public class ActivityLoader
    {
        public const string ActivityFolderName = "Activities";

        public static readonly string[] ValidActivityExtensions = new[] { ".act.rar" };

        private readonly string baseDirectory;

        public ActivityLoader(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public IList<ParsedActivity> LoadAll()
        {
            var activitiesDirectory = Path.Combine(baseDirectory, ActivityFolderName);
            var activityFolders = Directory.GetDirectories(activitiesDirectory, "*.act");

            return activityFolders.Select(GetActivityFromFolder).ToList();
        }

        public ActivityLuaFile LoadLuaFile(ParsedActivity activity)
        {
            var luaFilePath = Path.Combine(activity.FolderLocation, activity.LuaFileName);
            return new ActivityLuaFile(luaFilePath);
        }

        private ParsedActivity GetActivityFromFolder(string folder)
        {
            var files = Directory.GetFiles(folder);
            
            var activityFile = files.SingleOrDefault(x => Path.GetExtension(x) == ".act");

            using (var reader = new ActivityFileReader(activityFile))
            {
                var activity = reader.LoadActivity();
                activity.FolderLocation = folder;
                return activity;
            }
        }
    }
}
