namespace HelixToolkit.Wpf;

/// <summary>
/// Specifies the sorting method for the SortingVisual3D.
/// </summary>
public enum SortingMethod
{
    /// <summary>
    /// Sort on the distance from camera to bounding box center.
    /// </summary>
    BoundingBoxCenter,

    /// <summary>
    /// Sort on the minimum distance from camera to bounding box corners.
    /// </summary>
    BoundingBoxCorners,

    /// <summary>
    /// Sort on the minimum distance from camera to bounding sphere surface.
    /// </summary>
    BoundingSphereSurface
}
