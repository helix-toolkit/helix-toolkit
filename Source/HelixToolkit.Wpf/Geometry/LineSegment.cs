using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// Represents a line segment in two-dimensional space.
/// </summary>
public readonly struct LineSegment : IEquatable<LineSegment>
{
    /// <summary>
    /// The first point of the line segment.
    /// </summary>
    private readonly Point p1;

    /// <summary>
    /// The second point of the line segment.
    /// </summary>
    private readonly Point p2;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment"/> structure.
    /// </summary>
    /// <param name="p1">The first point of the line segment.</param>
    /// <param name="p2">The second point of the line segment.</param>
    public LineSegment(Point p1, Point p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    /// <summary>
    /// Gets the first point of the line segment.
    /// </summary>
    /// <value>The point.</value>
    public Point P1
    {
        get
        {
            return this.p1;
        }
    }

    /// <summary>
    /// Gets the second point of the line segment.
    /// </summary>
    /// <value>The point.</value>
    public Point P2
    {
        get
        {
            return this.p2;
        }
    }

    /// <summary>
    /// Gets the length of this line segment.
    /// </summary>
    public double Length => (this.p2 - this.p1).Length;

    /// <summary>
    /// Checks if there are any intersections of two line segments.
    /// </summary>
    /// <param name="a1">One vertex of line a.</param>
    /// <param name="a2">The other vertex of the line a.</param>
    /// <param name="b1">One vertex of line b.</param>
    /// <param name="b2">The other vertex of the line b.</param>
    /// <returns>
    /// <c>true</c>, if the two lines are crossed. Otherwise, it returns <c>false</c>.
    /// </returns>
    public static bool AreLineSegmentsIntersecting(Point a1, Point a2, Point b1, Point b2)
    {
        if (b1 == b2 || a1 == a2)
        {
            return false;
        }

        if ((((a2.X - a1.X) * (b1.Y - a1.Y)) - ((b1.X - a1.X) * (a2.Y - a1.Y)))
            * (((a2.X - a1.X) * (b2.Y - a1.Y)) - ((b2.X - a1.X) * (a2.Y - a1.Y))) > 0)
        {
            return false;
        }

        if ((((b2.X - b1.X) * (a1.Y - b1.Y)) - ((a1.X - b1.X) * (b2.Y - b1.Y)))
            * (((b2.X - b1.X) * (a2.Y - b1.Y)) - ((a2.X - b1.X) * (b2.Y - b1.Y))) > 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Indicates whether the specified line segment intersects with the current line segment.
    /// </summary>
    /// <param name="other">The line segment to check.</param>
    /// <returns>
    /// <c>true</c> if the specified line segment intersects with the current line segment; otherwise <c>false</c>.
    /// </returns>
    public bool IntersectsWith(LineSegment other)
    {
        return AreLineSegmentsIntersecting(this.p1, this.p2, other.p1, other.p2);
    }
    public override bool Equals(object? obj)
    {
        return obj is LineSegment segment && this.Equals(segment);
    }
    public bool Equals(LineSegment other)
    {
        return this.p1.Equals(other.p1) && this.p2.Equals(other.p2);
    }
    public override int GetHashCode()
    {
        return p1.GetHashCode() ^ p2.GetHashCode();
    }
    public static bool operator ==(LineSegment left, LineSegment right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(LineSegment left, LineSegment right)
    {
        return !(left == right);
    }
}
