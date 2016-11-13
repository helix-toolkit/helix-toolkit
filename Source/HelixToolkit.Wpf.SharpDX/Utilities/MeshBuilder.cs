// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilder.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Box face enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Core;

    using Plane3D = global::SharpDX.Plane;
    using Ray3D = global::SharpDX.Ray;
    using Ray = global::SharpDX.Ray;
    using Rect3D = System.Windows.Media.Media3D.Rect3D;
    using System.Collections.Concurrent;

    // TODO
    //[Flags]
    //public enum GeometryFlags
    //{
    //    HasNormals = 0x1,
    //    HasTexCoords = 0x2,    
    //}

    /// <summary>
    /// Box face enumeration.
    /// </summary>
    [Flags]
    public enum BoxFaces
    {
        /// <summary>
        /// The top.
        /// </summary>
        PositiveZ = 0x1,

        /// <summary>
        /// The bottom.
        /// </summary>
        NegativeZ = 0x2,

        /// <summary>
        /// The left side.
        /// </summary>
        NegativeY = 0x4,

        /// <summary>
        /// The right side.
        /// </summary>
        PositiveY = 0x8,

        /// <summary>
        /// The front side.
        /// </summary>
        PositiveX = 0x10,

        /// <summary>
        /// The back side.
        /// </summary>
        NegativeX = 0x20,

        /// <summary>
        /// All sides.
        /// </summary>
        All = PositiveZ | NegativeZ | NegativeY | PositiveY | PositiveX | NegativeX
    }

    /// <summary>
    /// Builds MeshGeometry3D objects.
    /// </summary>
    /// <remarks>
    /// Performance tips for MeshGeometry3D (http://msdn.microsoft.com/en-us/library/bb613553.aspx)
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
        /// 'Source mesh normals should not be null' exception message.
        /// </summary>
        private const string SourceMeshNormalsShouldNotBeNull = "Source mesh normals should not be null.";

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
        /// 'Wrong number of normals' exception message.
        /// </summary>
        private const string WrongNumberOfNormals = "Wrong number of normals.";

        /// <summary>
        /// 'Wrong number of texture coordinates' exception message.
        /// </summary>
        private const string WrongNumberOfTextureCoordinates = "Wrong number of texture coordinates.";

        /// <summary>
        /// The circle cache.
        /// </summary>
        private static readonly ConcurrentDictionary<int, IList<Vector2>> CircleCache = new ConcurrentDictionary<int, IList<Vector2>>();
        /// <summary>
        /// The circle cache.
        /// </summary>
        private static readonly ConcurrentDictionary<int, IList<Vector2>> ClosedCircleCache = new ConcurrentDictionary<int, IList<Vector2>>();

        /// <summary>
        /// The unit sphere cache.
        /// </summary>
        private static readonly Dictionary<int, MeshGeometry3D> UnitSphereCache = new Dictionary<int, MeshGeometry3D>();

        private Vector3Collection positions;
        private Vector3Collection normals;
        private Vector3Collection tangents;
        private Vector3Collection bitangents;
        private Vector2Collection textureCoordinates;
        private IntCollection triangleIndices;

        public Vector3Collection Positions { get { return this.positions; } }

        public Vector3Collection Normals { get { return this.normals; } set { this.normals = value; } }

        public Vector3Collection Tangents { get { return this.tangents; } set { this.tangents = value; } }

        public Vector3Collection BiTangents { get { return this.bitangents; } set { this.bitangents = value; } }

        public Vector2Collection TextureCoordinates { get { return this.textureCoordinates; } set { this.textureCoordinates = value; } }

        public IntCollection TriangleIndices { get { return this.triangleIndices;} }

        public bool HasNormals { get { return this.normals != null; } }
        
        public bool HasTexCoords { get { return this.textureCoordinates != null; } }
        
        public bool HasTangents { get { return this.tangents != null; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshBuilder"/> class.
        /// </summary>
        /// <remarks>
        /// Normal and texture coordinate generation are included.
        /// </remarks>
        public MeshBuilder(bool generateNormals = true, bool generateTexCoords = true, bool tangentSpace = false)
        {
            this.positions = new Vector3Collection();
            this.triangleIndices = new IntCollection();

            if (generateNormals)
            {
                this.normals = new Vector3Collection();
            }

            if (generateTexCoords)
            {
                this.textureCoordinates = new Vector2Collection();
            }

            if (tangentSpace)
            {
                this.tangents = new Vector3Collection();
                this.bitangents = new Vector3Collection();
            }
        }

        public void ComputeNormalsAndTangents(MeshFaces meshFaces, bool tangents = false)
        {
            if (!this.HasNormals & this.positions != null & this.triangleIndices != null)
            {
                ComputeNormals(this.positions, this.triangleIndices, out this.normals);
            }

            switch (meshFaces)
            {
                case MeshFaces.Default:
                    if (tangents & this.HasNormals & this.textureCoordinates != null)
                    {
                        Vector3Collection t1, t2;
                        ComputeTangents(this.positions, this.normals, this.textureCoordinates, this.triangleIndices, out t1, out t2);
                        this.tangents = t1;
                        this.bitangents = t2;
                    }
                    break;
                case MeshFaces.QuadPatches:
                    if (tangents & this.HasNormals & this.textureCoordinates != null)
                    {
                        Vector3Collection t1, t2;
                        ComputeTangentsQuads(this.positions, this.normals, this.textureCoordinates, this.triangleIndices, out t1, out t2);
                        this.tangents = t1;
                        this.bitangents = t2;
                    }
                    break;
                default:
                    break;
            }
        }
        
        public void ComputeTangents(MeshFaces meshFaces)
        {
            switch (meshFaces)
            {
                case MeshFaces.Default:
                    if (this.positions != null & this.triangleIndices != null & this.normals != null & this.textureCoordinates != null)
                    {
                        Vector3Collection t1, t2;
                        ComputeTangents(this.positions, this.normals, this.textureCoordinates, this.triangleIndices, out t1, out t2);
                        this.tangents = t1;
                        this.bitangents = t2;
                    }
                    break;
                case MeshFaces.QuadPatches:
                    if (this.positions != null & this.triangleIndices != null & this.normals != null & this.textureCoordinates != null)
                    {
                        Vector3Collection t1, t2;
                        ComputeTangentsQuads(this.positions, this.normals, this.textureCoordinates, this.triangleIndices, out t1, out t2);
                        this.tangents = t1;
                        this.bitangents = t2;
                    }
                    break;
                default:
                    break;
            }

        }

        private static void ComputeNormals(Vector3Collection positions, IntCollection triangleIndices, out Vector3Collection normals)
        {
            normals = new Vector3Collection(positions.Count);
            normals.AddRange(Enumerable.Repeat(Vector3.Zero, positions.Count));

            for (int t = 0; t < triangleIndices.Count; t += 3)
            {
                var i1 = triangleIndices[t];
                var i2 = triangleIndices[t + 1];
                var i3 = triangleIndices[t + 2];

                var v1 = positions[i1];
                var v2 = positions[i2];
                var v3 = positions[i3];

                var p1 = v2 - v1;
                var p2 = v3 - v1;
                var n = Vector3.Cross(p1, p2);
                // angle
                p1.Normalize();
                p2.Normalize();
                var a = (float)Math.Acos(Vector3.Dot(p1, p2));
                n.Normalize();
                normals[i1] += (a * n);
                normals[i2] += (a * n);
                normals[i3] += (a * n);
            }

            for (int i = 0; i < normals.Count; i++)
            {
                normals[i].Normalize();
            }
        }

        /// <summary>
        /// Tangent Space computation for IndexedTriangle meshes
        /// Based on:
        /// http://www.terathon.com/code/tangent.html
        /// </summary>
        public static void ComputeTangents(IList<Vector3> positions, IList<Vector3> normals, IList<Vector2> textureCoordinates, IList<int> triangleIndices,
            out Vector3Collection tangents, out Vector3Collection bitangents)
        {

            var tan1 = new Vector3[positions.Count];
            //var tan2 = new Vector3[positions.Count];
            for (int t = 0; t < triangleIndices.Count; t += 3)
            {
                var i1 = triangleIndices[t];
                var i2 = triangleIndices[t + 1];
                var i3 = triangleIndices[t + 2];

                var v1 = positions[i1];
                var v2 = positions[i2];
                var v3 = positions[i3];

                var w1 = textureCoordinates[i1];
                var w2 = textureCoordinates[i2];
                var w3 = textureCoordinates[i3];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                var udir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                //var vdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += udir;
                tan1[i2] += udir;
                tan1[i3] += udir;

                //tan2[i1] += vdir;
                //tan2[i2] += vdir;
                //tan2[i3] += vdir;
            }

            tangents = new Vector3Collection(positions.Count);
            bitangents = new Vector3Collection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                var n = normals[i];
                var t = tan1[i];
                t = (t - n * Vector3.Dot(n, t));
                t.Normalize();
                var b = Vector3.Cross(n, t);
                tangents.Add(t);
                bitangents.Add(b);
            }
        }

        public static void ComputeTangentsQuads(IList<Vector3> positions, IList<Vector3> normals, IList<Vector2> textureCoordinates, IList<int> indices,
            out Vector3Collection tangents, out Vector3Collection bitangents)
        {

            var tan1 = new Vector3[positions.Count];
            
            for (int t = 0; t < indices.Count; t += 4)
            {
                var i1 = indices[t];
                var i2 = indices[t + 1];
                var i3 = indices[t + 2];
                var i4 = indices[t + 3];

                var v1 = positions[i1];
                var v2 = positions[i2];
                var v3 = positions[i3];
                var v4 = positions[i4];

                var w1 = textureCoordinates[i1];
                var w2 = textureCoordinates[i2];
                var w3 = textureCoordinates[i3];
                var w4 = textureCoordinates[i4];

                float x1 = v2.X - v1.X;
                float x2 = v4.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v4.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v4.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w4.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w4.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                var udir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                //var vdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += udir;
                tan1[i2] += udir;
                tan1[i3] += udir;
                tan1[i4] += udir;

                //tan2[i1] += vdir;
                //tan2[i2] += vdir;
                //tan2[i3] += vdir;
            }

            tangents = new Vector3Collection(positions.Count);
            bitangents = new Vector3Collection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                var n = normals[i];
                var t = tan1[i];
                t = (t - n * Vector3.Dot(n, t));
                t.Normalize();
                var b = Vector3.Cross(n, t);
                tangents.Add(t);
                bitangents.Add(b);
            }
        }

        public static void ComputeTangents(MeshGeometry3D meshGeometry)
        {
            Vector3Collection t1, t2;
            ComputeTangents(meshGeometry.Positions, meshGeometry.Normals, meshGeometry.TextureCoordinates, meshGeometry.Indices, out t1, out t2);
            meshGeometry.Tangents = t1;
            meshGeometry.BiTangents = t2;
        }

        /// <summary>
        /// Converts the geometry to a <see cref="MeshGeometry3D"/> .        
        /// </summary>
        public MeshGeometry3D ToMeshGeometry3D()
        {
            if (this.HasTangents && this.tangents.Count==0)
            {
                Vector3Collection tan, bitan;
                ComputeTangents(this.positions, this.normals, this.textureCoordinates, this.triangleIndices, out tan, out bitan);
                this.tangents.AddRange(tan);
                this.bitangents.AddRange(bitan);
            }

            return new MeshGeometry3D()
            {
                Positions = this.positions,
                Indices = this.triangleIndices,
                Normals = (this.HasNormals) ? this.normals : null,
                TextureCoordinates = (this.HasTexCoords) ? this.textureCoordinates : null,
                Tangents = (this.HasTangents) ? this.tangents : null,
                BiTangents = (this.HasTangents) ? this.bitangents : null,
            };
        }
