using HelixToolkit.Geometry;
using System.Numerics;

namespace HelixToolkit;

/// <summary>
/// Triangulate a simple Polygon with the Sweep-Line Algorithm
/// </summary>
/// <remarks>
/// Based on http://www.cs.uu.nl/docs/vakken/ga/slides3.pdf
/// References
/// https://www.cs.ucsb.edu/~suri/cs235/Triangulation.pdf
/// </remarks>
public static class SweepLinePolygonTriangulator
{
    /// <summary>
    /// Range Extension when searching for the Helper and Edge
    /// </summary>
    public static float Epsilon = 0.0000001f;

    /// <summary>
    /// Perform the Triangulation of the Input.
    /// </summary>
    /// <param name="polygon">The Input Polygon</param>
    /// <param name="holes">The Input Polygon</param>
    /// <returns>List of Indices representing the Triangulation of the Polygon</returns>
    public static List<int>? Triangulate(IList<Vector2> polygon, List<List<Vector2>>? holes = null)
    {
        // Allocate and initialize List of Indices in Polygon
        var result = new List<int>();

        // Point-List from Input
        // (we don't want the first and last Point to be present two times)
        var points = polygon.ToList();
        if (points[0] == points[^1])
        {
            points.RemoveAt(points.Count - 1);
        }
        var count = points.Count;

        // Sort the Input and create the Datastructures
        // Make the Polygon CounterClockWise
        var didReverse = false;
        if (!IsCCW(polygon))
        {
            points.Reverse();
            didReverse = true;
        }

        // Skip Polygons that don't need Triangulation
        if (count < 3)
        {
            return null;
        }
        else if (count == 3)
        {
            if (!didReverse)
            {
                return new List<int> { 0, 1, 2 };
            }
            else
            {
                return new List<int> { 0, 2, 1 };
            }
        }

        var poly = new PolygonData(points);

        if (holes != null)
        {
            foreach (var hole in holes)
            {
                poly.AddHole(hole);
            }
        }
        // Sort Points from highest y to lowest y
        // and if two or more Points have the same y Value from lowest x to highest x Value
        var events = new List<PolygonPoint>(poly.Points);
        events.Sort();

        // Calculate the Diagonals in the Down Sweep
        var diagonals = CalculateDiagonals(events);
        // Reverse the Order of the Events
        events.Reverse();
        // Add the Diagonals in the Up Sweep (and remove duplicates)
        diagonals.AddRange(CalculateDiagonals(events, false));
        diagonals = diagonals.Distinct().ToList();

        // Use Diagonals to split into nonotone Polygons
        var monotonePolygons = SplitIntoPolygons(poly, diagonals);

        // y-Monotone Polygons
        // Triangulate
        foreach (var monoton in monotonePolygons.Where(m => m != null))
        {
            var indices = TriangulateMonotone(monoton);
            foreach (var index in indices)
            {
                result.Add(index);
            }
        }

        // If we reversed the Polygon,
        // we need to reverse the result also to get a correct Triangulation
        if (didReverse)
        {
            // Transform back every calculated Index
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = count - result[i] - 1;
            }
        }

