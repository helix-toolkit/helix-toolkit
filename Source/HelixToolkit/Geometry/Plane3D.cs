using HelixToolkit.Geometry;
using System.Numerics;

namespace HelixToolkit;

/// <summary>
/// Represents a plane in three-dimensional space.
/// </summary>
public class Plane3D
{
    /// <summary>
    /// Initializes a new instance of the <see cref = "Plane3D" /> class.
    /// </summary>
    public Plane3D()
    {
        Normal = Vector3.Zero;
        Position = Vector3.Zero;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plane3D"/> class.
    /// </summary>
    /// <param name="position">
    /// The p0.
    /// </param>
    /// <param name="normal">
    /// The n.
    /// </param>
    public Plane3D(Vector3 position, Vector3 normal)
    {
        this.Position = position;
        this.Normal = normal;
    }

    /// <summary>
    /// Gets or sets the normal.
    /// </summary>
    /// <value>The normal.</value>
    public Vector3 Normal { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Finds the intersection between the plane and a line.
    /// </summary>
    /// <param name="la">
    /// The first point defining the line.
    /// </param>
    /// <param name="lb">
    /// The second point defining the line.
    /// </param>
    /// <returns>
    /// The intersection point.
    /// </returns>
    public Vector3? LineIntersection(Vector3 la, Vector3 lb)
    {
        // http://en.wikipedia.org/wiki/Line-plane_intersection
        var l = lb - la;
        float a = Vector3.Dot(this.Position - la, this.Normal);
        float b = Vector3.Dot(l, this.Normal);

        if (a == 0 && b == 0)
        {
            return null;
        }

        if (b == 0)
        {
            return null;
        }

        return la + ((a / b) * l);
    }

    /// <summary>
    /// Calculates the distance from a point to a plane.
    /// </summary>
    /// <param name="point">The point used to calculate distance</param>
    /// <returns>
    /// The distance from given point to the given plane<br/>
    /// Equal zero: Point on the plane<br/>
    /// Greater than zero: The point is on the same side of the plane's normal vector<br/>
    /// Less than zero: The point is on the opposite side of the plane's normal vector<br/>
    /// </returns>
    public float DistanceTo(Vector3 point)
    {
        return point.DistanceToPlane(Position, Normal);
    }

    /// <summary>
    /// Calculates the projection of a point onto a plane.
    /// </summary>
    /// <param name="point">The point used to calculate projection</param>
    /// <returns>
    /// The projection of a given point on a given plane.
    /// </returns>
    public Vector3 Project(Vector3 point)
    {
        return point.ProjectOnPlane(Position, Normal);
    }

    /// <summary>
    /// Check whether a plane intersects with a given <see cref="Rect3D"/> box.
    /// </summary>
    /// <param name="rect">The Rect3D bounding box</param>
    /// <returns>
    /// Whether the two objects intersected.
    /// </returns>
    public PlaneIntersectionType Intersects(Rect3D rect)
    {
        return rect.Intersects(Position, Normal);
    }

    // public void SetYZ(double x, int dir)
    // {
    // Position = new Point3D(x, 0, Height / 2);
    // Normal = new Vector3D(dir, 0, 0);
    // Up = new Vector3D(0, 0, 1);
    // }

    // public void SetXY(double z, int dir)
    // {
    // Position = new Point3D(0, 0, z);
    // Normal = new Vector3D(0, 0, dir);
    // Up = new Vector3D(1, 0, 0);
    // }

    // public void SetXZ(double y, int dir)
    // {
    // Position = new Point3D(0, y, 0);
    // Normal = new Vector3D(0, dir, 0);
    // Up = new Vector3D(1, 0, 0);
    // }

    // public Point3D[] GetCornerPoints()
    // {
    // var pts = new Point3D[4];
    // Vector3D right = Vector3D.CrossProduct(Normal, Up);
    // pts[0] = Position + (-right * 0.5 * Width - Up * 0.5 * Height);
    // pts[1] = Position + (right * 0.5 * Width - Up * 0.5 * Height);
    // pts[2] = Position + (right * 0.5 * Width + Up * 0.5 * Height);
    // pts[3] = Position + (-right * 0.5 * Width + Up * 0.5 * Height);
    // return pts;
    // }
}
