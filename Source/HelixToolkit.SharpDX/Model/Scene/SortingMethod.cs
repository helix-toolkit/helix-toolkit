namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// Used for geometry sorting
/// </summary>
public enum SortingMethod
{
    /// <summary>
    /// Sort on the distance from camera to bounding bound center.
    /// </summary>
    BoundingBoxCenter,

    /// <summary>
    /// Sort on the minimum distance from camera to bounding bound corners.
    /// </summary>
    BoundingBoxCorners,

    /// <summary>
    /// Sort on the minimum distance from camera to bounding sphere surface.
    /// </summary>
    BoundingSphereSurface
}
