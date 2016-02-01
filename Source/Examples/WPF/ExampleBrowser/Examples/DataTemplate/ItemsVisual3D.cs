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
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsVisual3D),
            new PropertyMetadata(null, (s, e) => ((ItemsVisual3D)s).ItemsSourceChanged(e)));

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


        private Dictionary<object, Visual3D> models;

        public ItemsVisual3D()
        {
            this.models = new Dictionary<object, Visual3D>();
        }


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
            // if collection implements INotifyCollectionChanged
            INotifyCollectionChanged collec = ItemsSource as INotifyCollectionChanged;
            if (collec != null)
                collec.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (this.ItemsSource != null)
            {
                foreach (var item in this.ItemsSource)
                {
                    this.AddItem(item);
                }
            }          
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        this.AddItem(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var rem in e.OldItems)
                    {
                        if (this.models.ContainsKey(rem))
                        {
                            if (this.models[rem] != null)
                            {
                                this.Children.Remove(this.models[rem]);
                            }
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void AddItem(object item)
        {
            Visual3D visObject;
            if (this.ItemTemplate != null)
            {
                visObject = this.ItemTemplate.CreateItem(item);                
            }
            else
                visObject = item as Visual3D;

            if (visObject != null)
            {

                // todo: set up bindings?
                // Cannot set DataContext, set bindings manually
                // http://stackoverflow.com/questions/7725313/how-can-i-use-databinding-for-3d-elements-like-visual3d-or-uielement3d
                this.Children.Add(visObject);

                this.models[item] = visObject;

            }
            else
            {
                throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
            }
        }

    }
}
