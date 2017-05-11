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
    using SharpDX;
    using System.Windows.Media;
    using System.Diagnostics;
    using System.Windows.Markup;

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
            "ItemTemplate", typeof(DataTemplate), typeof(ItemsModel3D), new AffectsRenderPropertyMetadata(null));

        /// <summary>
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsModel3D),
            new AffectsRenderPropertyMetadata(null, (s, e) => ((ItemsModel3D)s).ItemsSourceChanged(e)));

        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManager),
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

        public IOctreeManager OctreeManager
        {
            set
            {
                SetValue(OctreeManagerProperty, value);
            }
            get
            {
                return (IOctreeManager)GetValue(OctreeManagerProperty);
            }
        }

        private readonly Dictionary<object, Model3D> mDictionary = new Dictionary<object, Model3D>();
        //private bool loaded = false;
        private IOctree Octree
        {
            get { return OctreeManager == null ? null : OctreeManager.Octree; }
        }

        public ItemsModel3D()
        {
            //this.Loaded += ItemsModel3D_Loaded;
            //this.Unloaded += ItemsModel3D_Unloaded;
        }

        //private void ItemsModel3D_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    loaded = false;
        //    OctreeManager?.Clear();
        //}

        //private void ItemsModel3D_Loaded(object sender, RoutedEventArgs e)
        //{
        //    loaded = true;
        //    UpdateBounds();
        //    //if (Children.Count > 0)
        //    //{
        //    //    OctreeManager?.RequestRebuild();
        //    //}
        //}

        private void UpdateOctree()
        {
            OctreeManager?.RebuildTree(this.Children);
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
            if (e.OldValue is INotifyCollectionChanged)
            {
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= ItemsModel3D_CollectionChanged;
            }

            foreach (Model3D item in Children)
            {
                item.DataContext = null;
            }

            OctreeManager?.Clear();
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
            if (Children.Count > 0)
            {
                OctreeManager?.RequestRebuild();
            }
        }

        protected void ItemsModel3D_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    OctreeManager?.Clear();
                    OctreeManager?.RequestRebuild();
                    break;
            }
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
                                if (model is GeometryModel3D)
                                    OctreeManager?.RemoveItem(model as GeometryModel3D);
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
                                    OctreeManager?.AddPendingItem(model);
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
                                if (mDictionary.ContainsKey(item))
                                {
                                    continue;
                                }
                                var model = item as Model3D;
                                if (model != null)
                                {
                                    OctreeManager?.AddPendingItem(model);
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
        }

        protected override void OnRender(RenderContext context)
        {
            base.OnRender(context);
            if (OctreeManager != null)
            {
                if (OctreeManager.RequestUpdateOctree)
                {
                    UpdateOctree();
                }
            }
        }

        protected override bool OnHitTest(IRenderMatrices context, global::SharpDX.Ray ray, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (Octree != null)
            {
                isHit = Octree.HitTest(context, this, modelMatrix, ray, ref hits);
#if DEBUG
                if (isHit)
                {
                    Debug.WriteLine("Octree hit test, hit at " + hits[0].PointHit);
                }
#endif
            }
            else
            {
                isHit = base.OnHitTest(context, ray, ref hits);
            }
            return isHit;
        }
    }
}