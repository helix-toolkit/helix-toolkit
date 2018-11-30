/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;

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

}