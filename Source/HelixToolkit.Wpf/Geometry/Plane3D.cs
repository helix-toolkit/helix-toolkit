using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

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
    public Plane3D(Point3D position, Vector3D normal)
    {
        this.Position = position;
        this.Normal = normal;
        this.Normal.Normalize();
    }

    /// <summary>
    /// Gets or sets the normal.
    /// </summary>
    /// <value>The normal.</value>
    public Vector3D Normal { get; set; } = default;

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point3D Position { get; set; } = default;

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
    public Vector3D? LineIntersection(Vector3D la, Vector3D lb)
    {
        // http://en.wikipedia.org/wiki/Line-plane_intersection
        var l = lb - la;
        var a = Vector3D.DotProduct(this.Position.ToVector3D() - la, this.Normal);
        var b = Vector3D.DotProduct(l, this.Normal);

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
    public double DistanceTo(Vector3D point)
    {
        var planeToPoint = point - Position.ToVector3D();
        return Vector3D.DotProduct(planeToPoint, Normal);
    }

    /// <summary>
    /// Calculates the projection of a point onto a plane.
    /// </summary>
    /// <param name="point">The point used to calculate projection</param>
    /// <returns>
    /// The projection of a given point on a given plane.
    /// </returns>
    public Vector3D Project(Vector3D point)
    {
        var distance = DistanceTo(point);
        return point - distance * Normal;
    }
}
