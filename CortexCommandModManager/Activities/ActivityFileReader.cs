using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace CortexCommandModManager.Activities
{
    class InvalidActivityItemException : Exception { }
    class ActivityFileReader
    {
        public bool RetryFiles { get; set; }

        private int GroupTabs;

        public ActivityFileReader()
        {
            RetryFiles = true;
        }

        public IEnumerable<ActivityItem> GetActivitiesFromFile(string file)
        {
            var includedFiles = new List<string>();

            StreamReader reader = null;
            for (var i = 0; i < 100; i++)
            {
                try
                {
                    reader = new StreamReader(file);
                    break;
                }
                catch (IOException ex) 
                {
                    if (RetryFiles)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    else
                    {
                        throw ex;
                    }
                }
                throw new IOException("Failed to load file after 100 tries.");
            }

            var commented = false;
            var loadLine = true;
            string line = "";

            while (!reader.EndOfStream)
            {
                if (loadLine)
                    line = reader.ReadLine();
                else
                    loadLine = true;
                if (lineStartsWithComment(line))
                {
                    commented = true;
                }
                if (commented && lineEndsWithComment(line))
                {
                    commented = false;
                }
                if (line.Contains('=') && hasInclude(line) && !commented)
                {
                    includedFiles.Add(getInclude(line));
                }
                else if (line.Contains('=') && hasItemEntry(line) && !commented)
                {
                    var activity = new ActivityItem();
                    try
                    {
                        activity.Group = getItemGroup(line);
                        activity.Type = getItemType(line);
                    }
                    catch (InvalidActivityItemException)
                    {
                        continue;
                    }

                    GroupTabs = tabIndex(line) + 1;

                    while (true)
                    {
                        if(reader.EndOfStream) break;

                        line = reader.ReadLine();

                        if (tabIndex(line) < GroupTabs)
                        {
                            loadLine = false;
                            break;
                        }

                        if (hasNameEntry(line))
                            activity.PresetName = getNameEntry(line);
                        if (hasDescriptionEntry(line))
                            activity.Description = getDescriptionEntry(line);
                        if (hasSpriteEntry(line))
                            activity.SpritePath = getSpriteEntry(line, reader);
                        if (hasBuyableLine(line))
                            activity.Buyable = getBuyableEntry(line);
                    }

                    if (activity.PresetName == null || activity.PresetName.Trim() == "") continue;

                    yield return activity;
                }
            }
            var ccDirectory = Grabber.Settings.Get().CCInstallDirectory;
            foreach (var includedFile in includedFiles)
            {
                foreach (var item in GetActivitiesFromFile(Path.Combine(ccDirectory, includedFile)))
                {
                    yield return item;
                }
            }
            reader.Close();
        }

        private bool getBuyableEntry(string line)
        {
            int value;
            try
            {
                value = Int32.Parse(line.Substring(line.IndexOf('=') + 1).Trim());
                return value == 1;
            }
            catch(FormatException)
            {
                return true;
            }
        }

        private bool hasBuyableLine(string line)
        {
            line = line.ToLower();
            return line.Contains('=') && line.Substring(0, line.IndexOf('=')).Contains("buyable") && tabIndex(line) == GroupTabs;
        }

        private bool lineEndsWithComment(string line)
        {
            return line.Trim().IndexOf("*/") == line.Trim().Length - 2;
        }

        private bool lineStartsWithComment(string line)
        {
            return line.Trim().IndexOf("/*") == 0;
        }

        private string getInclude(string line)
        {
            return line.Substring(line.IndexOf('=') + 1).Trim();
        }

        private bool hasInclude(string line)
        {
            line = line.ToLower();
            return line.Contains("includefile") && line.Trim().IndexOf("//") != 0;
        }

        private string getSpriteEntry(string line, StreamReader reader)
        {
            //Next line has file
            line = reader.ReadLine();
            if(!line.Contains('=') || !line.ToLower().Substring(0, line.IndexOf('=') + 1).Contains("filepath"))
                throw new InvalidDataException("Expected FilePath, got " + line);
            return line.Substring(line.IndexOf('=') + 1).Trim();
        }

        private bool hasSpriteEntry(string line)
        {
            return line.Contains('=') && tabIndex(line) == GroupTabs && line.ToLower().Substring(0, line.IndexOf('=')).Contains("spritefile");
        }

        private string getDescriptionEntry(string line)
        {
            return line.Substring(line.IndexOf('=') + 1).Trim();
        }

        private bool hasDescriptionEntry(string line)
        {
            return line.Contains('=') && tabIndex(line) == GroupTabs && line.ToLower().Substring(0, line.IndexOf('=')).Contains("description");
        }

        private string getNameEntry(string line)
        {
            return line.Substring(line.IndexOf('=') + 1).TrimStart();
        }

        private bool hasNameEntry(string line)
        {
            return line.Contains('=') && tabIndex(line) == GroupTabs && 
                line.TrimEnd() == line && //Neccesary for CC bug where spaces at the end of a presetname will crash the game.
               (line.ToLower().Substring(0, line.IndexOf('=')).Contains("presetname") ||
                line.ToLower().Substring(0, line.IndexOf('=')).Contains("instancename"));
        }

        private int tabIndex(string line)
        {
            return line.TrimEnd('\t').Count(x => x == '\t');
        }

        private bool hasItemEntry(string line)
        {
            line = line.ToLower();
            if (line.Contains("//") && line.IndexOf("//") >= 0 && line.Substring(0, line.IndexOf("//")).Trim() == "") return false;
            return (line.Contains("adddevice") && !isInvalid(line, "mosrotating")) || 
                   (line.Contains("addactor") && (line.Contains("ahuman") || line.Contains("acrab") || line.Contains("acdropship") || line.Contains("acrocket")));
        }
        private bool isInvalid(string line, params string[] invalid)
        {
            return invalid.Any(x => line.Contains(x));
        }

        private ActivityItemType getItemType(string line)
        {
            if (line.Contains("//")) line = line.Substring(0, line.IndexOf("//"));
            if (line.Substring(line.IndexOf('=') + 1).Trim() == "") throw new InvalidActivityItemException();
            try
            {
                return (ActivityItemType)Enum.Parse(typeof(ActivityItemType), line.Substring(line.IndexOf('=') + 1).Trim(), true);
            }
            catch (ArgumentException)
            {
                throw new InvalidActivityItemException();
            }
        }

        private ActivityItemGroup getItemGroup(string line)
        {
            line = line.ToLower();
            if (line.Contains("hdfirearm") || line.Contains("tdexplosive") || line.Contains("helddevice"))
                return ActivityItemGroup.Weapon;
            if (line.Contains("ahuman") || line.Contains("acrab"))
                return ActivityItemGroup.Actor;
            if (line.Contains("acrocket") || line.Contains("acdropship"))
                return ActivityItemGroup.Craft;
            throw new InvalidDataException("Expected item type, got " + line);
        }
    }
}
