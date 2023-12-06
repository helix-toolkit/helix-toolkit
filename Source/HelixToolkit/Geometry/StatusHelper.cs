namespace HelixToolkit.Geometry;

/// <summary>
/// Helper Class that is used in the calculation Process of the Diagonals.
/// </summary>
internal sealed class StatusHelper
{
    /// <summary>
    /// List of StatusHelperElements that are currently present at the Sweeper's Position
    /// </summary>
    public List<StatusHelperElement> EdgesHelpers { get; } = new List<StatusHelperElement>();

    /// <summary>
    /// Adds a StatusHelperElement to the List
    /// </summary>
    /// <param name="element"></param>
    public void Add(StatusHelperElement element)
    {
        this.EdgesHelpers.Add(element);
    }

    /// <summary>
    /// Removes all StatusHelperElements with a specific Edge
    /// </summary>
    /// <param name="edge"></param>
    public void Remove(PolygonEdge? edge)
    {
        this.EdgesHelpers.RemoveAll(she => she.Edge == edge);
    }

    /// <summary>
    /// Searches the nearest StatusHelperElement from the given Point
    /// </summary>
    /// <param name="point">The Point to search a StatusHelperElement for</param>
    /// <returns>The nearest StatusHelperElement that is positioned left of the Poin</returns>
    public StatusHelperElement? SearchLeft(PolygonPoint point)
    {
        // The found StatusHelperElement and the Distance Variables
        StatusHelperElement? result = null;
        var dist = float.PositiveInfinity;

        var px = point.X;
        var py = point.Y;
        // Search for the right StatusHelperElement
        foreach (var she in this.EdgesHelpers)
        {
            // No need to calculate the X-Value
            if (she.MinX > px)
                continue;

            if (she.Edge?.PointOne is null)
            {
                continue;
            }

            // Calculate the x-Coordinate of the Intersection between
            // a horizontal Line from the Point to the Left and the Edge of the StatusHelperElement
            var xValue = she.Edge.PointOne.X + (py - she.Edge.PointOne.Y) * she.Factor;

            // If the xValue is smaller than or equal to the Point's x-Coordinate
            // (i.e. it lies on the left Side of it - allows a small Error)
            if (xValue <= (px + SweepLinePolygonTriangulator.Epsilon))
            {
                // Calculate the Distance
                var sheDist = px - xValue;

                // Update, if the Distance is smaller than a previously found Result
                if (sheDist < dist)
                {
                    dist = sheDist;
                    result = she;
                }
            }
        }

        // Return the nearest found StatusHelperElement
        return result;
    }
}
