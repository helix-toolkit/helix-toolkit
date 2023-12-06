namespace HelixToolkit.Geometry;

/// <summary>
/// Describes the result of an intersection with a plane in three dimensions.
/// </summary>
public enum PlaneIntersectionType
{
    /// <summary>
    /// There is no intersection, the object is behind the plane.
    /// </summary>
    Back,

    /// <summary>
    /// There is no intersection, the object is in front of the plane.
    /// </summary>
    Front,

    /// <summary>
    /// The object is intersecting the plane.
    /// </summary>
    Intersecting
}