#if Sphere

                
        private static Vector3 GetPosition(double theta, double phi, double radius)
        {
            double x = radius * Math.Sin(theta) * Math.Sin(phi);
            double y = radius * Math.Cos(phi);
            double z = radius * Math.Cos(theta) * Math.Sin(phi);
            return new Vector3((float)x, (float)y, (float)z);
        }

        private static Vector3 GetNormal(double theta, double phi)
        {
            return (Vector3)GetPosition(theta, phi, 1.0);
        }

        private static double DegToRad(double degrees)
        {
            return (degrees / 180.0) * Math.PI;
        }

        private static Vector2 GetTextureCoordinate(double theta, double phi)
        {
            return new Vector2((float)(theta / (2 * Math.PI)), (float)(phi / (Math.PI)));
        }

        /// <summary>
        /// Tesselates the element and returns a MeshGeometry3D representing the 
        /// tessellation based on the parameters given 
        /// </summary>        
        public void AppendSphere(Vector3 center, double radius = 1, int thetaSteps = 64, int phiSteps = 64)
        {
            Vector3Collection pos, nor;
            Vector2Collection tcoord;
            IntCollection tind;

            AppendSphere(center, radius, thetaSteps, phiSteps, out pos, out nor, out tcoord, out tind);

            int i0 = positions.Count;
            this.positions.AddRange(pos);
            this.normals.AddRange(nor);
            this.textureCoordinates.AddRange(tcoord);
            this.triangleIndices.AddRange(tind.Select(x => x + i0));
        }

        private static void AppendSphere(Vector3 center, double radius, int thetaSteps, int phiSteps,
            out Vector3Collection positions, out Vector3Collection normals, out Vector2Collection textureCoordinates, out IntCollection triangleIndices)
        {
            positions = new Vector3Collection();
            normals = new Vector3Collection();
            textureCoordinates = new Vector2Collection();
            triangleIndices = new IntCollection();

            double dt = DegToRad(360.0) / thetaSteps;
            double dp = DegToRad(180.0) / phiSteps;

            for (int pi = 0; pi <= phiSteps; pi++)
            {
                double phi = pi * dp;
                for (int ti = 0; ti <= thetaSteps; ti++)
                {
                    // we want to start the mesh on the x axis
                    double theta = ti * dt;

                    positions.Add(GetPosition(theta, phi, radius) + center);
                    normals.Add(GetNormal(theta, phi));
                    textureCoordinates.Add(GetTextureCoordinate(theta, phi));
                }
            }

            for (int pi = 0; pi < phiSteps; pi++)
            {
                for (int ti = 0; ti < thetaSteps; ti++)
                {
                    int x0 = ti;
                    int x1 = ti + 1;
                    int y0 = pi * (thetaSteps + 1);
                    int y1 = (pi + 1) * (thetaSteps + 1);

                    triangleIndices.Add(x0 + y0);
                    triangleIndices.Add(x0 + y1);
                    triangleIndices.Add(x1 + y0);

                    triangleIndices.Add(x1 + y0);
                    triangleIndices.Add(x0 + y1);
                    triangleIndices.Add(x1 + y1);
                }
            }
        }

