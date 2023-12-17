using HelixToolkit.SharpDX.Core2D;
using SharpDX;
using SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public abstract class ContentNode2D : PresenterNode2D
{
    private HorizontalAlignment horizontalContentAlignment = HorizontalAlignment.Center;

    public HorizontalAlignment HorizontalContentAlignment
    {
        set
        {
            if (Set(ref horizontalContentAlignment, value))
            {
                InvalidateMeasure();
            }
        }
        get
        {
            return horizontalContentAlignment;
        }
    }

    private VerticalAlignment verticalContentAlignment = VerticalAlignment.Center;

    public VerticalAlignment VerticalContentAlignment
    {
        set
        {
            if (Set(ref verticalContentAlignment, value))
            {
                InvalidateMeasure();
            }
        }
        get
        {
            return verticalContentAlignment;
        }
    }

    public Brush? Background
    {
        set
        {
            if (RenderCore is BorderRenderCore2D core)
            {
                core.Background = value;
            }
        }
        get
        {
            if (RenderCore is BorderRenderCore2D core)
            {
                return core.Background;
            }

            return null;
        }
    }

    protected override RenderCore2D CreateRenderCore()
    {
        return new BorderRenderCore2D();
    }

    protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        if (Content != null && LayoutBoundWithTransform.Contains(mousePoint))
        {
            return Content.HitTest(mousePoint, out hitResult);
        }
        else
        {
            hitResult = null;
            return false;
        }
    }

    protected override Vector2 MeasureOverride(Vector2 availableSize)
    {
        var maxContentSize = new Vector2();
        foreach (var item in Items)
        {
            if (item is SceneNode2D e)
            {
                e.HorizontalAlignment = HorizontalContentAlignment;
                e.VerticalAlignment = VerticalContentAlignment;
                e.Measure(availableSize);
                maxContentSize.X = Math.Max(maxContentSize.X, e.DesiredSize.X);
                maxContentSize.Y = Math.Max(maxContentSize.Y, e.DesiredSize.Y);
            }
        }
        if (HorizontalAlignment == HorizontalAlignment.Center)
        {
            availableSize.X = Math.Min(availableSize.X, maxContentSize.X);
        }
        else
        {
            if (float.IsInfinity(availableSize.X))
            {
                if (float.IsInfinity(Width))
                {
                    availableSize.X = maxContentSize.X;
                }
                else
                {
                    availableSize.X = Width;
                }
            }
        }

        if (VerticalAlignment == VerticalAlignment.Center)
        {
            availableSize.Y = Math.Min(availableSize.Y, maxContentSize.Y);
        }
        else
        {
            if (float.IsInfinity(availableSize.Y))
            {
                if (float.IsInfinity(Height))
                {
                    availableSize.Y = maxContentSize.Y;
                }
                else
                {
                    availableSize.Y = Height;
                }
            }
        }

        return availableSize;
    }
}
