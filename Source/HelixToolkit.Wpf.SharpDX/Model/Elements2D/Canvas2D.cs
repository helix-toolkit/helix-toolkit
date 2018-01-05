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
    using global::SharpDX;

    /// <summary>
    /// Supports both ItemsSource binding and Xaml children. Binds with ObservableElement2DCollection 
    /// </summary>
    [ContentProperty("Children")]
    public class Canvas2D : Model2D
    {
        #region Attached Properties
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Canvas2D),
            new AffectsRenderPropertyMetadata(0.0, (d,e)=> { (d as Element2D).LayoutTranslate = new Vector2((float)(double)e.NewValue, (d as Element2D).LayoutTranslate.Y); }));

        public static void SetLeft(Element2D element, double value)
        {
            element.SetValue(LeftProperty, value);
        }

        public static double GetLeft(Element2D element)
        {
            return (double)element.GetValue(LeftProperty);
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Canvas2D),
            new AffectsRenderPropertyMetadata(0.0, (d, e) => { (d as Element2D).LayoutTranslate = new Vector2((d as Element2D).LayoutTranslate.X, (float)(double)e.NewValue); }));
        public static void SetTop(Element2D element, double value)
        {
            element.SetValue(TopProperty, value);
        }

        public static double GetTop(Element2D element)
        {
            return (double)element.GetValue(TopProperty);
        }
        #endregion

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
            DependencyProperty.Register("ItemsSource", typeof(IList<Element2D>), typeof(Canvas2D),
                new AffectsRenderPropertyMetadata(null, 
                    (d, e) => {
                        (d as Canvas2D).OnItemsSourceChanged(e.NewValue as IList<Element2D>);
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

        public Canvas2D()
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

        protected override bool CanRender(IRenderContext2D context)
        {
            return IsAttached && isRenderingInternal;
        }

        protected override void PreRender(IRenderContext2D context)
        {
            
        }

        protected override void OnRender(IRenderContext2D context)
        {
            foreach (var c in this.Items)
            {
                c.PushLayoutTranslate(this.LayoutTranslate);
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
                c.PopLayoutTranslate();
            }
        }

        protected override void OnLayoutTranslationChanged(Vector2 translation)
        {

        }

        protected override IRenderable2D CreateRenderCore(ID2DTarget host)
        {
            return null;
        }

        protected override void OnRenderTargetChanged(global::SharpDX.Direct2D1.RenderTarget newTarget)
        {
            
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            foreach (var item in Items.Reverse())
            {
                if (item.HitTest(mousePoint, out hitResult))
                {
                    return true;
                }
            }
            return false;
        }
    }
}