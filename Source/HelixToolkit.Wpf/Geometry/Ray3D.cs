using System.Numerics;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// Represents a ray in three-dimensional space.
/// </summary>
public class Ray3D
{
    /// <summary>
    /// The direction
    /// </summary>
    private Vector3D direction;

    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    /// <value>The direction.</value>
    public Vector3D Direction
    {
        get
        {
            return this.direction;
        }

        set
        {
            this.direction = value;
        }
    }
    /// <summary>
    /// The origin
    /// </summary>
    private Point3D origin;

    /// <summary>
    /// Gets or sets the origin.
    /// </summary>
    /// <value>The origin.</value>
    public Point3D Origin
    {
        get
        {
            return this.origin;
        }

        set
        {
            this.origin = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "Ray3D" /> class.
    /// </summary>
    public Ray3D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray3D"/> class.
    /// </summary>
    /// <param name="origin">
    /// The origin.
    /// </param>
    /// <param name="direction">
    /// The direction.
    /// </param>
    public Ray3D(Point3D origin, Vector3D direction)
    {
        this.Origin = origin;
        this.Direction = direction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray3D"/> class.
    /// </summary>
    /// <param name="p0">
    /// The p0.
    /// </param>
    /// <param name="p1">
    /// The p1.
    /// </param>
    public Ray3D(Point3D p0, Point3D p1)
    {
        this.Origin = p0;
        this.Direction = p1 - p0;
    }

    /// <summary>
    /// Gets the point on the ray that is nearest the specified point.
    /// </summary>
    /// <param name="p3">
    /// The point.
    /// </param>
    /// <returns>
    /// The nearest point on the ray.
    /// </returns>
    public Point3D GetNearest(Point3D p3)
    {
        return this.ToRay().GetNearest(p3.ToVector3()).ToWndPoint3D();
    }

    /// <summary>
    /// Finds the intersection with a plane.
    /// </summary>
    /// <param name="position">
    /// A point on the plane.
    /// </param>
    /// <param name="normal">
    /// The normal of the plane.
    /// </param>
    /// <returns>
    /// The intersection point.
    /// </returns>
    public Point3D? PlaneIntersection(Point3D position, Vector3D normal)
    {
        if (this.PlaneIntersection(position, normal, out Point3D intersection))
        {
            return intersection;
        }
        return null;
    }

    /// <summary>
    /// Finds the intersection with a plane.
    /// </summary>
    /// <param name="position">A point on the plane.</param>
    /// <param name="normal">The normal of the plane.</param>
    /// <param name="intersection">The intersection point.</param>
    /// <returns>
    /// True if a intersection was found.
    /// </returns>
    public bool PlaneIntersection(Point3D position, Vector3D normal, out Point3D intersection)
    {
        var plane = Maths.PlaneHelper.Create(position.ToVector3(), Vector3.Normalize(normal.ToVector3()));
        bool isIntersect = this.ToRay().Intersects(ref plane, out Vector3 point);
        intersection = point.ToWndPoint3D();
        return isIntersect;
    }
}
