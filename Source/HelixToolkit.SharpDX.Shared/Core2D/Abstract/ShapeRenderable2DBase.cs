/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public abstract class ShapeRenderable2DBase : Renderable2DBase
    {
        private D2D.Brush fillBrush = null;
        public D2D.Brush FillBrush
        {
            set
            {
                if (fillBrush == value) { return; }
                RemoveAndDispose(ref fillBrush);
                fillBrush = value;
                Collect(fillBrush);
            }
            get
            {
                return fillBrush;
            }
        }

        private D2D.Brush strokeBrush = null;
        public D2D.Brush StrokeBrush
        {
            set
            {
                if (strokeBrush == value) { return; }
                RemoveAndDispose(ref strokeBrush);
                strokeBrush = value;
                Collect(strokeBrush);
            }
            get
            {
                return strokeBrush;
            }
        }
        public int StrokeWidth
        {
            set; get;
        } = 1;

        private D2D.StrokeStyle strokeStyle = null;
        public D2D.StrokeStyle StrokeStyle
        {
            set
            {
                if(strokeStyle == value) { return; }
                RemoveAndDispose(ref strokeStyle);
                strokeStyle = value;
                Collect(strokeStyle);
            }
            get
            {
                return strokeStyle;
            }
        }
    }
}
