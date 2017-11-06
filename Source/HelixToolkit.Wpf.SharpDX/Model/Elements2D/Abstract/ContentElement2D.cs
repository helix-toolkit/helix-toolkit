using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public abstract class ContentElement2D : Clickable2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D", typeof(Element2D), typeof(ContentElement), 
            new AffectsRenderPropertyMetadata(null, (d,e)=>
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
                    model.contentInternal.Attach(model.renderHost);
                    model.AddLogicalChild(model.contentInternal);
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

        protected override bool OnAttach(IRenderHost host)
        {
            contentInternal?.Attach(host);
            return base.OnAttach(host) && contentInternal.IsAttached; 
        }

        protected override void OnDetach()
        {
            contentInternal?.Detach();
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            return IsAttached && isRenderingInternal;
        }

        protected override void OnRender(RenderContext context)
        {
            contentInternal?.Render(context);
        }

        protected override void OnLayoutTranslationChanged(Vector2 translation)
        {
            base.OnLayoutTranslationChanged(translation);
            if (contentInternal != null)
            {
                contentInternal.LayoutTranslate = translation;
            }
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
