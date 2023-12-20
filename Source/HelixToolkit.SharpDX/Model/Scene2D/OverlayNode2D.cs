using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class OverlayNode2D : PanelNode2D
{
    protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        hitResult = null;
        if (LayoutBoundWithTransform.Contains(mousePoint))
        {
            foreach (var item in Items.Reverse())
            {
                if (item.HitTest(mousePoint, out hitResult))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected override Vector2 MeasureOverride(Vector2 availableSize)
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
