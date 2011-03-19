using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CortexCommandModManager
{
    public class SkirmishWavesScanner
    {
        private const string DefaultSkirmishWavesFile = "Missions.rte\\Skirmish Activities.ini";

        private string fileBuffer;
        private int[] attackerIndexes;
        private string[] attackersSubsets;
        private Inventory skirmishWaves;
        private string lastLineRead;
        private string optionsPart;

        public SkirmishWavesScanner() 
        {
            
        }
        public Inventory GetAllWaves()
        {
            Reset();
            GetFileBuffer();
            GetAttackerIndexes();
            GetIndividualAttackers();
            MakeWaves();
            return skirmishWaves;
        }

        private void Reset()
        {
            fileBuffer = String.Empty;
            attackerIndexes = new int[] { };
            attackersSubsets = new string[] { };
            skirmishWaves = new Inventory();
            lastLineRead = String.Empty;
            optionsPart = String.Empty;
        }

        private void MakeWaves()
        {
            if (skirmishWaves == null)
            {
                skirmishWaves = new Inventory();
            }
            foreach (string attackerSubset in attackersSubsets)
            {
                InventoryItem skirmishWave = MakeWave(attackerSubset);
                skirmishWaves.Add(skirmishWave);
            }
        }
        private string GetSkirmishWavesFile()
        {
            var ccDirectory = Grabber.Settings.Get()
                .CCInstallDirectory;

            return string.Format("{0}\\{1}\\{2}", ccDirectory, EnhancedSkirmish.ActivitiesFolderName, EnhancedSkirmish.ActivitiesFileName);
        }

        private InventoryItem MakeWave(string attackerSubset)
        {
            InventoryItem skirmishWave;
            using (StringReader reader = new StringReader(attackerSubset))
            {
                skirmishWave = new InventoryItem();
                skirmishWave.Name = GetValueFromLine(reader.ReadLine());
                skirmishWave.CopyOf = GetValueFromLine(reader.ReadLine());
                skirmishWave.SubItems = GetInventory(reader);
            }
            return skirmishWave;
        }
        private Inventory GetInventory(StringReader reader, string initialLine, int desiredTabIndex)
        {
            //I am assuming the the inventory of something is defined by the tab index before it. Sub-inventories are tabbed one more in.
            Inventory thisInventory = new Inventory();

            string lineText;
            if (initialLine == null)
            {
                lineText = reader.ReadLine();
                lastLineRead = lineText;
            }
            else
            {
                lineText = initialLine;
            }

            int lineTabIndex;
            if(desiredTabIndex == -1)
            {
                lineTabIndex = GetTabIndex(lineText);
            }
            else
            {
                lineTabIndex = desiredTabIndex;
            }

            while (lineText != null)
            {
                if (GetPropertyFromLine(lineText) == "AddInventory")
                {
                    if (GetTabIndex(lineText) == lineTabIndex)
                    {
                        InventoryItem inventoryItem = new InventoryItem();
                        inventoryItem.Name = GetValueFromLine(lineText);
                        //This assumes that CopyOf will be the first line after a GetInventory
                        lineText = reader.ReadLine();
                        lastLineRead = lineText;
                        inventoryItem.CopyOf = GetValueFromLine(lineText);
                        inventoryItem.SubItems = GetInventory(reader, null, GetTabIndex(lineText));
                        lineText = lastLineRead;
                        inventoryItem.ParentInventory = thisInventory;
                        thisInventory.Add(inventoryItem);
                        continue;
                    }
                    if (GetTabIndex(lineText) < lineTabIndex)
                    {
                        return thisInventory;
                    }
                }
                lineText = reader.ReadLine();
                lastLineRead = lineText;
            }
            return thisInventory;
        }
        private Inventory GetInventory(StringReader reader, string initialLine)
        {
            return GetInventory(reader, null, -1);
        }
        private Inventory GetInventory(StringReader reader)
        {
            return GetInventory(reader, null);
        }
        private int GetTabIndex(string line)
        {
            return line.Split(new char[] { '\t' }).Length - 1;
        }
        private string GetValueFromLine(string line)
        {
            return line.Substring(line.IndexOf('=') + 1).Trim();
        }
        private string GetPropertyFromLine(string line)
        {
            if (line.Contains("="))
            {
                return line.Substring(0, line.IndexOf('=')).Trim().Replace("\t", String.Empty);
            }
            else
            {
                return String.Empty;
            }
        }

        private void GetIndividualAttackers()
        {
            string[] subsets = new string[attackerIndexes.Length];
            for (int i = 0; i < attackerIndexes.Length; i++)
            {
                string attackerSubset;
                if (i == attackerIndexes.Length - 1)
                {
                    attackerSubset = fileBuffer.Substring(attackerIndexes[i]);
                }
                else
                {
                    attackerSubset = fileBuffer.Substring(attackerIndexes[i], 
                        attackerIndexes[i + 1] - attackerIndexes[i]);
                }
                subsets[i] = attackerSubset;
            }
            this.attackersSubsets = subsets;
        }

        private void GetAttackerIndexes()
        {
            MatchCollection matches = Regex.Matches(fileBuffer, "AddAttackerSpawn *= *[A-Za-z0-9_ /]+");
            int[] matchIndexes = new int[matches.Count];
            for(int i=0; i<matches.Count; i++)
            {
                matchIndexes[i] = matches[i].Index;
            }
            attackerIndexes = matchIndexes;
        }

        private void GetFileBuffer()
        {
            try
            {
                using (StreamReader reader = new StreamReader(GetSkirmishWavesFile()))
                {
                    fileBuffer = reader.ReadToEnd();
                }
            }
            catch (IOException ex)
            {
                throw new IOException("There was a problem reading the file for the skirmish waves.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("There was a problem reading the file for the skirmish waves due to a security error. Trying running CCMM as administrator or moving Cortex Command out of a secure directory", ex);
            }
        }
        public void SetSkirmishWaves(Inventory skirmishWaves)
        {
            GetFileBuffer();
            GetOptionsPart();
            WriteWaves(skirmishWaves);
        }

        private void WriteWaves(Inventory skirmishWaves)
        {
            //Could be done better with recursion
            try
            {
                File.Delete(GetSkirmishWavesFile());
            }
            catch (IOException ex)
            {
                throw new IOException("There was a problem deleting the skirmish waves file.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("There was a problem deleting the skirmish waves file due to a security error. Try running CCMM as administrator or moving Cortex Command out of a secure directory.", ex);
            }
            
            string fileText;

            using (StringWriter writer = new StringWriter())
            {
                writer.Write(optionsPart);
                foreach (InventoryItem waveItem in skirmishWaves)
                {
                    int depth = 1;
                    writer.Write(Tabs(depth));
                    writer.WriteLine("AddAttackerSpawn = " + waveItem.Name);
                    depth++;
                    writer.Write(Tabs(depth));
                    writer.WriteLine("CopyOf = " + waveItem.CopyOf);
                    foreach (InventoryItem actorItem in waveItem.SubItems)
                    {
                        writer.Write(Tabs(depth));
                        writer.WriteLine("AddInventory = " + actorItem.Name);
                        depth++;
                        writer.Write(Tabs(depth));
                        writer.WriteLine("CopyOf = " + actorItem.CopyOf);
                        foreach(InventoryItem weaponItem in actorItem.SubItems)
                        {
                            writer.Write(Tabs(depth));
                            writer.WriteLine("AddInventory = " + weaponItem.Name);
                            depth++;
                            writer.Write(Tabs(depth));
                            writer.WriteLine("CopyOf = " + weaponItem.CopyOf);
                            depth--;
                        }
                        depth--;
                    }
                    writer.WriteLine(Tabs(3));
                    writer.WriteLine(Tabs(3));
                }
                fileText = writer.ToString();
            }

            StreamWriter streamWriter;
            try
            {
                streamWriter = new StreamWriter(File.Create(GetSkirmishWavesFile()));
            }
            catch (IOException ex)
            {
                string fileName = BackupSaver.Backup(fileText);
                throw new IOException("There was a problem creating a new file for the skirmish waves. A backup has been saved to " + fileName, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                string fileName = BackupSaver.Backup(fileText);
                throw new IOException("There was a problem creating a new file for the skirmish waves due to a security error. Try running CCMM as administrator or moving Cortex Command out of a secure directory. A backup has been saved to " + fileName, ex);
            }
            try
            {
                streamWriter.Write(fileText);
            }
            catch (Exception ex)
            {
                string fileName = BackupSaver.Backup(fileText);
                throw new IOException("There was a problem writing data to the skirmish waves file. A backup has been saved to " + fileName,ex);
            }
            streamWriter.Close();
        }
        private string Tabs(int amount)
        {
            string s = String.Empty;
            for (int i = 0; i < amount; i++)
            {
                s += '\t';
            }
            return s;
        }
        private void GetOptionsPart()
        {
            GetAttackerIndexes();
            try
            {
                int firstAttackerIndex = attackerIndexes[0];
                optionsPart = fileBuffer.Substring(0, GetFirstAttackerLineIndex(firstAttackerIndex));
            }
            catch (IndexOutOfRangeException)
            {
                int firstAttackerIndex = fileBuffer.Length;
                optionsPart = fileBuffer.Substring(0, firstAttackerIndex);
            }
        }
        private int GetFirstAttackerLineIndex(int firstAttackerIndex)
        {
            return firstAttackerIndex - 1;
        }
    }
}
