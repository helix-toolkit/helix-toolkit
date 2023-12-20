using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class PresenterNode2D : SceneNode2D
{
    private SceneNode2D? content;

    public SceneNode2D? Content
    {
        set
        {
            if (content != value)
            {
                if (content != null)
                {
                    content.Detach();
                    content.Parent = null;
                    ItemsInternal.Clear();
                }
                content = value;
                if (content != null)
                {
                    content.Parent = this;
                    if (IsAttached)
                    {
                        content.Attach(RenderHost);
                    }

                    if (value is not null)
                    {
                        ItemsInternal.Add(value);
                    }
                }
                InvalidateMeasure();
            }
        }
        get
        {
            return content;
        }
    }

    public PresenterNode2D()
    {
        ItemsInternal = new ObservableFastList<SceneNode2D>();
        Items = new ReadOnlyObservableFastList<SceneNode2D>(ItemsInternal);
    }

    protected override bool OnAttach(IRenderHost host)
    {
        if (base.OnAttach(host))
        {
            content?.Attach(host);
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void OnDetach()
    {
        content?.Detach();
        base.OnDetach();
    }

    //protected override void OnRender(RenderContext2D context)
    //{
    //    base.OnRender(context);
    //    if (content != null)
    //    {
    //        content.Render(context);
    //    }
    //}

    protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        if (content != null)
        {
            return content.HitTest(mousePoint, out hitResult);
        }
        else
        {
            hitResult = null;
            return false;
        }
    }

    protected override Vector2 MeasureOverride(Vector2 availableSize)
    {
        if (content != null)
        {
            content.Measure(availableSize);
            return new Vector2(content.DesiredSize.X, content.DesiredSize.Y);
        }
        else
        {
            return new Vector2();
        }
    }

    protected override RectangleF ArrangeOverride(RectangleF finalSize)
    {
        if (content != null)
        {
            content.Arrange(finalSize);
            return new RectangleF(0, 0, content.DesiredSize.X, content.DesiredSize.Y);
        }
        else
        {
            return finalSize;
        }
    }
}
