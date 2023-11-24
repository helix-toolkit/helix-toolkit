namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IHitable
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="hitContext">Contains all the hit test variables.</param>
    /// <param name="hits"></param>
    /// <returns>Return all hitted details with distance from nearest to farest.</returns>
    bool HitTest(HitTestContext hitContext, ref List<HitTestResult> hits);

    /// <summary>
    /// Indicates, if this element should be hit-tested.        
    /// default is true
    /// </summary>
    bool IsHitTestVisible
    {
        get; set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [always hittable].
    /// Set to true if you want object to be hit tested even it is not rendering.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [always hittable]; otherwise, <c>false</c>.
    /// </value>
    bool AlwaysHittable
    {
        set; get;
    }
}
