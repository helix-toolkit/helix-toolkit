/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
                Set(ref fillBrush, value);
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
                Set(ref strokeBrush, value);
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
                Set(ref strokeStyle, value);
            }
            get
            {
                return strokeStyle;
            }
        }
    }
}
