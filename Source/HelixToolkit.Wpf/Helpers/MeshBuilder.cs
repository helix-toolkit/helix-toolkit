// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Builds MeshGeometry3D objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Builds MeshGeometry3D objects.
    /// </summary>
    /// <remarks>
    /// Performance tips for MeshGeometry3D (See <a href="http://msdn.microsoft.com/en-us/library/bb613553.aspx">MSDN</a>)
    /// <para>
    /// High impact:
    /// Mesh animation—changing the individual vertices of a mesh on a per-frame basis—is not always efficient in
    /// Windows Presentation Foundation (WPF).  To minimize the performance impact of change notifications when
    /// each vertex is modified, detach the mesh from the visual tree before performing per-vertex modification.
    /// Once the mesh has been modified, reattach it to the visual tree.  Also, try to minimize the size of meshes
    /// that will be animated in this way.
    /// </para>
    /// <para>
    /// Medium impact:
    /// When a mesh is defined as abutting triangles with shared vertices and those vertices have the same position,
    /// normal, and texture coordinates, define each shared vertex only once and then define your triangles by
    /// index with TriangleIndices.
    /// </para>
    /// <para>
    /// Low impact:
    /// To minimize the construction time of large collections in Windows Presentation Foundation (WPF),
    /// such as a MeshGeometry3D’s Positions, Normals, TextureCoordinates, and TriangleIndices, pre-size
    /// the collections before value population. If possible, pass the collections’ constructors prepopulated
    /// data structures such as arrays or Lists.
    /// </para>
    /// </remarks>
    public class MeshBuilder
    {
        /// <summary>
        /// 'All curves should have the same number of points' exception message.
        /// </summary>
        private const string AllCurvesShouldHaveTheSameNumberOfPoints =
            "All curves should have the same number of points";

        /// <summary>
        /// 'Source mesh normal vectors should not be null' exception message.
        /// </summary>
        private const string SourceMeshNormalsShouldNotBeNull = "Source mesh normal vectors should not be null.";

        /// <summary>
        /// 'Source mesh texture coordinates should not be null' exception message.
        /// </summary>
        private const string SourceMeshTextureCoordinatesShouldNotBeNull =
            "Source mesh texture coordinates should not be null.";

        /// <summary>
        /// 'Wrong number of diameters' exception message.
        /// </summary>
        private const string WrongNumberOfDiameters = "Wrong number of diameters.";

        /// <summary>
        /// 'Wrong number of positions' exception message.
        /// </summary>
        private const string WrongNumberOfPositions = "Wrong number of positions.";

        /// <summary>
        /// 'Wrong number of normal vectors' exception message.
        /// </summary>
        private const string WrongNumberOfNormals = "Wrong number of normal vectors.";

        /// <summary>
        /// 'Wrong number of texture coordinates' exception message.
        /// </summary>
        private const string WrongNumberOfTextureCoordinates = "Wrong number of texture coordinates.";

        /// <summary>
        /// The circle cache.
        /// </summary>
        private static readonly Dictionary<int, IList<Point>> CircleCache = new Dictionary<int, IList<Point>>();

        /// <summary>
        /// The unit sphere cache.
        /// </summary>
        private static readonly Dictionary<int, MeshGeometry3D> UnitSphereCache = new Dictionary<int, MeshGeometry3D>();

        /// <summary>
        /// The normal vectors.
        /// </summary>
        private Vector3DCollection normals;

        /// <summary>
        /// The positions.
        /// </summary>
        private Point3DCollection positions;

        /// <summary>
        /// The texture coordinates.
        /// </summary>
        private PointCollection textureCoordinates;

        /// <summary>
        /// The triangle indices.
        /// </summary>
        private Int32Collection triangleIndices;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshBuilder"/> class.
        /// </summary>
        /// <remarks>
        /// Normal and texture coordinate generation are included.
        /// </remarks>
        public MeshBuilder()
            : this(true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshBuilder"/> class.
        /// </summary>
        /// <param name="generateNormals">
        /// Generate normal vectors.
        /// </param>
        /// <param name="generateTextureCoordinates">
        /// Generate texture coordinates.
        /// </param>
        public MeshBuilder(bool generateNormals, bool generateTextureCoordinates)
        {
            this.positions = new Point3DCollection();
            this.triangleIndices = new Int32Collection();

            if (generateNormals)
            {
                this.normals = new Vector3DCollection();
            }

            if (generateTextureCoordinates)
            {
                this.textureCoordinates = new PointCollection();
            }
        }

        /// <summary>
        /// Box face enumeration.
        /// </summary>
        [Flags]
        public enum BoxFaces
        {
            /// <summary>
            /// The top.
            /// </summary>
            Top = 0x1,

            /// <summary>
            /// The bottom.
            /// </summary>
            Bottom = 0x2,

            /// <summary>
            /// The left side.
            /// </summary>
            Left = 0x4,

            /// <summary>
            /// The right side.
            /// </summary>
            Right = 0x8,

            /// <summary>
            /// The front side.
            /// </summary>
            Front = 0x10,

            /// <summary>
            /// The back side.
            /// </summary>
            Back = 0x20,

            /// <summary>
            /// All sides.
            /// </summary>
            All = Top | Bottom | Left | Right | Front | Back
        }

        /// <summary>
        /// Gets the normal vectors of the mesh.
        /// </summary>
        /// <value>The normal vectors.</value>
        public Vector3DCollection Normals
        {
            get
            {
                return this.normals;
            }
        }

        /// <summary>
        /// Gets the positions collection of the mesh.
        /// </summary>
        /// <value> The positions. </value>
        public Point3DCollection Positions
        {
            get
            {
                return this.positions;
            }
        }

        /// <summary>
        /// Gets the texture coordinates of the mesh.
        /// </summary>
        /// <value>The texture coordinates.</value>
        public PointCollection TextureCoordinates
        {
            get
            {
                return this.textureCoordinates;
            }
        }

        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        /// <value>The triangle indices.</value>
        public Int32Collection TriangleIndices
        {
            get
            {
                return this.triangleIndices;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create normal vectors.
        /// </summary>
        /// <value>
        /// <c>true</c> if normal vectors should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateNormals
        {
            get
            {
                return this.normals != null;
            }

            set
            {
                if (value && this.normals == null)
                {
                    this.normals = new Vector3DCollection();
                }

                if (!value)
                {
                    this.normals = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create texture coordinates.
        /// </summary>
        /// <value>
        /// <c>true</c> if texture coordinates should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateTextureCoordinates
        {
            get
            {
                return this.textureCoordinates != null;
            }

            set
            {
                if (value && this.textureCoordinates == null)
                {
                    this.textureCoordinates = new PointCollection();
                }

                if (!value)
                {
                    this.textureCoordinates = null;
                }
            }
        }

        /// <summary>
        /// Gets a circle section (cached).
        /// </summary>
        /// <param name="thetaDiv">
        /// The number of division.
        /// </param>
        /// <returns>
        /// A circle.
        /// </returns>
        public static IList<Point> GetCircle(int thetaDiv)
        {
            IList<Point> circle;
            if (!CircleCache.TryGetValue(thetaDiv, out circle))
            {
                circle = new PointCollection();
                CircleCache.Add(thetaDiv, circle);
                for (int i = 0; i < thetaDiv; i++)
                {
                    double theta = Math.PI * 2 * ((double)i / (thetaDiv - 1));
                    circle.Add(new Point(Math.Cos(theta), -Math.Sin(theta)));
                }
            }

            return circle;
        }

        /// <summary>
        /// Adds an arrow to the mesh.
        /// </summary>
        /// <param name="point1">
        /// The start point.
        /// </param>
        /// <param name="point2">
        /// The end point.
        /// </param>
        /// <param name="diameter">
        /// The diameter of the arrow cylinder.
        /// </param>
        /// <param name="headLength">
        /// Length of the head (relative to diameter).
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the arrow.
        /// </param>
        public void AddArrow(Point3D point1, Point3D point2, double diameter, double headLength = 3, int thetaDiv = 18)
        {
            var dir = point2 - point1;
            double length = dir.Length;
            double r = diameter / 2;

            var pc = new PointCollection
                {
                    new Point(0, 0),
                    new Point(0, r),
                    new Point(length - (diameter * headLength), r),
                    new Point(length - (diameter * headLength), r * 2),
                    new Point(length, 0)
                };

            this.AddRevolvedGeometry(pc, point1, dir, thetaDiv);
        }

        /// <summary>
        /// Adds the edges of a bounding box as cylinders.
        /// </summary>
        /// <param name="boundingBox">
        /// The bounding box.
        /// </param>
        /// <param name="diameter">
        /// The diameter of the cylinders.
        /// </param>
        public void AddBoundingBox(Rect3D boundingBox, double diameter)
        {
            var p0 = new Point3D(boundingBox.X, boundingBox.Y, boundingBox.Z);
            var p1 = new Point3D(boundingBox.X, boundingBox.Y + boundingBox.SizeY, boundingBox.Z);
            var p2 = new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y + boundingBox.SizeY, boundingBox.Z);
            var p3 = new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y, boundingBox.Z);
            var p4 = new Point3D(boundingBox.X, boundingBox.Y, boundingBox.Z + boundingBox.SizeZ);
            var p5 = new Point3D(boundingBox.X, boundingBox.Y + boundingBox.SizeY, boundingBox.Z + boundingBox.SizeZ);
            var p6 = new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y + boundingBox.SizeY, boundingBox.Z + boundingBox.SizeZ);
            var p7 = new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y, boundingBox.Z + boundingBox.SizeZ);

            Action<Point3D, Point3D> addEdge = (c1, c2) => this.AddCylinder(c1, c2, diameter, 10);

            addEdge(p0, p1);
            addEdge(p1, p2);
            addEdge(p2, p3);
            addEdge(p3, p0);

            addEdge(p4, p5);
            addEdge(p5, p6);
            addEdge(p6, p7);
            addEdge(p7, p4);

            addEdge(p0, p4);
            addEdge(p1, p5);
            addEdge(p2, p6);
            addEdge(p3, p7);
        }

        /// <summary>
        /// Adds a box aligned with the X, Y and Z axes.
        /// </summary>
        /// <param name="center">
        /// The center point of the box.
        /// </param>
        /// <param name="xlength">
        /// The length of the box along the X axis.
        /// </param>
        /// <param name="ylength">
        /// The length of the box along the Y axis.
        /// </param>
        /// <param name="zlength">
        /// The length of the box along the Z axis.
        /// </param>
        public void AddBox(Point3D center, double xlength, double ylength, double zlength)
        {
            this.AddBox(center, xlength, ylength, zlength, BoxFaces.All);
        }

        /// <summary>
        /// Adds a box aligned with the X, Y and Z axes.
        /// </summary>
        /// <param name="rectangle">
        /// The 3-D "rectangle".
        /// </param>
        public void AddBox(Rect3D rectangle)
        {
            this.AddBox(
                new Point3D(rectangle.X + (rectangle.SizeX * 0.5), rectangle.Y + (rectangle.SizeY * 0.5), rectangle.Z + (rectangle.SizeZ * 0.5)),
                rectangle.SizeX,
                rectangle.SizeY,
                rectangle.SizeZ,
                BoxFaces.All);
        }

        /// <summary>
        /// Adds a box with the specified faces, aligned with the X, Y and Z axes.
        /// </summary>
        /// <param name="center">
        /// The center point of the box.
        /// </param>
        /// <param name="xlength">
        /// The length of the box along the X axis.
        /// </param>
        /// <param name="ylength">
        /// The length of the box along the Y axis.
        /// </param>
        /// <param name="zlength">
        /// The length of the box along the Z axis.
        /// </param>
        /// <param name="faces">
        /// The faces to include.
        /// </param>
        public void AddBox(Point3D center, double xlength, double ylength, double zlength, BoxFaces faces)
        {
            if ((faces & BoxFaces.Front) == BoxFaces.Front)
            {
                this.AddCubeFace(center, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.Back) == BoxFaces.Back)
            {
                this.AddCubeFace(center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.Left) == BoxFaces.Left)
            {
                this.AddCubeFace(center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.Right) == BoxFaces.Right)
            {
                this.AddCubeFace(center, new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.Top) == BoxFaces.Top)
            {
                this.AddCubeFace(center, new Vector3D(0, 0, 1), new Vector3D(0, 1, 0), zlength, xlength, ylength);
            }

            if ((faces & BoxFaces.Bottom) == BoxFaces.Bottom)
            {
                this.AddCubeFace(center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), zlength, xlength, ylength);
            }
        }

        /// <summary>
        /// Adds a (possibly truncated) cone.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="direction">
        /// The direction (normalization not required).
        /// </param>
        /// <param name="baseRadius">
        /// The base radius.
        /// </param>
        /// <param name="topRadius">
        /// The top radius.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="baseCap">
        /// Include a base cap if set to <c>true</c> .
        /// </param>
        /// <param name="topCap">
        /// Include the top cap if set to <c>true</c> .
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the cone.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Cone_(geometry).
        /// </remarks>
        public void AddCone(
            Point3D origin,
            Vector3D direction,
            double baseRadius,
            double topRadius,
            double height,
            bool baseCap,
            bool topCap,
            int thetaDiv)
        {
            var pc = new PointCollection();
            if (baseCap)
            {
                pc.Add(new Point(0, 0));
            }

            pc.Add(new Point(0, baseRadius));
            pc.Add(new Point(height, topRadius));
            if (topCap)
            {
                pc.Add(new Point(height, 0));
            }

            this.AddRevolvedGeometry(pc, origin, direction, thetaDiv);
        }

        /// <summary>
        /// Adds a cone.
        /// </summary>
        /// <param name="origin">The origin point.</param>
        /// <param name="apex">The apex point.</param>
        /// <param name="baseRadius">The base radius.</param>
        /// <param name="baseCap">
        /// Include a base cap if set to <c>true</c> .
        /// </param>
        /// <param name="thetaDiv">The theta div.</param>
        public void AddCone(Point3D origin, Point3D apex, double baseRadius, bool baseCap, int thetaDiv)
        {
            var dir = apex - origin;
            this.AddCone(origin, dir, baseRadius, 0, dir.Length, baseCap, false, thetaDiv);
        }

        /// <summary>
        /// Adds a cube face.
        /// </summary>
        /// <param name="center">
        /// The center of the cube.
        /// </param>
        /// <param name="normal">
        /// The normal vector for the face.
        /// </param>
        /// <param name="up">
        /// The up vector for the face.
        /// </param>
        /// <param name="dist">
        /// The distance from the center of the cube to the face.
        /// </param>
        /// <param name="width">
        /// The width of the face.
        /// </param>
        /// <param name="height">
        /// The height of the face.
        /// </param>
        public void AddCubeFace(Point3D center, Vector3D normal, Vector3D up, double dist, double width, double height)
        {
            var right = Vector3D.CrossProduct(normal, up);
            var n = normal * dist / 2;
            up *= height / 2;
            right *= width / 2;
            var p1 = center + n - up - right;
            var p2 = center + n - up + right;
            var p3 = center + n + up + right;
            var p4 = center + n + up - right;

            int i0 = this.positions.Count;
            this.positions.Add(p1);
            this.positions.Add(p2);
            this.positions.Add(p3);
            this.positions.Add(p4);
            if (this.normals != null)
            {
                this.normals.Add(normal);
                this.normals.Add(normal);
                this.normals.Add(normal);
                this.normals.Add(normal);
            }

            if (this.textureCoordinates != null)
            {
                this.textureCoordinates.Add(new Point(1, 1));
                this.textureCoordinates.Add(new Point(0, 1));
                this.textureCoordinates.Add(new Point(0, 0));
                this.textureCoordinates.Add(new Point(1, 0));
            }

            this.triangleIndices.Add(i0 + 2);
            this.triangleIndices.Add(i0 + 1);
            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 3);
            this.triangleIndices.Add(i0 + 2);
        }

        /// <summary>
        /// Adds a cylinder to the mesh.
        /// </summary>
        /// <param name="p1">
        /// The first point.
        /// </param>
        /// <param name="p2">
        /// The second point.
        /// </param>
        /// <param name="diameter">
        /// The diameters.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the cylinder.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Cylinder_(geometry).
        /// </remarks>
        public void AddCylinder(Point3D p1, Point3D p2, double diameter, int thetaDiv)
        {
            Vector3D n = p2 - p1;
            double l = n.Length;
            n.Normalize();
            this.AddCone(p1, n, diameter / 2, diameter / 2, l, false, false, thetaDiv);
        }

        /// <summary>
        /// Adds a collection of edges as cylinders.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="edges">
        /// The edge indices.
        /// </param>
        /// <param name="diameter">
        /// The diameter of the cylinders.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the cylinders.
        /// </param>
        public void AddEdges(IList<Point3D> points, IList<int> edges, double diameter, int thetaDiv)
        {
            for (int i = 0; i < edges.Count - 1; i += 2)
            {
                this.AddCylinder(points[edges[i]], points[edges[i + 1]], diameter, thetaDiv);
            }
        }

        /// <summary>
        /// Adds an extruded surface of the specified curve.
        /// </summary>
        /// <param name="points">
        /// The 2D points describing the curve to extrude.
        /// </param>
        /// <param name="xaxis">
        /// The x-axis.
        /// </param>
        /// <param name="p0">
        /// The start origin of the extruded surface.
        /// </param>
        /// <param name="p1">
        /// The end origin of the extruded surface.
        /// </param>
        /// <remarks>
        /// The y-axis is determined by the cross product between the specified x-axis and the p1-p0 vector.
        /// </remarks>
        public void AddExtrudedGeometry(IList<Point> points, Vector3D xaxis, Point3D p0, Point3D p1)
        {
            var ydirection = Vector3D.CrossProduct(xaxis, p1 - p0);
            ydirection.Normalize();
            xaxis.Normalize();

            int index0 = this.positions.Count;
            int np = 2 * points.Count;
            foreach (var p in points)
            {
                var v = (xaxis * p.X) + (ydirection * p.Y);
                this.positions.Add(p0 + v);
                this.positions.Add(p1 + v);
                v.Normalize();
                if (this.normals != null)
                {
                    this.normals.Add(v);
                    this.normals.Add(v);
                }

                if (this.textureCoordinates != null)
                {
                    this.textureCoordinates.Add(new Point(0, 0));
                    this.textureCoordinates.Add(new Point(1, 0));
                }

                int i1 = index0 + 1;
                int i2 = (index0 + 2) % np;
                int i3 = ((index0 + 2) % np) + 1;

                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(index0);

                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i3);
                this.triangleIndices.Add(i2);
            }
        }

        /// <summary>
        /// Adds a lofted surface.
        /// </summary>
        /// <param name="positionsList">
        /// List of lofting sections.
        /// </param>
        /// <param name="normalList">
        /// The normal list.
        /// </param>
        /// <param name="textureCoordinateList">
        /// The texture coordinate list.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Loft_(3D).
        /// </remarks>
        public void AddLoftedGeometry(
            IList<IList<Point3D>> positionsList,
            IList<IList<Vector3D>> normalList,
            IList<IList<Point>> textureCoordinateList)
        {
            int index0 = this.positions.Count;
            int n = -1;
            for (int i = 0; i < positionsList.Count; i++)
            {
                var pc = positionsList[i];

                // check that all curves have same number of points
                if (n == -1)
                {
                    n = pc.Count;
                }

                if (pc.Count != n)
                {
                    throw new InvalidOperationException(AllCurvesShouldHaveTheSameNumberOfPoints);
                }

                // add the points
                foreach (var p in pc)
                {
                    this.positions.Add(p);
                }

                // add normals
                if (this.normals != null && normalList != null)
                {
                    var nc = normalList[i];
                    foreach (var normal in nc)
                    {
                        this.normals.Add(normal);
                    }
                }

                // add texcoords
                if (this.textureCoordinates != null && textureCoordinateList != null)
                {
                    var tc = textureCoordinateList[i];
                    foreach (var t in tc)
                    {
                        this.textureCoordinates.Add(t);
                    }
                }
            }

            for (int i = 0; i + 1 < positionsList.Count; i++)
            {
                for (int j = 0; j + 1 < n; j++)
                {
                    int i0 = index0 + (i * n) + j;
                    int i1 = i0 + n;
                    int i2 = i1 + 1;
                    int i3 = i0 + 1;
                    this.triangleIndices.Add(i0);
                    this.triangleIndices.Add(i1);
                    this.triangleIndices.Add(i2);

                    this.triangleIndices.Add(i2);
                    this.triangleIndices.Add(i3);
                    this.triangleIndices.Add(i0);
                }
            }
        }

        /// <summary>
        /// Adds a single node.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <param name="textureCoordinate">
        /// The texture coordinate.
        /// </param>
        public void AddNode(Point3D position, Vector3D normal, Point textureCoordinate)
        {
            this.positions.Add(position);

            if (this.normals != null)
            {
                this.normals.Add(normal);
            }

            if (this.textureCoordinates != null)
            {
                this.textureCoordinates.Add(textureCoordinate);
            }
        }

        /// <summary>
        /// Adds a (possibly hollow) pipe.
        /// </summary>
        /// <param name="point1">
        /// The start point.
        /// </param>
        /// <param name="point2">
        /// The end point.
        /// </param>
        /// <param name="innerDiameter">
        /// The inner diameter.
        /// </param>
        /// <param name="diameter">
        /// The outer diameter.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the pipe.
        /// </param>
        public void AddPipe(Point3D point1, Point3D point2, double innerDiameter, double diameter, int thetaDiv)
        {
            var dir = point2 - point1;

            double height = dir.Length;
            dir.Normalize();

            var pc = new PointCollection
                {
                    new Point(0, innerDiameter / 2),
                    new Point(0, diameter / 2),
                    new Point(height, diameter / 2),
                    new Point(height, innerDiameter / 2)
                };

            if (innerDiameter > 0)
            {
                // Add the inner surface
                pc.Add(new Point(0, innerDiameter / 2));
            }

            this.AddRevolvedGeometry(pc, point1, dir, thetaDiv);
        }

        /// <summary>
        /// Adds a polygon.
        /// </summary>
        /// <param name="points">
        /// The points of the polygon.
        /// </param>
        /// <remarks>
        /// If the number of points is greater than 4, a triangle fan is used.
        /// </remarks>
        public void AddPolygon(IList<Point3D> points)
        {
            switch (points.Count)
            {
                case 3:
                    this.AddTriangle(points[0], points[1], points[2]);
                    break;
                case 4:
                    this.AddQuad(points[0], points[1], points[2], points[3]);
                    break;
                default:
                    this.AddTriangleFan(points);
                    break;
            }
        }

        /// <summary>
        /// Adds a pyramid.
        /// </summary>
        /// <param name="center">
        /// The center.
        /// </param>
        /// <param name="sideLength">
        /// Length of the sides of the pyramid.
        /// </param>
        /// <param name="height">
        /// The height of the pyramid.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Pyramid_(geometry).
        /// </remarks>
        public void AddPyramid(Point3D center, double sideLength, double height)
        {
            var p1 = new Point3D(center.X - (sideLength * 0.5), center.Y - (sideLength * 0.5), center.Z);
            var p2 = new Point3D(center.X + (sideLength * 0.5), center.Y - (sideLength * 0.5), center.Z);
            var p3 = new Point3D(center.X + (sideLength * 0.5), center.Y + (sideLength * 0.5), center.Z);
            var p4 = new Point3D(center.X - (sideLength * 0.5), center.Y + (sideLength * 0.5), center.Z);
            var p5 = new Point3D(center.X, center.Y, center.Z + height);
            this.AddTriangle(p1, p2, p5);
            this.AddTriangle(p2, p3, p5);
            this.AddTriangle(p3, p4, p5);
            this.AddTriangle(p4, p1, p5);
        }

        /// <summary>
        /// Adds a quadrilateral polygon.
        /// </summary>
        /// <param name="p0">
        /// The first point.
        /// </param>
        /// <param name="p1">
        /// The second point.
        /// </param>
        /// <param name="p2">
        /// The third point.
        /// </param>
        /// <param name="p3">
        /// The fourth point.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Quadrilateral.
        /// </remarks>
        public void AddQuad(Point3D p0, Point3D p1, Point3D p2, Point3D p3)
        {
            //// The nodes are arranged in counter-clockwise order
            //// p3               p2
            //// +---------------+
            //// |               |
            //// |               |
            //// +---------------+
            //// p0               p1
            var uv0 = new Point(0, 0);
            var uv1 = new Point(1, 0);
            var uv2 = new Point(1, 1);
            var uv3 = new Point(0, 1);
            this.AddQuad(p0, p1, p2, p3, uv0, uv1, uv2, uv3);
        }

        /// <summary>
        /// Adds a quadrilateral polygon.
        /// </summary>
        /// <param name="p0">
        /// The first point.
        /// </param>
        /// <param name="p1">
        /// The second point.
        /// </param>
        /// <param name="p2">
        /// The third point.
        /// </param>
        /// <param name="p3">
        /// The fourth point.
        /// </param>
        /// <param name="uv0">
        /// The first texture coordinate.
        /// </param>
        /// <param name="uv1">
        /// The second texture coordinate.
        /// </param>
        /// <param name="uv2">
        /// The third texture coordinate.
        /// </param>
        /// <param name="uv3">
        /// The fourth texture coordinate.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Quadrilateral.
        /// </remarks>
        public void AddQuad(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Point uv0, Point uv1, Point uv2, Point uv3)
        {
            //// The nodes are arranged in counter-clockwise order
            //// p3               p2
            //// +---------------+
            //// |               |
            //// |               |
            //// +---------------+
            //// p0               p1
            int i0 = this.positions.Count;

            this.positions.Add(p0);
            this.positions.Add(p1);
            this.positions.Add(p2);
            this.positions.Add(p3);

            if (this.textureCoordinates != null)
            {
                this.textureCoordinates.Add(uv0);
                this.textureCoordinates.Add(uv1);
                this.textureCoordinates.Add(uv2);
                this.textureCoordinates.Add(uv3);
            }

            if (this.normals != null)
            {
                var w = Vector3D.CrossProduct(p3 - p0, p1 - p0);
                w.Normalize();
                this.normals.Add(w);
                this.normals.Add(w);
                this.normals.Add(w);
                this.normals.Add(w);
            }

            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 1);
            this.triangleIndices.Add(i0 + 2);

            this.triangleIndices.Add(i0 + 2);
            this.triangleIndices.Add(i0 + 3);
            this.triangleIndices.Add(i0 + 0);
        }

        /// <summary>
        /// Adds a list of quadrilateral polygons.
        /// </summary>
        /// <param name="quadPositions">
        /// The points.
        /// </param>
        /// <param name="quadNormals">
        /// The normal vectors.
        /// </param>
        /// <param name="quadTextureCoordinates">
        /// The texture coordinates.
        /// </param>
        public void AddQuads(
            IList<Point3D> quadPositions, IList<Vector3D> quadNormals, IList<Point> quadTextureCoordinates)
        {
            if (quadPositions == null)
            {
                throw new ArgumentNullException("quadPositions");
            }

            if (this.normals != null && quadNormals == null)
            {
                throw new ArgumentNullException("quadNormals");
            }

            if (this.textureCoordinates != null && quadTextureCoordinates == null)
            {
                throw new ArgumentNullException("quadTextureCoordinates");
            }

            if (quadNormals != null && quadNormals.Count != quadPositions.Count)
            {
                throw new InvalidOperationException(WrongNumberOfNormals);
            }

            if (quadTextureCoordinates != null && quadTextureCoordinates.Count != quadPositions.Count)
            {
                throw new InvalidOperationException(WrongNumberOfTextureCoordinates);
            }

            Debug.Assert(quadPositions.Count > 0 && quadPositions.Count % 4 == 0, "Wrong number of positions.");

            int index0 = this.positions.Count;
            foreach (var p in quadPositions)
            {
                this.positions.Add(p);
            }

            if (this.textureCoordinates != null && quadTextureCoordinates != null)
            {
                foreach (var tc in quadTextureCoordinates)
                {
                    this.textureCoordinates.Add(tc);
                }
            }

            if (this.normals != null && quadNormals != null)
            {
                foreach (var n in quadNormals)
                {
                    this.normals.Add(n);
                }
            }

            int indexEnd = this.positions.Count;
            for (int i = index0; i + 3 < indexEnd; i++)
            {
                this.triangleIndices.Add(i);
                this.triangleIndices.Add(i + 1);
                this.triangleIndices.Add(i + 2);

                this.triangleIndices.Add(i + 2);
                this.triangleIndices.Add(i + 3);
                this.triangleIndices.Add(i);
            }
        }

        /// <summary>
        /// Adds a rectangular mesh (m x n points).
        /// </summary>
        /// <param name="points">
        /// The one-dimensional array of points. The points are stored row-by-row.
        /// </param>
        /// <param name="columns">
        /// The number of columns in the rectangular mesh.
        /// </param>
        public void AddRectangularMesh(IList<Point3D> points, int columns)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            int index0 = this.Positions.Count;

            foreach (var pt in points)
            {
                this.positions.Add(pt);
            }

            int rows = points.Count / columns;

            this.AddRectangularMeshTriangleIndices(index0, rows, columns);
            if (this.normals != null)
            {
                this.AddRectangularMeshNormals(index0, rows, columns);
            }

            if (this.textureCoordinates != null)
            {
                this.AddRectangularMeshTextureCoordinates(rows, columns);
            }
        }

        /// <summary>
        /// Adds a rectangular mesh defined by a two-dimensional array of points.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="texCoords">
        /// The texture coordinates (optional).
        /// </param>
        /// <param name="closed0">
        /// set to <c>true</c> if the mesh is closed in the first dimension.
        /// </param>
        /// <param name="closed1">
        /// set to <c>true</c> if the mesh is closed in the second dimension.
        /// </param>
        public void AddRectangularMesh(
            Point3D[,] points, Point[,] texCoords = null, bool closed0 = false, bool closed1 = false)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            int rows = points.GetUpperBound(0) + 1;
            int columns = points.GetUpperBound(1) + 1;
            int index0 = this.positions.Count;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.positions.Add(points[i, j]);
                }
            }

            this.AddRectangularMeshTriangleIndices(index0, rows, columns, closed0, closed1);

            if (this.normals != null)
            {
                this.AddRectangularMeshNormals(index0, rows, columns);
            }

            if (this.textureCoordinates != null)
            {
                if (texCoords != null)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            this.textureCoordinates.Add(texCoords[i, j]);
                        }
                    }
                }
                else
                {
                    this.AddRectangularMeshTextureCoordinates(rows, columns);
                }
            }
        }

        /// <summary>
        /// Adds a regular icosahedron.
        /// </summary>
        /// <param name="center">
        /// The center.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="shareVertices">
        /// Share vertices if set to <c>true</c> .
        /// </param>
        /// <remarks>
        /// See <a href="http://en.wikipedia.org/wiki/Icosahedron">Wikipedia</a> and <a href="http://www.gamedev.net/community/forums/topic.asp?topic_id=283350">link</a>.
        /// </remarks>
        public void AddRegularIcosahedron(Point3D center, double radius, bool shareVertices)
        {
            double a = Math.Sqrt(2.0 / (5.0 + Math.Sqrt(5.0)));

            double b = Math.Sqrt(2.0 / (5.0 - Math.Sqrt(5.0)));

            var icosahedronIndices = new[]
                {
                    1, 4, 0, 4, 9, 0, 4, 5, 9, 8, 5, 4, 1, 8, 4, 1, 10, 8, 10, 3, 8, 8, 3, 5, 3, 2, 5, 3, 7, 2, 3, 10, 7,
                    10, 6, 7, 6, 11, 7, 6, 0, 11, 6, 1, 0, 10, 1, 6, 11, 0, 9, 2, 11, 9, 5, 2, 9, 11, 2, 7
                };

            var icosahedronVertices = new[]
                {
                    new Vector3D(-a, 0, b), new Vector3D(a, 0, b), new Vector3D(-a, 0, -b), new Vector3D(a, 0, -b),
                    new Vector3D(0, b, a), new Vector3D(0, b, -a), new Vector3D(0, -b, a), new Vector3D(0, -b, -a),
                    new Vector3D(b, a, 0), new Vector3D(-b, a, 0), new Vector3D(b, -a, 0), new Vector3D(-b, -a, 0)
                };

            if (shareVertices)
            {
                int index0 = this.positions.Count;
                foreach (var v in icosahedronVertices)
                {
                    this.positions.Add(center + (v * radius));
                }

                foreach (int i in icosahedronIndices)
                {
                    this.triangleIndices.Add(index0 + i);
                }
            }
            else
            {
                for (int i = 0; i + 2 < icosahedronIndices.Length; i += 3)
                {
                    this.AddTriangle(
                        center + (icosahedronVertices[icosahedronIndices[i]] * radius),
                        center + (icosahedronVertices[icosahedronIndices[i + 1]] * radius),
                        center + (icosahedronVertices[icosahedronIndices[i + 2]] * radius));
                }
            }
        }

        /// <summary>
        /// Adds a surface of revolution.
        /// </summary>
        /// <param name="points">
        /// The points (x coordinates are radius, y coordinates are distance from the origin along the axis of revolution)
        /// </param>
        /// <param name="origin">
        /// The origin of the revolution axis.
        /// </param>
        /// <param name="direction">
        /// The direction of the revolution axis.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the mesh.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Surface_of_revolution.
        /// </remarks>
        public void AddRevolvedGeometry(IList<Point> points, Point3D origin, Vector3D direction, int thetaDiv)
        {
            direction.Normalize();

            // Find two unit vectors orthogonal to the specified direction
            var u = direction.FindAnyPerpendicular();
            var v = Vector3D.CrossProduct(direction, u);

            u.Normalize();
            v.Normalize();

            var circle = GetCircle(thetaDiv);

            int index0 = this.positions.Count;
            int n = points.Count;

            int totalNodes = (points.Count - 1) * 2 * thetaDiv;
            int rowNodes = (points.Count - 1) * 2;

            for (int i = 0; i < thetaDiv; i++)
            {
                var w = (v * circle[i].X) + (u * circle[i].Y);

                for (int j = 0; j + 1 < n; j++)
                {
                    // Add segment
                    var q1 = origin + (direction * points[j].X) + (w * points[j].Y);
                    var q2 = origin + (direction * points[j + 1].X) + (w * points[j + 1].Y);

                    // todo:should not add segment if q1==q2 (corner point)
                    // const double eps = 1e-6;
                    // if (Point3D.Subtract(q1, q2).LengthSquared < eps)
                    // continue;
                    double tx = points[j + 1].X - points[j].X;
                    double ty = points[j + 1].Y - points[j].Y;

                    var normal = (-direction * ty) + (w * tx);
                    normal.Normalize();

                    this.positions.Add(q1);
                    this.positions.Add(q2);

                    if (this.normals != null)
                    {
                        this.normals.Add(normal);
                        this.normals.Add(normal);
                    }

                    if (this.textureCoordinates != null)
                    {
                        this.textureCoordinates.Add(new Point((double)i / (thetaDiv - 1), (double)j / (n - 1)));
                        this.textureCoordinates.Add(new Point((double)i / (thetaDiv - 1), (double)(j + 1) / (n - 1)));
                    }

                    int i0 = index0 + (i * rowNodes) + (j * 2);
                    int i1 = i0 + 1;
                    int i2 = index0 + ((((i + 1) * rowNodes) + (j * 2)) % totalNodes);
                    int i3 = i2 + 1;

                    this.triangleIndices.Add(i1);
                    this.triangleIndices.Add(i0);
                    this.triangleIndices.Add(i2);

                    this.triangleIndices.Add(i1);
                    this.triangleIndices.Add(i2);
                    this.triangleIndices.Add(i3);
                }
            }
        }

        /// <summary>
        /// Adds a sphere (by subdividing a regular icosahedron).
        /// </summary>
        /// <param name="center">
        /// The center of the sphere.
        /// </param>
        /// <param name="radius">
        /// The radius of the sphere.
        /// </param>
        /// <param name="subdivisions">
        /// The number of triangular subdivisions of the original icosahedron.
        /// </param>
        /// <remarks>
        /// See <a href="http://www.fho-emden.de/~hoffmann/ikos27042002.pdf">link</a>.
        /// </remarks>
        public void AddSubdivisionSphere(Point3D center, double radius, int subdivisions)
        {
            int p0 = this.positions.Count;
            this.Append(GetUnitSphere(subdivisions));
            int p1 = this.positions.Count;
            for (int i = p0; i < p1; i++)
            {
                this.positions[i] = center + (radius * this.positions[i].ToVector3D());
            }
        }

        /// <summary>
        /// Adds a sphere.
        /// </summary>
        /// <param name="center">
        /// The center of the sphere.
        /// </param>
        /// <param name="radius">
        /// The radius of the sphere.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the sphere.
        /// </param>
        /// <param name="phiDiv">
        /// The number of divisions from top to bottom of the sphere.
        /// </param>
        public void AddSphere(Point3D center, double radius, int thetaDiv = 20, int phiDiv = 10)
        {
            int index0 = this.Positions.Count;
            double dt = 2 * Math.PI / thetaDiv;
            double dp = Math.PI / phiDiv;

            for (int pi = 0; pi <= phiDiv; pi++)
            {
                double phi = pi * dp;

                for (int ti = 0; ti <= thetaDiv; ti++)
                {
                    // we want to start the mesh on the x axis
                    double theta = ti * dt;

                    // Spherical coordinates
                    // http://mathworld.wolfram.com/SphericalCoordinates.html
                    double x = Math.Cos(theta) * Math.Sin(phi);
                    double y = Math.Sin(theta) * Math.Sin(phi);
                    double z = Math.Cos(phi);

                    var p = new Point3D(center.X + (radius * x), center.Y + (radius * y), center.Z + (radius * z));
                    this.positions.Add(p);

                    if (this.normals != null)
                    {
                        var n = new Vector3D(x, y, z);
                        this.normals.Add(n);
                    }

                    if (this.textureCoordinates != null)
                    {
                        var uv = new Point(theta / (2 * Math.PI), phi / Math.PI);
                        this.textureCoordinates.Add(uv);
                    }
                }
            }

            this.AddRectangularMeshTriangleIndices(index0, phiDiv + 1, thetaDiv + 1, true);
        }

        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="p0">
        /// The first point.
        /// </param>
        /// <param name="p1">
        /// The second point.
        /// </param>
        /// <param name="p2">
        /// The third point.
        /// </param>
        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            var uv0 = new Point(0, 0);
            var uv1 = new Point(1, 0);
            var uv2 = new Point(0, 1);
            this.AddTriangle(p0, p1, p2, uv0, uv1, uv2);
        }

        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="p0">
        /// The first point.
        /// </param>
        /// <param name="p1">
        /// The second point.
        /// </param>
        /// <param name="p2">
        /// The third point.
        /// </param>
        /// <param name="uv0">
        /// The first texture coordinate.
        /// </param>
        /// <param name="uv1">
        /// The second texture coordinate.
        /// </param>
        /// <param name="uv2">
        /// The third texture coordinate.
        /// </param>
        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2, Point uv0, Point uv1, Point uv2)
        {
            int i0 = this.positions.Count;

            this.positions.Add(p0);
            this.positions.Add(p1);
            this.positions.Add(p2);

            if (this.textureCoordinates != null)
            {
                this.textureCoordinates.Add(uv0);
                this.textureCoordinates.Add(uv1);
                this.textureCoordinates.Add(uv2);
            }

            if (this.normals != null)
            {
                var w = Vector3D.CrossProduct(p1 - p0, p2 - p0);
                w.Normalize();
                this.normals.Add(w);
                this.normals.Add(w);
                this.normals.Add(w);
            }

            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 1);
            this.triangleIndices.Add(i0 + 2);
        }

        /// <summary>
        /// Adds a triangle fan.
        /// </summary>
        /// <param name="vertices">
        /// The vertex indices of the triangle fan.
        /// </param>
        public void AddTriangleFan(IList<int> vertices)
        {
            for (int i = 0; i + 2 < vertices.Count; i++)
            {
                this.triangleIndices.Add(vertices[0]);
                this.triangleIndices.Add(vertices[i + 1]);
                this.triangleIndices.Add(vertices[i + 2]);
            }
        }

        /// <summary>
        /// Adds a triangle fan to the mesh
        /// </summary>
        /// <param name="fanPositions">
        /// The points of the triangle fan.
        /// </param>
        /// <param name="fanNormals">
        /// The normal vectors of the triangle fan.
        /// </param>
        /// <param name="fanTextureCoordinates">
        /// The texture coordinates of the triangle fan.
        /// </param>
        public void AddTriangleFan(
            IList<Point3D> fanPositions, IList<Vector3D> fanNormals = null, IList<Point> fanTextureCoordinates = null)
        {
            if (this.positions == null)
            {
                throw new ArgumentNullException("fanPositions");
            }

            if (this.normals != null && this.normals == null)
            {
                throw new ArgumentNullException("fanNormals");
            }

            if (this.textureCoordinates != null && this.textureCoordinates == null)
            {
                throw new ArgumentNullException("fanTextureCoordinates");
            }

            int index0 = this.positions.Count;
            foreach (var p in fanPositions)
            {
                this.positions.Add(p);
            }

            if (this.textureCoordinates != null && fanTextureCoordinates != null)
            {
                foreach (var tc in fanTextureCoordinates)
                {
                    this.textureCoordinates.Add(tc);
                }
            }

            if (this.normals != null && fanNormals != null)
            {
                foreach (var n in fanNormals)
                {
                    this.normals.Add(n);
                }
            }

            int indexEnd = this.positions.Count;
            for (int i = index0; i + 2 < indexEnd; i++)
            {
                this.triangleIndices.Add(index0);
                this.triangleIndices.Add(i + 1);
                this.triangleIndices.Add(i + 2);
            }
        }

        /// <summary>
        /// Adds a triangle strip to the mesh.
        /// </summary>
        /// <param name="stripPositions">
        /// The points of the triangle strip.
        /// </param>
        /// <param name="stripNormals">
        /// The normal vectors of the triangle strip.
        /// </param>
        /// <param name="stripTextureCoordinates">
        /// The texture coordinates of the triangle strip.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Triangle_strip.
        /// </remarks>
        public void AddTriangleStrip(
            IList<Point3D> stripPositions,
            IList<Vector3D> stripNormals = null,
            IList<Point> stripTextureCoordinates = null)
        {
            if (stripPositions == null)
            {
                throw new ArgumentNullException("stripPositions");
            }

            if (this.normals != null && stripNormals == null)
            {
                throw new ArgumentNullException("stripNormals");
            }

            if (this.textureCoordinates != null && stripTextureCoordinates == null)
            {
                throw new ArgumentNullException("stripTextureCoordinates");
            }

            if (stripNormals != null && stripNormals.Count != stripPositions.Count)
            {
                throw new InvalidOperationException(WrongNumberOfNormals);
            }

            if (stripTextureCoordinates != null && stripTextureCoordinates.Count != stripPositions.Count)
            {
                throw new InvalidOperationException(WrongNumberOfTextureCoordinates);
            }

            int index0 = this.positions.Count;
            for (int i = 0; i < stripPositions.Count; i++)
            {
                this.positions.Add(stripPositions[i]);
                if (this.normals != null && stripNormals != null)
                {
                    this.normals.Add(stripNormals[i]);
                }

                if (this.textureCoordinates != null && stripTextureCoordinates != null)
                {
                    this.textureCoordinates.Add(stripTextureCoordinates[i]);
                }
            }

            int indexEnd = this.positions.Count;
            for (int i = index0; i + 2 < indexEnd; i += 2)
            {
                this.triangleIndices.Add(i);
                this.triangleIndices.Add(i + 1);
                this.triangleIndices.Add(i + 2);

                if (i + 3 < indexEnd)
                {
                    this.triangleIndices.Add(i + 1);
                    this.triangleIndices.Add(i + 3);
                    this.triangleIndices.Add(i + 2);
                }
            }
        }

        /// <summary>
        /// Adds a polygon specified by vertex index (uses a triangle fan).
        /// </summary>
        /// <param name="vertexIndices">The vertex indices.</param>
        public void AddPolygon(IList<int> vertexIndices)
        {
            int n = vertexIndices.Count;
            for (int i = 0; i + 2 < n; i++)
            {
                this.triangleIndices.Add(vertexIndices[0]);
                this.triangleIndices.Add(vertexIndices[i + 1]);
                this.triangleIndices.Add(vertexIndices[i + 2]);
            }
        }

        /// <summary>
        /// Adds a polygon defined by vertex indices (uses the cutting ears algorithm).
        /// </summary>
        /// <param name="vertexIndices">The vertex indices.</param>
        public void AddPolygonByCuttingEars(IList<int> vertexIndices)
        {
            var points = vertexIndices.Select(vi => this.positions[vi]).ToList();

            var poly3D = new Polygon3D(points);

            // Transform the polygon to 2D
            var poly2D = poly3D.Flatten();

            // Triangulate
            var triangulatedIndices = poly2D.Triangulate();
            if (triangulatedIndices != null)
            {
                foreach (var i in triangulatedIndices)
                {
                    this.triangleIndices.Add(vertexIndices[i]);
                }
            }
        }

        /// <summary>
        /// Adds a list of triangles.
        /// </summary>
        /// <param name="trianglePositions">
        /// The points (the number of points must be a multiple of 3).
        /// </param>
        /// <param name="triangleNormals">
        /// The normal vectors (corresponding to the points).
        /// </param>
        /// <param name="triangleTextureCoordinates">
        /// The texture coordinates (corresponding to the points).
        /// </param>
        public void AddTriangles(
            IList<Point3D> trianglePositions,
            IList<Vector3D> triangleNormals = null,
            IList<Point> triangleTextureCoordinates = null)
        {
            if (trianglePositions == null)
            {
                throw new ArgumentNullException("trianglePositions");
            }

            if (this.normals != null && triangleNormals == null)
            {
                throw new ArgumentNullException("triangleNormals");
            }

            if (this.textureCoordinates != null && triangleTextureCoordinates == null)
            {
                throw new ArgumentNullException("triangleTextureCoordinates");
            }

            if (trianglePositions.Count % 3 != 0)
            {
                throw new InvalidOperationException(WrongNumberOfPositions);
            }

            if (triangleNormals != null && triangleNormals.Count != trianglePositions.Count)
            {
                throw new InvalidOperationException(WrongNumberOfNormals);
            }

            if (triangleTextureCoordinates != null && triangleTextureCoordinates.Count != trianglePositions.Count)
            {
                throw new InvalidOperationException(WrongNumberOfTextureCoordinates);
            }

            int index0 = this.positions.Count;
            foreach (var p in trianglePositions)
            {
                this.positions.Add(p);
            }

            if (this.textureCoordinates != null && triangleTextureCoordinates != null)
            {
                foreach (var tc in triangleTextureCoordinates)
                {
                    this.textureCoordinates.Add(tc);
                }
            }

            if (this.normals != null && triangleNormals != null)
            {
                foreach (var n in triangleNormals)
                {
                    this.normals.Add(n);
                }
            }

            int indexEnd = this.positions.Count;
            for (int i = index0; i < indexEnd; i++)
            {
                this.triangleIndices.Add(i);
            }
        }

        /// <summary>
        /// Adds a tube.
        /// </summary>
        /// <param name="path">
        /// A list of points defining the centers of the tube.
        /// </param>
        /// <param name="values">
        /// The texture coordinate X-values (optional).
        /// </param>
        /// <param name="diameters">
        /// The diameters (optional).
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the tube.
        /// </param>
        /// <param name="isTubeClosed">
        /// Set to true if the tube path is closed.
        /// </param>
        public void AddTube(IList<Point3D> path, double[] values, double[] diameters, int thetaDiv, bool isTubeClosed)
        {
            var circle = GetCircle(thetaDiv);
            this.AddTube(path, values, diameters, circle, isTubeClosed, true);
        }

        /// <summary>
        /// Adds a tube.
        /// </summary>
        /// <param name="path">
        /// A list of points defining the centers of the tube.
        /// </param>
        /// <param name="diameter">
        /// The diameter of the tube.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the tube.
        /// </param>
        /// <param name="isTubeClosed">
        /// Set to true if the tube path is closed.
        /// </param>
        public void AddTube(IList<Point3D> path, double diameter, int thetaDiv, bool isTubeClosed)
        {
            this.AddTube(path, null, new[] { diameter }, thetaDiv, isTubeClosed);
        }

        /// <summary>
        /// Adds a tube with a custom section.
        /// </summary>
        /// <param name="path">
        /// A list of points defining the centers of the tube.
        /// </param>
        /// <param name="values">
        /// The texture coordinate X values (optional).
        /// </param>
        /// <param name="diameters">
        /// The diameters (optional).
        /// </param>
        /// <param name="section">
        /// The section to extrude along the tube path.
        /// </param>
        /// <param name="isTubeClosed">
        /// If the tube is closed set to <c>true</c> .
        /// </param>
        /// <param name="isSectionClosed">
        /// if set to <c>true</c> [is section closed].
        /// </param>
        public void AddTube(
            IList<Point3D> path,
            IList<double> values,
            IList<double> diameters,
            IList<Point> section,
            bool isTubeClosed,
            bool isSectionClosed)
        {
            if (values != null && values.Count == 0)
            {
                throw new InvalidOperationException(WrongNumberOfTextureCoordinates);
            }

            if (diameters != null && diameters.Count == 0)
            {
                throw new InvalidOperationException(WrongNumberOfDiameters);
            }

            int index0 = this.positions.Count;
            int pathLength = path.Count;
            int sectionLength = section.Count;
            if (pathLength < 2 || sectionLength < 2)
            {
                return;
            }

            var up = (path[1] - path[0]).FindAnyPerpendicular();

            int diametersCount = diameters != null ? diameters.Count : 0;
            int valuesCount = values != null ? values.Count : 0;

            for (int i = 0; i < pathLength; i++)
            {
                double r = diameters != null ? diameters[i % diametersCount] / 2 : 1;
                int i0 = i > 0 ? i - 1 : i;
                int i1 = i + 1 < pathLength ? i + 1 : i;

                var forward = path[i1] - path[i0];
                var right = Vector3D.CrossProduct(up, forward);
                up = Vector3D.CrossProduct(forward, right);
                up.Normalize();
                right.Normalize();
                var u = right;
                var v = up;
                for (int j = 0; j < sectionLength; j++)
                {
                    var w = (section[j].X * u * r) + (section[j].Y * v * r);
                    var q = path[i] + w;
                    this.positions.Add(q);
                    if (this.normals != null)
                    {
                        w.Normalize();
                        this.normals.Add(w);
                    }

                    if (this.textureCoordinates != null)
                    {
                        this.textureCoordinates.Add(
                            values != null
                                ? new Point(values[i % valuesCount], (double)j / (sectionLength - 1))
                                : new Point());
                    }
                }
            }

            this.AddRectangularMeshTriangleIndices(index0, pathLength, sectionLength, isSectionClosed, isTubeClosed);
        }

        /// <summary>
        /// Appends the specified mesh.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        public void Append(MeshBuilder mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            this.Append(mesh.positions, mesh.triangleIndices, mesh.normals, mesh.textureCoordinates);
        }

        /// <summary>
        /// Appends the specified mesh.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        public void Append(MeshGeometry3D mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            this.Append(mesh.Positions, mesh.TriangleIndices, mesh.Normals, mesh.TextureCoordinates);
        }

        /// <summary>
        /// Appends the specified points and triangles.
        /// </summary>
        /// <param name="positionsToAppend">
        /// The points to append.
        /// </param>
        /// <param name="triangleIndicesToAppend">
        /// The triangle indices to append.
        /// </param>
        /// <param name="normalsToAppend">
        /// The normal vectors to append.
        /// </param>
        /// <param name="textureCoordinatesToAppend">
        /// The texture coordinates to append.
        /// </param>
        public void Append(
            IList<Point3D> positionsToAppend,
            IList<int> triangleIndicesToAppend,
            IList<Vector3D> normalsToAppend = null,
            IList<Point> textureCoordinatesToAppend = null)
        {
            if (positionsToAppend == null)
            {
                throw new ArgumentNullException("positionsToAppend");
            }

            if (this.normals != null && normalsToAppend == null)
            {
                throw new InvalidOperationException(SourceMeshNormalsShouldNotBeNull);
            }

            if (this.textureCoordinates != null && textureCoordinatesToAppend == null)
            {
                throw new InvalidOperationException(SourceMeshTextureCoordinatesShouldNotBeNull);
            }

            if (normalsToAppend != null && normalsToAppend.Count != positionsToAppend.Count)
            {
                throw new InvalidOperationException(WrongNumberOfNormals);
            }

            if (textureCoordinatesToAppend != null && textureCoordinatesToAppend.Count != positionsToAppend.Count)
            {
                throw new InvalidOperationException(WrongNumberOfTextureCoordinates);
            }

            int index0 = this.positions.Count;
            foreach (var p in positionsToAppend)
            {
                this.positions.Add(p);
            }

            if (this.normals != null && normalsToAppend != null)
            {
                foreach (var n in normalsToAppend)
                {
                    this.normals.Add(n);
                }
            }

            if (this.textureCoordinates != null && textureCoordinatesToAppend != null)
            {
                foreach (var t in textureCoordinatesToAppend)
                {
                    this.textureCoordinates.Add(t);
                }
            }

            foreach (int i in triangleIndicesToAppend)
            {
                this.triangleIndices.Add(index0 + i);
            }
        }

        /// <summary>
        /// Chamfers the specified corner (experimental code).
        /// </summary>
        /// <param name="p">
        /// The corner point.
        /// </param>
        /// <param name="d">
        /// The chamfer distance.
        /// </param>
        /// <param name="eps">
        /// The corner search limit distance.
        /// </param>
        /// <param name="chamferPoints">
        /// If this parameter is provided, the collection will be filled with the generated chamfer points.
        /// </param>
        public void ChamferCorner(Point3D p, double d, double eps = 1e-6, IList<Point3D> chamferPoints = null)
        {
            this.NoSharedVertices();

            this.normals = null;
            this.textureCoordinates = null;

            var cornerNormal = this.FindCornerNormal(p, eps);

            var newCornerPoint = p - (cornerNormal * d);
            int index0 = this.positions.Count;
            this.positions.Add(newCornerPoint);

            var plane = new Plane3D(newCornerPoint, cornerNormal);

            int ntri = this.triangleIndices.Count;

            for (int i = 0; i < ntri; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                var p0 = this.positions[this.triangleIndices[i0]];
                var p1 = this.positions[this.triangleIndices[i1]];
                var p2 = this.positions[this.triangleIndices[i2]];
                double d0 = (p - p0).LengthSquared;
                double d1 = (p - p1).LengthSquared;
                double d2 = (p - p2).LengthSquared;
                double mind = Math.Min(d0, Math.Min(d1, d2));
                if (mind > eps)
                {
                    continue;
                }

                if (d1 < eps)
                {
                    i0 = i + 1;
                    i1 = i + 2;
                    i2 = i;
                }

                if (d2 < eps)
                {
                    i0 = i + 2;
                    i1 = i;
                    i2 = i + 1;
                }

                p0 = this.positions[this.triangleIndices[i0]];
                p1 = this.positions[this.triangleIndices[i1]];
                p2 = this.positions[this.triangleIndices[i2]];

                // p0 is the corner vertex (at index i0)
                // find the intersections between the chamfer plane and the two edges connected to the corner
                var p01 = plane.LineIntersection(p0, p1);
                var p02 = plane.LineIntersection(p0, p2);

                if (p01 == null)
                {
                    continue;
                }

                if (p02 == null)
                {
                    continue;
                }

                if (chamferPoints != null)
                {
                    // add the chamfered points
                    if (!chamferPoints.Contains(p01.Value))
                    {
                        chamferPoints.Add(p01.Value);
                    }

                    if (!chamferPoints.Contains(p02.Value))
                    {
                        chamferPoints.Add(p02.Value);
                    }
                }

                int i01 = i0;

                // change the original triangle to use the first chamfer point
                this.positions[this.triangleIndices[i01]] = p01.Value;

                int i02 = this.positions.Count;
                this.positions.Add(p02.Value);

                // add a new triangle for the other chamfer point
                this.triangleIndices.Add(i01);
                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(i02);

                // add a triangle connecting the chamfer points and the new corner point
                this.triangleIndices.Add(index0);
                this.triangleIndices.Add(i01);
                this.triangleIndices.Add(i02);
            }

            this.NoSharedVertices();
        }

        /// <summary>
        /// Checks the performance limits.
        /// </summary>
        /// <remarks>
        /// See <a href="http://msdn.microsoft.com/en-us/library/bb613553.aspx">MSDN</a>.
        /// Try to keep mesh sizes under these limits:
        /// Positions : 20,001 point instances
        /// TriangleIndices : 60,003 integer instances
        /// </remarks>
        public void CheckPerformanceLimits()
        {
            if (this.positions.Count > 20000)
            {
                Trace.WriteLine(string.Format("Too many positions ({0}).", this.positions.Count));
            }

            if (this.triangleIndices.Count > 60002)
            {
                Trace.WriteLine(string.Format("Too many triangle indices ({0}).", this.triangleIndices.Count));
            }
        }

        /// <summary>
        /// Scales the positions (and normal vectors).
        /// </summary>
        /// <param name="scaleX">
        /// The X scale factor.
        /// </param>
        /// <param name="scaleY">
        /// The Y scale factor.
        /// </param>
        /// <param name="scaleZ">
        /// The Z scale factor.
        /// </param>
        public void Scale(double scaleX, double scaleY, double scaleZ)
        {
            for (int i = 0; i < this.Positions.Count; i++)
            {
                this.Positions[i] = new Point3D(
                    this.Positions[i].X * scaleX, this.Positions[i].Y * scaleY, this.Positions[i].Z * scaleZ);
            }

            if (this.Normals != null)
            {
                for (int i = 0; i < this.Normals.Count; i++)
                {
                    this.Normals[i] = new Vector3D(
                        this.Normals[i].X * scaleX, this.Normals[i].Y * scaleY, this.Normals[i].Z * scaleZ);
                    this.Normals[i].Normalize();
                }
            }
        }

        /// <summary>
        /// Performs a linear subdivision of the mesh.
        /// </summary>
        /// <param name="barycentric">
        /// Barycentric(?) if set to <c>true</c> .
        /// </param>
        public void SubdivideLinear(bool barycentric = false)
        {
            if (barycentric)
            {
                this.SubdivideBarycentric();
            }
            else
            {
                this.Subdivide4();
            }
        }

        /// <summary>
        /// Converts the geometry to a <see cref="MeshGeometry3D"/> .
        /// </summary>
        /// <param name="freeze">
        /// freeze the mesh if set to <c>true</c> .
        /// </param>
        /// <returns>
        /// A mesh geometry.
        /// </returns>
        public MeshGeometry3D ToMesh(bool freeze = false)
        {
            if (this.normals != null && this.positions.Count != this.normals.Count)
            {
                throw new InvalidOperationException(WrongNumberOfNormals);
            }

            if (this.textureCoordinates != null && this.positions.Count != this.textureCoordinates.Count)
            {
                throw new InvalidOperationException(WrongNumberOfTextureCoordinates);
            }

            var mg = new MeshGeometry3D
                {
                    Positions = this.positions,
                    TriangleIndices = this.triangleIndices,
                    Normals = this.normals,
                    TextureCoordinates = this.textureCoordinates
                };
            if (freeze)
            {
                mg.Freeze();
            }

            return mg;
        }

        /// <summary>
        /// Gets a unit sphere from the cache.
        /// </summary>
        /// <param name="subdivisions">
        /// The number of subdivisions.
        /// </param>
        /// <returns>
        /// A unit sphere mesh.
        /// </returns>
        private static MeshGeometry3D GetUnitSphere(int subdivisions)
        {
            if (UnitSphereCache.ContainsKey(subdivisions))
            {
                return UnitSphereCache[subdivisions];
            }

            var mb = new MeshBuilder(false, false);
            mb.AddRegularIcosahedron(new Point3D(), 1, false);
            for (int i = 0; i < subdivisions; i++)
            {
                mb.SubdivideLinear();
            }

            for (int i = 0; i < mb.positions.Count; i++)
            {
                var v = mb.Positions[i].ToVector3D();
                v.Normalize();
                mb.Positions[i] = v.ToPoint3D();
            }

            var mesh = mb.ToMesh();
            UnitSphereCache[subdivisions] = mesh;
            return mesh;
        }

        /// <summary>
        /// Adds normal vectors for a rectangular mesh.
        /// </summary>
        /// <param name="index0">
        /// The index 0.
        /// </param>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        private void AddRectangularMeshNormals(int index0, int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                int i1 = i + 1;
                if (i1 == rows)
                {
                    i1--;
                }

                int i0 = i1 - 1;
                for (int j = 0; j < columns; j++)
                {
                    int j1 = j + 1;
                    if (j1 == columns)
                    {
                        j1--;
                    }

                    int j0 = j1 - 1;
                    var u = Point3D.Subtract(
                        this.positions[index0 + (i1 * columns) + j0], this.positions[index0 + (i0 * columns) + j0]);
                    var v = Point3D.Subtract(
                        this.positions[index0 + (i0 * columns) + j1], this.positions[index0 + (i0 * columns) + j0]);
                    var normal = Vector3D.CrossProduct(u, v);
                    normal.Normalize();
                    this.normals.Add(normal);
                }
            }
        }

        /// <summary>
        /// Adds texture coordinates for a rectangular mesh.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        private void AddRectangularMeshTextureCoordinates(int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                double v = (double)i / (rows - 1);
                for (int j = 0; j < columns; j++)
                {
                    double u = (double)j / (columns - 1);
                    this.textureCoordinates.Add(new Point(u, v));
                }
            }
        }

        /// <summary>
        /// Add triangle indices for a rectangular mesh.
        /// </summary>
        /// <param name="index0">
        /// The index offset.
        /// </param>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <param name="isSpherical">
        /// set the flag to true to create a sphere mesh (triangles at top and bottom).
        /// </param>
        private void AddRectangularMeshTriangleIndices(int index0, int rows, int columns, bool isSpherical = false)
        {
            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < columns - 1; j++)
                {
                    int ij = (i * columns) + j;
                    if (!isSpherical || i > 0)
                    {
                        this.triangleIndices.Add(index0 + ij);
                        this.triangleIndices.Add(index0 + ij + 1 + columns);
                        this.triangleIndices.Add(index0 + ij + 1);
                    }

                    if (!isSpherical || i < rows - 2)
                    {
                        this.triangleIndices.Add(index0 + ij + 1 + columns);
                        this.triangleIndices.Add(index0 + ij);
                        this.triangleIndices.Add(index0 + ij + columns);
                    }
                }
            }
        }

        /// <summary>
        /// Adds triangular indices for a rectangular mesh.
        /// </summary>
        /// <param name="index0">
        /// The index 0.
        /// </param>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <param name="rowsClosed">
        /// True if rows are closed.
        /// </param>
        /// <param name="columnsClosed">
        /// True if columns are closed.
        /// </param>
        private void AddRectangularMeshTriangleIndices(
            int index0, int rows, int columns, bool rowsClosed, bool columnsClosed)
        {
            int m2 = rows - 1;
            int n2 = columns - 1;
            if (columnsClosed)
            {
                m2++;
            }

            if (rowsClosed)
            {
                n2++;
            }

            for (int i = 0; i < m2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    int i00 = index0 + (i * columns) + j;
                    int i01 = index0 + (i * columns) + ((j + 1) % columns);
                    int i10 = index0 + (((i + 1) % rows) * columns) + j;
                    int i11 = index0 + (((i + 1) % rows) * columns) + ((j + 1) % columns);
                    this.triangleIndices.Add(i00);
                    this.triangleIndices.Add(i11);
                    this.triangleIndices.Add(i01);

                    this.triangleIndices.Add(i11);
                    this.triangleIndices.Add(i00);
                    this.triangleIndices.Add(i10);
                }
            }
        }

        /// <summary>
        /// Finds the average normal to the specified corner (experimental code).
        /// </summary>
        /// <param name="p">
        /// The corner point.
        /// </param>
        /// <param name="eps">
        /// The corner search limit distance.
        /// </param>
        /// <returns>
        /// The normal.
        /// </returns>
        private Vector3D FindCornerNormal(Point3D p, double eps)
        {
            var sum = new Vector3D();
            int count = 0;
            var addedNormals = new HashSet<Vector3D>();
            for (int i = 0; i < this.triangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                var p0 = this.positions[this.triangleIndices[i0]];
                var p1 = this.positions[this.triangleIndices[i1]];
                var p2 = this.positions[this.triangleIndices[i2]];

                // check if any of the vertices are on the corner
                double d0 = (p - p0).LengthSquared;
                double d1 = (p - p1).LengthSquared;
                double d2 = (p - p2).LengthSquared;
                double mind = Math.Min(d0, Math.Min(d1, d2));
                if (mind > eps)
                {
                    continue;
                }

                // calculate the triangle normal and check if this face is already added
                var normal = Vector3D.CrossProduct(p1 - p0, p2 - p0);
                normal.Normalize();

                // todo: need to use the epsilon value to compare the normals?
                if (addedNormals.Contains(normal))
                {
                    continue;
                }

                // todo: this does not work yet
                // double dp = 1;
                // foreach (var n in addedNormals)
                // {
                // dp = Math.Abs(Vector3D.DotProduct(n, normal) - 1);
                // if (dp < eps)
                // continue;
                // }
                // if (dp < eps)
                // {
                // continue;
                // }
                count++;
                sum += normal;
                addedNormals.Add(normal);
            }

            if (count == 0)
            {
                return new Vector3D();
            }

            return sum * (1.0 / count);
        }

        /// <summary>
        /// Makes sure no triangles share the same vertex.
        /// </summary>
        private void NoSharedVertices()
        {
            var p = new Point3DCollection();
            var ti = new Int32Collection();
            Vector3DCollection n = null;
            if (this.normals != null)
            {
                n = new Vector3DCollection();
            }

            PointCollection tc = null;
            if (this.textureCoordinates != null)
            {
                tc = new PointCollection();
            }

            for (int i = 0; i < this.triangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                int index0 = this.triangleIndices[i0];
                int index1 = this.triangleIndices[i1];
                int index2 = this.triangleIndices[i2];
                var p0 = this.positions[index0];
                var p1 = this.positions[index1];
                var p2 = this.positions[index2];
                p.Add(p0);
                p.Add(p1);
                p.Add(p2);
                ti.Add(i0);
                ti.Add(i1);
                ti.Add(i2);
                if (n != null)
                {
                    n.Add(this.normals[index0]);
                    n.Add(this.normals[index1]);
                    n.Add(this.normals[index2]);
                }

                if (tc != null)
                {
                    tc.Add(this.textureCoordinates[index0]);
                    tc.Add(this.textureCoordinates[index1]);
                    tc.Add(this.textureCoordinates[index2]);
                }
            }

            this.positions = p;
            this.triangleIndices = ti;
            this.normals = n;
            this.textureCoordinates = tc;
        }

        /// <summary>
        /// Subdivides each triangle into four sub-triangles.
        /// </summary>
        private void Subdivide4()
        {
            // Each triangle is divided into four subtriangles, adding new vertices in the middle of each edge.
            int ip = this.Positions.Count;
            int ntri = this.TriangleIndices.Count;
            for (int i = 0; i < ntri; i += 3)
            {
                int i0 = this.TriangleIndices[i];
                int i1 = this.TriangleIndices[i + 1];
                int i2 = this.TriangleIndices[i + 2];
                var p0 = this.Positions[i0];
                var p1 = this.Positions[i1];
                var p2 = this.Positions[i2];
                var v01 = p1 - p0;
                var v12 = p2 - p1;
                var v20 = p0 - p2;
                var p01 = p0 + (v01 * 0.5);
                var p12 = p1 + (v12 * 0.5);
                var p20 = p2 + (v20 * 0.5);

                int i01 = ip++;
                int i12 = ip++;
                int i20 = ip++;

                this.Positions.Add(p01);
                this.Positions.Add(p12);
                this.Positions.Add(p20);

                if (this.normals != null)
                {
                    var n = this.Normals[i0];
                    this.Normals.Add(n);
                    this.Normals.Add(n);
                    this.Normals.Add(n);
                }

                if (this.textureCoordinates != null)
                {
                    var uv0 = this.TextureCoordinates[i0];
                    var uv1 = this.TextureCoordinates[i0 + 1];
                    var uv2 = this.TextureCoordinates[i0 + 2];
                    var t01 = uv1 - uv0;
                    var t12 = uv2 - uv1;
                    var t20 = uv0 - uv2;
                    var u01 = uv0 + (t01 * 0.5);
                    var u12 = uv1 + (t12 * 0.5);
                    var u20 = uv2 + (t20 * 0.5);
                    this.TextureCoordinates.Add(u01);
                    this.TextureCoordinates.Add(u12);
                    this.TextureCoordinates.Add(u20);
                }

                // TriangleIndices[i ] = i0;
                this.TriangleIndices[i + 1] = i01;
                this.TriangleIndices[i + 2] = i20;

                this.TriangleIndices.Add(i01);
                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(i12);

                this.TriangleIndices.Add(i12);
                this.TriangleIndices.Add(i2);
                this.TriangleIndices.Add(i20);

                this.TriangleIndices.Add(i01);
                this.TriangleIndices.Add(i12);
                this.TriangleIndices.Add(i20);
            }
        }

        /// <summary>
        /// Subdivides each triangle into six triangles. Adds a vertex at the midpoint of each triangle.
        /// </summary>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Barycentric_subdivision
        /// </remarks>
        private void SubdivideBarycentric()
        {
            // The BCS of a triangle S divides it into six triangles; each part has one vertex v2 at the
            // barycenter of S, another one v1 at the midpoint of some side, and the last one v0 at one
            // of the original vertices.
            int im = this.Positions.Count;
            int ntri = this.TriangleIndices.Count;
            for (int i = 0; i < ntri; i += 3)
            {
                int i0 = this.TriangleIndices[i];
                int i1 = this.TriangleIndices[i + 1];
                int i2 = this.TriangleIndices[i + 2];
                var p0 = this.Positions[i0];
                var p1 = this.Positions[i1];
                var p2 = this.Positions[i2];
                var v01 = p1 - p0;
                var v12 = p2 - p1;
                var v20 = p0 - p2;
                var p01 = p0 + (v01 * 0.5);
                var p12 = p1 + (v12 * 0.5);
                var p20 = p2 + (v20 * 0.5);
                var m = new Point3D((p0.X + p1.X + p2.X) / 3, (p0.Y + p1.Y + p2.Y) / 3, (p0.Z + p1.Z + p2.Z) / 3);

                int i01 = im + 1;
                int i12 = im + 2;
                int i20 = im + 3;

                this.Positions.Add(m);
                this.Positions.Add(p01);
                this.Positions.Add(p12);
                this.Positions.Add(p20);

                if (this.normals != null)
                {
                    var n = this.Normals[i0];
                    this.Normals.Add(n);
                    this.Normals.Add(n);
                    this.Normals.Add(n);
                    this.Normals.Add(n);
                }

                if (this.textureCoordinates != null)
                {
                    var uv0 = this.TextureCoordinates[i0];
                    var uv1 = this.TextureCoordinates[i0 + 1];
                    var uv2 = this.TextureCoordinates[i0 + 2];
                    var t01 = uv1 - uv0;
                    var t12 = uv2 - uv1;
                    var t20 = uv0 - uv2;
                    var u01 = uv0 + (t01 * 0.5);
                    var u12 = uv1 + (t12 * 0.5);
                    var u20 = uv2 + (t20 * 0.5);
                    var uvm = new Point((uv0.X + uv1.X) * 0.5, (uv0.Y + uv1.Y) * 0.5);
                    this.TextureCoordinates.Add(uvm);
                    this.TextureCoordinates.Add(u01);
                    this.TextureCoordinates.Add(u12);
                    this.TextureCoordinates.Add(u20);
                }

                // TriangleIndices[i ] = i0;
                this.TriangleIndices[i + 1] = i01;
                this.TriangleIndices[i + 2] = im;

                this.TriangleIndices.Add(i01);
                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(im);

                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(i12);
                this.TriangleIndices.Add(im);

                this.TriangleIndices.Add(i12);
                this.TriangleIndices.Add(i2);
                this.TriangleIndices.Add(im);

                this.TriangleIndices.Add(i2);
                this.TriangleIndices.Add(i20);
                this.TriangleIndices.Add(im);

                this.TriangleIndices.Add(i20);
                this.TriangleIndices.Add(i0);
                this.TriangleIndices.Add(im);

                im += 4;
            }
        }
    }
}