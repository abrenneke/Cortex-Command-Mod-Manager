using System;
using System.Collections.Generic;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for AddSkirmishItem.xaml
    /// </summary>
    public partial class AddSkirmishItem : Window
    {
        private SkirmishItem skirmishItem;
        private readonly string[] defaultNames = new string[] { "Choose...", "ADoor", "ACDropShip", "ACRocket", "ACrab","AHuman","HDFirearm","HeldDevice","TDExplosive"};
        public AddSkirmishItem(SkirmishItem item)
        {
            InitializeComponent();
            this.skirmishItem = item;
            itemNameBox.Focus();
            defaultsCombo.ItemsSource = defaultNames; 
            defaultsCombo.SelectedIndex = 0;

            MakeTreeView();
        }
        private void MakeTreeView()
        {
            MakeEnhancedTree();
        }
        private void MakeEnhancedTree()
        {
            TreeViewItem actorsBase = new TreeViewItem();
            TreeViewItem shipsBase = new TreeViewItem();
            TreeViewItem weaponsBase = new TreeViewItem();
            actorsBase.Header = "Actors";
            shipsBase.Header = "Transports";
            weaponsBase.Header = "Weapons";

            List<ModBase> factions = EnhancedSkirmish.LoadedMods;
            foreach (ModBase faction in factions)
            {
                TreeViewItem actorsFaction = new TreeViewItem();
                TreeViewItem shipsFaction = new TreeViewItem();
                TreeViewItem weaponsFaction = new TreeViewItem();
                actorsFaction.Header = string.IsNullOrEmpty(faction.Name) ? faction.ModPathName : faction.Name;
                shipsFaction.Header = string.IsNullOrEmpty(faction.Name) ? faction.ModPathName : faction.Name;
                weaponsFaction.Header = string.IsNullOrEmpty(faction.Name) ? faction.ModPathName : faction.Name;

                List<TreeViewItem> subWeaponItems = new List<TreeViewItem>();
                List<TreeViewItem> subActorItems = new List<TreeViewItem>();
                List<TreeViewItem> subShipItems = new List<TreeViewItem>();

                foreach (ModWeapon subWeapon in faction.Weapons)
                {
                    TreeViewItem subWeaponItem = new TreeViewItem();
                    subWeaponItem.Header = subWeapon.Name;
                    subWeaponItem.MouseDoubleClick += new MouseButtonEventHandler(DefaultItems_TreeViewItemMouseDoubleClick);
                    weaponsFaction.Items.Add(subWeaponItem);
                }
                foreach (ModActor subActor in faction.Actors)
                {
                    TreeViewItem subActorItem = new TreeViewItem();
                    subActorItem.Header = subActor.Name;
                    subActorItem.MouseDoubleClick += new MouseButtonEventHandler(DefaultItems_TreeViewItemMouseDoubleClick);
                    actorsFaction.Items.Add(subActorItem);
                }
                foreach (ModShip subShip in faction.Ships)
                {
                    TreeViewItem subShipItem = new TreeViewItem();
                    subShipItem.Header = subShip.Name;
                    subShipItem.MouseDoubleClick += new MouseButtonEventHandler(DefaultItems_TreeViewItemMouseDoubleClick);
                    shipsFaction.Items.Add(subShipItem);
                }

                actorsBase.Items.Add(actorsFaction);
                shipsBase.Items.Add(shipsFaction);
                weaponsBase.Items.Add(weaponsFaction);
            }
            defaultItemsView.Items.Add(actorsBase);
            defaultItemsView.Items.Add(shipsBase);
            defaultItemsView.Items.Add(weaponsBase);
        }
        private List<TreeViewItem> GetSubItems(Type type, string faction, DefaultType desiredType)
        {
            List<TreeViewItem> returnTreeItems = new List<TreeViewItem>();
            System.Reflection.PropertyInfo[] properties = type.GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in properties)
            {
                if (faction == propertyInfo.Name)
                {
                    string[] items = propertyInfo.GetValue(desiredType, null) as string[];
                    foreach (string item in items)
                    {
                        TreeViewItem treeViewItem = new TreeViewItem();
                        treeViewItem.Header = item;
                        returnTreeItems.Add(treeViewItem);
                    }
                }
            }
            return returnTreeItems;
        }
        private List<TreeViewItem> GetSubWeaponItems(string faction)
        {
            return GetSubItems(typeof(DefaultWeapons), faction, DefaultItems.Weapons);   
        }
        private List<TreeViewItem> GetSubActorItems(string faction)
        {
            return GetSubItems(typeof(DefaultActors), faction, DefaultItems.Actors);
        }
        private List<TreeViewItem> GetSubShipItems(string faction)
        {
            return GetSubItems(typeof(DefaultShips), faction, DefaultItems.Ships);
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            skirmishItem.Name = itemNameBox.Text;
            skirmishItem.CopyOf = itemCopyOfBox.Text;
            if (String.IsNullOrEmpty(skirmishItem.CopyOf) || String.IsNullOrEmpty(skirmishItem.Name))
            {
                return;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            skirmishItem.CopyOf = null;
            skirmishItem.Name = null;
            this.Close();
        }

        private void itemCopyOfBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

        private void itemNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

        private void defaultsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = ((ComboBox)sender).SelectedItem as string;
            itemNameBox.Text = (text == "Choose...") ? null : text;
        }
        private void DefaultItems_TreeViewItemMouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeViewItem doubleClickedItem = sender as TreeViewItem;
            if (doubleClickedItem == null)
                return;
            if (String.IsNullOrEmpty(doubleClickedItem.Header as string))
                return;
            itemCopyOfBox.Text = doubleClickedItem.Header as string;
        }
    }
    public class SkirmishItem
    {
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
        public static implicit operator InventoryItem(SkirmishItem item)
        {
            InventoryItem invItem = new InventoryItem();
            invItem.CopyOf = item.CopyOf;
            invItem.Name = item.Name;
            invItem.SubItems = new Inventory();
            return invItem;
        }
    }
}
