// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;
    using System.Linq;
    using System.Collections;
    using Core2D;
    using System;
    using SharpDX;

    /// <summary>
    /// Supports both ItemsSource binding and Xaml children. Binds with ObservableElement2DCollection 
    /// </summary>
    [ContentProperty("Children")]
    public class GroupElement2D : Model2D
    {
        private IList<Element2D> itemsSourceInternal;
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement2DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public IList<Element2D> ItemsSource
        {
            get { return (IList<Element2D>)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }
        /// <summary>
        /// ItemsSource for binding to collection. Please use ObservableElement2DCollection for observable, otherwise may cause memory leak.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<Element2D>), typeof(GroupElement2D),
                new AffectsRenderPropertyMetadata(null, 
                    (d, e) => {
                        (d as GroupElement2D).OnItemsSourceChanged(e.NewValue as IList<Element2D>);
                    }));

        public IEnumerable<Element2D> Items
        {
            get
            {
                return itemsSourceInternal == null ? Children : Children.Concat(itemsSourceInternal);
            }
        }

        public ObservableElement2DCollection Children
        {
            get;
        } = new ObservableElement2DCollection();

        public GroupElement2D()
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
            foreach (Element2D c in children)
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
            foreach (Element2D c in children)
            {
                c.Detach();
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);
                }
            }
        }

        private void OnItemsSourceChanged(IList<Element2D> itemsSource)
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
                if (itemsSourceInternal is ObservableElement2DCollection)
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
            return IsAttached && isRenderingInternal;
        }

        protected override void PreRender(RenderContext context)
        {
            
        }

        protected override void OnRender(RenderContext context)
        {
            foreach (var c in this.Items)
            {
                var model = c as ITransformable2D;
                if (model != null)
                {
                    model.PushMatrix(this.TransformMatrix);
                    c.Render(context);
                    model.PopMatrix();
                }
                else
                {
                    c.Render(context);
                }
            }
        }

        protected override IRenderable2D CreateRenderCore(IRenderHost host)
        {
            return null;
        }

        protected override void OnRenderTargetChanged(global::SharpDX.Direct2D1.RenderTarget newTarget)
        {
            
        }

        protected override bool OnHitTest(ref global::SharpDX.Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            foreach (var item in Items.Reverse())
            {
                var model = item as ITransformable2D;
                if (model != null)
                {
                    model.PushMatrix(this.TransformMatrix);
                    bool isHit = item.HitTest(mousePoint, out hitResult);
                    model.PopMatrix();
                    if (isHit)
                    { return true; }
                }
                else if (item.HitTest(mousePoint, out hitResult))
                {
                    return true;
                }
            }
            return false;
        }
    }
}