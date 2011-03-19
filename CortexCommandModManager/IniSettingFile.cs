using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CortexCommandModManager
{
    /// <summary></summary>
	public class IniSettingFile
	{
        private const string NormalSettingRegex = @" *= *([a-zA-Z0-9_/ ]+)";

        private string fileLocation;
        private string fileBuffer;
        public IniSettingFile(string fileLocation)
        {
            this.fileLocation = fileLocation;
        }
        public string Get(string key)
        {
            getFileBuffer();
            return getSetting(key);
        }

        private string getSetting(string key)
        {
            if (String.IsNullOrEmpty(fileBuffer))
                return String.Empty;

            var match = getNormalSettingRegex(key).Match(fileBuffer);

            if (!match.Success)
                return String.Empty;

            return match.Groups[1].Value;
            
        }
        public void Set(string key, string value)
        {
            getFileBuffer();
            setSettingInBuffer(key, value);
            writeFromBuffer();
        }

        private void setSettingInBuffer(string key, string value)
        {
            if (String.IsNullOrEmpty(fileBuffer))
                return;

            var match = getNormalSettingRegex(key).Match(fileBuffer);

            if (!match.Success)
                return;

            var matchPosition = match.Groups[1].Index;
            var matchLength = match.Groups[1].Value.Length;

            var bufferWithoutValue = fileBuffer.Substring(0, matchPosition) + 
                fileBuffer.Substring(matchPosition + matchLength);

            var withNewValue = bufferWithoutValue.Insert(matchPosition, value);
            fileBuffer = withNewValue;
        }

        private Regex getNormalSettingRegex(string key)
        {
            return new Regex(key + NormalSettingRegex);
        }
        private void writeFromBuffer()
        {
            if (String.IsNullOrEmpty(fileBuffer))
                throw new InvalidOperationException("File buffer was empty. Should not be empty.");
            try
            {
                File.Delete(fileLocation);
                var stream = File.Create(fileLocation);
                var writer = new StreamWriter(stream);
                writer.Write(fileBuffer);
                writer.Close();
            }
            catch (Exception ex)
            {
                error("Unable to delete and create the ini file located at {0} to write to it. An IO error occurred.", ex);
            }
        }
        private void getFileBuffer()
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileLocation))
                {
                    fileBuffer = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                error("There was a problem reading the ini file located at {0}. An IO error occurred.", ex);
            }
        }
        private void error(string message, Exception exception)
        {
            throw new IOException(String.Format(message, fileLocation), exception);
        }
	}
}
