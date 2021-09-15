/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct2D1;
using System;

#if !NETFX_CORE && !WINUI
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene2D
    {
        using Core2D;

        public class BorderNode2D : ContentNode2D
        {
            public float CornerRadius
            {
                set { (RenderCore as BorderRenderCore2D).CornerRadius = value; }
                get { return (RenderCore as BorderRenderCore2D).CornerRadius; }
            }

            private Thickness padding = new Thickness(0);

            public Thickness Padding
            {
                set
                {
                    SetAffectsMeasure(ref padding, value);
                }
                get { return padding; }
            }

            public Brush BorderBrush
            {
                set { (RenderCore as BorderRenderCore2D).StrokeBrush = value; }
                get { return (RenderCore as BorderRenderCore2D).StrokeBrush; }
            }

            private CapStyle strokeDashCap = CapStyle.Flat;

            public CapStyle StrokeDashCap
            {
                set
                {
                    if (SetAffectsRender(ref strokeDashCap, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get { return strokeDashCap; }
            }

            private CapStyle strokeStartLineCap = CapStyle.Flat;

            public CapStyle StrokeStartLineCap
            {
                set
                {
                    if (SetAffectsRender(ref strokeStartLineCap, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get { return strokeStartLineCap; }
            }

            private CapStyle strokeEndLineCap = CapStyle.Flat;

            public CapStyle StrokeEndLineCap
            {
                set
                {
                    if (SetAffectsRender(ref strokeEndLineCap, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get { return strokeEndLineCap; }
            }

            private DashStyle strokeDashStyle = DashStyle.Solid;

            public DashStyle StrokeDashStyle
            {
                set
                {
                    if (SetAffectsRender(ref strokeDashStyle, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get { return strokeDashStyle; }
            }

            private float strokeDashOffset = 0;

            public float StrokeDashOffset
            {
                set
                {
                    if (SetAffectsRender(ref strokeDashOffset, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get { return strokeDashOffset; }
            }

            private LineJoin strokeLineJoin = LineJoin.Miter;

            public LineJoin StrokeLineJoin
            {
                set
                {
                    if (SetAffectsRender(ref strokeLineJoin, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get
                {
                    return strokeLineJoin;
                }
            }

            private float strokeMiterLimit = 1;

            public float StrokeMiterLimit
            {
                set
                {
                    if (SetAffectsRender(ref strokeMiterLimit, value))
                    {
                        strokeStyleChanged = true;
                    }
                }
                get { return strokeMiterLimit; }
            }

            private Thickness borderThickness;

            public Thickness BorderThickness
            {
                set
                {
                    if (SetAffectsMeasure(ref borderThickness, value))
                    {
                        (RenderCore as BorderRenderCore2D).BorderThickness = value;
                    }
                }
                get
                {
                    return borderThickness;
                }
            }

            private bool strokeStyleChanged = true;

            protected override bool OnAttach(IRenderHost host)
            {
                if (base.OnAttach(host))
                {
                    strokeStyleChanged = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void Update(RenderContext2D context)
            {
                base.Update(context);
                if (strokeStyleChanged)
                {
                    (RenderCore as BorderRenderCore2D).StrokeStyle = new StrokeStyle(context.DeviceResources.Factory2D,
                        new StrokeStyleProperties()
                        {
                            DashCap = StrokeDashCap,
                            StartCap = StrokeStartLineCap,
                            EndCap = StrokeEndLineCap,
                            DashOffset = StrokeDashOffset,
                            LineJoin = StrokeLineJoin,
                            MiterLimit = Math.Max(1, StrokeMiterLimit),
                            DashStyle = StrokeDashStyle
                        });
                    strokeStyleChanged = false;
                }
            }

            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                if (Content != null)
                {
                    var margin = new Size2F((BorderThickness.Left / 2 + Padding.Left + BorderThickness.Right / 2 + Padding.Right),
                        (BorderThickness.Top / 2 + Padding.Top + BorderThickness.Bottom / 2 + Padding.Bottom));
                    margin.Width *= DpiScale;
                    margin.Height *= DpiScale;
                    var childAvail = new Size2F(Math.Max(0, availableSize.Width - margin.Width), Math.Max(0, availableSize.Height - margin.Height));

                    var size = base.MeasureOverride(childAvail);
                    if (Width != float.PositiveInfinity && Height != float.PositiveInfinity)
                    {
                        return availableSize;
                    }
                    else
                    {
                        if (Width != float.PositiveInfinity)
                        {
                            size.Width = Width * DpiScale;
                        }
                        if (Height != float.PositiveInfinity)
                        {
                            size.Height = Height * DpiScale;
                        }
                        return size;
                    }
                }
                else
                {
                    var size = new Size2F((float)(BorderThickness.Left / 2 + Padding.Left + BorderThickness.Right / 2 + Padding.Right + MarginWidthHeight.X + Width == float.PositiveInfinity ? 0 : Width),
                        (float)(BorderThickness.Top / 2 + Padding.Top + BorderThickness.Bottom / 2 + Padding.Bottom + MarginWidthHeight.Y) + Height == float.PositiveInfinity ? 0 : Height);
                    size.Width *= DpiScale;
                    size.Height *= DpiScale;
                    return size;
                }
            }

            protected override RectangleF ArrangeOverride(RectangleF finalSize)
            {
                var contentRect = new RectangleF(finalSize.Left, finalSize.Top, finalSize.Width, finalSize.Height);
                contentRect.Left += (float)(BorderThickness.Left / 2 + Padding.Left) * DpiScale;
                contentRect.Right -= (float)(BorderThickness.Right / 2 + Padding.Right) * DpiScale;
                contentRect.Top += (float)(BorderThickness.Top / 2 + Padding.Top) * DpiScale;
                contentRect.Bottom -= (float)(BorderThickness.Bottom / 2 + Padding.Bottom) * DpiScale;
                base.ArrangeOverride(contentRect);
                return finalSize;
            }
        }
    }

}