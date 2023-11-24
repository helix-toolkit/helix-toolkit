using System.Numerics;

namespace HelixToolkit.Geometry;

/// <summary>
/// Helper Class for the Polygon-Triangulation.
/// </summary>
internal sealed class PolygonData
{
    /// <summary>
    /// The List of Polygonpoints that define this Polygon
    /// </summary>
    public List<PolygonPoint> Points { get; }

    /// <summary>
    /// Are there Holes present
    /// </summary>
    public bool HasHoles => Holes.Count > 0;

    /// <summary>
    /// The Holes of the Polygon
    /// </summary>
    public List<List<PolygonPoint>> Holes { get; }

    /// <summary>
    /// Constructor that uses a List of Points and an optional List of Point-Indices
    /// </summary>
    /// <param name="points">The Polygon-Defining Points</param>
    /// <param name="indices">Optional List of Point-Indices</param>
    public PolygonData(List<Vector2> points, List<int>? indices = null)
    {
        // Initialize
        Points = new List<PolygonPoint>(points.Select(p => new PolygonPoint(p)));
        Holes = new List<List<PolygonPoint>>();

        // If no Indices were specified, add them manually
        if (indices == null)
        {
            for (var i = 0; i < Points.Count; i++)
            {
                Points[i].Index = i;
            }
        }
        // If there were Indices specified, use them to set the PolygonPoint's Index Property
        else
        {
            for (var i = 0; i < Points.Count; i++)
            {
                Points[i].Index = indices[i];
            }
        }

        // Add Edges between the Points (to be able to navigate along the Polygon easily later)
        var cnt = Points.Count;
        for (var i = 0; i < cnt; i++)
        {
            var lastIdx = (i + cnt - 1) % cnt;
            var edge = new PolygonEdge(Points[lastIdx], Points[i]);
            Points[lastIdx].EdgeTwo = edge;
            Points[i].EdgeOne = edge;
        }
    }

    /// <summary>
    /// Constructor that takes a List of PolygonPoints
    /// Calls the first Constructor by splitting the Input-Information (Points and Indices)
    /// </summary>
    /// <param name="points">The PolygonPoints</param>
    public PolygonData(List<PolygonPoint> points)
        : this(points.Select(p => p.Point).ToList(), points.Select(p => p.Index).ToList())
    {
    }

    /// <summary>
    /// Add Points of a Hole to the PolygonData
    /// </summary>
    /// <param name="points">The Points that define the Hole in the Polygon</param>
    internal void AddHole(List<Vector2> points)
    {
        // Make Hole Clockwise
        if (SweepLinePolygonTriangulator.IsCCW(points))
        {
            points.Reverse();
        }
        // The Hole Points
        var polyPoints = points.Select(p => new PolygonPoint(p)).ToList();
        // If Endpoint equals Startpoint
        if (polyPoints[0].Equals(polyPoints[polyPoints.Count - 1]))
            polyPoints.RemoveAt(polyPoints.Count - 1);
        Holes.Add(polyPoints);

        var cntBefore = Points.Count;
        var pointCount = points.Count;
        // Add the PolygonPoints for this Polygon Object
        Points.AddRange(polyPoints);

        // Add the Indices
        for (var i = cntBefore; i < Points.Count; i++)
        {
            polyPoints[i - cntBefore].Index = i;
        }

        // Add Edges between the Points (to be able to navigate along the Polygon easily later)
        var cnt = Points.Count;
        for (var i = 0; i < pointCount; i++)
        {
            var lastIdx = (i + pointCount - 1) % pointCount;
            var edge = new PolygonEdge(polyPoints[lastIdx], polyPoints[i]);
            polyPoints[lastIdx].EdgeTwo = edge;
            polyPoints[i].EdgeOne = edge;
        }
    }
}
