/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Controls;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Controls;
namespace HelixToolkit.UWP
#endif
{
#if WINDOWS_UWP
    using Model.Scene;
#endif
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
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<Element3D>), typeof(GroupElement3D),
                new PropertyMetadata(null,
                    (d, e) =>
                    {
                        if (d is GroupElement3D g && g.IsAttached)
                        {
                            g.OnItemsSourceChanged(e.NewValue as IList<Element3D>);
                        }
                    }));

        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManagerWrapper),
            typeof(GroupElement3D), new PropertyMetadata(null, (s, e) =>
            {
                var d = s as GroupElement3D;
                if (e.OldValue is Element3D elem_old)
                {
                    d.Items.Remove(elem_old);
                }

                if (e.NewValue is Element3D elem)
                {
                    d.Items.Add(elem);
                }
                (d.SceneNode as GroupNode).OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
            }));

        // Using a DependencyProperty as the backing store for AlwaysHittable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlwaysHittableProperty =
            DependencyProperty.Register("AlwaysHittable", typeof(bool), typeof(GroupElement3D), new PropertyMetadata(false, (d, e) =>
            {
                (d as GroupElement3D).SceneNode.AlwaysHittable = (bool)e.NewValue;
            }));


        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public IList<Element3D> ItemsSource
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

        /// <summary>
        /// Gets or sets a value indicating whether [always hittable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [always hittable]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysHittable
        {
            get
            {
                return (bool)GetValue(AlwaysHittableProperty);
            }
            set
            {
                SetValue(AlwaysHittableProperty, value);
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

        private readonly ItemsControl itemsControl = new ItemsControl();
        public ItemCollection Items => itemsControl.Items;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupElement3D"/> class.
        /// </summary>
        public GroupElement3D()
        {
            Children.CollectionChanged += Items_CollectionChanged;
            SceneNode.Attached += SceneNode_Attached;
            SceneNode.Detached += SceneNode_Detached;
        }

        private void SceneNode_Attached(object sender, EventArgs e)
        {
            if (ItemsSource != null)
            {
                OnItemsSourceChanged(ItemsSource);
            }
        }

        private void SceneNode_Detached(object sender, EventArgs e)
        {
            if (itemsSourceInternal != null)
            {
                OnItemsSourceChanged(null);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachChild(itemsControl);
            Items.Clear();
            foreach (var item in Children)
            {
                if (item.Parent != this)
                {
                    Items.Add(item);
                }
            }
            if (OctreeManager is Element3D elem)
            {
                Items.Add(elem);
            }
        }

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode() { AlwaysHittable = AlwaysHittable };
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
                                Items?.Remove(item);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Items?.Clear();
                    node?.Clear();
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
                                Items?.Add(item);
                            }
                        }
                    }
                    if (OctreeManager is Element3D elem)
                    {
                        Items?.Add(elem);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Element3D item in e.NewItems)
                    {
                        if (node.AddChildNode(item.SceneNode))
                        {
                            Items?.Add(item);
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
            if (itemsSourceInternal == itemsSource)
            { return; }
            if (itemsSourceInternal != null)
            {
                if (itemsSourceInternal is INotifyCollectionChanged s)
                {
                    s.CollectionChanged -= S_CollectionChanged;
                }
            }
            if (itemsSourceInternal == null && itemsSource != null && Children.Count > 0)
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
