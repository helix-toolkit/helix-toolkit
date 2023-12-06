using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Provides a hit test result.
/// </summary>
public class HitTestResult : IComparable<HitTestResult>
{

    /// <summary>
    /// Gets or sets the distance from the hit ray origin to the <see cref="PointHit"/>
    /// </summary>
    /// <value>
    /// The distance.
    /// </value>
    public double Distance
    {
        get; set;
    }

    /// <summary>
    /// Gets the Model3D intersected by the ray along which the hit test was performed.
    /// Model3D intersected by the ray.
    /// </summary>        
    public object? ModelHit
    {
        get; set;
    }

    /// <summary>
    /// Gets the Point at the intersection between the ray along which the hit
    /// test was performed and the hit object.
    /// Point at which the hit object was intersected by the ray.
    /// </summary>
    public Vector3 PointHit
    {
        get; set;
    }

    /// <summary>
    /// The normal vector of the triangle hit.
    /// </summary>
    public Vector3 NormalAtHit
    {
        get; set;
    }

    /// <summary>
    /// Indicates if this Result has data from a valid hit.
    /// </summary>
    public bool IsValid
    {
        get; set;
    }

    /// <summary>
    /// This is a tag to add additional data.
    /// </summary>
    public object? Tag
    {
        get; set;
    }
    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    /// <value>
    /// The geometry.
    /// </value>
    public Geometry3D? Geometry
    {
        set; get;
    }
    /// <summary>
    /// The hitted triangle vertex indices.
    /// </summary>
    public System.Tuple<int, int, int>? TriangleIndices
    {
        set; get;
    }

    public int CompareTo(HitTestResult? other)
    {
        if (other == null)
        {
            return 1;
        }
        else
        {
            return this.Distance.CompareTo(other.Distance);
        }
    }
    /// <summary>
    /// Shallow copy all the properties from another result.
    /// </summary>
    /// <param name="result">The result.</param>
    public void ShallowCopy(HitTestResult result)
    {
        Distance = result.Distance;
        ModelHit = result.ModelHit;
        PointHit = result.PointHit;
        NormalAtHit = result.NormalAtHit;
        IsValid = result.IsValid;
        Tag = result.Tag;
        Geometry = result.Geometry;
        TriangleIndices = result.TriangleIndices;
    }

    /// <summary>
    /// Get a descirption of the HitTestResult
    /// </summary>
    public override string ToString()
    {
        return $"{nameof(HitTestResult)} {nameof(ModelHit)}: {ModelHit}, {nameof(Distance)}: {Distance}, {nameof(IsValid)}: {IsValid}, {nameof(PointHit)}: {PointHit}, {nameof(NormalAtHit)}: {NormalAtHit}";
    }
}
