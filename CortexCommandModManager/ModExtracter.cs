using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SevenZip;

namespace CortexCommandModManager
{
    public class ModExtracter
    {
        private const string TempDirectoryName = "ModExtractorTemp";

        private static readonly string[] SupportedFileExtensions = new[] { ".rar", ".7z", ".zip" };

        private FileInfo packedFile;

        private string TempDirectory { get { return Path.Combine(Grabber.ModManagerDirectory, TempDirectoryName); } }

        public ModExtracter()
        {

        }

        private void AssertTempDirectoryExists()
        {
            if (!Directory.Exists(TempDirectory))
                Directory.CreateDirectory(TempDirectory);
        }

        public IList<string> Unpack(string packedModPath)
        {
            IList<string> extracted;

            SetAndVerifyPath(packedModPath);
            try
            {
                CleanTempDirectory();
                ExtractToTempDirectory();
                extracted = MoveModFilesToCCDirectory();
            }
            finally
            {
                CleanTempDirectory();
            }

            return extracted;
        }

        public IList<string> Unpack(IEnumerable<string> packedMods)
        {
            var extracted = new List<string>();
            foreach (var packed in packedMods)
            {
                var subExtracted = Unpack(packed);
                extracted.AddRange(subExtracted);
            }
            return extracted;
        }

        private void SetAndVerifyPath(string packedModPath)
        {
            var fileInfo = new FileInfo(packedModPath);

            if (!fileInfo.Exists)
                throw new ArgumentException("Packed mod path was not an actual file path.");

            if (!IsSupported(fileInfo))
                throw new ArgumentException("Sorry, the " + fileInfo.Extension + " file type is not supported for extracting.");

            AssertTempDirectoryExists();

            packedFile = fileInfo;
        }

        private void CleanTempDirectory()
        {
            foreach (var directory in Directory.GetDirectories(TempDirectory))
                Directory.Delete(directory, true);

            foreach (var file in Directory.GetFiles(TempDirectory))
                File.Delete(file);
        }

        private IList<string> MoveModFilesToCCDirectory()
        {
            var extracted = new List<string>();

            var info = new DirectoryInfo(TempDirectory);
            var subDirectories = info.GetDirectories();

            foreach (var directory in subDirectories)
            {
                if (IsValidModFile(directory))
                {
                    MoveDirectoryToCCDirectory(directory);
                    extracted.Add(directory.Name);
                }
                else
                {
                    //Check its subdirectories - only one level, though
                    foreach (var innerDirectory in directory.GetDirectories())
                    {
                        if (IsValidModFile(innerDirectory))
                        {
                            MoveDirectoryToCCDirectory(directory);
                            extracted.Add(directory.Name);
                        }
                    }
                }
            }

            return extracted;
        }

        private bool IsValidModFile(DirectoryInfo directory)
        {
            if(!directory.Name.EndsWith(".rte", StringComparison.OrdinalIgnoreCase))
                return false;

            var ccDirectory = new DirectoryInfo(Grabber.Settings.Get().CCInstallDirectory);
            var newDirectory = new DirectoryInfo(Path.Combine(ccDirectory.FullName, directory.Name));

            if(newDirectory.Exists)
                return false;

            return true;
        }

        private void MoveDirectoryToCCDirectory(DirectoryInfo directory)
        {
            var ccDirectory = new DirectoryInfo(Grabber.Settings.Get().CCInstallDirectory);
            var newDirectory = new DirectoryInfo(Path.Combine(ccDirectory.FullName, directory.Name));

            if (Directory.GetDirectoryRoot(directory.FullName) == Directory.GetDirectoryRoot(newDirectory.FullName))
            {
                Directory.Move(directory.FullName, newDirectory.FullName);
            }
            else
            {
                CopyDirectory(directory, newDirectory);
            }
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
                Directory.CreateDirectory(destination.FullName);

            foreach (var file in source.GetFiles())
            {
                File.Copy(file.FullName, Path.Combine(destination.FullName, file.Name));
            }

            foreach (var directory in source.GetDirectories())
            {
                CopyDirectory(directory, new DirectoryInfo(Path.Combine(destination.FullName, directory.Name)));
            }
        }

        private void ExtractToTempDirectory()
        {
            using (var stream = File.OpenRead(packedFile.FullName))
            {
                try
                {
                    var extractor = new SevenZipExtractor(stream);
                    extractor.ExtractArchive(TempDirectory);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Sorry, extracting " + packedFile.FullName + " is not supported.", ex);
                }
            }
        }

        private bool IsSupported(FileInfo fileInfo)
        {
            return SupportedFileExtensions.Contains(fileInfo.Extension);
        }
    }
}
