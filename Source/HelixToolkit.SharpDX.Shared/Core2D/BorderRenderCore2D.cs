/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;
using System.Collections.Generic;
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
    namespace Core2D
    {

        /// <summary>
        /// 
        /// </summary>
        public class BorderRenderCore2D : RenderCore2DBase
        {
            private readonly PathRenderCore2D[] borderRenderCore = new PathRenderCore2D[4] { new PathRenderCore2D(), new PathRenderCore2D(), new PathRenderCore2D(), new PathRenderCore2D() };

            private bool isBorderGeometryChanged = false;

            private D2D.Brush background;
            /// <summary>
            /// Gets or sets the background.
            /// </summary>
            /// <value>
            /// The background.
            /// </value>
            public D2D.Brush Background
            {
                set
                {
                    var old = background;
                    if(SetAffectsRender(ref background, value))
                    {
                        RemoveAndDispose(ref old);
                        Collect(value);
                    }
                }
                get { return background; }
            }

            private D2D.Brush strokeBrush;
            /// <summary>
            /// Gets or sets the stroke brush.
            /// </summary>
            /// <value>
            /// The stroke brush.
            /// </value>
            public D2D.Brush StrokeBrush
            {
                set
                {
                    var old = strokeBrush;
                    if(SetAffectsRender(ref strokeBrush, value))
                    {
                        RemoveAndDispose(ref old);
                        Collect(value);
                        foreach(var core in borderRenderCore)
                        {
                            core.StrokeBrush = value;
                        }
                    }
                }
                get
                {
                    return strokeBrush;
                }
            }

            private Vector4 borderThickness = Vector4.Zero;
            /// <summary>
            /// Gets or sets the stroke thickness.
            /// </summary>
            /// <value>
            /// The stroke thickness.
            /// </value>
            public Vector4 BorderThickness
            {
                set { SetAffectsRender(ref borderThickness, value); }
                get { return borderThickness; }
            }

            private D2D.StrokeStyle strokeStyle;
            /// <summary>
            /// Gets or sets the stroke style.
            /// </summary>
            /// <value>
            /// The stroke style.
            /// </value>
            public D2D.StrokeStyle StrokeStyle
            {
                set
                {
                    var old = strokeStyle;
                    if(SetAffectsRender(ref strokeStyle, value))
                    {
                        RemoveAndDispose(ref old);
                        Collect(value);
                        foreach (var core in borderRenderCore)
                        {
                            core.StrokeStyle = value;
                        }
                    }
                }
                get
                {
                    return strokeStyle;
                }
            }

            private float cornerRadius = 0;
            /// <summary>
            /// Gets or sets the corner radius.
            /// </summary>
            /// <value>
            /// The corner radius.
            /// </value>
            public float CornerRadius
            {
                set
                {
                    if(SetAffectsRender(ref cornerRadius, value))
                    {
                        isBorderGeometryChanged = true;
                    }
                }
                get { return cornerRadius; }
            }

            protected override bool OnAttach(IRenderHost host)
            {
                if (base.OnAttach(host))
                {
                    isBorderGeometryChanged = true;
                    foreach(var core in borderRenderCore)
                    {
                        core.Attach(host);
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
                foreach (var core in borderRenderCore)
                {
                    core.Detach();
                }
                base.OnDetach();
            }

            protected override void OnLayoutBoundChanged(RectangleF layoutBound)
            {
                base.OnLayoutBoundChanged(layoutBound);
                isBorderGeometryChanged = true;
            }

            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="context">The context.</param>
            protected override void OnRender(RenderContext2D context)
            {
                var roundRect = new D2D.RoundedRectangle() { Rect = LayoutBound, RadiusX = CornerRadius, RadiusY = CornerRadius };
                if (Background != null)
                {
                    context.DeviceContext.FillRoundedRectangle(roundRect, Background);
                }

                if (!borderThickness.IsZero && StrokeBrush != null && StrokeStyle != null)
                {
                    if(borderThickness.X == borderThickness.Y && borderThickness.X == borderThickness.Z && borderThickness.X == borderThickness.W)
                    {
                        context.DeviceContext.DrawRoundedRectangle(roundRect, StrokeBrush, borderThickness.X, StrokeStyle);
                    }
                    else
                    {
                        if (isBorderGeometryChanged)
                        {
                    
                            var topLeft = LayoutBound.TopLeft + new Vector2(0, CornerRadius);
                            var topRight = LayoutBound.TopRight - new Vector2(CornerRadius, 0);
                            var bottomRight = LayoutBound.BottomRight - new Vector2(0, CornerRadius);
                            var bottomLeft = LayoutBound.BottomLeft + new Vector2(CornerRadius, 0);

                            if (borderThickness.X > 0)
                            {
                                var figures = new List<Figure>();     
                                var figure = new Figure(topLeft, false, false);
                                if (CornerRadius > 0)
                                {
                                    figure.AddSegment(new ArcSegment(LayoutBound.TopLeft + new Vector2(CornerRadius, 0), new Size2F(CornerRadius, CornerRadius), 0, 
                                        D2D.SweepDirection.Clockwise, D2D.ArcSize.Small));
                                }
                                figure.AddSegment(new LineSegment(topRight));
                                figures.Add(figure);
                                borderRenderCore[0].Figures = figures;
                                borderRenderCore[0].StrokeWidth = borderThickness.X;
                            }
                            else
                            {
                                borderRenderCore[0].Figures = null;
                            }
                            if(borderThickness.Y > 0)
                            {
                                var figures = new List<Figure>();
                                var figure = new Figure(topRight, false, false);
                                if(CornerRadius > 0)
                                {
                                    figure.AddSegment(new ArcSegment(LayoutBound.TopRight + new Vector2(0, CornerRadius) , new Size2F(CornerRadius, CornerRadius), 0,
                                        D2D.SweepDirection.Clockwise, D2D.ArcSize.Small));
                                }
                                figure.AddSegment(new LineSegment(bottomRight));
                                figures.Add(figure);
                                borderRenderCore[1].Figures = figures;
                                borderRenderCore[1].StrokeWidth = borderThickness.Y;
                            }
                            else
                            {
                                borderRenderCore[1].Figures = null;
                            }
                            if (borderThickness.Z > 0)
                            {
                                var figures = new List<Figure>();
                                var figure = new Figure(bottomRight, false, false);
                                if (CornerRadius > 0)
                                {
                                    figure.AddSegment(new ArcSegment(LayoutBound.BottomRight - new Vector2(CornerRadius, 0), new Size2F(CornerRadius, CornerRadius), 0,
                                        D2D.SweepDirection.Clockwise, D2D.ArcSize.Small));
                                }
                                figure.AddSegment(new LineSegment(bottomLeft));
                                figures.Add(figure);
                                borderRenderCore[2].Figures = figures;
                                borderRenderCore[2].StrokeWidth = borderThickness.Z;
                            }
                            else
                            {
                                borderRenderCore[2].Figures = null;
                            }
                            if (borderThickness.W > 0)
                            {
                                var figures = new List<Figure>();
                                var figure = new Figure(bottomLeft, false, false);
                                if (CornerRadius > 0)
                                {
                                    figure.AddSegment(new ArcSegment(LayoutBound.BottomLeft - new Vector2(0, CornerRadius), new Size2F(CornerRadius, CornerRadius), 0,
                                        D2D.SweepDirection.Clockwise, D2D.ArcSize.Small));
                                }
                                figure.AddSegment(new LineSegment(topLeft));
                                figures.Add(figure);
                                borderRenderCore[3].Figures = figures;
                                borderRenderCore[3].StrokeWidth = borderThickness.W;
                            }
                            else
                            {
                                borderRenderCore[3].Figures = null;
                            }
                            isBorderGeometryChanged = false;
                        }
                        foreach(var core in borderRenderCore)
                        {
                            core.Transform = this.Transform;
                            core.LocalTransform = this.LocalTransform;
                            core.Render(context);
                        }
                    }
                }
            }
        }
    }

}
