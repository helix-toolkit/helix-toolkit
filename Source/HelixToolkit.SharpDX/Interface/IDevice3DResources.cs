using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.ShaderManager;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IDevice3DResources
{
    /// <summary>
    /// 
    /// </summary>
    int AdapterIndex
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    Device? Device
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    DriverType DriverType
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    IStatePoolManager? StateManager
    {
        get;
    }
    /// <summary>
    /// Gets the geometry buffer manager.
    /// </summary>
    /// <value>
    /// The geometry buffer manager.
    /// </value>
    IGeometryBufferManager? GeometryBufferManager
    {
        get;
    }
    /// <summary>
    /// Gets the material texture manager.
    /// </summary>
    /// <value>
    /// The material texture manager.
    /// </value>
    ITextureResourceManager? MaterialTextureManager
    {
        get;
    }
    /// <summary>
    /// Gets the material variable manager.
    /// </summary>
    /// <value>
    /// The material variable manager.
    /// </value>
    IMaterialVariablePool? MaterialVariableManager
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    IConstantBufferPool? ConstantBufferPool
    {
        get;
    }
    /// <summary>
    /// Gets the device context pool.
    /// </summary>
    /// <value>
    /// The device context pool.
    /// </value>
    IDeviceContextPool? DeviceContextPool
    {
        get;
    }
}
