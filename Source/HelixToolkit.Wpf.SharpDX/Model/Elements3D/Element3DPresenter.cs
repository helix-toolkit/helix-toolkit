using HelixToolkit.Wpf.SharpDX.Render;
using SharpDX;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX
{
    [ContentProperty("Content")]
    public class Element3DPresenter : Element3D
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public Element3D Content
        {
            get { return (Element3D)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// The content property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(Element3D), typeof(Element3DPresenter), new PropertyMetadata(null, (d,e)=> 
            {
                var model = d as Element3DPresenter;               
                model.contentArray[0] = model.contentInternal = e.NewValue as Element3D;
                if(e.OldValue != null)
                {
                    (e.OldValue as Element3D).Detach();
                    model.RemoveLogicalChild(e.OldValue);
                }
                if(e.NewValue != null)
                {
                    model.AddLogicalChild(e.NewValue);
                    if (model.IsAttached)
                    {
                        (e.NewValue as Element3D).Attach(model.RenderHost);
                    }
                }
            }));

        private Element3D contentInternal;
        private readonly IList<IRenderable> contentArray = new IRenderable[1] { null };

        public override IList<IRenderable> Items
        {
            get
            {
                return contentInternal == null ? Constants.EmptyRenderable : contentArray;
            }
        }


        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            if (Content != null && Content is IHitable h)
            {
                if (h.IsHitTestVisible)
                {
                    return h.HitTest(context, ray, ref hits);
                }
            }
            return false;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                Content?.Attach(host);
                return true;
            }
            else { return false; }
        }

        protected override void OnDetach()
        {
            Content?.Detach();
            base.OnDetach();
        }

        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && contentInternal != null;
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            Content?.Render(context, deviceContext);
        }
    }
}