#endif



        /////////////////////////////////////////////////////////////////////////////////////////////
        //// below all functions taken from "Helix 3D Toolkit" MeshBuilder 
        /////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Gets a circle section (cached).
        /// </summary>
        /// <param name="thetaDiv">
        /// The number of division.
        /// </param>
        /// <param name="closed">
        /// Is the circle closed?
        /// If true, the last point will not be at the same position than the first one.
        /// </param>
        /// <returns>
        /// A circle.
        /// </returns>
        public static IList<Vector2> GetCircle(int thetaDiv, bool closed = false)
        {
            IList<Vector2> circle = null;
            // If the circle can't be found in one of the two caches
            if ((!closed && !CircleCache.TryGetValue(thetaDiv, out circle)) ||
                (closed && !ClosedCircleCache.TryGetValue(thetaDiv, out circle)))
            {
                circle = new Vector2Collection();
                // Add to the cache
                if (!closed)
                {
                    CircleCache.TryAdd(thetaDiv, circle);
                }
                else
                {
                    ClosedCircleCache.TryAdd(thetaDiv, circle);
                }
                // Determine the angle steps
                var num = closed ? thetaDiv : thetaDiv - 1;
                for (int i = 0; i < thetaDiv; i++)
                {
                    double theta = Math.PI * 2 * ((double)i / num);
                    circle.Add(new Vector2((float)Math.Cos(theta), (float)-Math.Sin(theta)));
                }
            }

            // Since Vector2Collection is not Freezable,
            // return new IList<Vector> to avoid manipulation of the Cached Values
            IList<Vector2> result = new List<Vector2>();
            foreach (var point in circle)
            {
                result.Add(new Vector2(point.X, point.Y));
            }
            return result;
        }
        
        /// <summary>
        /// Gets a circle segment section.
        /// </summary>
        /// <param name="thetaDiv">The number of division.</param>
        /// <param name="totalAngle">The angle of the circle segment.</param>
        /// <param name="angleOffset">The angle-offset to use.</param>
        /// <returns>
        /// A circle segment.
        /// </returns>
        public static IList<Vector2> GetCircleSegment(int thetaDiv, float totalAngle = 2 * (float)Math.PI, float angleOffset = 0)
        {
            IList <Vector2> circleSegment;
            circleSegment = new Vector2Collection();
            for (int i = 0; i < thetaDiv; i++)
            {
                double theta = totalAngle * ((double)i / (thetaDiv - 1)) + angleOffset;
                circleSegment.Add(new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)));
            }

            return circleSegment;
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
        public void AddArrow(Vector3 point1, Vector3 point2, double diameter, double headLength = 3, int thetaDiv = 18)
        {
            var dir = point2 - point1;
            var length = dir.Length();
            var r = (float)diameter / 2;

            var pc = new Vector2Collection
                {
                    new Vector2(0, 0),
                    new Vector2(0, r),
                    new Vector2(length - (float)(diameter * headLength), r),
                    new Vector2(length - (float)(diameter * headLength), r * 2),
                    new Vector2(length, 0)
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
        public void AddBoundingBox(System.Windows.Media.Media3D.Rect3D boundingBox, double diameter)
        {
            var p0 = new Vector3((float)boundingBox.X, (float)boundingBox.Y, (float)boundingBox.Z);
            var p1 = new Vector3((float)boundingBox.X, (float)boundingBox.Y + (float)boundingBox.SizeY, (float)boundingBox.Z);
            var p2 = new Vector3((float)boundingBox.X + (float)boundingBox.SizeX, (float)boundingBox.Y + (float)boundingBox.SizeY, (float)boundingBox.Z);
            var p3 = new Vector3((float)boundingBox.X + (float)boundingBox.SizeX, (float)boundingBox.Y, (float)boundingBox.Z);
            var p4 = new Vector3((float)boundingBox.X, (float)boundingBox.Y, (float)boundingBox.Z + (float)boundingBox.SizeZ);
            var p5 = new Vector3((float)boundingBox.X, (float)boundingBox.Y + (float)boundingBox.SizeY, (float)boundingBox.Z + (float)boundingBox.SizeZ);
            var p6 = new Vector3((float)boundingBox.X + (float)boundingBox.SizeX, (float)boundingBox.Y + (float)boundingBox.SizeY, (float)boundingBox.Z + (float)boundingBox.SizeZ);
            var p7 = new Vector3((float)boundingBox.X + (float)boundingBox.SizeX, (float)boundingBox.Y, (float)boundingBox.Z + (float)boundingBox.SizeZ);

            Action<Vector3, Vector3> addEdge = (c1, c2) => this.AddCylinder(c1, c2, diameter, 10);

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
        /// Adds a box with the specifed faces, aligned with the X, Y and Z axes.
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
        public void AddBox(Vector3 center, double xlength, double ylength, double zlength, BoxFaces faces = BoxFaces.All)
        {
            if ((faces & BoxFaces.PositiveX) == BoxFaces.PositiveX)
            {
                this.AddCubeFace(center, new Vector3(1, 0, 0), new Vector3(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.NegativeX) == BoxFaces.NegativeX)
            {
                this.AddCubeFace(center, new Vector3(-1, 0, 0), new Vector3(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.NegativeY) == BoxFaces.NegativeY)
            {
                this.AddCubeFace(center, new Vector3(0, -1, 0), new Vector3(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.PositiveY) == BoxFaces.PositiveY)
            {
                this.AddCubeFace(center, new Vector3(0, 1, 0), new Vector3(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.PositiveZ) == BoxFaces.PositiveZ)
            {
                this.AddCubeFace(center, new Vector3(0, 0, 1), new Vector3(0, 1, 0), zlength, xlength, ylength);
            }

            if ((faces & BoxFaces.NegativeZ) == BoxFaces.NegativeZ)
            {
                this.AddCubeFace(center, new Vector3(0, 0, -1), new Vector3(0, 1, 0), zlength, xlength, ylength);
            }
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
        public void AddBox(Vector3 center, double xlength, double ylength, double zlength)
        {
            this.AddBox(center, (float)xlength, (float)ylength, (float)zlength, BoxFaces.All);
        }

        /// <summary>
        /// Adds a box aligned with the X, Y and Z axes.
        /// </summary>
        /// <param name="rectangle">
        /// The 3-D "rectangle".
        /// </param>
        public void AddBox(Rect3D rectangle, BoxFaces faces = BoxFaces.All)
        {
            this.AddBox(
                new Vector3((float)rectangle.X + (float)(rectangle.SizeX * 0.5), (float)rectangle.Y + (float)(rectangle.SizeY * 0.5), (float)rectangle.Z + (float)(rectangle.SizeZ * 0.5)),
                rectangle.SizeX,
                rectangle.SizeY,
                rectangle.SizeZ,
                faces);
        }


        public void AddCube(BoxFaces faces = BoxFaces.All)
        {
            if ((faces & BoxFaces.PositiveX) == BoxFaces.PositiveX)
            {
                AddFacePX();
            }

            if ((faces & BoxFaces.NegativeX) == BoxFaces.NegativeX)
            {
                AddFaceNX();
            }

            if ((faces & BoxFaces.NegativeY) == BoxFaces.NegativeY)
            {
                AddFaceNY();
            }

            if ((faces & BoxFaces.PositiveY) == BoxFaces.PositiveY)
            {
                AddFacePY();
            }

            if ((faces & BoxFaces.PositiveZ) == BoxFaces.PositiveZ)
            {
                AddFacePZ();
            }

            if ((faces & BoxFaces.NegativeZ) == BoxFaces.NegativeZ)
            {
                AddFaceNZ();
            }
        }

        public void AddFacePZ()
        {
            var positions = new Vector3[]
            {
                new Vector3(0,0,1),
                new Vector3(0,1,1),
                new Vector3(1,1,1),
                new Vector3(1,0,1),
            };
            var normals = new Vector3[]
            {
                Vector3.UnitZ,
                Vector3.UnitZ,
                Vector3.UnitZ,
                Vector3.UnitZ,
            };


            int i0 = this.positions.Count;
            var indices = new int[]
            {
                i0+0,i0+3,i0+2,
                i0+0,i0+2,i0+1,
            };
            var texcoords = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            this.positions.AddRange(positions);
            this.normals.AddRange(normals);
            this.triangleIndices.AddRange(indices);
            this.textureCoordinates.AddRange(texcoords);
        }

        public void AddFaceNZ()
        {
            var positions = new Vector3[]
            {                
                new Vector3(0,1,0), //p1
                new Vector3(0,0,0), //p0                
                new Vector3(1,0,0), //p3
                new Vector3(1,1,0), //p2
            };
            var normals = new Vector3[]
            {
                -Vector3.UnitZ,
                -Vector3.UnitZ,
                -Vector3.UnitZ,
                -Vector3.UnitZ,
            };

            int i0 = this.positions.Count;
            var indices = new int[]
            {
                i0+0,i0+3,i0+2,
                i0+0,i0+2,i0+1,
            };
            var texcoords = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            this.positions.AddRange(positions);
            this.normals.AddRange(normals);
            this.triangleIndices.AddRange(indices);
            this.textureCoordinates.AddRange(texcoords);
        }

        public void AddFacePX()
        {
            var positions = new Vector3[]
            {
                new Vector3(1,0,0), //p0
                new Vector3(1,0,1), //p1
                new Vector3(1,1,1), //p2   
                new Vector3(1,1,0), //p3                             
            };
            var normals = new Vector3[]
            {
                Vector3.UnitX,
                Vector3.UnitX,
                Vector3.UnitX,
                Vector3.UnitX,
            };


            int i0 = this.positions.Count;
            var indices = new int[]
            {
                i0+0,i0+3,i0+2,
                i0+0,i0+2,i0+1,
            };
            var texcoords = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            this.positions.AddRange(positions);
            this.normals.AddRange(normals);
            this.triangleIndices.AddRange(indices);
            this.textureCoordinates.AddRange(texcoords);
        }

        public void AddFaceNX()
        {
            var positions = new Vector3[]
            {                
                new Vector3(0,0,1), //p1
                new Vector3(0,0,0), //p0                
                new Vector3(0,1,0), //p3 
                new Vector3(0,1,1), //p2               
            };
            var normals = new Vector3[]
            {
                -Vector3.UnitX,
                -Vector3.UnitX,
                -Vector3.UnitX,
                -Vector3.UnitX,
            };


            int i0 = this.positions.Count;
            var indices = new int[]
            {
                i0+0,i0+3,i0+2,
                i0+0,i0+2,i0+1,
            };
            var texcoords = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            this.positions.AddRange(positions);
            this.normals.AddRange(normals);
            this.triangleIndices.AddRange(indices);
            this.textureCoordinates.AddRange(texcoords);
        }

        public void AddFacePY()
        {
            var positions = new Vector3[]
            {                                                 
                new Vector3(1,1,0), //p3  
                new Vector3(1,1,1), //p2  
                new Vector3(0,1,1), //p1
                new Vector3(0,1,0), //p0
            };
            var normals = new Vector3[]
            {
                Vector3.UnitY,
                Vector3.UnitY,
                Vector3.UnitY,
                Vector3.UnitY,
            };


            int i0 = this.positions.Count;
            var indices = new int[]
            {
                i0+0,i0+3,i0+2,
                i0+0,i0+2,i0+1,
            };
            var texcoords = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            this.positions.AddRange(positions);
            this.normals.AddRange(normals);
            this.triangleIndices.AddRange(indices);
            this.textureCoordinates.AddRange(texcoords);
        }

        public void AddFaceNY()
        {
            var positions = new Vector3[]
            {                                                                                                  
                new Vector3(0,0,0), //p0
                new Vector3(0,0,1), //p1
                new Vector3(1,0,1), //p2
                new Vector3(1,0,0), //p3
            };
            var normals = new Vector3[]
            {
                -Vector3.UnitY,
                -Vector3.UnitY,
                -Vector3.UnitY,
                -Vector3.UnitY,
            };


            int i0 = this.positions.Count;
            var indices = new int[]
            {
                i0+0,i0+3,i0+2,
                i0+0,i0+2,i0+1,
            };
            var texcoords = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(0,0),
            };

            this.positions.AddRange(positions);
            this.normals.AddRange(normals);
            this.triangleIndices.AddRange(indices);
            this.textureCoordinates.AddRange(texcoords);
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
        /// The dist from the center of the cube to the face.
        /// </param>
        /// <param name="width">
        /// The width of the face.
        /// </param>
        /// <param name="height">
        /// The height of the face.
        /// </param>
        private void AddCubeFace(Vector3 center, Vector3 normal, Vector3 up, double dist, double width, double height)
        {
            var right = Vector3.Cross(normal, up);
            var n = normal * (float)dist / 2;
            up *= (float)height / 2f;
            right *= (float)width / 2f;
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
                this.textureCoordinates.Add(new Vector2(1, 1));
                this.textureCoordinates.Add(new Vector2(0, 1));
                this.textureCoordinates.Add(new Vector2(0, 0));
                this.textureCoordinates.Add(new Vector2(1, 0));
            }

            this.triangleIndices.Add(i0 + 2);
            this.triangleIndices.Add(i0 + 1);
            this.triangleIndices.Add(i0 + 0);

            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 3);
            this.triangleIndices.Add(i0 + 2);
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
            Vector3 origin,
            Vector3 direction,
            double baseRadius,
            double topRadius,
            double height,
            bool baseCap,
            bool topCap,
            int thetaDiv)
        {
            var pc = new Vector2Collection();
            if (baseCap)
            {
                pc.Add(new Vector2(0, 0));
            }

            pc.Add(new Vector2(0, (float)baseRadius));
            pc.Add(new Vector2((float)height, (float)topRadius));
            if (topCap)
            {
                pc.Add(new Vector2((float)height, 0));
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
        public void AddCone(Vector3 origin, Vector3 apex, double baseRadius, bool baseCap, int thetaDiv)
        {
            var dir = apex - origin;
            this.AddCone(origin, dir, (float)baseRadius, 0, (float)dir.Length(), baseCap, false, thetaDiv);
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
        /// <param name="radius">
        /// The diameters.
        /// </param>
        /// <param name="thetaDiv">
        /// The number of divisions around the cylinder.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Cylinder_(geometry).
        /// </remarks>
        public void AddCylinder(Vector3 p1, Vector3 p2, double radius = 1, int thetaDiv = 32, bool cap1 = true, bool cap2 = true)
        {
            Vector3 n = p2 - p1;
            double l = n.Length();
            n.Normalize();
            this.AddCone(p1, n, radius, radius, l, cap1, cap2, thetaDiv);
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
        public void AddPipes(IList<Vector3> points, IList<int> edges, double diameter = 1, int thetaDiv = 32)
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
        public void AddExtrudedGeometry(IList<Vector2> points, Vector3 xaxis, Vector3 p0, Vector3 p1)
        {
            var ydirection = Vector3.Cross(p1 - p0, xaxis);
            ydirection.Normalize();
            xaxis.Normalize();

            int index0 = this.positions.Count;
            int np = 2 * points.Count;
            foreach (var p in points)
            {
                var v = (xaxis * p.X) + (ydirection * p.Y);

                this.positions.Add(p0 + v);
                this.positions.Add(p1 + v);
            }

            int ii = 0;
            for (int i = 0; i < points.Count - 1; i++, ii += 2)
            {
                int i0 = index0 + ii;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int i3 = i0 + 3;


                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(i0);

                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i3);
                this.triangleIndices.Add(i2);
            }
            if (this.textureCoordinates != null)
            {
                for (int i = 0; i < np; i++)
                {
                    this.textureCoordinates.Add(new Vector2());
                }
            }

            //foreach (var p in points)
            //{                
            //    if (this.normals != null)
            //    {                    
            //        //this.normals.Add(Vector3.UnitZ);
            //        //this.normals.Add(Vector3.UnitZ);
            //    }

            //    if (this.textureCoordinates != null)
            //    {
            //        this.textureCoordinates.Add(new Vector2(0, 0));
            //        this.textureCoordinates.Add(new Vector2(1, 0));
            //    }

            //    int i1 = index0 + 1;
            //    int i2 = (index0 + 2) % np;
            //    int i3 = ((index0 + 2) % np) + 1;

            //    this.triangleIndices.Add(i1);
            //    this.triangleIndices.Add(i2);
            //    this.triangleIndices.Add(index0);

            //    this.triangleIndices.Add(i1);
            //    this.triangleIndices.Add(i3);
            //    this.triangleIndices.Add(i2);
            //}

            ComputeNormals(this.positions, this.triangleIndices, out this.normals);
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
        public void AddLoftedGeometry(IList<IList<Vector3>> positionsList, IList<IList<Vector3>> normalList, IList<IList<Vector2>> textureCoordinateList)
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
        public void AddNode(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
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
        public void AddPipe(Vector3 point1, Vector3 point2, double innerDiameter, double diameter, int thetaDiv)
        {
            var dir = point2 - point1;

            double height = dir.Length();
            dir.Normalize();

            var pc = new Vector2Collection
                {
                    new Vector2(0, (float)innerDiameter / 2),
                    new Vector2(0, (float)diameter / 2),
                    new Vector2((float)height, (float)diameter / 2),
                    new Vector2((float)height, (float)innerDiameter / 2)
                };

            if (innerDiameter > 0)
            {
                // Add the inner surface
                pc.Add(new Vector2(0, (float)innerDiameter / 2));
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
        public void AddPolygon(IList<Vector3> points)
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
        /// <param name="closeBase">
        /// Add triangles to the base of the pyramid or not.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Pyramid_(geometry).
        /// </remarks>
        public void AddPyramid(Vector3 center, double sideLength, double height, bool closeBase = false)
        {
            var p1 = new Vector3(center.X - (float)(sideLength * 0.5), center.Y - (float)(sideLength * 0.5), center.Z);
            var p2 = new Vector3(center.X + (float)(sideLength * 0.5), center.Y - (float)(sideLength * 0.5), center.Z);
            var p3 = new Vector3(center.X + (float)(sideLength * 0.5), center.Y + (float)(sideLength * 0.5), center.Z);
            var p4 = new Vector3(center.X - (float)(sideLength * 0.5), center.Y + (float)(sideLength * 0.5), center.Z);
            var p5 = new Vector3(center.X, center.Y, center.Z + (float)height);
            this.AddTriangle(p1, p2, p5);
            this.AddTriangle(p2, p3, p5);
            this.AddTriangle(p3, p4, p5);
            this.AddTriangle(p4, p1, p5);
            if (closeBase)
            {
                this.AddTriangle(p1, p3, p2);
                this.AddTriangle(p3, p1, p4);
            }
        }

        /// <summary>
        /// Adds an octahedron.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="forward">The normal vector.</param>
        /// <param name="up">The up vector.</param>
        /// <param name="sideLength">Length of the side.</param>
        /// <param name="height">The half height of the octahedron.</param>
        /// <remarks>See <a href="http://en.wikipedia.org/wiki/Octahedron">Octahedron</a>.</remarks>
        public void AddOctahedron(Vector3 center, Vector3 forward, Vector3 up, double sideLength, double height)
        {
            var right = Vector3.Cross(forward, up);
            var n = forward * (float)sideLength / 2;
            up *= (float)height / 2;
            right *= (float)sideLength / 2;

            var p1 = center - n - up - right;
            var p2 = center - n - up + right;
            var p3 = center + n - up + right;
            var p4 = center + n - up - right;
            var p5 = center + up;
            var p6 = center - up;

            this.AddTriangle(p1, p2, p5);
            this.AddTriangle(p2, p3, p5);
            this.AddTriangle(p3, p4, p5);
            this.AddTriangle(p4, p1, p5);

            this.AddTriangle(p2, p1, p6);
            this.AddTriangle(p3, p2, p6);
            this.AddTriangle(p4, p3, p6);
            this.AddTriangle(p1, p4, p6);
        }

        /// <summary>
        /// Add a tetrahedron.
        /// </summary>
        /// <param name="center">The Center of Mass.</param>
        /// <param name="forward">Direction to first Base-Point (in Base-Plane).</param>
        /// <param name="up">Up Vector.</param>
        /// <param name="sideLength">The Sidelength.</param>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Tetrahedron and
        /// https://en.wikipedia.org/wiki/Equilateral_triangle.
        /// </remarks>
        public void AddTetrahedron(Vector3 center, Vector3 forward, Vector3 up, double sideLength)
        {
            // Helper Variables
            var right = Vector3.Cross(up, forward);
            var heightSphere = (float)Math.Sqrt(6) / 3f * (float)sideLength;
            var radiusSphere = (float)Math.Sqrt(6) / 4f * (float)sideLength;
            var heightFace = (float)Math.Sqrt(3) / 2f * (float)sideLength;
            var radiusFace = (float)Math.Sqrt(3) / 3f * (float)sideLength;
            var smallHeightSphere = heightSphere - radiusSphere;
            var smallHeightFace = heightFace - radiusFace;
            var halfLength = (float)sideLength * 0.5f;

            // The Vertex Positions
            var p1 = center + forward * radiusFace - up * smallHeightSphere;
            var p2 = center - forward * smallHeightFace - right * halfLength - up * smallHeightSphere;
            var p3 = center - forward * smallHeightFace + right * halfLength - up * smallHeightSphere;
            var p4 = center + up * radiusSphere;


            // Triangles
            this.AddTriangle(p1, p2, p3);
            this.AddTriangle(p1, p4, p2);
            this.AddTriangle(p2, p4, p3);
            this.AddTriangle(p3, p4, p1);
        }

        /// <summary>
        /// Generate a Dodecahedron
        /// </summary>
        /// <param name="center">The Center of the Dodecahedron</param>
        /// <param name="forward">The Direction to the first Point (normalized).</param>
        /// <param name="up">The Up-Dirextion (normalized, perpendicular to the <see cref="forward"/> Direction)</param>
        /// <param name="sideLength">Length of the Edges of the Dodecahedron</param>
        /// <remarks>
        /// See:
        /// https://en.wikipedia.org/wiki/Dodecahedron
        /// https://en.wikipedia.org/wiki/Pentagon
        /// https://en.wikipedia.org/wiki/Isosceles_triangle
        /// </remarks>
        public void AddDodecahedron(Vector3 center, Vector3 forward, Vector3 up, double sideLength)
        {
            /// If points already exist in the MeshBuilder
            var positionsCount = this.positions.Count;

            var right = Vector3.Cross(up, forward);
            // Distance from the Center to the Dodekaeder-Points
            var radiusSphere = 0.25f * (float)Math.Sqrt(3) * (1 + (float)Math.Sqrt(5)) * (float)sideLength;
            var radiusFace = 0.1f * (float)Math.Sqrt(50 + 10 * (float)Math.Sqrt(5)) * (float)sideLength;
            var vectorDown = (float)Math.Sqrt(radiusSphere * radiusSphere - radiusFace * radiusFace);

            // Add Points
            var baseCenter = center - up * vectorDown;
            var pentagonPoints = GetCircle(5, true);
            // Base Points
            var basePoints = new List<Vector3>();
            foreach (var point in pentagonPoints)
            {
                var newPoint = baseCenter + forward * point.X * radiusFace + right * point.Y * radiusFace;
                basePoints.Add(newPoint);
                this.positions.Add(newPoint);
            }
            // Angle of Projected Isosceles triangle
            var gamma = (float)Math.Acos(1 - (sideLength * sideLength / (2 * radiusSphere * radiusSphere)));
            // Base Upper Points
            foreach (var point in basePoints)
            {
                var baseCenterToPoint = point - baseCenter;
                baseCenterToPoint.Normalize();
                var centerToPoint = point - center;
                centerToPoint.Normalize();
                var tempRight = Vector3.Cross(up, baseCenterToPoint);
                var newPoint = new Vector3(radiusSphere * (float)Math.Cos(gamma), 0, radiusSphere * (float)Math.Sin(gamma));
                var tempUp = Vector3.Cross(centerToPoint, tempRight);
                this.positions.Add(center + centerToPoint * newPoint.X + tempUp * newPoint.Z);
            }

            // Top Points
            var topCenter = center + up * vectorDown;
            var topPoints = new List<Vector3>();
            foreach (var point in pentagonPoints)
            {
                var newPoint = topCenter - forward * point.X * radiusFace + right * point.Y * radiusFace;
                topPoints.Add(newPoint);
            }
            // Top Lower Points
            foreach (var point in topPoints)
            {
                var topCenterToPoint = point - topCenter;
                topCenterToPoint.Normalize();
                var centerToPoint = point - center;
                centerToPoint.Normalize();
                var tempRight = Vector3.Cross(up, topCenterToPoint);
                var newPoint = new Vector3(radiusSphere * (float)Math.Cos(gamma), 0, radiusSphere * (float)Math.Sin(gamma));
                var tempUp = Vector3.Cross(tempRight, centerToPoint);
                this.positions.Add(center + centerToPoint * newPoint.X + tempUp * newPoint.Z);
            }
            // Add top Points at last
            foreach (var point in topPoints)
            {
                this.positions.Add(point);
            }

            // Add Normals if wanted
            if (this.normals != null)
            {
                for (int i = positionsCount; i < this.positions.Count; i++)
                {
                    var centerToPoint = this.positions[i] - center;
                    centerToPoint.Normalize();
                    this.normals.Add(centerToPoint);
                }
            }

            // Add Texture Coordinates
            if (this.textureCoordinates != null)
            {
                for (int i = positionsCount; i < this.positions.Count; i++)
                {
                    var centerToPoint = this.positions[i] - center;
                    centerToPoint.Normalize();
                    var cTPUpValue = Vector3.Dot(centerToPoint, up);
                    var planeCTP = centerToPoint - up * cTPUpValue;
                    planeCTP.Normalize();
                    var u = (float)Math.Atan2(Vector3.Dot(planeCTP, forward), Vector3.Dot(planeCTP, right));
                    var v = cTPUpValue * 0.5f + 0.5f;
                    this.textureCoordinates.Add(new Vector2(u, v));
                }
            }

            // Add Faces
            // Base Polygon
            this.AddPolygonByTriangulation(this.positions.Skip(positionsCount).Take(5).Select((p, i) => i).ToList());
            // Top Polygon
            this.AddPolygonByTriangulation(this.positions.Skip(positionsCount + 15).Select((p, i) => 15 + i).ToList());
            // SidePolygons
            for (int i = 0; i < 5; i++)
            {
                // Polygon one
                var pIndices = new List<int>() {
                    (i + 1) % 5 + positionsCount,
                    i, i + 5 + positionsCount,
                    (5 - i + 2) % 5 + 10 + positionsCount,
                    (i + 1) % 5 + 5 + positionsCount
                };
                this.AddPolygonByTriangulation(pIndices);

                // Polygon two
                pIndices = new List<int>() {
                    i + 15 + positionsCount,
                    i + 10 + positionsCount,
                    (5 - i + 2) % 5 + 5 + positionsCount,
                    (i + 1) % 5 + 10 + positionsCount,
                    (i + 1) % 5 + 15 + positionsCount
                };
                this.AddPolygonByTriangulation(pIndices);
            }
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
        public void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //// The nodes are arranged in counter-clockwise order
            //// p3               p2
            //// +---------------+
            //// |               |
            //// |               |
            //// +---------------+
            //// p0               p1
            var uv0 = new Vector2(0, 0);
            var uv1 = new Vector2(1, 0);
            var uv2 = new Vector2(0, 1);
            var uv3 = new Vector2(1, 1);
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
        public void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
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
                var w = Vector3.Cross(p3 - p0, p1 - p0);
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
        /// The normals.
        /// </param>
        /// <param name="quadTextureCoordinates">
        /// The texture coordinates.
        /// </param>
        public void AddQuads(IList<Vector3> quadPositions, IList<Vector3> quadNormals, IList<Vector2> quadTextureCoordinates)
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

            System.Diagnostics.Debug.Assert(quadPositions.Count > 0 && quadPositions.Count % 4 == 0, "Wrong number of positions.");

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
        public void AddRectangularMesh(IList<Vector3> points, int columns, bool flipTriangles = false)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            int index0 = this.positions.Count;

            foreach (var pt in points)
            {
                this.positions.Add(pt);
            }

            int rows = points.Count / columns;

            if (flipTriangles)
            {
                this.AddRectangularMeshTriangleIndicesFlipped(index0, rows, columns);
            }
            else
            {
                this.AddRectangularMeshTriangleIndices(index0, rows, columns);
            }

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
        /// Adds a rectangular mesh defined by a two-dimensional arrary of points.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="texCoords">
        /// The texture coordinates (optional).
        /// </param>
        /// <param name="closed0">
        /// set to <c>true</c> if the mesh is closed in the 1st dimension.
        /// </param>
        /// <param name="closed1">
        /// set to <c>true</c> if the mesh is closed in the 2nd dimension.
        /// </param>
        public void AddRectangularMesh(Vector3[,] points, Vector2[,] texCoords = null, bool closed0 = false, bool closed1 = false)
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
        /// Generates a rectangles mesh on the axis-aligned plane given by the box-face.
        /// </summary>
        /// <param name="plane">Box face which determines the plane the grid lies on.</param>
        /// <param name="columns">width of the grid, i.e. horizontal resolution </param>
        /// <param name="rows">height of the grid, i.e. vertical resolution</param>
        /// <param name="width">total size in horizontal </param>
        /// <param name="height">total vertical size</param>
        /// <param name="flipTriangles">flips the triangle faces</param>
        /// <param name="flipTexCoordsUAxis">flips the u-axis (horizontal) of the texture coords.</param>
        /// <param name="flipTexCoordsVAxis">flips the v-axis (vertical) of the tex.coords.</param>
        public void AddRectangularMesh(BoxFaces plane, int columns, int rows, float width, float height, bool flipTriangles = false, bool flipTexCoordsUAxis = false, bool flipTexCoordsVAxis = false)
        {
            // checks
            if (columns < 2 || rows < 2)
            {
                throw new ArgumentNullException("columns or rows too small");
            }
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentNullException("width or height too small");
            }

            // index0
            int index0 = this.positions.Count;

            // positions
            var stepy = height / (rows-1);
            var stepx = width / (columns-1);
            //rows++;
            //columns++;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    this.positions.Add(new Vector3(x * stepx, y * stepy, 0));
                }
            }

            // indices
            if (flipTriangles)
            {
                this.AddRectangularMeshTriangleIndicesFlipped(index0, rows, columns);
            }
            else
            {
                this.AddRectangularMeshTriangleIndices(index0, rows, columns);
            }

            // normals
            if (this.normals != null)
            {
                this.AddRectangularMeshNormals(index0, rows, columns);
            }

            // texcoords
            if (this.textureCoordinates != null)
            {
                this.AddRectangularMeshTextureCoordinates(rows, columns, flipTexCoordsVAxis, flipTexCoordsUAxis);
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
        /// share vertices if set to <c>true</c> .
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Icosahedron and http://www.gamedev.net/community/forums/topic.asp?topic_id=283350.
        /// </remarks>
        public void AddRegularIcosahedron(Vector3 center, double radius, bool shareVertices)
        {
            float a = (float)Math.Sqrt(2.0 / (5.0 + Math.Sqrt(5.0)));

            float b = (float)Math.Sqrt(2.0 / (5.0 - Math.Sqrt(5.0)));

            var icosahedronIndices = new[]
                {
                    1, 4, 0, 4, 9, 0, 4, 5, 9, 8, 5, 4, 1, 8, 4, 1, 10, 8, 10, 3, 8, 8, 3, 5, 3, 2, 5, 3, 7, 2, 3, 10, 7,
                    10, 6, 7, 6, 11, 7, 6, 0, 11, 6, 1, 0, 10, 1, 6, 11, 0, 9, 2, 11, 9, 5, 2, 9, 11, 2, 7
                };

            var icosahedronVertices = new[]
                {
                    new Vector3(-a, 0, b), new Vector3(a, 0, b), new Vector3(-a, 0, -b), new Vector3(a, 0, -b),
                    new Vector3(0, b, a), new Vector3(0, b, -a), new Vector3(0, -b, a), new Vector3(0, -b, -a),
                    new Vector3(b, a, 0), new Vector3(-b, a, 0), new Vector3(b, -a, 0), new Vector3(-b, -a, 0)
                };

            if (shareVertices)
            {
                int index0 = this.positions.Count;
                foreach (var v in icosahedronVertices)
                {
                    this.positions.Add(center + (v * (float)radius));
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
                        center + (icosahedronVertices[icosahedronIndices[i]] * (float)radius),
                        center + (icosahedronVertices[icosahedronIndices[i + 1]] * (float)radius),
                        center + (icosahedronVertices[icosahedronIndices[i + 2]] * (float)radius));
                }
            }
        }

        /// <summary>
        /// Adds a surface of revolution.
        /// </summary>
        /// <param name="points">
        /// The points (y coordinates are radius, x coordinates are distance from the origin along the axis of revolution)
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
        public void AddRevolvedGeometry(IList<Vector2> points, Vector3 origin, Vector3 direction, int thetaDiv)
        {
            direction.Normalize();

            // Find two unit vectors orthogonal to the specified direction
            var u = direction.FindAnyPerpendicular();
            var v = Vector3.Cross(direction, u);

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
                    // if (Vector3.Subtract(q1, q2).LengthSquared < eps)
                    // continue;
                    float tx = points[j + 1].X - points[j].X;
                    float ty = points[j + 1].Y - points[j].Y;

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
                        this.textureCoordinates.Add(new Vector2((float)i / (thetaDiv - 1), (float)j / (n - 1)));
                        this.textureCoordinates.Add(new Vector2((float)i / (thetaDiv - 1), (float)(j + 1) / (n - 1)));
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
        /// Adds a sphere (by subdiving a regular icosahedron).
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
        /// See http://www.fho-emden.de/~hoffmann/ikos27042002.pdf.
        /// </remarks>
        public void AddSubdivisionSphere(Vector3 center, double radius, int subdivisions)
        {
            int p0 = this.positions.Count;
            this.Append(GetUnitSphere(subdivisions));
            int p1 = this.positions.Count;
            for (int i = p0; i < p1; i++)
            {
                this.positions[i] = center + ((float)radius * this.positions[i]);
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
        public void AddSphere(Vector3 center, double radius = 1, int thetaDiv = 32, int phiDiv = 32)
        {
            int index0 = this.positions.Count;
            float dt = (float)(2 * Math.PI / thetaDiv);
            float dp = (float)(Math.PI / phiDiv);

            for (int pi = 0; pi <= phiDiv; pi++)
            {
                float phi = pi * dp;

                for (int ti = 0; ti <= thetaDiv; ti++)
                {
                    // we want to start the mesh on the x axis
                    float theta = ti * dt;

                    // Spherical coordinates
                    // http://mathworld.wolfram.com/SphericalCoordinates.html
                    //float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                    //float y = (float)(Math.Sin(theta) * Math.Sin(phi));
                    //float z = (float)(Math.Cos(phi));

                    float x = (float)(Math.Sin(theta) * Math.Sin(phi));
                    float y = (float)(Math.Cos(phi));
                    float z = (float)(Math.Cos(theta) * Math.Sin(phi));

                    var p = new Vector3(center.X + ((float)radius * x), center.Y + ((float)radius * y), center.Z + ((float)radius * z));
                    this.positions.Add(p);

                    if (this.normals != null)
                    {
                        var n = new Vector3(x, y, z);
                        this.normals.Add(n);
                    }

                    if (this.textureCoordinates != null)
                    {
                        var uv = new Vector2((float)(theta / (2 * Math.PI)), (float)(phi / Math.PI));
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
        public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {           
            var uv0 = new Vector2(0, 0);
            var uv1 = new Vector2(1, 0);
            var uv2 = new Vector2(0, 1);
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
        public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector2 uv0, Vector2 uv1, Vector2 uv2)
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
                var w = Vector3.Cross(p1 - p0, p2 - p0);
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
        /// The normals of the triangle fan.
        /// </param>
        /// <param name="fanTextureCoordinates">
        /// The texture coordinates of the triangle fan.
        /// </param>
        public void AddTriangleFan(IList<Vector3> fanPositions, IList<Vector3> fanNormals = null, IList<Vector2> fanTextureCoordinates = null)
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
        /// The normals of the triangle strip.
        /// </param>
        /// <param name="stripTextureCoordinates">
        /// The texture coordinates of the triangle strip.
        /// </param>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Triangle_strip.
        /// </remarks>
        public void AddTriangleStrip(IList<Vector3> stripPositions, IList<Vector3> stripNormals = null, IList<Vector2> stripTextureCoordinates = null)
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
        /// Adds a triangle (exactely 3 indices)
        /// </summary>
        /// <param name="vertexIndices">The vertex indices.</param>
        public void AddTriangle(IList<int> vertexIndices)
        {            
            for (int i = 0; i < 3; i++)
            {
                this.triangleIndices.Add(vertexIndices[i]); 
            }
        }

        /// <summary>
        /// Adds a quad (exactely 4 indices)
        /// </summary>
        /// <param name="vertexIndices">The vertex indices.</param>
        public void AddQuad(IList<int> vertexIndices)
        {            
            for (int i = 0; i < 4; i++)
            {
                this.triangleIndices.Add(vertexIndices[i]);
            }
        }

        /// <summary>
        /// Adds a polygon defined by vertex indices (uses the cutting ears algorithm).
        /// </summary>
        /// <param name="vertexIndices">The vertex indices.</param>
        [Obsolete("Please use the faster version AddPolygon instead")]
        public void AddPolygonByCuttingEars(IList<int> vertexIndices)
        {
            var points = vertexIndices.Select(vi => this.positions[vi]).ToList();

            var poly3D = new Polygon3D(points);

            // Transform the polygon to 2D
            var poly2D = poly3D.Flatten();

            // Triangulate
            var triangulatedIndices = CuttingEarsTriangulator.Triangulate(poly2D.Points);
            if (triangulatedIndices != null)
            {
                foreach (var i in triangulatedIndices)
                {
                    this.triangleIndices.Add(vertexIndices[i]);
                }
            }
        }

        /// <summary>
        /// Adds a polygon defined by vertex indices (uses the sweep line algorithm).
        /// </summary>
        /// <param name="vertexIndices">The vertex indices.</param>
        public void AddPolygonByTriangulation(IList<int> vertexIndices)
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
        /// The normals (corresponding to the points).
        /// </param>
        /// <param name="triangleTextureCoordinates">
        /// The texture coordinates (corresponding to the points).
        /// </param>
        public void AddTriangles(IList<Vector3> trianglePositions, IList<Vector3> triangleNormals = null, IList<Vector2> triangleTextureCoordinates = null)
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
        /// <param name="frontCap">
        /// Add cap to tube front
        /// </param>
        /// <param name="backCap">
        /// Add cap to tube back
        /// </param>
        public void AddTube(IList<Vector3> path, double[] values, double[] diameters, int thetaDiv, bool isTubeClosed, bool frontCap = false, bool backCap = false)
        {
            var circle = GetCircle(thetaDiv);
            this.AddTube(path, values, diameters, circle, isTubeClosed, true, frontCap, backCap);
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
        /// <param name="frontCap">
        /// Add cap to tube front
        /// </param>
        /// <param name="backCap">
        /// Add cap to tube back
        /// </param>
        public void AddTube(IList<Vector3> path, double diameter, int thetaDiv, bool isTubeClosed, bool frontCap = false, bool backCap = false)
        {
            this.AddTube(path, null, new[] { diameter }, thetaDiv, isTubeClosed, frontCap, backCap);
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
        /// <param name="frontCap">
        /// Add cap to tube front
        /// </param>
        /// <param name="backCap">
        /// Add cap to tube back
        /// </param>
        public void AddTube(
            IList<Vector3> path,
            IList<double> values,
            IList<double> diameters,
            IList<Vector2> section,
            bool isTubeClosed,
            bool isSectionClosed, bool frontCap, bool backCap)
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
                float r = (float)(diameters != null ? diameters[i % diametersCount] / 2 : 1);
                int i0 = i > 0 ? i - 1 : i;
                int i1 = i + 1 < pathLength ? i + 1 : i;

                var forward = path[i1] - path[i0];
                var right = Vector3.Cross(up, forward);
                up = Vector3.Cross(forward, right);
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
                                ? new Vector2((float)values[i % valuesCount], (float)(j / (sectionLength - 1)))
                                : new Vector2());
                    }
                }
            }

            this.AddRectangularMeshTriangleIndices(index0, pathLength, sectionLength, isSectionClosed, isTubeClosed);
            if(!isTubeClosed && ( frontCap || backCap ) && path.Count > 1 && Positions.Count >= section.Count)
            {
                var sCount = section.Count;
                var normals = new Vector3[sCount];
                var fanTextures = new Vector2[sCount];
                var pCount = path.Count;
                if (backCap)
                {                    
                    var circleBack = Positions.Skip(Positions.Count - sCount).Take(sCount).ToArray();
                    var normal = path[pCount - 1] - path[pCount - 2];
                    normal.Normalize();
                    for (int i = 0; i < normals.Length; ++i)
                    {
                        normals[i] = normal;
                    }
                    this.AddTriangleFan(circleBack, normals, fanTextures);
                }
                if (frontCap)
                {
                    var circleFront = Positions.Take(sCount).ToArray();
                    var normal = path[0] - path[1];
                    normal.Normalize();
                
                    for(int i=0; i<normals.Length; ++i)
                    {
                        normals[i] = normal;
                    }
                    this.AddTriangleFan(circleFront, normals, fanTextures);
                }
            }
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

            this.Append(mesh.Positions, mesh.Indices, mesh.Normals, mesh.TextureCoordinates);
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
        /// The normals to append.
        /// </param>
        /// <param name="textureCoordinatesToAppend">
        /// The texture coordinates to append.
        /// </param>
        public void Append(
            IList<Vector3> positionsToAppend,
            IList<int> triangleIndicesToAppend,
            IList<Vector3> normalsToAppend = null,
            IList<Vector2> textureCoordinatesToAppend = null)
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
        public void ChamferCorner(Vector3 p, double d, double eps = 1e-6, IList<Vector3> chamferPoints = null)
        {
            this.NoSharedVertices();

            this.normals = null;
            this.textureCoordinates = null;

            var cornerNormal = this.FindCornerNormal(p, eps);

            var newCornerPoint = p - (cornerNormal * (float)d);
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
                var d0 = (p - p0).LengthSquared();
                var d1 = (p - p1).LengthSquared();
                var d2 = (p - p2).LengthSquared();
                var mind = Math.Min(d0, Math.Min(d1, d2));
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
                var line1 = new Ray(p0, p1 - p0);
                var line2 = new Ray(p0, p2 - p0);
                Vector3 p01, p02;

                if (!plane.Intersects(ref line1, out p01))
                {
                    continue;
                }

                if (!plane.Intersects(ref line2, out p02))
                {
                    continue;
                }

                if (chamferPoints != null)
                {
                    // add the chamfered points
                    if (!chamferPoints.Contains(p01))
                    {
                        chamferPoints.Add(p01);
                    }

                    if (!chamferPoints.Contains(p02))
                    {
                        chamferPoints.Add(p02);
                    }
                }

                int i01 = i0;

                // change the original triangle to use the first chamfer point
                this.positions[this.triangleIndices[i01]] = p01;

                int i02 = this.positions.Count;
                this.positions.Add(p02);

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
        /// See http://msdn.microsoft.com/en-us/library/bb613553.aspx.
        /// Try to keep mesh sizes under these limits:
        /// Positions : 20,001 Vector3 instances
        /// TriangleIndices : 60,003 Int32 instances
        /// </remarks>
        public void CheckPerformanceLimits()
        {
            if (this.positions.Count > 20000)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Too many positions ({0}).", this.positions.Count));
            }

            if (this.triangleIndices.Count > 60002)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Too many triangle indices ({0}).", this.triangleIndices.Count));
            }
        }

        /// <summary>
        /// Scales the positions (and normals).
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
        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            for (int i = 0; i < this.positions.Count; i++)
            {
                this.positions[i] = new Vector3(
                    this.positions[i].X * scaleX, this.positions[i].Y * scaleY, this.positions[i].Z * scaleZ);
            }

            if (this.normals != null)
            {
                for (int i = 0; i < this.normals.Count; i++)
                {
                    this.normals[i] = new Vector3(
                        this.normals[i].X * scaleX, this.normals[i].Y * scaleY, this.normals[i].Z * scaleZ);
                    this.normals[i].Normalize();
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
            mb.AddRegularIcosahedron(new Vector3(), 1, false);
            for (int i = 0; i < subdivisions; i++)
            {
                mb.SubdivideLinear();
            }

            for (int i = 0; i < mb.positions.Count; i++)
            {
                var v = mb.positions[i];
                v.Normalize();
                mb.positions[i] = v;
            }

            var mesh = mb.ToMeshGeometry3D();
            lock (UnitSphereCache)
            {
                if(!UnitSphereCache.ContainsKey(subdivisions))
                {
                    UnitSphereCache.Add(subdivisions, mesh);
                }
            }
            return mesh;
        }

        /// <summary>
        /// Adds normals for a rectangular mesh.
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
                    var u = Vector3.Subtract(
                        this.positions[index0 + (i1 * columns) + j0], this.positions[index0 + (i0 * columns) + j0]);
                    var v = Vector3.Subtract(
                        this.positions[index0 + (i0 * columns) + j1], this.positions[index0 + (i0 * columns) + j0]);
                    var normal = Vector3.Cross(u, v);
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
        private void AddRectangularMeshTextureCoordinates(int rows, int columns, bool flipRowsAxis = false, bool flipColumnsAxis = false)
        {
            for (int i = 0; i < rows; i++)
            {
                float v = flipRowsAxis ? (1 - (float)i / (rows - 1)) : (float)i / (rows - 1);

                for (int j = 0; j < columns; j++)
                {
                    float u = flipColumnsAxis ? (1 - (float)j / (columns - 1)) : (float)j / (columns - 1);
                    this.textureCoordinates.Add(new Vector2(u, v));
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
        /// Add triangle indices for a rectangular mesh with flipped triangles.
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
        private void AddRectangularMeshTriangleIndicesFlipped(int index0, int rows, int columns, bool isSpherical = false)
        {
            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < columns - 1; j++)
                {
                    int ij = (i * columns) + j;
                    if (!isSpherical || i > 0)
                    {
                        this.triangleIndices.Add(index0 + ij);
                        this.triangleIndices.Add(index0 + ij + 1);
                        this.triangleIndices.Add(index0 + ij + 1 + columns);
                    }

                    if (!isSpherical || i < rows - 2)
                    {
                        this.triangleIndices.Add(index0 + ij + 1 + columns);
                        this.triangleIndices.Add(index0 + ij + columns);
                        this.triangleIndices.Add(index0 + ij);
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
        private void AddRectangularMeshTriangleIndices(int index0, int rows, int columns, bool rowsClosed, bool columnsClosed)
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
        private Vector3 FindCornerNormal(Vector3 p, double eps)
        {
            var sum = new Vector3();
            int count = 0;
            var addedNormals = new HashSet<Vector3>();
            for (int i = 0; i < this.triangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                var p0 = this.positions[this.triangleIndices[i0]];
                var p1 = this.positions[this.triangleIndices[i1]];
                var p2 = this.positions[this.triangleIndices[i2]];

                // check if any of the vertices are on the corner
                double d0 = (p - p0).LengthSquared();
                double d1 = (p - p1).LengthSquared();
                double d2 = (p - p2).LengthSquared();
                double mind = Math.Min(d0, Math.Min(d1, d2));
                if (mind > eps)
                {
                    continue;
                }

                // calculate the triangle normal and check if this face is already added
                var normal = Vector3.Cross(p1 - p0, p2 - p0);
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
                // dp = Math.Abs(Vector3.DotProduct(n, normal) - 1);
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
                return new Vector3();
            }

            return sum * (1.0f / count);
        }

        /// <summary>
        /// Makes sure no triangles share the same vertex.
        /// </summary>
        private void NoSharedVertices()
        {
            var p = new Vector3Collection();
            var ti = new IntCollection();
            Vector3Collection n = null;
            if (this.normals != null)
            {
                n = new Vector3Collection();
            }

            Vector2Collection tc = null;
            if (this.textureCoordinates != null)
            {
                tc = new Vector2Collection();
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
        /// Subdivides each triangle into four subtriangles.
        /// </summary>
        private void Subdivide4()
        {
            // Each triangle is divided into four subtriangles, adding new vertices in the middle of each edge.
            int ip = this.positions.Count;
            int ntri = this.triangleIndices.Count;
            for (int i = 0; i < ntri; i += 3)
            {
                int i0 = this.triangleIndices[i];
                int i1 = this.triangleIndices[i + 1];
                int i2 = this.triangleIndices[i + 2];
                var p0 = this.positions[i0];
                var p1 = this.positions[i1];
                var p2 = this.positions[i2];
                var v01 = p1 - p0;
                var v12 = p2 - p1;
                var v20 = p0 - p2;
                var p01 = p0 + (v01 * 0.5f);
                var p12 = p1 + (v12 * 0.5f);
                var p20 = p2 + (v20 * 0.5f);

                int i01 = ip++;
                int i12 = ip++;
                int i20 = ip++;

                this.positions.Add(p01);
                this.positions.Add(p12);
                this.positions.Add(p20);

                if (this.normals != null)
                {
                    var n = this.normals[i0];
                    this.normals.Add(n);
                    this.normals.Add(n);
                    this.normals.Add(n);
                }

                if (this.textureCoordinates != null)
                {
                    var uv0 = this.textureCoordinates[i0];
                    var uv1 = this.textureCoordinates[i0 + 1];
                    var uv2 = this.textureCoordinates[i0 + 2];
                    var t01 = uv1 - uv0;
                    var t12 = uv2 - uv1;
                    var t20 = uv0 - uv2;
                    var u01 = uv0 + (t01 * 0.5f);
                    var u12 = uv1 + (t12 * 0.5f);
                    var u20 = uv2 + (t20 * 0.5f);
                    this.textureCoordinates.Add(u01);
                    this.textureCoordinates.Add(u12);
                    this.textureCoordinates.Add(u20);
                }

                // TriangleIndices[i ] = i0;
                this.triangleIndices[i + 1] = i01;
                this.triangleIndices[i + 2] = i20;

                this.triangleIndices.Add(i01);
                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i12);

                this.triangleIndices.Add(i12);
                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(i20);

                this.triangleIndices.Add(i01);
                this.triangleIndices.Add(i12);
                this.triangleIndices.Add(i20);
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
            int im = this.positions.Count;
            int ntri = this.triangleIndices.Count;
            for (int i = 0; i < ntri; i += 3)
            {
                int i0 = this.triangleIndices[i];
                int i1 = this.triangleIndices[i + 1];
                int i2 = this.triangleIndices[i + 2];
                var p0 = this.positions[i0];
                var p1 = this.positions[i1];
                var p2 = this.positions[i2];
                var v01 = p1 - p0;
                var v12 = p2 - p1;
                var v20 = p0 - p2;
                var p01 = p0 + (v01 * 0.5f);
                var p12 = p1 + (v12 * 0.5f);
                var p20 = p2 + (v20 * 0.5f);
                var m = new Vector3((p0.X + p1.X + p2.X) / 3, (p0.Y + p1.Y + p2.Y) / 3, (p0.Z + p1.Z + p2.Z) / 3);

                int i01 = im + 1;
                int i12 = im + 2;
                int i20 = im + 3;

                this.positions.Add(m);
                this.positions.Add(p01);
                this.positions.Add(p12);
                this.positions.Add(p20);

                if (this.normals != null)
                {
                    var n = this.normals[i0];
                    this.normals.Add(n);
                    this.normals.Add(n);
                    this.normals.Add(n);
                    this.normals.Add(n);
                }

                if (this.textureCoordinates != null)
                {
                    var uv0 = this.textureCoordinates[i0];
                    var uv1 = this.textureCoordinates[i0 + 1];
                    var uv2 = this.textureCoordinates[i0 + 2];
                    var t01 = uv1 - uv0;
                    var t12 = uv2 - uv1;
                    var t20 = uv0 - uv2;
                    var u01 = uv0 + (t01 * 0.5f);
                    var u12 = uv1 + (t12 * 0.5f);
                    var u20 = uv2 + (t20 * 0.5f);
                    var uvm = new Vector2((uv0.X + uv1.X) * 0.5f, (uv0.Y + uv1.Y) * 0.5f);
                    this.textureCoordinates.Add(uvm);
                    this.textureCoordinates.Add(u01);
                    this.textureCoordinates.Add(u12);
                    this.textureCoordinates.Add(u20);
                }

                // TriangleIndices[i ] = i0;
                this.triangleIndices[i + 1] = i01;
                this.triangleIndices[i + 2] = im;

                this.triangleIndices.Add(i01);
                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(im);

                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i12);
                this.triangleIndices.Add(im);

                this.triangleIndices.Add(i12);
                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(im);

                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(i20);
                this.triangleIndices.Add(im);

                this.triangleIndices.Add(i20);
                this.triangleIndices.Add(i0);
                this.triangleIndices.Add(im);

                im += 4;
            }
        }
        /// <summary>
        /// Adds a torus.
        /// </summary>
        /// <param name="torusDiameter">The diameter of the torus.</param>
        /// <param name="tubeDiameter">The diameter of the torus "tube".</param>
        /// <param name="thetaDiv">The number of subdivisions around the torus.</param>
        /// <param name="phiDiv">The number of subdividions of the torus' "tube.</param>
        public void AddTorus(double torusDiameter, double tubeDiameter, int thetaDiv = 36, int phiDiv = 24)
        {
            var positionsCount = this.positions.Count;
            // No Torus Diameter means we treat the Visual3D like a Sphere
            if (torusDiameter == 0.0)
            {
                this.AddSphere(new Vector3(), tubeDiameter, thetaDiv, phiDiv);
            }
            // If the second Diameter is zero, we can't build out torus
            else if (tubeDiameter == 0.0)
                throw new HelixToolkitException("Torus must have a Diameter bigger than 0");
            // Values result in a Torus
            else
            {
                // Points of the Cross-Section of the torus "tube"
                IList<Vector2> crossSectionPoints;
                // Self-intersecting Torus, if the "Tube" Diameter is bigger than the Torus Diameter
                var selfIntersecting = tubeDiameter > torusDiameter;
                if (selfIntersecting)
                {
                    // Angle-Calculations for Circle Segment https://en.wikipedia.org/wiki/Isosceles_triangle
                    var angleIcoTriangle = (float)Math.Acos(1 - ((torusDiameter * torusDiameter) / (2 * (tubeDiameter * tubeDiameter * .25))));
                    var circleAngle = (float)Math.PI + angleIcoTriangle;
                    var offset = -circleAngle / 2f;
                    // The Cross-Section is defined by only a Segment of a Circle
                    crossSectionPoints = GetCircleSegment(phiDiv, circleAngle, offset);
                }
                // "normal" Torus (with a Circle as Cross-Section of the Torus
                else
                {
                    crossSectionPoints = GetCircle(phiDiv, true);
                }
                // Transform Crosssection to real Size
                crossSectionPoints = crossSectionPoints.Select(p => new Vector2(p.X * (float)tubeDiameter * .5f, p.Y * (float)tubeDiameter * .5f)).ToList();
                // Transform the Cross-Section Points to 3D Space
                var crossSection3DPoints = crossSectionPoints.Select(p => new Vector3(p.X, 0, p.Y)).ToList();

                // Add the needed Vertex-Positions of the Torus
                for (int i = 0; i < thetaDiv; i++)
                {
                    // Angle of the current Cross-Section in the XY-Plane
                    var angle = (float)Math.PI * 2 * ((float)i / thetaDiv);
                    // Rotate the Cross-Section around the Origin by using the angle and the defined torusDiameter
                    var rotatedPoints = crossSection3DPoints.Select(p3D => new Vector3((float)Math.Cos(angle) * (p3D.X + (float)torusDiameter * .5f), (float)Math.Sin(angle) * (p3D.X + (float)torusDiameter * .5f), p3D.Z)).ToList();
                    for (int j = 0; j < phiDiv; j++)
                    {
                        // If selfintersecting Torus, skip the first and last Point of the Cross-Sections, when not the first Cross Section.
                        // We only need the first and last Point of the first Cross-Section once!
                        if (selfIntersecting && i > 0 && (j == 0 || j == (phiDiv - 1)))
                            continue;
                        // Add the Position
                        this.positions.Add(rotatedPoints[j]);
                    }
                }
                // Add all Normals, if they need to be calculated
                if (this.normals != null)
                {
                    for (int i = 0; i < thetaDiv; i++)
                    {
                        // Transform the Cross-Section as well as the Origin of the Cross-Section
                        var angle = Math.PI * 2 * ((double)i / thetaDiv);
                        var rotatedPoints = crossSection3DPoints.Select(p3D => new Vector3((float)Math.Cos(angle) * (p3D.X + (float)torusDiameter * .5f), (float)Math.Sin(angle) * (p3D.X + (float)torusDiameter * .5f), p3D.Z)).ToList();
                        // We don't need the first and last Point of the rotated Points, if we are not in the first Cross-Section
                        if (selfIntersecting && i > 0)
                        {
                            rotatedPoints.RemoveAt(0);
                            rotatedPoints.RemoveAt(rotatedPoints.Count - 1);
                        }
                        // Transform the Center of the Cross-Section
                        var rotatedOrigin = new Vector3((float)Math.Cos(angle) * (float)torusDiameter * .5f, (float)Math.Sin(angle) * (float)torusDiameter * .5f, 0);
                        // Add the Normal of the Vertex
                        for (int j = 0; j < rotatedPoints.Count; j++)
                        {
                            // The default Normal has the same Direction as the Vector from the Center to the Vertex
                            var normal = rotatedPoints[j] - rotatedOrigin;
                            normal.Normalize();
                            // If self-intersecting Torus and first Point of first Cross-Section,
                            // modify Normal
                            if (selfIntersecting && i == 0 && j == 0)
                            {
                                normal = new Vector3(0, 0, -1);
                            }
                            // If self-intersecting Torus and last Point of first Cross-Section
                            // modify Normal
                            else if (selfIntersecting && i == 0 && j == (phiDiv - 1))
                            {
                                normal = new Vector3(0, 0, 1);
                            }
                            // Add the Normal
                            this.normals.Add(normal);
                        }
                    }
                }
                // Add all Texture Coordinates, if they need to be calculated
                if (this.textureCoordinates != null)
                {
                    // For all Points, calculate a simple uv Coordinate
                    for (int i = 0; i < thetaDiv; i++)
                    {
                        // Determine the Number of Vertices of this Cross-Section present in the positions Collection
                        var numCS = (selfIntersecting && i > 0) ? phiDiv - 2 : phiDiv;
                        for (int j = 0; j < numCS; j++)
                        {
                            // Calculate u- and v- Coordinates for the Points
                            var u = (float)i / thetaDiv;
                            float v = 0;
                            if (i > 0 && selfIntersecting)
                                v = (float)(j + 1) / phiDiv;
                            else
                                v = (float)j / phiDiv;
                            // Add the Texture-Coordinate
                            this.textureCoordinates.Add(new Vector2(u, v));
                        }
                    }
                }
                // Add Triangle-Indices
                for (int i = 0; i < thetaDiv; i++)
                {
                    // Normal non-selfintersecting Torus
                    // Just add Triangle-Strips between all neighboring Cross-Sections
                    if (!selfIntersecting)
                    {
                        var firstPointIdx = i * phiDiv;
                        var firstPointIdxNextCircle = ((i + 1) % thetaDiv) * phiDiv;
                        for (int j = 0; j < phiDiv; j++)
                        {
                            var jNext = (j + 1) % phiDiv;
                            this.triangleIndices.Add(firstPointIdx + j + positionsCount);
                            this.triangleIndices.Add(firstPointIdx + jNext + positionsCount);
                            this.triangleIndices.Add(firstPointIdxNextCircle + j + positionsCount);

                            this.triangleIndices.Add(firstPointIdxNextCircle + j + positionsCount);
                            this.triangleIndices.Add(firstPointIdx + jNext + positionsCount);
                            this.triangleIndices.Add(firstPointIdxNextCircle + jNext + positionsCount);
                        }
                    }
                    // Selfintersecting Torus
                    else
                    {
                        // Add intermediate Triangles like for the non-selfintersecting Torus
                        // Skip the first and last Triangles, the "Caps" will be added later
                        // Determine the Index of the first Point of the first Cross-Section
                        var firstPointIdx = i * (phiDiv - 2) + 1;
                        firstPointIdx += i > 0 ? 1 : 0;
                        // Determine the Index of the first Point of the next Cross-Section
                        var firstPointIdxNextCircle = phiDiv + firstPointIdx - 1;
                        firstPointIdxNextCircle -= i > 0 ? 1 : 0;
                        if (firstPointIdxNextCircle >= this.positions.Count)
                        {
                            firstPointIdxNextCircle %= this.positions.Count;
                            firstPointIdxNextCircle++;
                        }
                        // Add Triangles between the "middle" Parts of the neighboring Cross-Sections
                        for (int j = 1; j < phiDiv - 2; j++)
                        {
                            this.triangleIndices.Add(firstPointIdx + j - 1 + positionsCount);
                            this.triangleIndices.Add(firstPointIdxNextCircle + j - 1 + positionsCount);
                            this.triangleIndices.Add(firstPointIdx + j + positionsCount);

                            this.triangleIndices.Add(firstPointIdxNextCircle + j - 1 + positionsCount);
                            this.triangleIndices.Add(firstPointIdxNextCircle + j + positionsCount);
                            this.triangleIndices.Add(firstPointIdx + j + positionsCount);
                        }
                    }
                }
                // For selfintersecting Tori
                if (selfIntersecting)
                {
                    // Add bottom Cap by creating a List of Vertex-Indices
                    // and using them to create a Triangle-Fan
                    var verts = new List<int>();
                    verts.Add(0);
                    for (int i = 0; i < thetaDiv; i++)
                    {
                        if (i == 0)
                        {
                            verts.Add(1 + positionsCount);
                        }
                        else
                        {
                            verts.Add(phiDiv + (i - 1) * (phiDiv - 2) + positionsCount);
                        }
                    }
                    verts.Add(1 + positionsCount);
                    verts.Reverse();
                    AddTriangleFan(verts);

                    // Add top Cap by creating a List of Vertex-Indices
                    // and using them to create a Triangle-Fan
                    verts = new List<int>();
                    verts.Add(phiDiv - 1 + positionsCount);
                    for (int i = 0; i < thetaDiv; i++)
                    {
                        if (i == 0)
                        {
                            verts.Add(phiDiv - 2 + positionsCount);
                        }
                        else
                        {
                            verts.Add(phiDiv + i * (phiDiv - 2) - 1 + positionsCount);
                        }
                    }
                    verts.Add(phiDiv - 2 + positionsCount);
                    AddTriangleFan(verts);
                }
            }
        }
    }
}