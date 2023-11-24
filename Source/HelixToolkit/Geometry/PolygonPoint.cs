using System.Numerics;

namespace HelixToolkit.Geometry;

/// <summary>
/// Helper Class for the PolygonData Object.
/// </summary>
internal sealed class PolygonPoint : IComparable<PolygonPoint?>
{
    /// <summary>
    /// The actual Point of this PolygonPoint
    /// </summary>
    private Vector2 mPoint;

    /// <summary>
    /// Accessor for the Point-Data
    /// </summary>
    public Vector2 Point
    {
        get
        {
            return mPoint;
        }

        set
        {
            mPoint = value;
        }
    }

    /// <summary>
    /// Accessor for the X-Coordinate of the Point
    /// </summary>
    public float X
    {
        get
        {
            return this.mPoint.X;
        }
        set
        {
            this.mPoint.X = value;
        }
    }

    /// <summary>
    /// Accessor for the Y-Coordinate of the Point
    /// </summary>
    public float Y
    {
        get
        {
            return this.mPoint.Y;
        }
        set
        {
            this.mPoint.Y = value;
        }
    }

    /// <summary>
    /// The "incoming" Edge of this PolygonPoint
    /// </summary>
    public PolygonEdge? EdgeOne { get; set; }

    /// <summary>
    /// The "outgoing" Edge of this PolygonPoint
    /// </summary>
    public PolygonEdge? EdgeTwo { get; set; }

    /// <summary>
    /// The Index of this Point in the original Polygon
    /// that needs to be triangulated
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The "last" neighboring Point, which is connected throught the incoming Edge
    /// </summary>
    public PolygonPoint? Last => EdgeOne?.PointOne;

    /// <summary>
    /// The "next" neighboring Point, which is connected throught the outgoing Edge
    /// </summary>
    public PolygonPoint? Next => EdgeTwo?.PointTwo;

    /// <summary>
    /// Comparison Operator, that is used to determine the Class of the PolygonPoints
    /// </summary>
    /// <param name="first">The first PolygonPoint</param>
    /// <param name="second">The second PolygonPoint</param>
    /// <returns>Returns true if the first PolygonPoint is smaller, compared to the second PolygonPoint, false otherwise</returns>
    public static bool operator <(PolygonPoint? first, PolygonPoint? second)
    {
        return first?.CompareTo(second) == 1;
    }

    /// <summary>
    /// Comparison Operator, that is used to determine the Class of the PolygonPoints
    /// </summary>
    /// <param name="first">The first PolygonPoint</param>
    /// <param name="second">The second PolygonPoint</param>
    /// <returns>Returns true if the first PolygonPoint is bigger, compared to the second PolygonPoint, false otherwise</returns>
    public static bool operator >(PolygonPoint? first, PolygonPoint? second)
    {
        return first?.CompareTo(second) == -1;
    }

    /// <summary>
    /// Constructor using a Point
    /// </summary>
    /// <param name="p">The Point-Data to use</param>
    internal PolygonPoint(Vector2 p)
    {
        // Set the Point-Data, the Index must be set later
        this.mPoint = p;
        this.Index = -1;
    }

    /// <summary>
    /// Detrmines the Class of the PolygonPoint, depending on the sweeping Direction
    /// </summary>
    /// <param name="reverse">The Sweeping direction, top-to-bottom if false, bottom-to-top otherwise</param>
    /// <returns>The Class of the PolygonPoint</returns>
    internal PolygonPointClass PointClass(bool reverse = false)
    {
        // If the Point has no Next- and Last-PolygonPoint, there's an Error
        if (Next == null || Last == null)
            throw new Exception("No closed Polygon");

        // If we use the normal Order (top-to-bottom)
        if (!reverse)
        {
            // Both neighboring PolygonPoints are below this Point and the Point is concave
            if (Last < this && Next < this && this.IsConvexPoint())
                return PolygonPointClass.Start;
            // Both neighboring PolygonPoints are above this Point and the Point is concave
            else if (Last > this && Next > this && this.IsConvexPoint())
                return PolygonPointClass.Stop;
            // Both neighboring PolygonPoints are below this Point and the Point is convex
            else if (Last < this && Next < this)
                return PolygonPointClass.Split;
            // Both neighboring PolygonPoints are above this Point and the Point is convex
            else if (Last > this && Next > this)
                return PolygonPointClass.Merge;
            // Regular Point in all other Cases
            else
                return PolygonPointClass.Regular;
        }
        else
        {
            // Both neighboring PolygonPoints are below this Point and the Point is concave
            if (Last < this && Next < this && this.IsConvexPoint())
                return PolygonPointClass.Stop;
            // Both neighboring PolygonPoints are above this Point and the Point is concave
            else if (Last > this && Next > this && this.IsConvexPoint())
                return PolygonPointClass.Start;
            // Both neighboring PolygonPoints are below this Point and the Point is convex
            else if (Last < this && Next < this)
                return PolygonPointClass.Merge;
            // Both neighboring PolygonPoints are above this Point and the Point is convex
            else if (Last > this && Next > this)
                return PolygonPointClass.Split;
            // Regular Point in all other Cases
            else
                return PolygonPointClass.Regular;
        }
    }

    /// <summary>
    /// Calculates for a Point, if it is a convex Point or not
    /// (the assumption is, that we are dealing with a CCW Polygon orientation!)
    /// </summary>
    /// <returns>Returns true, if convex, false if concave (or "reflex" Vertex)</returns>
    private bool IsConvexPoint()
    {
        // If the Point has no Next- and Last-PolygonPoint, there's an Error
        if (Next == null || Last == null)
            throw new Exception("No closed Polygon");
        // Calculate the necessary Vectors
        // From-last-to-this Vector
        var vecFromLast = Vector2.Normalize(this.Point - this.Last.Point);
        // "Left" Vector (pointing "inward")
        var vecLeft = new Vector2(-vecFromLast.Y, vecFromLast.X);
        // From-this-to-next Vector
        var vecToNext = Vector2.Normalize(this.Next.Point - this.Point);
        // If the next Vector is pointing to the left Vector's direction,
        // the current Point is a convex Point (Dot-Product bigger than 0)
        if ((vecLeft.X * vecToNext.X + vecLeft.Y * vecToNext.Y) >= 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Override the ToString (for Debugging Purposes)
    /// </summary>
    /// <returns>String representing this Point</returns>
    public override string ToString()
    {
        return this.Index + " X:" + this.X + " Y:" + this.Y;
    }

    /// <summary>
    /// Comparison of two Points, used to sort the Polygons from top to bottom (left to right)
    /// </summary>
    /// <param name="second">Other Point to compare to</param>
    /// <returns>-1 if this Point is bigger, 0 if the same, 1 if smaller</returns>
    public int CompareTo(PolygonPoint? second)
    {
        if (this == null || second == null)
            return 0;
        if (this.Y > second.Y || (this.Y == second.Y && this.X < second.X))
            return -1;
        else if (this.Y == second.Y && this.X == second.X)
            return 0;
        else
            return 1;
    }
}
