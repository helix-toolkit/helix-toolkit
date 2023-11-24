using HelixToolkit.SharpDX;

namespace HelixToolkit.WinUI.SharpDX;

public abstract class Mouse3DEventArgs
{
    public HitTestResult HitTestResult { get; private set; }

    public Viewport3DX? Viewport { get; private set; }

    public Point Position { get; private set; }

    /// <summary>
    /// The original mouse/touch event that generated this one.
    /// 
    /// Useful for knowing what mouse button got pressed.
    /// </summary>
    public PointerRoutedEventArgs? OriginalInputEventArgs { get; private set; }

    public Mouse3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
    {
        this.HitTestResult = hitTestResult;
        this.Position = position;
        this.Viewport = viewport;
        this.OriginalInputEventArgs = originalInputEventArgs;
    }
}
