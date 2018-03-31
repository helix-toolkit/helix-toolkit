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
    public class Node2DRectangle : Node2DShape
    {
        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new RectangleRenderCore2D();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                hitResult = new HitTest2DResult(HitTestSource);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
