#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Provides data for the manipulation events.
/// </summary>
public sealed class ManipulationEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManipulationEventArgs"/> class.
    /// </summary>
    /// <param name="currentPosition">
    /// The current position.
    /// </param>
    public ManipulationEventArgs(Point currentPosition)
    {
        this.CurrentPosition = currentPosition;
    }

    /// <summary>
    /// Gets the current position.
    /// </summary>
    /// <value>The current position.</value>
    public Point CurrentPosition { get; private set; }
}
