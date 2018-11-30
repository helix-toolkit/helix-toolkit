/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGBOUNDS
using SharpDX;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using D2D = global::SharpDX.Direct2D1;

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
        public class PathRenderCore2D : ShapeRenderCore2DBase
        {
            /// <summary>
            /// The is geometry changed
            /// </summary>
            protected bool isGeometryChanged = true;
            /// <summary>
            /// The geometry
            /// </summary>
            protected D2D.PathGeometry1 geometry;

            private List<Figure> figures = new List<Figure>();
            /// <summary>
            /// Gets or sets the figures.
            /// </summary>
            /// <value>
            /// The figures.
            /// </value>
            public List<Figure> Figures
            {
                set
                {
                    if(SetAffectsRender(ref figures, value))
                    {
                        isGeometryChanged = true;
                    }
                }
                get
                {
                    return figures;
                }
            }

            private D2D.FillMode fillMode = D2D.FillMode.Alternate;
            /// <summary>
            /// Gets or sets the fill mode.
            /// </summary>
            /// <value>
            /// The fill mode.
            /// </value>
            public D2D.FillMode FillMode
            {
                set
                {
                    if(SetAffectsRender(ref fillMode, value))
                    {
                        isGeometryChanged = true;
                    }
                }
                get { return fillMode; }
            }
            /// <summary>
            /// Called when [attach].
            /// </summary>
            /// <param name="host">The host.</param>
            /// <returns></returns>
            protected override bool OnAttach(IRenderHost host)
            {
                isGeometryChanged = true;
                return base.OnAttach(host);
            }

            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="context">The context.</param>
            [SuppressMessage("Microsoft.Usage", "CA2202: Do not dispose objects multiple times", Justification = "False positive.")]
            protected override void OnRender(RenderContext2D context)
            {
                if (isGeometryChanged)
                {               
                    RemoveAndDispose(ref geometry);
                    if(Figures == null || Figures.Count == 0)
                    {
                        return;
                    }
                    geometry = Collect(new D2D.PathGeometry1(context.DeviceResources.Factory2D));
                    using (var sink = geometry.Open())
                    {
                        sink.SetFillMode(FillMode);
                        foreach(var figure in Figures)
                        {
                            figure.Create(sink);
                        }
                        sink.Close();
                    }
                    isGeometryChanged = false;
                }
                if (StrokeBrush != null && StrokeWidth > 0 && StrokeStyle != null)
                {
                    context.DeviceContext.DrawGeometry(geometry, StrokeBrush, StrokeWidth, StrokeStyle);
                }
                if(FillBrush != null)
                {
                    context.DeviceContext.FillGeometry(geometry, FillBrush);
                }
            }
        }
    }

}
