using System;
using System.IO;
using XmlConfig.Core;

namespace CortexCommandModManager
{
    public class Grabber
    {
        public static XmlConfig.Core.Settings<SettingsObject> Settings { get; private set; }

        public const string ActivityFileExtension = ".ccmm";

        public static void SetSettings(Settings<SettingsObject> settings)
        {
            if (Settings == null)
            {
                Settings = settings;
            }
            else
            {
                throw new InvalidOperationException("Settings should only be set once.");
            }
        }
        public static string ModManagerDirectory 
        { 
            get 
            { 
                return new DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Parent.FullName; 
            } 
        }

        public static Activities.ActivityItemCache ActivityItemCache { get; set; }
    }
}
