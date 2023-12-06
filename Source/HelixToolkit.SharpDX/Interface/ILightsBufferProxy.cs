using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILightsBufferProxy<T> where T : unmanaged
{
    /// <summary>
    /// Gets the size of the buffer.
    /// </summary>
    /// <value>
    /// The size of the buffer.
    /// </value>
    int BufferSize
    {
        get;
    }
    /// <summary>
    /// Gets the light array
    /// </summary>
    /// <value>
    /// The lights.
    /// </value>
    T[] Lights
    {
        get;
    }
    /// <summary>
    /// Gets or sets the ambient light.
    /// </summary>
    /// <value>
    /// The ambient light.
    /// </value>
    Color4 AmbientLight
    {
        set; get;
    }
    /// <summary>
    /// Gets the light count.
    /// </summary>
    /// <value>
    /// The light count.
    /// </value>
    int LightCount
    {
        get;
    }
    /// <summary>
    /// Resets the light count. Must call before calling light render
    /// </summary>
    void ResetLightCount();
    /// <summary>
    /// Increments the light count. Increment during each light render (except Ambient light).
    /// </summary>
    void IncrementLightCount();
    /// <summary>
    /// Upload light models to constant buffer.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="context">The context.</param>
    void UploadToBuffer(IBufferProxy buffer, DeviceContextProxy context);
}
