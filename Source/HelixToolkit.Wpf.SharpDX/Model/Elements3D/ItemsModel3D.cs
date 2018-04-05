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

    using Model.Scene;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;

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
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManagerWrapper),
            typeof(ItemsModel3D), new PropertyMetadata(null, (s, e) =>
            {
                var d = s as ItemsModel3D;
                if (e.OldValue != null)
                {
                    d.RemoveLogicalChild(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue);
                }
                (d.SceneNode as GroupNode).OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
            }));

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

        public IOctreeManagerWrapper OctreeManager
        {
            set
            {
                SetValue(OctreeManagerProperty, value);
            }
            get
            {
                return (IOctreeManagerWrapper)GetValue(OctreeManagerProperty);
            }
        }

        private IOctree Octree
        {
            get { return (SceneNode as GroupNode).OctreeManager == null ? null : (SceneNode as GroupNode).OctreeManager.Octree; }
        }

        private readonly Dictionary<object, Element3D> elementDict = new Dictionary<object, Element3D>();

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
            if (e.OldValue is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= ItemsModel3D_CollectionChanged;
            }

            foreach (Element3D item in Children)
            {
                item.DataContext = null;
            }

            Clear();

            if (e.NewValue is INotifyCollectionChanged n)
            {
                n.CollectionChanged -= ItemsModel3D_CollectionChanged;
                n.CollectionChanged += ItemsModel3D_CollectionChanged;
            }

            if (ItemsSource == null)
            {
                return;
            }

            if (this.ItemTemplate == null)
            {
                foreach (var item in this.ItemsSource)
                {
                    var model = item as Element3D;
                    if (model != null)
                    {
                        this.Children.Add(model);
                        elementDict.Add(item, model);
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
                    var model = this.ItemTemplate.LoadContent() as Element3D;
                    if (model != null)
                    {
                        model.DataContext = item;
                        this.Children.Add(model);
                        elementDict.Add(item, model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                    }
                }
            }
            if (Children.Count > 0)
            {
                var groupNode = SceneNode as GroupNode;
                groupNode.OctreeManager?.RequestRebuild();
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
                            Element3D element;
                            if(elementDict.TryGetValue(item, out element))
                            {
                                Children.Remove(element);
                                elementDict.Remove(item);
                            }
                        }
                        InvalidateRender();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (this.ItemsSource != null)
                    {
                        if (this.ItemTemplate == null)
                        {
                            foreach (var item in this.ItemsSource)
                            {
                                var model = item as Element3D;
                                if (model != null)
                                {
                                    this.Children.Add(model);
                                    elementDict.Add(item, model);
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
                                var model = this.ItemTemplate.LoadContent() as Element3D;
                                if (model != null)
                                {
                                    model.DataContext = item;
                                    this.Children.Add(model);
                                    elementDict.Add(item, model);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                                }
                            }
                        }
                    }
                    InvalidateRender();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        if (this.ItemTemplate != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var model = this.ItemTemplate.LoadContent() as Element3D;
                                if (model != null)
                                {                                    
                                    model.DataContext = item;
                                    this.Children.Add(model);
                                    elementDict.Add(item, model);
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
                                var model = item as Element3D;
                                if (model != null)
                                {                                    
                                    this.Children.Add(model);
                                    elementDict.Add(item, model);
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
        }

        public override void Clear()
        {
            elementDict.Clear();
            base.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            elementDict.Clear();
            base.Dispose(disposing);
        }
    }
}