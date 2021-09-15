/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Linq;

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
        public class OverlayNode2D : PanelNode2D
        {
            protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
            {
                hitResult = null;
                if (LayoutBoundWithTransform.Contains(mousePoint))
                {
                    foreach (var item in Items.Reverse())
                    {
                        if (item.HitTest(mousePoint, out hitResult))
                        { return true; }
                    }
                }
                return false;
            }

            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                foreach (var item in Items)
                {
                    if (item is SceneNode2D e)
                    {
                        e.Measure(availableSize);
                    }
                }
                return availableSize;
            }
        }
    }

}