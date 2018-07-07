/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml.Markup;
using System.Collections.Generic;
using Windows.UI.Xaml;
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
            if (e.OldItems != null)
            {
                DetachChildren(e.OldItems);
            }           
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Items.Clear();
                var node = SceneNode as GroupNode;
                node.Clear();
                AttachChildren(sender as IList);
                if (OctreeManager != null)
                {
                    Items.Add(OctreeManager);
                }
            }
            else if (e.NewItems != null)
            {
                AttachChildren(e.NewItems);
            }
        }
        /// <summary>
        /// Attaches the children.
        /// </summary>
        /// <param name="children">The children.</param>
        protected void AttachChildren(IList children)
        {
            var node = SceneNode as GroupNode;
            foreach (Element3D c in children)
            {
                if (node.AddChildNode(c))
                {
                    Items.Add(c);
                }               
            }
        }
        /// <summary>
        /// Detaches the children.
        /// </summary>
        /// <param name="children">The children.</param>
        protected void DetachChildren(IList children)
        {
            var node = SceneNode as GroupNode;
            foreach (Element3D c in children)
            {                
                if(node.RemoveChildNode(c))
                {
                    Items.Remove(c);
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
                foreach (var child in itemsSourceInternal)
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
                foreach (var child in itemsSourceInternal)
                {
                    Children.Add(child);
                }
            }
        }

        private void S_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Element3D item in e.OldItems)
                {
                    Children.Remove(item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (Element3D item in e.NewItems)
                {
                    Children.Add(item);
                }
            }
        }
    }
}
