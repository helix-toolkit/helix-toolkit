using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    internal sealed class Overlay : ContentElement2D
    {
        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                foreach (var item in Items.Reverse())
                {
                    if (item is IHitable2D && (item as IHitable2D).HitTest(mousePoint, out hitResult))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
