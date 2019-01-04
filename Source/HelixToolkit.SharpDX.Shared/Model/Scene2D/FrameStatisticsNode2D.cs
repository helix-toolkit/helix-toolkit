/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using global::SharpDX.Direct2D1;
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
    namespace Model.Scene2D
    {
        using Core2D;
    

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

}
