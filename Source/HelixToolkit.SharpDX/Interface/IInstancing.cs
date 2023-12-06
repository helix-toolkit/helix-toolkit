using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IInstancing
{
    /// <summary>
    /// Gets the instance buffer.
    /// </summary>
    /// <value>
    /// The instance buffer.
    /// </value>
    IElementsBufferModel<Matrix> InstanceBuffer
    {
        get;
    }
}
