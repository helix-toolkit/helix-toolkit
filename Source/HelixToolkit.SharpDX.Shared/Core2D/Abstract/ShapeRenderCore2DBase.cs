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
    public abstract class ShapeRenderCore2DBase : RenderCore2DBase
    {
        private D2D.Brush fillBrush = null;
        public D2D.Brush FillBrush
        {
            set
            {
                var old = fillBrush;
                if(SetAffectsRender(ref fillBrush, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
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
                var old = strokeBrush;
                if(SetAffectsRender(ref strokeBrush, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
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
                var old = strokeStyle;
                if(SetAffectsRender(ref strokeStyle, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return strokeStyle;
            }
        }
    }
}
