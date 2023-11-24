namespace HelixToolkit.SharpDX;

/// <summary>
/// A specialized line hit test result.
/// </summary>
public class LineHitTestResult : HitTestResult
{
    /// <summary>
    /// Gets or sets the index of the line segment that was hit.
    /// </summary>
    public int LineIndex { get; set; } = -1;

    /// <summary>
    /// Gets or sets the shortest distance between the hit test ray and the line that was hit.
    /// </summary>
    public double RayToLineDistance
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the scalar of the closest point on the hit test ray.
    /// </summary>
    public double RayHitPointScalar
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the scalar of the closest point on the line that was hit.
    /// </summary>
    public double LineHitPointScalar
    {
        get; set;
    }
}
