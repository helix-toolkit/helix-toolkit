// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;
    using System.Linq;
    using System.Collections;

    /// <summary>
    /// Supports both ItemsSource binding and Xaml children. Binds with ObservableElement3DCollection 
    /// </summary>
    [ContentProperty("Children")]
    public abstract class GroupElement3D : Element3D //, IElement3DCollection
    {
        private IList<Element3D> itemsSourceInternal;
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public IList<Element3D> ItemsSource
        {
            get { return (IList<Element3D>)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<Element3D>), typeof(GroupElement3D),
                new AffectsRenderPropertyMetadata(null, 
                    (d, e) => {
                        (d as GroupElement3D).OnItemsSourceChanged(e.NewValue as IList<Element3D>);
                    }));

        public IEnumerable<Element3D> Items
        {
            get
            {
                return itemsSourceInternal == null ? Children : Children.Concat(itemsSourceInternal);
            }
        }

        public ObservableElement3DCollection Children
        {
            get;
        } = new ObservableElement3DCollection();

        public GroupElement3D()
        {
            Children.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                DetachChildren(e.OldItems);
            }
            if (IsAttached)
            {               
                if(e.Action== NotifyCollectionChangedAction.Reset)
                {
                    AttachChildren(sender as IEnumerable);
                }
                else if(e.NewItems != null)
                {
                    AttachChildren(e.NewItems);
                }
            }
        }

        protected void AttachChildren(IEnumerable children)
        {
            foreach (Element3D c in children)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);
                }

                c.Attach(renderHost);
            }
        }

        protected void DetachChildren(IEnumerable children)
        {
            foreach (Element3D c in children)
            {
                c.Detach();
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
                if (itemsSourceInternal is INotifyCollectionChanged)
                {
                    (itemsSourceInternal as INotifyCollectionChanged).CollectionChanged -= Items_CollectionChanged;
                }
                DetachChildren(this.itemsSourceInternal);
            }
            itemsSourceInternal = itemsSource;
            if (itemsSourceInternal != null)
            {
                if (itemsSourceInternal is ObservableElement3DCollection)
                {
                    (itemsSourceInternal as INotifyCollectionChanged).CollectionChanged += Items_CollectionChanged;
                }
                if (IsAttached)
                {
                    AttachChildren(this.itemsSourceInternal); 
                }            
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            AttachChildren(Items);
            return true;
        }

        protected override void OnDetach()
        {
            DetachChildren(Items);
            base.OnDetach();
        }        

        protected override bool CanRender(RenderContext context)
        {
            return IsAttached && isRenderingInternal && visibleInternal;
        }

        protected override void OnRender(RenderContext context)
        {
            foreach (var c in this.Items)
            {
                c.Render(context);
            }
        }
    }
}