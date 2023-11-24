using System.Windows.Media;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides a weak event listener that pass the CompositionTarget.Rendering event to the specified handler.
/// </summary>
public class RenderingEventListener : WeakEventListener<RenderingEventManager, RenderingEventArgs>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderingEventListener"/> class.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public RenderingEventListener(EventHandler<RenderingEventArgs?> handler)
        : base(handler)
    {
    }
}
