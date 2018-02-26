using SharpDX;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    internal sealed class Overlay : Panel2D
    {
        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                foreach (var item in Items.Reverse())
                {
                    if (item is IHitable2D h)
                    {
                        if (h.HitTest(mousePoint, out hitResult))
                        { return true; }
                    }
                }
            }
            return false;
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            foreach (var item in Items)
            {
                if (item is Element2D e)
                {
                    e.Measure(availableSize);
                }
            }
            return availableSize;
        }
    }
}
