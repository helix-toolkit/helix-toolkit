/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX.Direct2D1;
using global::SharpDX.DirectWrite;
using SharpDX;
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

        public class TextNode2D : SceneNode2D
        {
            private string text = "";
            public string Text
            {
                set
                {
                    if(SetAffectsMeasure(ref text, value))
                    {
                        (RenderCore as TextRenderCore2D).Text = value;
                    }
                }
                get
                {
                    return text;
                }
            }

            public Brush Foreground
            {
                set
                {
                    (RenderCore as TextRenderCore2D).Foreground = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).Foreground;
                }
            }

            public Brush Background
            {
                set
                {
                    (RenderCore as TextRenderCore2D).Background = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).Background;
                }
            }

            public int FontSize
            {
                set
                {
                    (RenderCore as TextRenderCore2D).FontSize = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).FontSize;
                }
            }

            public FontWeight FontWeight
            {
                set
                {
                    (RenderCore as TextRenderCore2D).FontWeight = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).FontWeight;
                }
            }

            public FontStyle FontStyle
            {
                set
                {
                    (RenderCore as TextRenderCore2D).FontStyle = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).FontStyle;
                }
            }

            public TextAlignment TextAlignment
            {
                set
                {
                    (RenderCore as TextRenderCore2D).TextAlignment = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).TextAlignment;
                }
            }

            public FlowDirection FlowDirection
            {
                set
                {
                    (RenderCore as TextRenderCore2D).FlowDirection = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).FlowDirection;
                }
            }

            public string FontFamily
            {
                set
                {
                    (RenderCore as TextRenderCore2D).FontFamily = value;
                }
                get
                {
                    return (RenderCore as TextRenderCore2D).FontFamily;
                }
            }

            private TextRenderCore2D textRenderable;

            protected override RenderCore2D CreateRenderCore()
            {
                textRenderable = new TextRenderCore2D();
                return textRenderable;
            }

            protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
            {
                hitResult = null;
                if (LayoutBoundWithTransform.Contains(mousePoint))
                {
                    hitResult = new HitTest2DResult(WrapperSource);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                textRenderable.MaxWidth = availableSize.Width;
                textRenderable.MaxHeight = availableSize.Height;
                var metrices = textRenderable.Metrices;
                return new Size2F(metrices.WidthIncludingTrailingWhitespace, metrices.Height);
            }

            protected override RectangleF ArrangeOverride(RectangleF finalSize)
            {
                textRenderable.MaxWidth = finalSize.Width;
                textRenderable.MaxHeight = finalSize.Height;
                var metrices = textRenderable.Metrices;
                return finalSize;
            }
        }
    }

}