using HelixToolkit.SharpDX.Core2D;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class TextNode2D : SceneNode2D
{
    private string text = string.Empty;
    public string Text
    {
        set
        {
            if (SetAffectsMeasure(ref text, value))
            {
                if (RenderCore is TextRenderCore2D core)
                {
                    core.Text = value;
                }
            }
        }
        get
        {
            return text;
        }
    }

    public Brush? Foreground
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.Foreground = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
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
            if (RenderCore is TextRenderCore2D core)
            {
                core.Background = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.Background;
            }

            return null;
        }
    }

    public int FontSize
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.FontSize = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.FontSize;
            }

            return 0;
        }
    }

    public FontWeight FontWeight
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.FontWeight = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.FontWeight;
            }

            return FontWeight.Normal;
        }
    }

    public FontStyle FontStyle
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.FontStyle = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.FontStyle;
            }

            return FontStyle.Normal;
        }
    }

    public TextAlignment TextAlignment
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.TextAlignment = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.TextAlignment;
            }

            return TextAlignment.Leading;
        }
    }

    public FlowDirection FlowDirection
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.FlowDirection = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.FlowDirection;
            }

            return FlowDirection.TopToBottom;
        }
    }

    public string FontFamily
    {
        set
        {
            if (RenderCore is TextRenderCore2D core)
            {
                core.FontFamily = value;
            }
        }
        get
        {
            if (RenderCore is TextRenderCore2D core)
            {
                return core.FontFamily;
            }

            return string.Empty;
        }
    }

    private TextRenderCore2D? textRenderable;

    protected override RenderCore2D CreateRenderCore()
    {
        textRenderable = new TextRenderCore2D();
        return textRenderable;
    }

    protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        hitResult = null;
        if (LayoutBoundWithTransform.Contains(mousePoint))
        {
            hitResult = new HitTest2DResult(WrapperSource);
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override Size2F MeasureOverride(Size2F availableSize)
    {
        if (textRenderable is null)
        {
            return Size2F.Zero;
        }

        textRenderable.MaxWidth = availableSize.Width;
        textRenderable.MaxHeight = availableSize.Height;
        var metrices = textRenderable.Metrices;
        return new Size2F(metrices.WidthIncludingTrailingWhitespace, metrices.Height);
    }

    protected override RectangleF ArrangeOverride(RectangleF finalSize)
    {
        if (textRenderable is null)
        {
            return RectangleF.Empty;
        }

        textRenderable.MaxWidth = finalSize.Width;
        textRenderable.MaxHeight = finalSize.Height;
        var metrices = textRenderable.Metrices;
        return finalSize;
    }
}
