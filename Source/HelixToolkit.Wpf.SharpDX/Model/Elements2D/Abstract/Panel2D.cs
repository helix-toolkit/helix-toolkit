using SharpDX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    [ContentProperty("Children")]
    public abstract class Panel2D : Element2D
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
            DependencyProperty.Register("ItemsSource", typeof(IList<Element2D>), typeof(Panel2D),
                new PropertyMetadata(null,
                    (d, e) => {
                        (d as Panel2D).OnItemsSourceChanged(e.NewValue as IList<Element2D>);
                    }));

        public override IEnumerable<IRenderable2D> Items
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

        public Panel2D()
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
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    AttachChildren(sender as IEnumerable);
                }
                else if (e.NewItems != null)
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

                c.Attach(RenderHost);
                c.Layout(RenderSize);
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
            if (base.OnAttach(host))
            {
                AttachChildren(Items);
                return true;
            }
            else
            {
                return false;
            }
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

        protected override void OnRender(IRenderContext2D context)
        {
            foreach (var c in this.Items)
            {
                c.Render(context);
            }
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            foreach (var item in Items.Reverse())
            {
                if (item is IHitable2D && (item as IHitable2D).HitTest(mousePoint, out hitResult))
                {
                    return true;
                }
            }
            return false;
        }

        protected override Vector2 MeasureOverride(Vector2 availableSizeWithoutMargins)
        {
            var requiredSize = DesiredSize;
            foreach (var ctl in Items)
            {
                ctl.Measure(availableSizeWithoutMargins);
            }
            return requiredSize;
        }

        protected override Vector2 ArrangeOverride(Vector2 availableSizeWithoutMargins)
        {
            foreach (var ctl in Items)
            {
                ctl.Arrange(availableSizeWithoutMargins);
            }

            return availableSizeWithoutMargins;
        }
    }
}
