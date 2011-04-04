using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CortexCommandModManager.Extensions;

namespace CortexCommandModManager.NewActivities
{
    public class ActivityFileReader : IDisposable
    {
        private StreamReader reader;
        private ParsedActivity workingActivity;

        public ActivityFileReader(string activityFilePath)
        {
            reader = new StreamReader(File.OpenRead(activityFilePath));
        }

        public ParsedActivity LoadActivity()
        {
            workingActivity = new ParsedActivity();

            string line = reader.ReadLine();
            while (line != null)
            {
                ParseLine(line);
                line = reader.ReadLine();
            }

            return workingActivity;
        }

        private bool LineHasBrackets(string line)
        {
            return line.StartsWith("[", StringComparison.Ordinal) && line.EndsWith("]", StringComparison.Ordinal);
        }

        private void ParseLine(string line)
        {
            line = line.Trim();
            if (!LineHasBrackets(line))
                return;

            if (!line.Contains('=')) // isn't a property line
            {
                ParseNonVariable(line);
                return;
            }

            var properties = ParsePropertiesLine(line);
            if (properties.ContainsKey("variable"))
            {
                ParseVariable(properties);
            }
            else if (properties.ContainsKey("activity"))
            {
                var trimLine = line.Substring(1, line.Length - 2); //remove []

                workingActivity.Name = trimLine.Substring(
                    trimLine.ToLower().IndexOf("name=") + "name=".Length);
            }
            else if (properties.ContainsKey("luafile"))
            {
                workingActivity.LuaFileName = properties["luafile"];
            }
        }

        private void ParseNonVariable(string line)
        {
            line = line.Substring(1, line.Length - 2); //remove []

            if (line.ToLower() == "iniformat")
            {
                ParseIniFormat();
                return;
            }
        }

        private void ParseIniFormat()
        {
            var builder = new StringBuilder();
            var nextLine = reader.ReadLine();

            while (nextLine != null && nextLine.Trim().ToLower() != "[/iniformat]")
            {
                builder.AppendLine(nextLine);
                nextLine = reader.ReadLine();
            }

            workingActivity.IniFormat = builder.ToString();
        }

        private void ParseVariable(IDictionary<string, string> properties)
        {
            var variable = new ActivityVariable();
            variable.Name = properties["variable"];

            foreach (var property in properties.Where(x => x.Key != "variable"))
                variable.Properties.Add(property);

            workingActivity.Variables.Add(variable);
        }

        private IDictionary<string, string> ParsePropertiesLine(string line)
        {
            line = line.Substring(1, line.Length - 2); //remove []
            var properties = line.Split(' ');

            return properties.Select(x =>
                {
                    var split = x.Split('=');
                    return new { Key = split[0].ToLower(), Value = split.Length > 1 ? split[1] : null };
                }).ToDictionary(x => x.Key, x => x.Value);
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
