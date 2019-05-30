/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    using Core2D;
    using global::SharpDX.Direct2D1;

    public class FrameStatisticsNode2D : SceneNode2D
    {
        public Brush Foreground
        {
            set
            {
                (RenderCore as FrameStatisticsRenderCore).Foreground = value;
            }
            get
            {
                return (RenderCore as FrameStatisticsRenderCore).Foreground;
            }
        }

        public Brush Background
        {
            set
            {
                (RenderCore as FrameStatisticsRenderCore).Background = value;
            }
            get
            {
                return (RenderCore as FrameStatisticsRenderCore).Background;
            }
        }

        public FrameStatisticsNode2D()
        {
            HorizontalAlignment = HorizontalAlignment.Right;
            VerticalAlignment = VerticalAlignment.Top;
            EnableBitmapCache = false;
        }

        protected override RenderCore2D CreateRenderCore()
        {
            return new FrameStatisticsRenderCore();
        }

        protected override bool CanHitTest()
        {
            return false;
        }

        protected override bool OnHitTest(ref global::SharpDX.Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}
