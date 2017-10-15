// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a model that can be used to present a collection of items.supports generating child items by a
//   DataTemplate3D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataTemplateDemo
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using HelixToolkit.Wpf;
    using System.Linq;

    /// <summary>
    ///     Represents a model that can be used to present a collection of items.supports generating child items by a
    ///     <see cref="DataTemplate3D" />.
    /// </summary>
    /// <remarks>
    ///     Use the ItemsSource property to specify the collection to use to generate the content of your ItemsVisual3D. You can set the ItemsSource
    ///     property to any type that implements IEnumerable. ItemsSource is typically used to display a data collection or to bind an
    ///     ItemsControl to a collection object.
    /// </remarks>
    public class ItemsVisual3D : ModelVisual3D
    {
        /// <summary>
        ///     The item template property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof(DataTemplate3D), typeof(ItemsVisual3D), new PropertyMetadata(null));

        /// <summary>
        ///     The item template selector property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
            "ItemTemplateSelector", typeof(DataTemplateSelector3D), typeof(ItemsVisual3D), new PropertyMetadata(new DefaultDataTemplateSelctor3D()));

        /// <summary>
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsVisual3D),
            new PropertyMetadata(null, (s, e) => ((ItemsVisual3D)s).ItemsSourceChanged(e)));

        // Using a DependencyProperty as the backing store for RefreshChildrenOnChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshChildrenOnChangeProperty = DependencyProperty.Register(
            "RefreshChildrenOnChange", typeof(bool), typeof(ItemsVisual3D), new PropertyMetadata(true));

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate3D" /> used to display each item.
        /// </summary>
        /// <value>
        ///     The item template.
        /// </value>
        public DataTemplate3D ItemTemplate
        {
            get
            {
                return (DataTemplate3D)this.GetValue(ItemTemplateProperty);
            }

            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplateSelector3D" /> used to locate the <see cref="DataTemplate3D"/> to use.
        /// </summary>
        /// <value>
        ///     The item template selector.
        /// </value>
        public DataTemplateSelector3D ItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector3D)this.GetValue(ItemTemplateSelectorProperty);
            }

            set
            {
                this.SetValue(ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content of the <see cref="ItemsVisual3D" />.
        /// </summary>
        /// <value>
        ///     The items source.
        /// </value>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(ItemsSourceProperty);
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether to refresh all children on a change.
        /// </summary>
        /// <remarks>
        /// This is done by re-attaching the <see cref="ItemsVisual3D"/> instance to the <see cref="Viewport.Children"/> collection.
        /// </remarks>
        public bool RefreshChildrenOnChange
        {
            get { return (bool)GetValue(RefreshChildrenOnChangeProperty); }
            set { SetValue(RefreshChildrenOnChangeProperty, value); }
        }

        /// <summary>
        /// Keeps track of the visuals created for each item.
        /// </summary>
        private readonly Dictionary<object, Visual3D> visuals = new Dictionary<object, Visual3D>();

        /// <summary>
        /// Handles changes in the ItemsSource property.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot create a Model3D from ItemTemplate.
        /// </exception>
        private void ItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldObservableCollection = e.OldValue as INotifyCollectionChanged;
            if (oldObservableCollection != null)
            {
                oldObservableCollection.CollectionChanged -= this.CollectionChanged;
            }

            var observableCollection = e.NewValue as INotifyCollectionChanged;
            if (observableCollection != null)
            {
                observableCollection.CollectionChanged += this.CollectionChanged;
            }

            if (this.ItemsSource != null)
            {
                AddItems(this.ItemsSource);
            }

            if (RefreshChildrenOnChange)
                RefreshChildren();
        }

        /// <summary>
        /// Re-attaches the instance to the viewport resulting in a refresh.
        /// </summary>
        public void RefreshChildren()
        {
            var viewPort = Visual3DHelper.GetViewport3D(this);
            var index = viewPort.Children.IndexOf(this);
            viewPort.Children.Remove(this);
            viewPort.Children.Insert(index, this);
        }

        private void AddItems(IEnumerable items)
        {
            if (items != null && items.Cast<object>().Any())
            {
                foreach (var item in items)
                    AddItem(item);
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(e.OldItems);
                    AddItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Children.Clear();
                    this.visuals.Clear();

                    this.AddItems(ItemsSource);
                    break;

                default:
                    break;
            }

            if (RefreshChildrenOnChange)
                RefreshChildren();
        }

        private void AddItem(object item)
        {
            var visual = CreateVisualFromModel(item);
            if (visual != null)
            {
                // Cannot set DataContext, set bindings manually
                // http://stackoverflow.com/questions/7725313/how-can-i-use-databinding-for-3d-elements-like-visual3d-or-uielement3d
                this.Children.Add(visual);

                this.visuals[item] = visual;
            }
            else
            {
                throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
            }
        }
        private void RemoveItems(IEnumerable items)
        {
            if (items == null)
                return;

            foreach (var rem in items)
            {
                if (visuals.ContainsKey(rem))
                {
                    if (visuals[rem] != null)
                    {
                        Children.Remove(visuals[rem]);
                    }
                }
            }
        }

        private Visual3D CreateVisualFromModel(object item)
        {
            if (this.ItemTemplate != null)
            {
                return this.ItemTemplate.CreateItem(item);
            }
            else if (ItemTemplateSelector != null)
            {
                var viewPort = Visual3DHelper.GetViewport3D(this);
                var dataTemplate = ItemTemplateSelector.SelectTemplate(item, viewPort);
                if (dataTemplate != null)
                {
                    return dataTemplate.CreateItem(item);
                }
                else
                {
                    return item as Visual3D;
                }
            }
            else
            {
                return item as Visual3D;
            }
        }
    }
}
