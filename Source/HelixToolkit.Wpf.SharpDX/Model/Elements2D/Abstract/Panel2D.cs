using SharpDX;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    [ContentProperty("Children")]
    public abstract class Panel2D : Element2D
    {
        public override IEnumerable<IRenderable2D> Items
        {
            get
            {
                return Children;
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
                //c.Layout(RenderSize);
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

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                foreach (var item in Items.Reverse())
                {
                    if (item is IHitable2D && (item as IHitable2D).HitTest(mousePoint, out hitResult))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //protected override Vector2 MeasureOverride(Vector2 availableSizeWithoutMargins)
        //{
        //    var requiredSize = DesiredSize;
        //    foreach (var ctl in Items)
        //    {
        //        ctl.Measure(availableSizeWithoutMargins);
        //    }
        //    return requiredSize;
        //}

        //protected override Vector2 ArrangeOverride(Vector2 finalSize)
        //{
        //    foreach (var ctl in Items)
        //    {
        //        ctl.Arrange(new RectangleF(0,0,finalSize.X, finalSize.Y));
        //    }

        //    return finalSize;
        //}
    }
}
