namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IElementsBufferModel<T> : IElementsBufferModel
{
    /// <summary>
    /// Gets or sets the elements.
    /// </summary>
    /// <value>
    /// The elements.
    /// </value>
    IList<T>? Elements
    {
        get; set;
    }
}
