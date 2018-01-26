using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    [ContentProperty("Content2D")]
    public abstract class ContentElement2D : Clickable2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D", typeof(Element2D), typeof(ContentElement2D), 
            new PropertyMetadata(null, (d,e)=>
            {
                var model = d as ContentElement2D;

                if (model.contentInternal != null)
                {
                    model.RemoveLogicalChild(model.contentInternal);
                    model.contentInternal.Detach();                
                }
                model.contentInternal = e.NewValue == null ? null : (Element2D)e.NewValue;
                if (model.contentInternal != null)
                {
                    model.contentInternal.Attach(model.RenderHost);
                    model.AddLogicalChild(model.contentInternal);
                    if (model.IsAttached)
                    { model.contentInternal.Measure(model.RenderSize); }
                }
            }));

        public Element2D Content2D
        {
            set
            {
                SetValue(Content2DProperty, value);
            }
            get
            {
                return (Element2D)GetValue(Content2DProperty);
            }
        }

        private Element2D contentInternal;

        public override IEnumerable<IRenderable2D> Items
        {
            get
            {
                return Content2D == null ? Enumerable.Empty<IRenderable2D>() : Enumerable.Repeat<IRenderable2D>(Content2D, 1);
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                Content2D?.Attach(host);
                //Layout(new Vector2((float)host.ActualWidth, (float)host.ActualHeight));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            Content2D?.Detach();
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
    }
}
