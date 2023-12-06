namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IElementsBufferModel : IVertexExtraBufferModel
{
    /// <summary>
    /// Occurs when [on element changed].
    /// </summary>
    event EventHandler<EventArgs>? ElementChanged;
    /// <summary>
    /// Gets a value indicating whether this instance has elements.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has elements; otherwise, <c>false</c>.
    /// </value>
    bool HasElements
    {
        get;
    }
    /// <summary>
    /// Gets the element count.
    /// </summary>
    /// <value>
    /// The element count.
    /// </value>
    int ElementCount
    {
        get;
    }
    /// <summary>
    /// Disposes internal buffer and reuse the object
    /// </summary>
    void DisposeAndClear();
}
