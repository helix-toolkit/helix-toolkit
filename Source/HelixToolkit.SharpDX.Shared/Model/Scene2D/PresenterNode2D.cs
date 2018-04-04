/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    public class PresenterNode2D : SceneNode2D
    {
        private SceneNode2D content;

        public SceneNode2D Content
        {
            set
            {
                if (content != value)
                {
                    if (content != null)
                    {
                        content.Detach();
                        content.Parent = null;
                    }
                    content = value;
                    contentArray[0] = value;
                    if (content != null)
                    {
                        content.Parent = this;
                        if (IsAttached)
                        {
                            content.Attach(RenderHost);
                        }
                    }
                    InvalidateMeasure();
                }
            }
            get
            {
                return content;
            }
        }

        private SceneNode2D[] contentArray = new SceneNode2D[1];

        public override IList<SceneNode2D> Items
        {
            get
            {
                return content == null ? Constants.EmptyRenderable2D : contentArray;
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                content?.Attach(host);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            content?.Detach();
            base.OnDetach();
        }

        //protected override void OnRender(IRenderContext2D context)
        //{
        //    base.OnRender(context);
        //    if (content != null)
        //    {
        //        content.Render(context);
        //    }
        //}

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            if (content != null)
            {
                return content.HitTest(mousePoint, out hitResult);
            }
            else
            {
                hitResult = null;
                return false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if (content != null)
            {
                content.Measure(availableSize);
                return new Size2F(content.DesiredSize.X, content.DesiredSize.Y);
            }
            else
            {
                return new Size2F();
            }
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            if (content != null)
            {
                content.Arrange(finalSize);
                return new RectangleF(0, 0, content.DesiredSize.X, content.DesiredSize.Y);
            }
            else
            {
                return finalSize;
            }
        }
    }
}