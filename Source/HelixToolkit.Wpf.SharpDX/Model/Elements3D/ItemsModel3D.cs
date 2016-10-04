// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a model that can be used to present a collection of items. supports generating child items by a
//   DataTemplate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    ///     Represents a model that can be used to present a collection of items. supports generating child items by a
    ///     <see cref="DataTemplate" />.
    /// </summary>
    /// <remarks>
    ///     Use the ItemsSource property to specify the collection to use to generate the content of your ItemsControl. You can set the ItemsSource
    ///     property to any type that implements IEnumerable. ItemsSource is typically used to display a data collection or to bind an
    ///     ItemsControl to a collection object.
    /// </remarks>
    public class ItemsModel3D : CompositeModel3D
    {
        /// <summary>
        ///     The item template property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof(DataTemplate), typeof(ItemsModel3D), new PropertyMetadata(null));

        /// <summary>
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsModel3D),
            new PropertyMetadata(null, (s, e) => ((ItemsModel3D)s).ItemsSourceChanged(e)));

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate" /> used to display each item.
        /// </summary>
        /// <value>
        ///     The item template.
        /// </value>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content of the <see cref="ItemsModel3D" />.
        /// </summary>
        /// <value>
        ///     The items source.
        /// </value>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private readonly Dictionary<object, Model3D> mDictionary = new Dictionary<object, Model3D>();
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
            if (this.ItemsSource != null)
            {
                if (e.OldValue is INotifyCollectionChanged)
                {
                    (e.OldValue as INotifyCollectionChanged).CollectionChanged -= ItemsModel3D_CollectionChanged;
                }
                if (ItemsSource is INotifyCollectionChanged)
                {
                    (ItemsSource as INotifyCollectionChanged).CollectionChanged += ItemsModel3D_CollectionChanged;
                }
                if (this.ItemTemplate == null)
                {
                    foreach (var item in this.ItemsSource)
                    {
                        var model = item as Model3D;
                        if (model != null)
                        {
                            this.Children.Add(model);
                            mDictionary.Add(item, model);
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                        }
                    }
                }
                else
                {
                    foreach (var item in this.ItemsSource)
                    {
                        var model = this.ItemTemplate.LoadContent() as Model3D;
                        if (model != null)
                        {
                            model.DataContext = item;
                            this.Children.Add(model);
                            mDictionary.Add(item, model);
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                        }
                    }
                }
            }
        }

        protected void ItemsModel3D_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (mDictionary.ContainsKey(item))
                            {
                                var model = mDictionary[item];
                                model.Detach();
                                this.Children.Remove(model);
                                mDictionary.Remove(item);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var array = this.Children.ToArray();
                    foreach (var item in array)
                    {
                        item.Detach();
                        this.Children.Remove(item);
                    }
                    mDictionary.Clear();
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (this.ItemTemplate == null)
                    {
                        foreach (var item in this.ItemsSource)
                        {
                            var model = item as Model3D;
                            if (model != null)
                            {
                                this.Children.Add(model);
                                mDictionary.Add(item, model);
                            }
                            else
                            {
                                throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in this.ItemsSource)
                        {
                            var model = this.ItemTemplate.LoadContent() as Model3D;
                            if (model != null)
                            {
                                model.DataContext = item;
                                this.Children.Add(model);
                                mDictionary.Add(item, model);
                            }
                            else
                            {
                                throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        if (this.ItemTemplate != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var model = this.ItemTemplate.LoadContent() as Model3D;
                                if (model != null)
                                {
                                    model.DataContext = item;
                                    this.Children.Add(model);
                                    mDictionary.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in e.NewItems)
                            {
                                var model = item as Model3D;
                                if (model != null)
                                {
                                    this.Children.Add(model);
                                    mDictionary.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                                }
                            }
                        }
                    }
                    break;
            }
            InvalidateRender();
        }
    }
}