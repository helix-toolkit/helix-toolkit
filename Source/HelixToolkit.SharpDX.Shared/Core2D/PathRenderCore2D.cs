/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGBOUNDS
using SharpDX;
using System.Collections.Generic;
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public class PathRenderCore2D : ShapeRenderCore2DBase
    {     
        protected bool isGeometryChanged = true;
        protected D2D.PathGeometry1 geometry;

        private IList<Figure> figures = new List<Figure>();
        public IList<Figure> Figures
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

        protected override bool OnAttach(IRenderHost host)
        {
            isGeometryChanged = true;
            return base.OnAttach(host);
        }

        protected override void OnRender(IRenderContext2D context)
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