        // Return all calculated Triangleindices
        return result;
    }

    /// <summary>
    /// Triangulate the y-Monotone Polygons.
    /// </summary>
    /// <param name="monoton">The y-Monotone Polygon to triangle</param>
    /// <returns>Index-List of Polygon Points (Indices from the original Polygon)</returns>
    private static List<int> TriangulateMonotone(PolygonData monoton)
    {
        // Collection to return
        var result = new List<int>();

        // Sort the Events
        var events = new List<PolygonPoint>(monoton.Points);
        events.Sort();

        // Stack of Events to push to and pop from
        var pointStack = new Stack<PolygonPoint>();

        // Push the first two Events
        pointStack.Push(events[0]);
        pointStack.Push(events[1]);

        // Left- and right Chain for Triangulation
        var left = (events[0].Next == events[1]) ? events[1] : events[0];
        var right = (events[0].Last == events[1]) ? events[1] : events[0];

        // Count of Points
        var pointCnt = monoton.Points.Count;

        // Handle the 3rd...n-th Point to triangle
        for (var i = 2; i < pointCnt; i++)
        {
            // The current Point
            var newPoint = events[i];
            var top = pointStack.Peek();
            // If the new Point is not on the same side as the last Point on the Stack
            //if (!(leftChain.Contains(top) && leftChain.Contains(newPoint) || rightChain.Contains(top) && rightChain.Contains(newPoint)))
            if (!(top.Last == newPoint || top.Next == newPoint))
            {
                // Determine this Point's Chain (left or right)
                if (left.Next == newPoint)
                {
                    left = newPoint;
                }
                else if (right.Last == newPoint)
                {
                    right = newPoint;
                }

                // Third triangle Point
                var p2 = top;
                // While there is a Point on the Stack
                while (pointStack.Count != 0)
                {
                    // Pop and set the third Point
                    top = pointStack.Pop();
                    p2 = top;
                    if (pointStack.Count != 0)
                    {
                        // Pop again
                        top = pointStack.Pop();

                        // Add to the result. The Order is depending on the Side
                        if (left == newPoint)
                        {
                            result.Add(newPoint.Index);
                            result.Add(p2.Index);
                            result.Add(top.Index);
                        }
                        else
                        {
                            result.Add(newPoint.Index);
                            result.Add(top.Index);
                            result.Add(p2.Index);
                        }
                    }
                    // If more Points are on the Stack,
                    // Push the Point back again, to be able to form the Triangles
                    if (pointStack.Count != 0)
                        pointStack.Push(top);
                }
                // Push the last to Points on the Stack
                pointStack.Push(events[i - 1]);
                pointStack.Push(newPoint);
            }
            // If the newPoint is on the same Side (i.e. Chain)
            else
            {
                // Get to Point on the Stack
                top = pointStack.Pop();
                var p2 = top;

                // Determine this Point's Chain (left or right)
                if (left.Next == newPoint && right.Last == newPoint)
                {
                    if (top.Last == newPoint)
                        right = newPoint;
                    else if (top.Next == newPoint)
                        left = newPoint;
                    else
                        throw new Exception("Triangulation error");
                }
                else if (left.Next == newPoint)
                {
                    left = newPoint;
                }
                else if (right.Last == newPoint)
                {
                    right = newPoint;
                }

                while (pointStack.Count != 0)
                {
                    // If the Triangle is possible, add it to the result (Point Order depends on the Side)
                    if (right == newPoint && IsCCW(new List<Vector2> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                    {
                        top = pointStack.Pop();
                        result.Add(newPoint.Index);
                        result.Add(p2.Index);
                        result.Add(top.Index);
                        p2 = top;
                    }
                    else if (left == newPoint && !IsCCW(new List<Vector2> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                    {
                        top = pointStack.Pop();
                        result.Add(newPoint.Index);
                        result.Add(top.Index);
                        result.Add(p2.Index);
                        p2 = top;
                    }
                    // No Triangle possible, just leave the Loop
                    else
                        break;
                }
                // Push the last two Points on the Stack
                pointStack.Push(p2);
                pointStack.Push(newPoint);
            }
        }
        // Return the Triangulation
        return result;
    }

    /// <summary>
    /// Calculate the Diagonals to add inside the Polygon.
    /// </summary>
    /// <param name="events">The Events in sorted Form</param>
    /// <param name="sweepDown">True in the first Stage (sweeping down), false in the following Stages (sweeping up)</param>
    /// <returns></returns>
    private static List<Tuple<int, int>> CalculateDiagonals(List<PolygonPoint> events, bool sweepDown = true)
    {
        // Diagonals to add to the Polygon to make it monotone after the Down- and Up-Sweeps
        var diagonals = new List<Tuple<int, int>>();

        // Construct Status and Helper, a List of Edges left of every Point of the Polygon
        // by shooting a Ray from the Vertex to the left.
        // The Helper Point of that Edge will be used to create Monotone Polygons
        // by adding Diagonals from/to Split- and Merge-Points
        var statusAndHelper = new StatusHelper();

        // Sweep through the Polygon using the sorted Polygon Points
        for (var i = 0; i < events.Count; i++)
        {
            var ev = events[i];
            // Get the Class of this event (depending on the sweeping direction)
            var evClass = ev.PointClass(!sweepDown);

            // Temporary StatusHelperElement
            StatusHelperElement? she;

            // Handle the different Point-Classes
            switch (evClass)
            {
                case PolygonPointClass.Start:
                    // Just add the left Edge (depending on the sweeping direction)
                    statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                    break;
                case PolygonPointClass.Stop:
                    // Just remove the left Edge (depending on the sweeping direction)
                    statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                    break;
                case PolygonPointClass.Regular:
                    // If the Polygon is positioned on the right Side of this Event
                    if (ev.Last > ev.Next)
                    {
                        // Replace the corresponding (old) StatusHelperElement with the new one
                        statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                        statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                    }
                    else
                    {
                        // Search Edge left of the Event and set Event as it's Helper
                        she = statusAndHelper.SearchLeft(ev);
                        if (she != null)
                            she.Helper = ev;
                    }
                    break;
                case PolygonPointClass.Merge:
                    // Just remove the left Edge (depending on the sweeping direction)
                    statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                    // Search Edge left of the Event and set Event as it's Helper
                    she = statusAndHelper.SearchLeft(ev);
                    if (she != null)
                        she.Helper = ev;
                    break;
                case PolygonPointClass.Split:
                    // Search Edge left of the Event
                    she = statusAndHelper.SearchLeft(ev);
                    if (she != null)
                    {
                        if (she.Helper is not null)
                        {
                            // Chose diagonal from Helper of Edge to Event.
                            var minP = Math.Min(she.Helper.Index, ev.Index);
                            var maxP = Math.Max(she.Helper.Index, ev.Index);
                            var diagonal = new Tuple<int, int>(minP, maxP);
                            diagonals.Add(diagonal);
                        }

                        // Replace the Helper of the StatusHelperElement by Event
                        she.Helper = ev;
                        // Insert the right Edge from Event
                        statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                    }
                    break;
            }
        }
        return diagonals;
    }

    /// <summary>
    /// Split Polygon into subpolagons using the calculated Diagonals
    /// </summary>
    /// <param name="poly">The Base-Polygon</param>
    /// <param name="diagonals">The Split-Diagonals</param>
    /// <returns>List of Subpolygons</returns>
    private static List<PolygonData> SplitIntoPolygons(PolygonData poly, List<Tuple<int, int>> diagonals)
    {
        if (diagonals.Count == 0)
            return new List<PolygonData>() { poly };

        diagonals = diagonals.OrderBy(d => d.Item1).ThenBy(d => d.Item2).ToList();
        var edges = new SortedDictionary<int, List<PolygonEdge>>();
        foreach (var edge in poly.Points.Select(p => p.EdgeTwo)
            .Union(diagonals.Select(d => new PolygonEdge(poly.Points[d.Item1], poly.Points[d.Item2])))
            .Union(diagonals.Select(d => new PolygonEdge(poly.Points[d.Item2], poly.Points[d.Item1]))))
        {
            if (edge?.PointOne is null)
            {
                continue;
            }

            if (!edges.ContainsKey(edge.PointOne.Index))
            {
                edges.Add(edge.PointOne.Index, new List<PolygonEdge>() { edge });
            }
            else
            {
                edges[edge.PointOne.Index].Add(edge);
            }
        }

        var subPolygons = new List<PolygonData>();

        var cnt = 0;
        foreach (var edge in edges)
        {
            cnt += edge.Value.Count;
        }

        // For each Diagonal
        while (edges.Count > 0)
        {
            // Start at first Diagonal Point
            var currentPoint = edges.First().Value.First().PointOne;
            var nextEdge = new PolygonEdge(null, null);
            var subPolygonPoints = new List<PolygonPoint>();
            // March along the edges to form a monotone Polygon
            // Until the current Point equals the StartPoint
            do
            {
                // Add the current Point
                if (currentPoint is not null)
                {
                    subPolygonPoints.Add(currentPoint);
                }
                // Select the next Edge
                var possibleEdges = edges[currentPoint!.Index].ToList();
                nextEdge = BestEdge(currentPoint, nextEdge, possibleEdges);
                // Remove Edge from possible Edges
                edges[currentPoint.Index].Remove(nextEdge);
                if (edges[currentPoint.Index].Count == 0)
                {
                    edges.Remove(currentPoint.Index);
                }

                // Move to the next Point
                currentPoint = nextEdge.PointTwo;
            }
            while (subPolygonPoints[0].Index != currentPoint!.Index);
            // Add the new SubPolygon
            subPolygons.Add(new PolygonData(subPolygonPoints));
        }

        return subPolygons;
    }

    /// <summary>
    /// For a Point, last used Edge and possible Edges, retrieve the best next Edge
    /// </summary>
    /// <param name="point">The current Point</param>
    /// <param name="lastEdge">The last used Edge</param>
    /// <param name="possibleEdges">The possible next Edges</param>
    /// <returns>Best next Edge</returns>
    internal static PolygonEdge BestEdge(PolygonPoint point, PolygonEdge lastEdge, List<PolygonEdge> possibleEdges)
    {
        // If just Starting, return the first possible Edge of the Point
        // If only one possibility, return that
        if ((lastEdge.PointOne == null && lastEdge.PointTwo == null) || possibleEdges.Count == 1)
        {
            return possibleEdges[0];
        }

        // Variables needed to determine the next Edge
        var bestEdge = possibleEdges[0];
        var bestAngle = (float)Math.PI * 2;
        // Vector from last Point to current Point
        var lastVector = Vector2.Normalize(lastEdge.PointTwo!.Point - lastEdge.PointOne!.Point);
        // Using CCW Point Order, so the left Vector always points towards the Polygon Center
        var insideVector = new Vector2(-lastVector.Y, lastVector.X);
        // Check all possible Edges
        foreach (var possibleEdge in possibleEdges)
        {
            // Next Edge Vector
            var edgeVector = Vector2.Normalize(possibleEdge.PointTwo!.Point - possibleEdge.PointOne!.Point);
            // Dot determines if the Vector also points towards the Polygon Center or not (> 0, yes, < 0, no)
            var dot = insideVector.X * edgeVector.X + insideVector.Y * edgeVector.Y;
            // Cos represents the Angle between the last Edge and the next Edge
            var cos = lastVector.X * edgeVector.X + lastVector.Y * edgeVector.Y;
            var angle = 0f;
            // Depending on the Dot-Value, calculate the actual "inner" Angle
            if ((insideVector.X * edgeVector.X + insideVector.Y * edgeVector.Y) > 0)
            {
                angle = (float)Math.PI - (float)Math.Acos(cos);
            }
            else
            {
                angle = (float)Math.PI + (float)Math.Acos(cos);
            }
            // Replace the old Values if a better Edge was found
            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestEdge = possibleEdge;
            }
        }
        return bestEdge;
    }

    /// <summary>
    /// Calculates the Orientation of a Polygon by usings it's (double-) Area as an Indicator.
    /// </summary>
    /// <param name="polygon">The Polygon.</param>
    /// <returns>True if the Polygon is present in a CCW manner.</returns>
    internal static bool IsCCW(IList<Vector2> polygon)
    {
        var n = polygon.Count;
        var area = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            area += polygon[p].X * polygon[q].Y - polygon[q].X * polygon[p].Y;
        }
        return area > 0.0f;
    }
}
