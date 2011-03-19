using System;
using System.Text.RegularExpressions;
using System.IO;

namespace CortexCommandModManager.Activities
{
    
    class ActivityLoader
    {
        private const double VERSION = 1.0;

        private static string fileBuffer;
        private static string file;
        private static Activity activity;

        private static readonly Regex CCMMFileAttributesRegex = new Regex("([a-zA-Z0-9_]+)=([a-zA-Z0-9_]+)",RegexOptions.Compiled);
        private static readonly Regex ActivityTagMatcher = new Regex("([a-zA-Z0-9_]+)=\"(.*?)(?<!\\\\)\"",RegexOptions.Compiled);

        public static Activity Load(string fileToLoad)
        {
            file = fileToLoad;
            CheckFile();
            LoadBuffer();
            CheckMMFile();
            LoadActivity();
            return activity;
        }
        private static void CheckFile()
        {
            FileInfo fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
            {
                throw new ActivityLoadException("Activity file does not exist");
            }
            if (fileInfo.Extension != Grabber.ActivityFileExtension)
            {
                throw new ActivityLoadException("Activity file does not have the correct extension to be loaded by the mod manager");
            }
            if (fileInfo.Length == 0)
            {
                throw new ActivityLoadException("Activity file was empty");
            }
        }

        private static void LoadBuffer()
        {
            using (StreamReader reader = new StreamReader(file))
            {
                fileBuffer = reader.ReadToEnd();
            }
        }
        private static void CheckMMFile()
        {
            string filePart = fileBuffer.Substring(0, fileBuffer.IndexOf(']'));
            MatchCollection matches = CCMMFileAttributesRegex.Matches(filePart);
            if (matches.Count != 2)
            {
                throw new ActivityLoadException("There were more than two attributes for the .ccmm file");
            }
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value == "Type")
                {
                    if (match.Groups[2].Value != "Activity")
                    {
                        throw new IncorrectCCMMFileTypeException();
                    }
                }
                if (match.Groups[1].Value == "Version")
                {
                    double value;
                    try
                    {
                        value = Double.Parse(match.Groups[2].Value);
                    }
                    catch (FormatException)
                    {
                        throw new ActivityLoadException("CCMM file was in an incorrect format");
                    }
                    if (value != VERSION)
                    {
                        throw new ActivityLoadException("CCMM file was an incorrect version for this mod loader");
                    }
                }
            }
        }
        private static void LoadActivity()
        {
            activity = new Activity();
            GetNameAndDescriptionOfActivity();
            int firstItemIndex = fileBuffer.IndexOf("[InventoryItem");
            InventoryItem item = MakeItem(firstItemIndex);
            activity.Root = item;
        }

        private static void GetNameAndDescriptionOfActivity()
        {
            int activityDefinitionIndex = fileBuffer.IndexOf("[Activity");
            string tag = GetFullTag(activityDefinitionIndex);
            MatchCollection matches = ActivityTagMatcher.Matches(tag);
            foreach (Match match in matches)
            {
                if (GetProperty(match) == "name")
                {
                    activity.Name = GetValue(match);
                }
                if (GetProperty(match) == "description")
                {
                    activity.Description = GetValue(match);
                }
            }
        }
        private static InventoryItem MakeItem(int indexOfTag)
        {
            InventoryItem item = new InventoryItem();

            string tag = GetFullTag(indexOfTag);
            MatchCollection matches = ActivityTagMatcher.Matches(tag);
            for (int i = 0; i < 2; i++)
            {
                Match match = matches[i];
                if (GetProperty(match) == "name")
                {
                    item.CopyOf = GetValue(match);
                }
                if (GetProperty(match) == "type")
                {
                    item.Name = GetValue(match);
                }
            }

            int inventoryIndex = tag.IndexOf("Inventory=[");
            //string subItems = tag.Substring(inventoryIndex + 11, (FindMatchingTagIndex(inventoryIndex + 10,tag) - 1) - (inventoryIndex + 11));
            int subItemsIndex = indexOfTag + inventoryIndex + 10;
            item.SubItems = FindRootInventoryItems(subItemsIndex);
            return item;
        }

        private static Inventory FindRootInventoryItems(int positionOfSubItems)
        {
            Inventory inventory = new Inventory();

            int depth = 0;
            int maxPosition = FindMatchingTagIndex(positionOfSubItems);
            for (int position = 0; position < maxPosition; position++)
            {
                int fullPosition = positionOfSubItems + position;
                string lookFor = "[InventoryItem";
                if (fullPosition + lookFor.Length > maxPosition)
                {
                    break;
                }
                string maybeFind = fileBuffer.Substring(fullPosition, lookFor.Length);
                if (maybeFind == lookFor)
                {
                    if (depth == 1)
                    {
                        inventory.Add(MakeItem(positionOfSubItems + position));
                    }
                }
                if (fileBuffer.Substring(fullPosition, 1) == "[")
                {
                    depth++;
                }
                if (fileBuffer.Substring(fullPosition, 1) == "]")
                {
                    depth--;
                }
            }
            return inventory;
        }
        private static string GetProperty(Match m)
        {
            return m.Groups[1].Value.ToLower();
        }
        private static string GetValue(Match m)
        {
            return m.Groups[2].Value.Replace("\\\"", "\"");
        }
        private static string GetFullTag(int index)
        {
            return fileBuffer.Substring(index, FindMatchingTagIndex(index) - index);
        }
        private static int FindMatchingTagIndex(int startTagIndex)
        {
            return FindMatchingTagIndex(startTagIndex, fileBuffer);
        }
        private static int FindMatchingTagIndex(int startTagIndex, string tag)
        {
            int position = startTagIndex;
            using (StringReader reader = new StringReader(tag.Substring(startTagIndex)))
            {
                //So that it doesn't read the first [
                reader.Read();
                position++;

                int depth = 0;
                int readChar = reader.Read();
                position++;
                while (readChar != -1)
                {
                    char character = (char)readChar;
                    if (character == ']')
                    {
                        if (depth == 0)
                        {
                            return position;
                        }
                        depth--;
                    }
                    if (character == '[')
                    {
                        depth++;
                    }
                    readChar = reader.Read();
                    position++;
                }
            }
            return position;
        }
        public static string GetHighestAvailableTemplateActivityString()
        {
            //TEMPLATE = "New Activity X";
            string[] activityFiles = Directory.GetFiles(Grabber.ModManagerDirectory + "\\Activities", "*" + Grabber.ActivityFileExtension);

            int runningNumber = 0;
            foreach (string activityFile in activityFiles)
            {
                FileInfo fileInfo = new FileInfo(activityFile);
                Match numberPart = Regex.Match(fileInfo.Name, "New_Activity_([0-9]+)\\.(c|C)(c|C)(m|M)(m|M)");
                if (numberPart.Success)
                {
                    int number;
                    try
                    {
                        number = Int32.Parse(numberPart.Groups[1].Value);
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                    runningNumber = number;
                }
            }
            return "New Activity " + (runningNumber + 1);
        }
    }
}
