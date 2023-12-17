using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class StackPanelNode2D : PanelNode2D
{
    private Orientation orientation = Orientation.Horizontal;

    public Orientation Orientation
    {
        set
        {
            SetAffectsMeasure(ref orientation, value);
        }
        get
        {
            return orientation;
        }
    }

    public StackPanelNode2D()
    {
        EnableBitmapCache = true;
    }

    protected override Vector2 MeasureOverride(Vector2 availableSize)
    {
        var constraint = availableSize;

        var size = new Vector2();
        switch (Orientation)
        {
            case Orientation.Horizontal:
                availableSize.X = float.PositiveInfinity;
                break;

            case Orientation.Vertical:
                availableSize.Y = float.PositiveInfinity;
                break;
        }

        foreach (var child in Items)
        {
            if (child is SceneNode2D c)
            {
                child.Measure(availableSize);
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        size.X += c.DesiredSize.X;
                        size.Y = Math.Max(size.Y, c.DesiredSize.Y);
                        break;

                    case Orientation.Vertical:
                        size.X = Math.Max(c.DesiredSize.X, size.X);
                        size.Y += c.DesiredSize.Y;
                        break;
                }
            }
        }

        return size;
    }

    protected override RectangleF ArrangeOverride(RectangleF finalSize)
    {
        float lastSize = 0;
        var totalSize = finalSize;
        foreach (var child in Items)
        {
            if (child is SceneNode2D c)
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        totalSize.Left += lastSize;
                        lastSize = c.DesiredSize.X;
                        totalSize.Right = totalSize.Left + lastSize;
                        //totalSize.Bottom = totalSize.Top + Math.Min(finalSize.Height, c.DesiredSize.Y);
                        break;

                    case Orientation.Vertical:
                        totalSize.Top += lastSize;
                        lastSize = c.DesiredSize.Y;
                        //totalSize.Right = totalSize.Left + Math.Min(finalSize.Width, c.DesiredSize.X);
                        totalSize.Bottom = totalSize.Top + lastSize;
                        break;
                }
                c.Arrange(totalSize);
            }
        }

        return finalSize;
    }
}
