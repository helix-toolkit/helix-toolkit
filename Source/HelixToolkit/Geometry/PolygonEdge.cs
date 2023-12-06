namespace HelixToolkit.Geometry;

/// <summary>
/// Helper Class for the PolygonData Object.
/// </summary>
internal sealed class PolygonEdge
{
    /// <summary>
    /// The "starting" Point of this Edge
    /// </summary>
    public PolygonPoint? PointOne { get; set; }

    /// <summary>
    /// The "ending" Point of this Edge
    /// </summary>
    public PolygonPoint? PointTwo { get; set; }

    /// <summary>
    /// The "last" neighboring Edge, which both share the Startpoint of this Edge
    /// </summary>
    public PolygonEdge? Last => PointOne?.EdgeOne;

    /// <summary>
    /// The "next" neighboring Edge, which both share the Endpoint of this Edge
    /// </summary>
    public PolygonEdge? Next => PointTwo?.EdgeTwo;

    /// <summary>
    /// Constructor that takes both Points of the Edge
    /// </summary>
    /// <param name="one">The Startpoint</param>
    /// <param name="two">The Endpoint</param>
    internal PolygonEdge(PolygonPoint? one, PolygonPoint? two)
    {
        this.PointOne = one;
        this.PointTwo = two;
    }

    /// <summary>
    /// Override the ToString (for Debugging Purposes)
    /// </summary>
    /// <returns>String representing this Edge</returns>
    public override string ToString()
    {
        return "From: {" + PointOne + "} To: {" + PointTwo + "}";
    }
}
