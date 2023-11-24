namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public interface IDeviceContextPool : IDisposable
{
    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <returns></returns>
    DeviceContextProxy Get();

    /// <summary>
    /// Puts the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    void Put(DeviceContextProxy context);

    /// <summary>
    /// Resets the draw calls.
    /// </summary>
    int ResetDrawCalls();
}
