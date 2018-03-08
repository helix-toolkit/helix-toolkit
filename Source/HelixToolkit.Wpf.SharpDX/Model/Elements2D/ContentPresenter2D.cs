using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    [ContentProperty("Content")]
    public class ContentPresenter2D : Element2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content", typeof(Element2DCore), 
            typeof(ContentPresenter2D),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure,
        (d, e) =>
        {
            var model = d as ContentPresenter2D;

            if (model.contentInternal != null)
            {
                model.RemoveLogicalChild(model.contentInternal);
                model.contentInternal.Detach();
            }
            model.contentInternal = (Element2DCore)e.NewValue;

            if (model.contentInternal != null)
            {
                model.AddLogicalChild(model.contentInternal);
                if (model.IsAttached)
                {
                    model.contentInternal.Attach(model.RenderHost);
                }
            }
        }));

        [Bindable(true)]
        public Element2DCore Content2D
        {
            set
            {
                SetValue(Content2DProperty, value);
            }
            get
            {
                return (Element2DCore)GetValue(Content2DProperty);
            }
        }

        protected Element2DCore contentInternal { private set; get; }
        private readonly IRenderable2D[] contentArray = new IRenderable2D[1];

        public override IList<IRenderable2D> Items
        {
            get
            {
                if(contentInternal != null)
                {
                    contentArray[0] = contentInternal;
                }
                return contentInternal == null ? Constants.EmptyRenderable2D : contentArray;
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                contentInternal?.Attach(host);
                if (contentInternal.Parent == null)
                {
                    this.AddLogicalChild(contentInternal);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            contentInternal?.Detach();
            if (contentInternal.Parent == this)
            {
                this.RemoveLogicalChild(contentInternal);
            }
            base.OnDetach();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            if (contentInternal != null)
            {
                return contentInternal.HitTest(mousePoint, out hitResult);
            }
            else
            {
                hitResult = null;
                return false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if (contentInternal != null)
            {
                contentInternal.Measure(availableSize);
                return new Size2F(contentInternal.DesiredSize.X, contentInternal.DesiredSize.Y);
            }
            else
            {
                return new Size2F();
            }
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            contentInternal.Arrange(finalSize);
            return new RectangleF(0, 0, contentInternal.DesiredSize.X, contentInternal.DesiredSize.Y);
        }
    }
}
