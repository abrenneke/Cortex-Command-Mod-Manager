using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Collections.Specialized;
using CortexCommandModManager.Extensions;

namespace CortexCommandModManager.MVVM.Utilities
{
    public class Attached
    {
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", 
                typeof(Behaviors), typeof(Attached), new FrameworkPropertyMetadata((Behaviors)null, BehaviorsChanged));

        public static void SetBehaviors(DependencyObject element, Behaviors value)
        {
            if (value == null)
                return;

            value.Owner = element;
            value.Each(x => x.Owner = element);

            element.SetValue(BehaviorsProperty, value);

            var collection = (INotifyCollectionChanged)value;
            collection.CollectionChanged += CollectionChanged;
        }

        public static Behaviors GetBehaviors(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var behaviors = element.GetValue(BehaviorsProperty) as Behaviors;
            if (behaviors == null)
            {
                behaviors = new Behaviors();
                behaviors.Owner = element;
                SetBehaviors(element, behaviors);
            }
            return behaviors;
        }

        public static void BehaviorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            SetBehaviors(obj, (Behaviors)e.NewValue);
        }

        private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var sourceCollection = (Behaviors)sender;
            switch (e.Action)
            {
                //when an item(s) is added we need to set the Owner property implicitly
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        e.NewItems.Cast<Behavior>().Each(x => x.Owner = sourceCollection.Owner);
                    break;
                //when an item(s) is removed we should Dispose the BehaviorBinding
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (Behavior item in e.OldItems)
                            item.InnerBehavior.Dispose();
                    break;

                //here we have to set the owner property to the new item and unregister the old item
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                        foreach (Behavior item in e.NewItems)
                            item.Owner = sourceCollection.Owner;

                    if (e.OldItems != null)
                        foreach (Behavior item in e.OldItems)
                            item.InnerBehavior.Dispose();
                    break;

                //when an item(s) is removed we should Dispose the BehaviorBinding
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                        foreach (Behavior item in e.OldItems)
                            item.InnerBehavior.Dispose();
                    break;

                case NotifyCollectionChangedAction.Move:
                default:
                    break;
            }
        }
    }
}
