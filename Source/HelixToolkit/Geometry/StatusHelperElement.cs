namespace HelixToolkit.Geometry;

/// <summary>
/// Helper Class that is used in the calculation Process of the Diagonals.
/// </summary>
internal sealed class StatusHelperElement
{
    /// <summary>
    /// The Edge of the StatusHelperElement
    /// </summary>
    public PolygonEdge? Edge { get; }

    /// <summary>
    /// The Helper of the Edge is a Polygon Point
    /// </summary>
    public PolygonPoint? Helper { get; set; }

    /// <summary>
    /// Factor used for x-Value Calculation
    /// </summary>
    public float Factor { get; }

    /// <summary>
    /// Used to early-skip the Search for the right Status and Helper
    /// </summary>
    public float MinX { get; }

    /// <summary>
    /// Constructor taking an Edge and a Helper
    /// </summary>
    /// <param name="edge">The Edge of the StatusHelperElement</param>
    /// <param name="point">The Helper for the Edge of the StatusHelperElement</param>
    public StatusHelperElement(PolygonEdge? edge, PolygonPoint? point)
    {
        this.Edge = edge;
        this.Helper = point;

        if (edge?.PointOne is null || edge?.PointTwo is null)
        {
            this.Factor = 0;
            this.MinX = 0;
        }
        else
        {
            var vector = edge.PointTwo.Point - edge.PointOne.Point;
            this.Factor = vector.X / vector.Y;
            this.MinX = Math.Min(edge.PointOne.X, edge.PointTwo.X);
        }
    }
}
