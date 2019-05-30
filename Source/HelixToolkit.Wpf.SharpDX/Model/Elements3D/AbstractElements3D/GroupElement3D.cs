// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Wpf.SharpDX.Model.Scene;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Supports both ItemsSource binding and Xaml children. Binds with ObservableElement3DCollection 
    /// </summary>
    [ContentProperty("Children")]
    public abstract class GroupElement3D : Element3D
    {
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
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
                    d.RemoveLogicalChild(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue);
                }
                (d.SceneNode as GroupNode).OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
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

        private IOctreeBasic Octree
        {
            get { return (SceneNode as GroupNode).OctreeManager == null ? null : (SceneNode as GroupNode).OctreeManager.Octree; }
        }

        private IList<Element3D> itemsSourceInternal;

        public ObservableElement3DCollection Children
        {
            get;
        } = new ObservableElement3DCollection();


        public GroupElement3D()
        {
            Children.CollectionChanged += Items_CollectionChanged;
            Loaded += GroupElement3D_Loaded;
        }

        private void GroupElement3D_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Element3D c in Children)
            {
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);
                }
            }
            foreach (Element3D c in Children)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);
                }
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                DetachChildren(e.OldItems);
            }            
            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                AttachChildren(sender as IEnumerable);
            }
            else if(e.NewItems != null)
            {
                AttachChildren(e.NewItems);
            }
        }

        protected void AttachChildren(IEnumerable children)
        {
            var node = SceneNode as GroupNodeBase;
            foreach (Element3D c in children)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);
                }
                node.AddChildNode(c);
            }
        }

        protected void DetachChildren(IEnumerable children)
        {
            var node = SceneNode as GroupNodeBase;
            foreach (Element3D c in children)
            {
                node.RemoveChildNode(c);
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);
                }
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
                foreach(var child in itemsSourceInternal)
                {
                    Children.Remove(child);
                }
            }
            itemsSourceInternal = itemsSource;
            if (itemsSourceInternal != null)
            {
                if (itemsSourceInternal is INotifyCollectionChanged s)
                {
                    s.CollectionChanged += S_CollectionChanged;
                }
                foreach(var child in itemsSourceInternal)
                {
                    Children.Add(child);
                }    
            }
        }

        private void S_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach(Element3D item in e.OldItems)
                {
                    Children.Remove(item);
                }
            }
            if (e.NewItems != null)
            {
                foreach(Element3D item in e.NewItems)
                {
                    Children.Add(item);
                }
            }
        }
    }
}