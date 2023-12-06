using System.Numerics;

namespace HelixToolkit;

/// <summary>
/// Represents a 2D polygon.
/// </summary>
public sealed class Polygon
{
    // http://softsurfer.com/Archive/algorithm_0101/algorithm_0101.htm
    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    /// <value>The points.</value>
    public List<Vector2> Points { get; set; } = new List<Vector2>();

    /// <summary>
    /// Triangulate the polygon by using the sweep line algorithm
    /// </summary>
    /// <returns>An index collection.</returns>
    public List<int>? Triangulate()
    {
        return SweepLinePolygonTriangulator.Triangulate(Points);
    }
}
