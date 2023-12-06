using SharpDX;

namespace HelixToolkit.SharpDX;

public interface IHitable2D
{
    bool HitTest(Vector2 mousePoint, out HitTest2DResult? hitResult);
    bool IsHitTestVisible
    {
        set; get;
    }
}
