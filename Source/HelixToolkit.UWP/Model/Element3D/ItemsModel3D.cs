using System;
using System.Collections;
using System.Collections.Generic;

using Windows.UI.Xaml;

namespace HelixToolkit.UWP
{
    using Model.Scene;

    using System.Collections.Specialized;

    public class ItemsModel3D : Element3D, IHitable
    {
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public new static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsModel3D),
                new PropertyMetadata(null,
                    (d, e) => {
                        (d as ItemsModel3D).OnItemsSourceChanged(e);
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
        public new IEnumerable ItemsSource
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

        private IOctreeBasic Octree
        {
            get { return (SceneNode as GroupNode).OctreeManager?.Octree; }
        }

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

        private readonly Dictionary<object, Element3D> elementDict = new Dictionary<object, Element3D>();
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupElement3D"/> class.
        /// </summary>
        public ItemsModel3D()
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
            if (OctreeManager != null)
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
                if (node.AddChildNode(c.SceneNode))
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
                if (node.RemoveChildNode(c.SceneNode))
                {
                    Items.Remove(c);
                }
            }
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
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

            AddItems(ItemsSource);

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
                            if (elementDict.TryGetValue(item, out Element3D element))
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
                    AddItems(ItemsSource);
                    InvalidateRender();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    AddItems(e.NewItems);
                    break;
            }
        }

        private void AddItems(IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (this.ItemTemplate != null)
                {
                    AddFromTemplate(item, this.ItemTemplate);
                }
                else if (this.ItemTemplateSelector != null)
                {
                    DataTemplate template = this.ItemTemplateSelector.SelectTemplate(item, this);

                    if (template != null)
                    {
                        AddFromTemplate(item, template);
                    }
                    else
                    {
                        throw new InvalidOperationException("No template for item.");
                    }
                }
                else
                {
                    if (item is Element3D model)
                    {
                        this.Children.Add(model);
                        elementDict.Add(item, model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Item is not a Model3D.");
                    }
                }
            }
        }

        private void AddFromTemplate(object item, DataTemplate template)
        {
            if (template.LoadContent() is Element3D model)
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

        public virtual void Clear()
        {
            elementDict.Clear();
            var node = SceneNode as GroupNode;
            node.Clear();
            Children.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            base.Dispose(disposing);
        }
    }
}
