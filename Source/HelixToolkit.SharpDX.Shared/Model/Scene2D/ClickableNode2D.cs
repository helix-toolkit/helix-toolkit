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
    public class ClickableNode2D : BorderNode2D
    {
        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                if (!base.OnHitTest(ref mousePoint, out hitResult))
                {
                    hitResult = new HitTest2DResult(WrapperSource);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}