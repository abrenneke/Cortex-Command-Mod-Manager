using System;
using System.IO;

namespace CortexCommandModManager
{
    class BackupSaver
    {
        private const string BackupPrefix = "CCMMBackup-";

        public static string Backup(string data)
        {
            string fileName = getNewBackupFileName();
            string filePath = Grabber.ModManagerDirectory + "\\" + fileName;
            writeBackupFile(data,filePath);
            return fileName;
        }

        private static void writeBackupFile(string data, string filePath)
        {
            StreamWriter writer;
            try
            {
                writer = File.CreateText(filePath);
            }
            catch (IOException)
            {
                throw new IOException("There was a problem creating the backup file.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new IOException("There was a problem creating the backup file due to security permissions. Try running CCMM as administrator or moving it outside of a secure directory.");
            }
            try
            {
                writer.Write(data);
                writer.Close();
            }
            catch(Exception)
            {
                throw new IOException("There was a problem writing the data to the backup file.");
            }
        }

        private static string getNewBackupFileName()
        {
            string ccmmDir = Grabber.ModManagerDirectory;
            string[] localFiles = Directory.GetFiles(ccmmDir);

            string date = makeNowDate();

            foreach (string localFile in localFiles)
            {
                if (localFile.Contains(BackupPrefix))
                {
                    if (getDatePartOfBackup(localFile) == date)
                    {
                        return makeDuplicateBackupName(date);
                    }
                }
            }
            return makeNormalBackupName(date);
        }

        private static string makeDuplicateBackupName(string date)
        {
            Random random = new Random();
            return BackupPrefix + date + "_" + random.Next(10000, 99999) + ".txt";
        }

        private static string makeNormalBackupName(string date)
        {
            return BackupPrefix + date + ".txt";
        }

        private static string makeNowDate()
        {
            return String.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }
        private static string getDatePartOfBackup(string backup)
        {
            backup = backup.Substring(backup.IndexOf(BackupPrefix) + BackupPrefix.Length);
            backup = backup.Substring(0, backup.IndexOf('.'));
            backup = (backup.Contains("_")) ? backup.Substring(0,backup.IndexOf('_')) : backup;
            return backup;
        }
    }
}
