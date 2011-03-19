using System;
using System.IO;

namespace CortexCommandModManager.Activities
{
    public class ActivitySaver
    {
        private const double VERSION = 1.0;
        private Activity activity;
        private readonly string defaultMMActivitiesDirectory;
        private string fileBuffer;
        
        public ActivitySaver(Activity activity)
        {
            this.activity = activity;
            fileBuffer = "";
            defaultMMActivitiesDirectory = Grabber.ModManagerDirectory + @"\Activities";
        }
        public void Save()
        {
            checkActivitiesFolder();
            addFormatHeader();
            addActivityHeader();
            addItems();
            saveFile();
        }

        private void checkActivitiesFolder()
        {
            if (!Directory.Exists(defaultMMActivitiesDirectory))
            {
                Directory.CreateDirectory(defaultMMActivitiesDirectory);
            }
        }

        private void saveFile()
        {
            string fileName = activity.Name;
            fileName = EscapeName(fileName);
            string fullFilePath = defaultMMActivitiesDirectory + "\\" + fileName + GetMMFileExtension();

            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            FileStream newFile = File.Create(fullFilePath);
            using (StreamWriter writer = new StreamWriter(newFile))
            {
                writer.Write(fileBuffer);
            }
        }

        private static string GetMMFileExtension()
        {
            return ".ccmm";
        }

        public static string EscapeName(string fileName)
        {
            return fileName.Replace(' ', '_').Replace('.', '_').Replace('/', '_').Replace('\\', '_');
        }
        private void addFormatHeader()
        {
            fileBuffer += "[MMFile Type=Activity Version=" + getActivityVersion() + "]";
        }
        private void addActivityHeader()
        {
            fileBuffer += "[Activity Name=\"" + activity.Name + "\" Description=\"" + activity.Description + "\"]";
        }
        private void addItems()
        {
            fileBuffer += makeItem(activity.Root);
        }
        private static string makeItem(InventoryItem item)
        {
            string type = (item.Name == null) ? String.Empty : item.Name.Replace("\"", "\\\"");
            string name = (item.CopyOf == null) ? String.Empty : item.CopyOf.Replace("\"", "\\\"");
            string temp = "";
            temp += "[InventoryItem Name=\"" + 
                name + 
                "\" Type=\"" + 
                type +
                "\" Inventory=[";
            foreach(InventoryItem subItem in item.SubItems)
            {
                temp += makeItem(subItem);
            }
            temp += "]]";
            return temp;
        }
        private static string getActivityVersion()
        {
         	return VERSION.ToString();
        }
    }
    public struct Activity
    {
        public InventoryItem Root { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
