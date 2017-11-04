using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public abstract class ContentElement2D : Clickable2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D", typeof(Element2D), typeof(ContentElement), 
            new AffectsRenderPropertyMetadata(null, (d,e)=>
            {
                var model = d as ContentElement2D;
                model.contentInternal?.Detach();
                model.contentInternal = e.NewValue == null ? null : (Element2D)e.NewValue;
                model.contentInternal?.Attach(model.renderHost);
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

        protected override bool CanRender(RenderContext context)
        {
            return IsAttached && isRenderingInternal;
        }

        protected override void OnRender(RenderContext context)
        {
            contentInternal?.Render(context);
        }
    }
}
