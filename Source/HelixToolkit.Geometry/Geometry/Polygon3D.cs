using CommunityToolkit.Diagnostics;
using System.Numerics;

namespace HelixToolkit.Geometry;

/// <summary>
/// Represents a 3D polygon.
/// </summary>
public sealed class Polygon3D
{
    /// <summary>
    /// Initializes a new instance of the <see cref = "Polygon3D" /> class.
    /// </summary>
    public Polygon3D()
    {
        this.Points = new List<Vector3>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Polygon3D"/> class.
    /// </summary>
    /// <param name="pts">
    /// The PTS.
    /// </param>
    public Polygon3D(IList<Vector3> pts)
    {
        this.Points = pts;
    }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    /// <value>The points.</value>
    public IList<Vector3> Points { get; set; }

    //// http://en.wikipedia.org/wiki/Polygon_triangulation
    //// http://en.wikipedia.org/wiki/Monotone_polygon
    //// http://www.codeproject.com/KB/recipes/hgrd.aspx LGPL
    //// http://www.springerlink.com/content/g805787811vr1v9v/

    /// <summary>
    /// Flattens this polygon.
    /// </summary>
    /// <returns>
    /// The 2D polygon.
    /// </returns>
    public Polygon Flatten()
    {
        // http://forums.xna.com/forums/p/16529/86802.aspx
        // http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
        var up = Vector3.Normalize(this.GetNormal());
        var right = Vector3.Cross(up, Math.Abs(up.X) > Math.Abs(up.Z) ? Vector3.UnitZ : Vector3.UnitX);
        var backward = Vector3.Cross(right, up);
        var m = new Matrix4x4(backward.X, right.X, up.X, 0, backward.Y, right.Y, up.Y, 0, backward.Z, right.Z, up.Z, 0, 0, 0, 0, 1);

        // make first point origin
        var offs = Vector3.Transform(Points[0], m);
        m.M41 = -offs.X;
        m.M42 = -offs.Y;

        var polygon = new Polygon { Points = new List<Vector2>(this.Points.Count) };
        foreach (var p in this.Points)
        {
            var pp = Vector3.Transform(p, m);
            polygon.Points.Add(new Vector2(pp.X, pp.Y));
        }

        return polygon;
    }

    /// <summary>
    /// Gets the normal of the polygon.
    /// </summary>
    /// <returns>
    /// The normal.
    /// </returns>
    public Vector3 GetNormal()
    {
        if (this.Points.Count < 3)
        {
            ThrowHelper.ThrowInvalidOperationException("At least three points required in the polygon to find a normal.");
        }

        var v1 = this.Points[1] - this.Points[0];
        for (var i = 2; i < this.Points.Count; i++)
        {
            var n = Vector3.Cross(v1, this.Points[i] - this.Points[0]);

            if (n.LengthSquared() > 1e-10f)
            {
                n = Vector3.Normalize(n);
                return n;
            }
        }

        var result = Vector3.Cross(v1, this.Points[2] - this.Points[0]);
        result = Vector3.Normalize(result);
        return result;
    }

    /// <summary>
    /// Determines whether this polygon is planar.
    /// </summary>
    /// <returns>
    /// The is planar.
    /// </returns>
    public bool IsPlanar()
    {
        if (this.Points.Count <= 2)
        {
            return true;
        }

        var v1 = this.Points[1] - this.Points[0];
        var normal = new Vector3();
        for (var i = 2; i < this.Points.Count; i++)
        {
            Vector3 n = Vector3.Cross(v1, this.Points[i] - this.Points[0]);
            n = Vector3.Normalize(n);

            if (i == 2)
            {
                normal = n;
            }
            else
            {
                float d = Math.Abs(Vector3.Dot(n, normal) - 1);

                if (d > 1e-6f)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
