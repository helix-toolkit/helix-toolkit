using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides an abstract base class for ModelVisual3D objects that listens to the CompositionTarget.Rendering event.
/// </summary>
/// <remarks>
/// The RenderingEventManager is used to ensure a weak reference to CompositionTargetRendering.
/// </remarks>
public abstract class RenderingModelVisual3D : ModelVisual3D
{
    /// <summary>
    /// The rendering event listener
    /// </summary>
    private readonly RenderingEventListener renderingEventListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderingModelVisual3D"/> class.
    /// </summary>
    protected RenderingModelVisual3D()
    {
        this.renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);
    }

    /// <summary>
    /// Subscribes to CompositionTarget.Rendering event.
    /// </summary>
    protected void SubscribeToRenderingEvent()
    {
        RenderingEventManager.AddListener(this.renderingEventListener);
    }

    /// <summary>
    /// Unsubscribes the CompositionTarget.Rendering event.
    /// </summary>
    protected void UnsubscribeRenderingEvent()
    {
        RenderingEventManager.RemoveListener(this.renderingEventListener);
    }

    /// <summary>
    /// Handles the CompositionTarget.Rendering event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The <see cref="System.Windows.Media.RenderingEventArgs"/> instance containing the event data.</param>
    protected abstract void OnCompositionTargetRendering(object? sender, RenderingEventArgs? eventArgs);
}
