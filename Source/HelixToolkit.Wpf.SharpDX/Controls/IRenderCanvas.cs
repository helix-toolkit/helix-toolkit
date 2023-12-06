using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.Wpf.SharpDX.Controls;

/// <summary>
/// Canvas holds the RenderHost. Provide entry point or render surface for RenderHost to render to.
/// </summary>
public interface IRenderCanvas
{
    /// <summary>
    /// Gets or sets the dpi scale.
    /// </summary>
    /// <value>
    /// The dpi scale.
    /// </value>
    double DpiScale
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable dpi scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable dpi scale]; otherwise, <c>false</c>.
    /// </value>
    bool EnableDpiScale
    {
        set; get;
    }
    /// <summary>
    /// Gets the render host.
    /// </summary>
    /// <value>
    /// The render host.
    /// </value>
    IRenderHost? RenderHost
    {
        get;
    }

    /// <summary>
    /// Fired whenever an exception occurred on this object.
    /// </summary>
    event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
}
