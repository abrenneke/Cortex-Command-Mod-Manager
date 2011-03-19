using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CortexCommandModManager.ModsDatabase;
using System.Text.RegularExpressions;

namespace CortexCommandModManager.MVVM.WindowViewModel.BrowseTab
{
    public class DownloadedModSaver
    {
        private const string TempDirectoryName = "_moddownload";

        private string TempDirectory { get { return Path.Combine(Grabber.ModManagerDirectory, TempDirectoryName); } }

        public FileInfo Save(ModDatabaseMod mod, byte[] fileData)
        {
            AssertTempDirectoryExists();

            var fileName = Regex.Replace(mod.Title.Replace(' ', '_'), "[^a-zA-Z0-9]", "") + ".downloaded";
            var filePath = Path.Combine(TempDirectory, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var stream = File.Create(filePath))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(fileData);
            }

            return new FileInfo(filePath);
        }

        private void AssertTempDirectoryExists()
        {
            if (!Directory.Exists(TempDirectory))
            {
                var directory = Directory.CreateDirectory(TempDirectory);
                directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }
    }
}
