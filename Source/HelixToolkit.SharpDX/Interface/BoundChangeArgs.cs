namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class BoundChangeArgs<T> : EventArgs where T : unmanaged
{
    /// <summary>
    /// Gets or sets the new bound.
    /// </summary>
    /// <value>
    /// The new bound.
    /// </value>
    public T NewBound
    {
        private set; get;
    }
    /// <summary>
    /// Gets or sets the old bound.
    /// </summary>
    /// <value>
    /// The old bound.
    /// </value>
    public T OldBound
    {
        private set; get;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundChangeArgs{T}"/> class.
    /// </summary>
    /// <param name="newBound">The new bound.</param>
    /// <param name="oldBound">The old bound.</param>
    public BoundChangeArgs(ref T newBound, ref T oldBound)
    {
        NewBound = newBound;
        OldBound = oldBound;
    }
}
