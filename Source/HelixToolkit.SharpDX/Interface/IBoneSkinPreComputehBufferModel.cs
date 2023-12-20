using HelixToolkit.SharpDX.Render;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IBoneSkinPreComputehBufferModel
{
    bool CanPreCompute
    {
        get;
    }
    /// <summary>
    /// Binds the skinned vertex buffer to output.
    /// </summary>
    /// <param name="context">The context.</param>
    void BindSkinnedVertexBufferToOutput(DeviceContextProxy context);
    /// <summary>
    /// Uns the bind skinned vertex buffer to output.
    /// </summary>
    /// <param name="context">The context.</param>
    void UnBindSkinnedVertexBufferToOutput(DeviceContextProxy context);
    /// <summary>
    /// Resets the skinned vertex buffer.
    /// </summary>
    /// <param name="context">The context.</param>
    void ResetSkinnedVertexBuffer(DeviceContextProxy context);
    /// <summary>
    /// Copies the skinned to array.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="array">The array.</param>
    /// <returns></returns>
    int CopySkinnedToArray(DeviceContextProxy context, Vector3[] array);
}
