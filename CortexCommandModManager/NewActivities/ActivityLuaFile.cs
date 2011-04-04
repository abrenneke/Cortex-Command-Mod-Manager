using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CortexCommandModManager.NewActivities
{
    public class ActivityLuaFile
    {
        public string FullUnparsedFile { get; private set; }

        public ActivityLuaFile(string luaFilePath)
        {
            var fullFile = File.ReadAllText(luaFilePath);
        }
    }
}
