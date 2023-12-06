using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IDeviceResources : IDevice3DResources, IDevice2DResources, IDisposable
{
    /// <summary>
    /// Occurs when [on dispose resources].
    /// </summary>
    event EventHandler<EventArgs> DisposingResources;
    /// <summary>
    /// Occurs when [device created].
    /// </summary>
    event EventHandler<EventArgs> Reinitialized;
}
