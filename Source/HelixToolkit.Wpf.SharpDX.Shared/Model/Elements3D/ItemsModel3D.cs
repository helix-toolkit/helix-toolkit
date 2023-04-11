// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a model that can be used to present a collection of items. supports generating child items by a
//   DataTemplate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
#if !COREWPF
    using Model.Scene;
#endif
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
            new PropertyMetadata(null, (s, e) =>
            {
                if (s is ItemsModel3D itemsModel && itemsModel.IsAttached)
                {
                    itemsModel.ItemsSourceChanged(e.NewValue as IEnumerable);
                }
            }));

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
            get
            {
                return (DataTemplate)this.GetValue(ItemTemplateProperty);
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content of the <see cref="ItemsModel3D" />.
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

        private IOctreeBasic Octree
        {
            get
            {
                return (SceneNode as GroupNode)?.OctreeManager?.Octree;
            }
        }

        private readonly Dictionary<object, Element3D> elementDict = new Dictionary<object, Element3D>();
        private IEnumerable itemsSourceInternal;

        public ItemsModel3D()
        {
            SceneNode.Attached += SceneNode_Attached;
            SceneNode.Detached += SceneNode_Detached;
        }

        private void SceneNode_Attached(object sender, EventArgs e)
        {
            if (ItemsSource != null)
            {
                ItemsSourceChanged(ItemsSource);
            }
        }

        private void SceneNode_Detached(object sender, EventArgs e)
        {
            if (itemsSourceInternal != null)
            {
                ItemsSourceChanged(null);
            }
        }

        private void ItemsSourceChanged(IEnumerable itemsSource)
        {
            if (itemsSourceInternal == itemsSource)
            { return; }
            if (itemsSourceInternal is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= ItemsModel3D_CollectionChanged;
            }
            if (itemsSourceInternal == null && itemsSource != null && Children.Count > 0)
            {
                throw new InvalidOperationException("Children must be empty before using ItemsSource");
            }

            elementDict.Clear();
            Children.Clear();

            itemsSourceInternal = itemsSource;

            if (itemsSourceInternal is INotifyCollectionChanged n)
            {
                n.CollectionChanged -= ItemsModel3D_CollectionChanged;
                n.CollectionChanged += ItemsModel3D_CollectionChanged;
            }

            if (itemsSourceInternal == null)
            {
                return;
            }

            if (this.ItemTemplate == null)
            {
                foreach (var item in this.itemsSourceInternal)
                {
                    if (item is Element3D model)
                    {
                        elementDict.Add(item, model);
                        Children.Add(model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                    }
                }
            }
            else
            {
                foreach (var item in this.itemsSourceInternal)
                {
                    if (this.ItemTemplate.LoadContent() is Element3D model)
                    {
                        model.DataContext = item;
                        elementDict.Add(item, model);
                        Children.Add(model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                    }
                }
            }
            if (Children.Count > 0)
            {
                (SceneNode as GroupNode).OctreeManager?.RequestRebuild();
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
                            if (elementDict.TryGetValue(item, out var model))
                            {
                                elementDict.Remove(item);
                                Children.Remove(model);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Children.Clear();
                    elementDict.Clear();
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
                                if (item is Element3D model)
                                {
                                    elementDict.Add(item, model);
                                    Children.Add(model);
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
                                if (this.ItemTemplate.LoadContent() is Element3D model)
                                {
                                    model.DataContext = item;
                                    elementDict.Add(item, model);
                                    Children.Add(model);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                                }
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
                                if (this.ItemTemplate.LoadContent() is Element3D model)
                                {
                                    model.DataContext = item;
                                    elementDict.Add(item, model);
                                    Children.Add(model);
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
                                if (item is Element3D model)
                                {
                                    elementDict.Add(item, model);
                                    Children.Add(model);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    Children.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }
        public virtual void Clear()
        {
            elementDict.Clear();
            var node = SceneNode as GroupNode;
            node.Clear();
            Children.Clear();
        }
    }
}