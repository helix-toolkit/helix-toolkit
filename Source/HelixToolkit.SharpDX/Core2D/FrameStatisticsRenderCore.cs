using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

/// <summary>
/// 
/// </summary>
public class FrameStatisticsRenderCore : RenderCore2DBase
{
    private IRenderStatistics? statistics;

    private D2D.Brush? foreground = null;
    /// <summary>
    /// Gets or sets the foreground.
    /// </summary>
    /// <value>
    /// The foreground.
    /// </value>
    public D2D.Brush? Foreground
    {
        set
        {
            var old = foreground;
            if (SetAffectsRender(ref foreground, value))
            {
                RemoveAndDispose(ref old);
            }
        }
        get
        {
            return foreground;
        }
    }

    private D2D.Brush? background = null;
    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>
    /// The background.
    /// </value>
    public D2D.Brush? Background
    {
        set
        {
            var old = background;
            if (SetAffectsRender(ref background, value))
            {
                RemoveAndDispose(ref old);
            }
        }
        get
        {
            return background;
        }
    }

    private TextLayout? textLayout;
    private Factory? factory;
    private TextFormat? format;
    private RectangleF renderBound = new(0, 0, 100, 0);
    private string previousStr = string.Empty;
    /// <summary>
    /// Called when [attach].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns></returns>
    protected override bool OnAttach(IRenderHost target)
    {
        factory = new Factory(FactoryType.Isolated);
        format = new TextFormat(factory, "Arial", 12 * target.DpiScale);
        previousStr = string.Empty;
        this.statistics = target.RenderStatistics;
        return base.OnAttach(target);
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref format);
        RemoveAndDispose(ref foreground);
        RemoveAndDispose(ref background);
        RemoveAndDispose(ref textLayout);
        RemoveAndDispose(ref factory);
        base.OnDetach();
    }
    /// <summary>
    /// Determines whether this instance can render the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>
    ///   <c>true</c> if this instance can render the specified context; otherwise, <c>false</c>.
    /// </returns>
    protected override bool CanRender(RenderContext2D context)
    {
        return base.CanRender(context) && statistics != null && statistics.FrameDetail != RenderDetail.None;
    }
    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="context">The context.</param>
    protected override void OnRender(RenderContext2D context)
    {
        if (background == null)
        {
            Background = new D2D.SolidColorBrush(context.DeviceContext, new RawColor4(0.8f, 0.8f, 0.8f, 0.6f));
        }
        if (foreground == null)
        {
            Foreground = new D2D.SolidColorBrush(context.DeviceContext, new RawColor4(0f, 0f, 1f, 1f));
        }
        var str = statistics?.GetDetailString() ?? string.Empty;
        if (str != previousStr || textLayout == null)
        {
            previousStr = str;
            RemoveAndDispose(ref textLayout);
            textLayout = new TextLayout(factory, str, format, float.MaxValue, float.MaxValue);
        }
        var metrices = textLayout.Metrics;
        renderBound.Width = Math.Max(metrices.Width, renderBound.Width);
        renderBound.Height = metrices.Height;
        context.DeviceContext.Transform = Matrix3x2Helper.Translation(new Vector2((float)context.ActualWidth - renderBound.Width, 0)).ToStruct<Matrix3x2, RawMatrix3x2>();
        context.DeviceContext.FillRectangle(renderBound.ToStruct<RectangleF, RawRectangleF>(), background);
        context.DeviceContext.DrawTextLayout(new RawVector2(), textLayout, foreground);
    }
}
