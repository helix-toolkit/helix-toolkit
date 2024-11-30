using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// Provides the events for Viewport3DX.
/// </summary>
public partial class Viewport3DX
{
    public event EventHandler<MouseDown3DEventArgs>? OnMouse3DDown;

    public event EventHandler<MouseUp3DEventArgs>? OnMouse3DUp;

    public event EventHandler<MouseMove3DEventArgs>? OnMouse3DMove;

    /// <summary>
    /// Fired whenever an exception occurred at rendering subsystem.
    /// </summary>
    public event EventHandler<RelayExceptionEventArgs>? RenderExceptionOccurred;
}
