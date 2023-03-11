/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
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
    namespace Core2D
    {
        public class TextRenderCore2D : RenderCore2DBase
        {
            private string text = string.Empty;
            public string Text
            {
                set
                {
                    if (SetAffectsRender(ref text, value))
                    {
                        textLayoutDirty = true;
                    }
                }
                get
                {
                    return text;
                }
            }

            private D2D.Brush foreground = null;
            public D2D.Brush Foreground
            {
                set
                {
                    var old = foreground;
                    if (SetAffectsRender(ref foreground, value))
                    {
                        RemoveAndDispose(ref old);
                    }
                }
                get
                {
                    return foreground;
                }
            }

            private D2D.Brush background = null;
            public D2D.Brush Background
            {
                set
                {
                    var old = background;
                    if (SetAffectsRender(ref background, value))
                    {
                        RemoveAndDispose(ref old);
                    }
                }
                get
                {
                    return background;
                }
            }

            private string fontFamily = "Arial";
            public string FontFamily
            {
                set
                {
                    if (SetAffectsRender(ref fontFamily, value) && IsAttached)
                    {
                        UpdateFontFormat();
                    }
                }
                get
                {
                    return fontFamily;
                }
            }

            private int fontSize = 12;
            public int FontSize
            {
                set
                {
                    if (SetAffectsRender(ref fontSize, value) && IsAttached)
                    {
                        UpdateFontFormat();
                    }
                }
                get
                {
                    return fontSize;
                }
            }

            private FontWeight fontWeight = FontWeight.Normal;
            public FontWeight FontWeight
            {
                set
                {
                    if (SetAffectsRender(ref fontWeight, value) && IsAttached)
                    {
                        UpdateFontFormat();
                    }
                }
                get
                {
                    return fontWeight;
                }
            }

            private FontStyle fontStyle = FontStyle.Normal;
            public FontStyle FontStyle
            {
                set
                {
                    if (SetAffectsRender(ref fontStyle, value) && IsAttached)
                    {
                        UpdateFontFormat();
                    }
                }
                get
                {
                    return fontStyle;
                }
            }

            public D2D.DrawTextOptions DrawingOptions { set; get; } = D2D.DrawTextOptions.None;

            private TextAlignment textAlignment = TextAlignment.Leading;
            public TextAlignment TextAlignment
            {
                set
                {
                    SetAffectsRender(ref textAlignment, value);
                }
                get
                {
                    return textAlignment;
                }
            }

            private FlowDirection flowDirection = FlowDirection.LeftToRight;
            public FlowDirection FlowDirection
            {
                set
                {
                    SetAffectsRender(ref flowDirection, value);
                }
                get
                {
                    return flowDirection;
                }
            }

            private Factory textFactory;
            private TextFormat textFormat;

            public TextMetrics Metrices
            {
                get
                {
                    UpdateTextLayout();
                    return textLayout.Metrics;
                }
            }

            private TextLayout textLayout;

            protected bool textLayoutDirty = true;

            private float maxWidth = 0;
            public float MaxWidth
            {
                set
                {
                    if (Set(ref maxWidth, value))
                    {
                        textLayoutDirty = true;
                    }
                }
                get
                {
                    return maxWidth;
                }
            }

            private float maxHeight = 0;
            public float MaxHeight
            {
                set
                {
                    if (Set(ref maxHeight, value))
                    {
                        textLayoutDirty = true;
                    }
                }
                get
                {
                    return maxHeight;
                }
            }

            protected override bool OnAttach(IRenderHost host)
            {
                if (base.OnAttach(host))
                {
                    textLayoutDirty = true;
                    textFactory = new Factory(FactoryType.Isolated);
                    textFormat = new TextFormat(textFactory, FontFamily, FontWeight, FontStyle, FontSize * host.DpiScale);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override void OnDetach()
            {
                RemoveAndDispose(ref textFormat);
                RemoveAndDispose(ref textLayout);
                RemoveAndDispose(ref foreground);
                RemoveAndDispose(ref background);
                RemoveAndDispose(ref textFactory);
                base.OnDetach();
            }

            private void UpdateFontFormat()
            {
                RemoveAndDispose(ref textFormat);
                textFormat = new TextFormat(textFactory, FontFamily, FontWeight, FontStyle, FontSize * RenderHost.DpiScale);
                textLayoutDirty = true;
            }

            private void UpdateTextLayout()
            {
                if (textLayoutDirty)
                {
                    RemoveAndDispose(ref textLayout);
                    textLayout = new TextLayout(textFactory, Text, textFormat, MaxWidth, MaxHeight);
                    textLayoutDirty = false;
                }
                textLayout.TextAlignment = TextAlignment;
            }

            protected override bool CanRender(RenderContext2D context)
            {
                return base.CanRender(context) && Foreground != null && Text != null;
            }

            protected override void OnRender(RenderContext2D context)
            {
                if (Background != null)
                {
                    context.DeviceContext.FillRectangle(LayoutBound, Background);
                }
                UpdateTextLayout();
                context.DeviceContext.DrawTextLayout(new Vector2(LayoutBound.Left, LayoutBound.Top), textLayout, Foreground, DrawingOptions);
            }
        }
    }
}
