/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct2D1;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene2D
    {
        using Core2D;

        public abstract class ContentNode2D : PresenterNode2D
        {
            private HorizontalAlignment horizontalContentAlignment = HorizontalAlignment.Center;

            public HorizontalAlignment HorizontalContentAlignment
            {
                set
                {
                    if (Set(ref horizontalContentAlignment, value))
                    {
                        InvalidateMeasure();
                    }
                }
                get { return horizontalContentAlignment; }
            }

            private VerticalAlignment verticalContentAlignment = VerticalAlignment.Center;

            public VerticalAlignment VerticalContentAlignment
            {
                set
                {
                    if (Set(ref verticalContentAlignment, value))
                    {
                        InvalidateMeasure();
                    }
                }
                get { return verticalContentAlignment; }
            }

            public Brush Background
            {
                set { (RenderCore as BorderRenderCore2D).Background = value; }
                get { return (RenderCore as BorderRenderCore2D).Background; }
            }

            protected override RenderCore2D CreateRenderCore()
            {
                return new BorderRenderCore2D();
            }

            protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
            {
                if (Content != null && LayoutBoundWithTransform.Contains(mousePoint))
                {
                    return Content.HitTest(mousePoint, out hitResult);
                }
                else
                {
                    hitResult = null;
                    return false;
                }
            }

            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                Size2F maxContentSize = new Size2F();
                foreach (var item in Items)
                {
                    if (item is SceneNode2D e)
                    {
                        e.HorizontalAlignment = HorizontalContentAlignment;
                        e.VerticalAlignment = VerticalContentAlignment;
                        e.Measure(availableSize);
                        maxContentSize.Width = Math.Max(maxContentSize.Width, e.DesiredSize.X);
                        maxContentSize.Height = Math.Max(maxContentSize.Height, e.DesiredSize.Y);
                    }
                }
                if (HorizontalAlignment == HorizontalAlignment.Center)
                {
                    availableSize.Width = Math.Min(availableSize.Width, maxContentSize.Width);
                }
                else
                {
                    if (float.IsInfinity(availableSize.Width))
                    {
                        if (float.IsInfinity(Width))
                        {
                            availableSize.Width = maxContentSize.Width;
                        }
                        else
                        {
                            availableSize.Width = Width;
                        }
                    }
                }

                if (VerticalAlignment == VerticalAlignment.Center)
                {
                    availableSize.Height = Math.Min(availableSize.Height, maxContentSize.Height);
                }
                else
                {
                    if (float.IsInfinity(availableSize.Height))
                    {
                        if (float.IsInfinity(Height))
                        {
                            availableSize.Height = maxContentSize.Height;
                        }
                        else
                        {
                            availableSize.Height = Height;
                        }
                    }
                }

                return availableSize;
            }
        }
    }

}