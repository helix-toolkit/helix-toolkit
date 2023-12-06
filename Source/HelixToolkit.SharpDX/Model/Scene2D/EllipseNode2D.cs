using HelixToolkit.SharpDX.Core2D;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class EllipseNode2D : ShapeNode2D
{
    protected override ShapeRenderCore2DBase CreateShapeRenderCore()
    {
        return new EllipseRenderCore2D();
    }

    protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        hitResult = null;
        return false;
    }
}
