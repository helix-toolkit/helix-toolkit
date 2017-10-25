using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;
using D2D = global::SharpDX.Direct2D1;
using Media = System.Windows.Media;
using HelixToolkit.Wpf.SharpDX.Extensions;

namespace HelixToolkit.SharpDX.Core2D
{
    public abstract class ShapeRenderable2DBase : Renderable2DBase
    {       
        private Media.Brush stroke = null;
        public Media.Brush Stroke
        {
            set
            {
                if(stroke == value) { return; }
                stroke = value;
                Disposer.RemoveAndDispose(ref strokeD2DBrush);
                strokeBrushChanged = true;
            }
            get
            {
                return stroke;
            }
        }
        private bool strokeBrushChanged = true;
        private D2D.Brush strokeD2DBrush = null;
        protected D2D.Brush StrokeBrush { get { return strokeD2DBrush; } }

        private Media.Brush fill = null;
        public Media.Brush Fill
        {
            set
            {
                if (fill == value) { return; }
                fill = value;
                Disposer.RemoveAndDispose(ref fillD2DBrush);
                fillBrushChanged = true;
            }
            get
            {
                return fill;
            }
        }

        private bool fillBrushChanged = true;
        private D2D.Brush fillD2DBrush = null;
        protected D2D.Brush FillBrush { get { return fillD2DBrush; } }

        public int StrokeWidth
        {
            set; get;
        } = 1;

        protected D2D.StrokeStyle StrokeStyle { set; get; }

        protected override void OnTargetChanged(D2D.RenderTarget target)
        {
            Disposer.RemoveAndDispose(ref strokeD2DBrush);
            Disposer.RemoveAndDispose(ref fillD2DBrush);
            fillBrushChanged = strokeBrushChanged = true;
            base.OnTargetChanged(target);
        }

        protected override void OnRender(IRenderMatrices matrices)
        {
            if (fillBrushChanged)
            {
                fillD2DBrush = Fill.ToD2DBrush(RenderTarget);
                fillBrushChanged = false;
            }
            if (strokeBrushChanged)
            {
                strokeD2DBrush = Stroke.ToD2DBrush(RenderTarget);
                strokeBrushChanged = false;
            }
        }

        public override void Dispose()
        {
            Disposer.RemoveAndDispose(ref strokeD2DBrush);
            Disposer.RemoveAndDispose(ref fillD2DBrush);
            fillBrushChanged = strokeBrushChanged = true;
            base.Dispose();
        }
    }
}
