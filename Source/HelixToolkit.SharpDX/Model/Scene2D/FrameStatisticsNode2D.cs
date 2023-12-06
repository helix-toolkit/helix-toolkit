using HelixToolkit.SharpDX.Core2D;
using SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class FrameStatisticsNode2D : SceneNode2D
{
    public Brush? Foreground
    {
        set
        {
            if (RenderCore is FrameStatisticsRenderCore core)
            {
                core.Foreground = value;
            }
        }
        get
        {
            if (RenderCore is FrameStatisticsRenderCore core)
            {
                return core.Foreground;
            }

            return null;
        }
    }

    public Brush? Background
    {
        set
        {
            if (RenderCore is FrameStatisticsRenderCore core)
            {
                core.Background = value;
            }
        }
        get
        {
            if (RenderCore is FrameStatisticsRenderCore core)
            {
                return core.Background;
            }

            return null;
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

    protected override bool OnHitTest(ref global::SharpDX.Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        hitResult = null;
        return false;
    }
}
