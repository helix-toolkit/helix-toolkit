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
    using HelixToolkit.SharpDX.Shared.Utilities;
    using SharpDX;

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
        /// Enable octree hit test to improve hit performance. Note: Octree does not support child using Transform. 
        /// </summary>
        public static readonly DependencyProperty UseOctreeHitTestProperty = DependencyProperty.Register("UseOctreeHitTest", typeof(bool), typeof(ItemsModel3D), 
            new PropertyMetadata(true,
                (s,e)=> {
                    var d = s as ItemsModel3D;
                    d.UpdateOctree();
                }));

        public static readonly DependencyProperty OctreeProperty = DependencyProperty.Register("Octree", typeof(IOctree), typeof(ItemsModel3D), new PropertyMetadata(null));

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

        /// <summary>
        /// Enable octree hit test to improve hit performance. Note: Octree does not support child using Transform. 
        /// </summary>
        public bool UseOctreeHitTest
        {
            set
            {
                SetValue(UseOctreeHitTestProperty, value);
            }
            get
            {
                return (bool)GetValue(UseOctreeHitTestProperty);
            }
        }

        public IOctree Octree
        {
            set
            {
                SetValue(OctreeProperty, value);
            }
            get
            {
                return (IOctree)GetValue(OctreeProperty);
            }
        }

        private readonly Dictionary<object, Model3D> mDictionary = new Dictionary<object, Model3D>();
        private volatile bool mRequestUpdateOctree = false;
        private readonly Queue<Model3D> mAddPendingQueue = new Queue<Model3D>();

        public ItemsModel3D()
        {
            this.Loaded += ItemsModel3D_Loaded;
        }

        private void ItemsModel3D_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBounds();
            if (mDictionary.Count > 0)
            {
                UpdateOctree();
            }
        }

        private void UpdateOctree()
        {
            if (UseOctreeHitTest)
            {
                if (Octree == null)
                {
                    Octree = RebuildOctree();
                }
                else
                {
                    while (mAddPendingQueue.Count > 0)
                    {
                        var model = mAddPendingQueue.Dequeue();
                        if(model is GeometryModel3D)
                        {
                            var tree = Octree as GeometryModel3DOctree;
                            Octree = null;
                            if (!tree.Add(model as GeometryModel3D))
                            {
                                Octree = RebuildOctree();
                            }
                            else
                            {
                                Octree = tree;
                            }
                        }

                    }
                }
                mRequestUpdateOctree = false;
            }
            else
            {
                mAddPendingQueue.Clear();
                Octree = null;
            }
        }

        private IOctree RebuildOctree()
        {
            var list = Children.Where(x => x is GeometryModel3D).Select(x => x as GeometryModel3D).ToList();
            var tree = new GeometryModel3DOctree(list);
            tree.BuildTree();
            return tree;
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
            Octree = null;
            mAddPendingQueue.Clear();
            if (e.OldValue is INotifyCollectionChanged)
            {
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= ItemsModel3D_CollectionChanged;
            }

            foreach(Model3D item in Children)
            {
                item.DataContext = null;
            }
            mDictionary.Clear();
            Children.Clear();

            if (e.NewValue is INotifyCollectionChanged)
            {
                (e.NewValue as INotifyCollectionChanged).CollectionChanged -= ItemsModel3D_CollectionChanged;
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += ItemsModel3D_CollectionChanged;
            }

            if (ItemsSource == null)
            {
                return;
            }
            if (this.ItemTemplate == null)
            {
                foreach (var item in this.ItemsSource)
                {
                    if (mDictionary.ContainsKey(item))
                    {
                        continue;
                    }
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
                    if (mDictionary.ContainsKey(item))
                    {
                        continue;
                    }
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
            mRequestUpdateOctree = true;
        }

        protected void ItemsModel3D_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        RemoveItemsFromOctree(e.OldItems);
                        foreach (var item in e.OldItems)
                        {
                            if (mDictionary.ContainsKey(item))
                            {
                                var model = mDictionary[item];
                                model.DataContext = null;
                                this.Children.Remove(model);
                                mDictionary.Remove(item);
                            }
                        }
                        InvalidateRender();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var array = this.Children.ToArray();
                    foreach (var item in array)
                    {
                        item.DataContext = null;
                        this.Children.Remove(item);
                    }
                    mDictionary.Clear();
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
                                if (mDictionary.ContainsKey(item))
                                {
                                    continue;
                                }
                                var model = this.ItemTemplate.LoadContent() as Model3D;
                                if (model != null)
                                {
                                    model.DataContext = item;
                                    this.Children.Add(model);
                                    mDictionary.Add(item, model);
                                    if (UseOctreeHitTest)
                                    {
                                        mAddPendingQueue.Enqueue(model);
                                    }
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
                                if (mDictionary.ContainsKey(item))
                                {
                                    continue;
                                }
                                var model = item as Model3D;
                                if (model != null)
                                {
                                    this.Children.Add(model);
                                    mDictionary.Add(item, model);
                                    if (UseOctreeHitTest)
                                    {
                                        mAddPendingQueue.Enqueue(model);
                                    }
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

            if (UseOctreeHitTest && Octree != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        break;
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Replace:
                        mRequestUpdateOctree = true;
                        break;
                    default:
                        Octree = null;
                        mRequestUpdateOctree = true;
                        break;
                }
            }
        }

        protected override void OnRender(RenderContext context)
        {
            base.OnRender(context);
            if (mRequestUpdateOctree)
            {
                UpdateOctree();
            }
        }

        private void RemoveItemsFromOctree(IList items)
        {
            var tree = Octree as GeometryModel3DOctree;
            if (tree != null)
            {
                Octree = null;
                foreach (var item in items)
                {
                    var holder = mDictionary[item];
                    if(holder is GeometryModel3D)
                    {
                        tree.Remove(holder as GeometryModel3D);
                    }
                }
                Octree = tree;
            }
            else
            {
                throw new InvalidOperationException("Octree does not support remove");
            }
        }

        public override bool HitTest(global::SharpDX.Ray ray, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (UseOctreeHitTest && Octree != null)
            {
                isHit= Octree.HitTest(this, modelMatrix, ray, ref hits);
            }
            else
            {
                isHit= base.HitTest(ray, ref hits);
            }
            return isHit;
        }
    }
}