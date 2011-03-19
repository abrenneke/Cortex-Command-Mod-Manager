using System;
using System.Collections.Generic;

namespace CortexCommandModManager
{
    public class Inventory : List<InventoryItem>
    {
        internal void RemoveFirst(InventoryItem itemToRemove)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].CompareTo(itemToRemove) != 0) continue;

                RemoveAt(i);
                return;
            }
        }
    }

    public class InventoryItem : IComparable<InventoryItem>
    {
        public InventoryItem()
        {
            subItems = new Inventory();
        }

        public InventoryItem(Activities.ActivityItem activityItem)
        {
            Name = Enum.GetName(typeof(Activities.ActivityItemType), activityItem.Type);
            CopyOf = activityItem.Mod.Folder.Remove(activityItem.Mod.Folder.IndexOf(".rte", StringComparison.OrdinalIgnoreCase)) + "/" + activityItem.PresetName;
            SubItems = new Inventory();
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string copyOf;

        public string CopyOf
        {
            get { return copyOf; }
            set { copyOf = value; }
        }
        private Inventory subItems;

        public Inventory SubItems
        {
            get { return subItems; }
            set { subItems = value; }
        }
        private Inventory parentInventory;

        public Inventory ParentInventory
        {
            get { return parentInventory; }
            set { parentInventory = value; }
        }
        public InventoryItem FindParentFromRoot(Inventory root)
        {
            foreach (InventoryItem tempItem in root)
            {
                if (tempItem.SubItems == this.ParentInventory)
                {
                    return tempItem;
                }
                return this.FindParentFromRoot(tempItem.SubItems);
            }
            return null;
        }

        public InventoryItem Copy()
        {
            InventoryItem newItem = new InventoryItem();
            Inventory subItems = new Inventory();

            foreach(InventoryItem subItem in this.subItems)
            {
                InventoryItem newSubItem = subItem.Copy();
                newSubItem.ParentInventory = subItems;
                subItems.Add(newSubItem);
            }

            newItem.SubItems = subItems;
            newItem.name = name;
            newItem.parentInventory = parentInventory;
            newItem.copyOf = copyOf;
            return newItem;
        }
        public int CompareTo(InventoryItem other)
        {
            if (this.copyOf == other.copyOf && this.name == other.name && this.parentInventory == other.parentInventory)
            {
                if(this.subItems.Count == other.subItems.Count)
                {
                    bool found = false;
                    for (int i = 0; i < this.subItems.Count; i++)
                    {
                        if (this.subItems[i].CompareTo(other.subItems[i]) != 0)
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }
    }
}