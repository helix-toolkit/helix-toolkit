namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public class HitTest2DResult
{
    /// <summary>
    /// Gets or sets the model hit.
    /// </summary>
    /// <value>
    /// The model hit.
    /// </value>
    public object? ModelHit
    {
        private set; get;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="HitTest2DResult"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public HitTest2DResult(object? model)
    {
        ModelHit = model;
    }
}
