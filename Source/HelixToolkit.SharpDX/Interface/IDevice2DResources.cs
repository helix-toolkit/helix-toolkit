namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IDevice2DResources
{
    /// <summary>
    /// Gets the factory2 d.
    /// </summary>
    /// <value>
    /// The factory2 d.
    /// </value>
    global::SharpDX.Direct2D1.Factory1? Factory2D
    {
        get;
    }
    /// <summary>
    /// Gets the device2d.
    /// </summary>
    /// <value>
    /// The device2d.
    /// </value>
    global::SharpDX.Direct2D1.Device? Device2D
    {
        get;
    }
    /// <summary>
    /// Gets the device context2d.
    /// </summary>
    /// <value>
    /// The device context2d.
    /// </value>
    global::SharpDX.Direct2D1.DeviceContext? DeviceContext2D
    {
        get;
    }
    /// <summary>
    /// Gets the wic img factory.
    /// </summary>
    /// <value>
    /// The wic img factory.
    /// </value>
    global::SharpDX.WIC.ImagingFactory? WICImgFactory
    {
        get;
    }
    /// <summary>
    /// Gets the direct write factory.
    /// </summary>
    /// <value>
    /// The direct write factory.
    /// </value>
    global::SharpDX.DirectWrite.Factory? DirectWriteFactory
    {
        get;
    }
}
