/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
namespace HelixToolkit.UWP
{
    using Model.Scene;
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3D" />
    [ContentProperty(Name = "Children")]
    public abstract class GroupElement3D : Element3D
    {
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public new static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<Element3D>), typeof(GroupElement3D),
                new PropertyMetadata(null,
                    (d, e) => {
                        (d as GroupElement3D).OnItemsSourceChanged(e.NewValue as IList<Element3D>);
                    }));

        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManagerWrapper),
            typeof(GroupElement3D), new PropertyMetadata(null, (s, e) =>
            {
                var d = s as GroupElement3D;
                if (e.OldValue != null)
                {
                    d.Items.Remove(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.Items.Add(e.NewValue);
                }
                (d.SceneNode as GroupNode).OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
            }));

        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public new IList<Element3D> ItemsSource
        {
            get { return (IList<Element3D>)this.GetValue(ItemsSourceProperty); }
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

        private IOctreeBasic Octree
        {
            get { return (SceneNode as GroupNode).OctreeManager?.Octree; }
        }

        private IList<Element3D> itemsSourceInternal;
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public ObservableElement3DCollection Children
        {
            get;
        } = new ObservableElement3DCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupElement3D"/> class.
        /// </summary>
        public GroupElement3D()
        {
            Children.CollectionChanged += Items_CollectionChanged;           
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Items.Clear();
            foreach (var item in Children)
            {
                if (item.Parent != this)
                {
                    Items.Add(item);
                }
            }
            if(OctreeManager != null)
            {
                Items.Add(OctreeManager);
            }
        }

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var node = SceneNode as GroupNode;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (Element3D item in e.OldItems)
                        {
                            if (node.RemoveChildNode(item.SceneNode))
                            {
                                Items.Remove(item);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Items.Clear();
                    node.Clear();
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (sender is IList list)
                    {
                        foreach (Element3D item in list)
                        {
                            if (node.AddChildNode(item.SceneNode))
                            {
                                Items.Add(item);
                            }
                        }
                    }
                    if (OctreeManager != null)
                    {
                        Items.Add(OctreeManager);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Element3D item in e.NewItems)
                    {
                        if (node.AddChildNode(item.SceneNode))
                        {
                            Items.Add(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    node.MoveChildNode(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }

        private void OnItemsSourceChanged(IList<Element3D> itemsSource)
        {
            if (itemsSourceInternal != null)
            {
                if (itemsSourceInternal is INotifyCollectionChanged s)
                {
                    s.CollectionChanged -= S_CollectionChanged;
                }
                foreach (var child in itemsSourceInternal)
                {
                    Children.Remove(child);
                }
            }
            if(itemsSourceInternal == null && itemsSource != null && Children.Count > 0)
            {
                throw new InvalidOperationException("Children must be empty before using ItemsSource");
            }
            Children.Clear();
            itemsSourceInternal = itemsSource;
            if (itemsSourceInternal != null)
            {
                if (itemsSourceInternal is INotifyCollectionChanged s)
                {
                    s.CollectionChanged += S_CollectionChanged;
                }
                foreach (var child in itemsSourceInternal)
                {
                    Children.Add(child);
                }
            }
        }

        private void S_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Children.Clear();
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Element3D item in e.OldItems)
                    {
                        Children.Remove(item);
                    }
                    break;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    foreach (Element3D item in itemsSourceInternal)
                    {
                        Children.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Element3D item in e.NewItems)
                    {
                        Children.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    Children.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }
    }
}
