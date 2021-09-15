using System;
using System.Collections.Generic;
using System.Text;

#if SHARPDX
#if NETFX_CORE
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
#else
namespace HelixToolkit.Wpf
#endif
{
#if SHARPDX
    using System;
    using System.Collections.Generic;

    using Matrix3D = global::SharpDX.Matrix;
    using Point = global::SharpDX.Vector2;
    using Point3D = global::SharpDX.Vector3;
    using PointCollection = System.Collections.Generic.List<global::SharpDX.Vector2>;
    using Vector3D = global::SharpDX.Vector3;
#else
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
#endif

    /// <summary>
    /// Represents a 3D polygon.
    /// </summary>
    public class Polygon3D
    {
        /// <summary>
        /// The points.
        /// </summary>
        private IList<Point3D> points;

        /// <summary>
        /// Initializes a new instance of the <see cref = "Polygon3D" /> class.
        /// </summary>
        public Polygon3D()
        {
            this.points = new List<Point3D>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="pts">
        /// The PTS.
        /// </param>
        public Polygon3D(IList<Point3D> pts)
        {
            this.points = pts;
        }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>The points.</value>
        public IList<Point3D> Points
        {
            get
            {
                return this.points;
            }

            set
            {
                this.points = value;
            }
        }

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
            var up = this.GetNormal();
            up.Normalize();
#if SHARPDX
            var right = Vector3D.Cross(
#else
            var right = Vector3D.CrossProduct(
#endif
                up, Math.Abs(up.X) > Math.Abs(up.Z) ? new Vector3D(0, 0, 1) : new Vector3D(1, 0, 0));
#if SHARPDX
            var backward = Vector3D.Cross(
#else
            var backward = Vector3D.CrossProduct(
#endif
                right, up);
            var m = new Matrix3D(backward.X, right.X, up.X, 0, backward.Y, right.Y, up.Y, 0, backward.Z, right.Z, up.Z, 0, 0, 0, 0, 1);

            // make first point origin
#if SHARPDX
            var offs = Vector3D.TransformCoordinate(Points[0], m);
            m.M41 = -offs.X;
            m.M42 = -offs.Y;
#else
            var offs = m.Transform(this.Points[0]);
            m.OffsetX = -offs.X;
            m.OffsetY = -offs.Y;
#endif

            var polygon = new Polygon { Points = new PointCollection(this.Points.Count) };
            foreach (var p in this.Points)
            {
#if SHARPDX
                var pp = Vector3D.TransformCoordinate(p, m);
#else
                var pp = m.Transform(p);
#endif
                polygon.Points.Add(new Point(pp.X, pp.Y));
            }

            return polygon;
        }

        /// <summary>
        /// Gets the normal of the polygon.
        /// </summary>
        /// <returns>
        /// The normal.
        /// </returns>
        public Vector3D GetNormal()
        {
            if (this.Points.Count < 3)
            {
                throw new InvalidOperationException("At least three points required in the polygon to find a normal.");
            }

            Vector3D v1 = this.Points[1] - this.Points[0];
            for (int i = 2; i < this.Points.Count; i++)
            {
#if SHARPDX
                var n = Vector3D.Cross(v1, this.Points[i] - this.Points[0]);

                if (n.LengthSquared() > 1e-10)
#else
                var n = Vector3D.CrossProduct(v1, this.Points[i] - this.Points[0]);

                if (n.LengthSquared > 1e-10)
#endif
                {
                    n.Normalize();
                    return n;
                }
            }

#if SHARPDX
            Vector3D result = Vector3D.Cross(v1, this.Points[2] - this.Points[0]);
#else
            Vector3D result = Vector3D.CrossProduct(v1, this.Points[2] - this.Points[0]);
#endif
            result.Normalize();
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
            Vector3D v1 = this.Points[1] - this.Points[0];
            var normal = new Vector3D();
            for (int i = 2; i < this.Points.Count; i++)
            {
#if SHARPDX
                var n = Vector3D.Cross(v1, this.Points[i] - this.Points[0]);
#else
                var n = Vector3D.CrossProduct(v1, this.Points[i] - this.Points[0]);
#endif
                n.Normalize();
                if (i == 2)
                {
                    normal = n;
                }
#if SHARPDX
                else if (Math.Abs(Vector3D.Dot(n, normal) - 1) > 1e-8)
#else
                else if (Math.Abs(Vector3D.DotProduct(n, normal) - 1) > 1e-8)
#endif
                {
                    return false;
                }
            }

            return true;
        }
    }
}