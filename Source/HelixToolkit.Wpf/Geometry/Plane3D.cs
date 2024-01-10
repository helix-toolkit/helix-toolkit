using HelixToolkit.Maths;
using System.Numerics;
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
    }

    /// <summary>
    /// Gets or sets the normal.
    /// </summary>
    /// <value>The normal.</value>
    public Vector3D Normal { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point3D Position { get; set; }

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
        Vector3 v1 = la.ToVector3();
        Vector3 v2 = lb.ToVector3();
        if (this.ToPlane().IntersectsLine(ref v1, ref v2, out Vector3 intersection))
        {
            return intersection.ToWndVector3D();
        }
        return null;
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
        Vector3 p = point.ToVector3();
        return this.ToPlane().DistanceTo(ref p);
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
        Vector3 p = point.ToVector3();
        return this.ToPlane().Project(ref p).ToWndVector3D();
    }
}
