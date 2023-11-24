using System.Windows.Media;

namespace HelixToolkit.Wpf;

/// <summary>
/// Represents a weak event manager for the CompositionTarget.Rendering event.
/// </summary>
public class RenderingEventManager : WeakEventManagerBase<RenderingEventManager>
{
    /// <summary>
    /// Start listening to the CompositionTarget.Rendering event.
    /// </summary>
    protected override void StartListening()
    {
#pragma warning disable CS8622
        CompositionTarget.Rendering += this.Handler;
#pragma warning restore CS8622
    }

    /// <summary>
    /// Stop listening to the CompositionTarget.Rendering event.
    /// </summary>
    protected override void StopListening()
    {
#pragma warning disable CS8622
        CompositionTarget.Rendering -= this.Handler;
#pragma warning restore CS8622
    }
}
