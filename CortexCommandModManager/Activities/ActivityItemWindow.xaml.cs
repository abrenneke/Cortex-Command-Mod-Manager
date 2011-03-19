using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CortexCommandModManager.Activities;
using System.ComponentModel;

namespace CortexCommandModManager
{
    /// <summary>
    /// Interaction logic for ActivityItemWindow.xaml
    /// </summary>
    public partial class ActivityItemWindow : Window
    {
        private IEnumerable<ActivityItem> ActivityItems;

        public ActivityItem SelectedItem { get; private set; }

        public ActivityItemWindow()
        {
            InitializeComponent();

            if (Grabber.ActivityItemCache == null || Grabber.ActivityItemCache.CacheIsValid == false)
            {

                ActivityItemLoaderWindow loaderWindow = new ActivityItemLoaderWindow();
                loaderWindow.ShowDialog();

                ActivityItems = loaderWindow.Activities;
            }
            else
            {
                ActivityItems = Grabber.ActivityItemCache.GetAll();
            }

            var weaponsView = (CollectionViewSource)FindResource("weaponsSource");
            weaponsView.Source = ActivityItems.Where(x => x.Group == ActivityItemGroup.Weapon);

            var actorsView = (CollectionViewSource)FindResource("actorsSource");
            actorsView.Source = ActivityItems.Where(x => x.Group == ActivityItemGroup.Actor);

            var craftView = (CollectionViewSource)FindResource("craftSource");
            craftView.Source = ActivityItems.Where(x => x.Group == ActivityItemGroup.Craft);
        }

        public void ItemListViewItemDoubleClick(object sender, MouseEventArgs args)
        {
            if (!(sender is ListViewItem)) { return; }
            var listItem = (ListViewItem)sender;

            ActivityItem item = null;
            item = weaponsListView.ItemContainerGenerator.ItemFromContainer(listItem) as ActivityItem ??
                   actorsListView.ItemContainerGenerator.ItemFromContainer(listItem) as ActivityItem ??
                   craftListView.ItemContainerGenerator.ItemFromContainer(listItem) as ActivityItem;
            if (item == null) return;

            SelectedItem = item;

            Close();
        }
    }
}
