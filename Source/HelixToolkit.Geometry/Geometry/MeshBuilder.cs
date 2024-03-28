using CommunityToolkit.Diagnostics;
using HelixToolkit.Geometry;
using System.Diagnostics;

namespace HelixToolkit.Geometry;

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
public sealed class MeshBuilder
{
    #region Static and Const
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
    /// 'Wrong number of angles' exception message.
    /// </summary>
    private const string WrongNumberOfAngles = "Wrong number of angles.";
    /// <summary>
    /// The circle cache.
    /// </summary>
    private static readonly ThreadLocal<Dictionary<int, IList<Vector2>>> CircleCache = new(() => new Dictionary<int, IList<Vector2>>());
    /// <summary>
    /// The closed circle cache.
    /// </summary>
    private static readonly ThreadLocal<Dictionary<int, IList<Vector2>>> ClosedCircleCache = new(() => new Dictionary<int, IList<Vector2>>());
    /// <summary>
    /// The unit sphere cache.
    /// </summary>
    private static readonly ThreadLocal<Dictionary<int, MeshGeometry3D>> UnitSphereCache = new(() => new Dictionary<int, MeshGeometry3D>());

    #endregion Static and Const


    #region Variables and Properties
    /// <summary>
    /// The positions.
    /// </summary>
    public Vector3Collection Positions { get; set; } = new();
    /// <summary>
    /// The triangle indices.
    /// </summary>
    public IntCollection TriangleIndices { get; set; } = new();
    /// <summary>
    /// The normal vectors.
    /// </summary>
    public Vector3Collection? Normals { get; set; }
    /// <summary>
    /// The texture coordinates.
    /// </summary>
    public Vector2Collection? TextureCoordinates { get; set; }
    /// <summary>
    /// The Tangents.
    /// </summary>
    public Vector3Collection? Tangents { get; set; }
    /// <summary>
    /// The Bi-Tangents.
    /// </summary>
    public Vector3Collection? BiTangents { get; set; }
    /// <summary>
    /// Do we have Normals or not.
    /// </summary>
    public bool HasNormals => this.Normals is not null;
    /// <summary>
    /// Do we have Texture Coordinates or not.
    /// </summary>
    public bool HasTexCoords => this.TextureCoordinates is not null;
    /// <summary>
    /// Do we have Tangents or not.
    /// </summary>
    public bool HasTangents => this.Tangents is not null;
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
            return this.Normals is not null;
        }
        set
        {
            if (value && this.Normals is null)
            {
                this.Normals = new Vector3Collection();
            }
            if (!value)
            {
                this.Normals = null;
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
            return this.TextureCoordinates is not null;
        }
        set
        {
            if (value && this.TextureCoordinates is null)
            {
                this.TextureCoordinates = new Vector2Collection();
            }
            if (!value)
            {
                this.TextureCoordinates = null;
            }
        }
    }
    #endregion Variables and Properties


    #region Constructors
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
    /// <param name="generateTexCoords">
    /// Generate texture coordinates.
    /// </param>
    /// <param name="tangentSpace">
    /// Generate tangents.
    /// </param>
    public MeshBuilder(bool generateNormals = true, bool generateTexCoords = true, bool tangentSpace = false)
    {
        if (generateNormals)
        {
            this.Normals = new();
        }
        if (generateTexCoords)
        {
            this.TextureCoordinates = new();
        }
        if (tangentSpace)
        {
            this.Tangents = new();
            this.BiTangents = new();
        }
    }
    #endregion Constructors


    #region Geometric Base Functions
    /// <summary>
    /// Gets a circle section (cached).
    /// </summary>
    /// <param name="thetaDiv">
    /// The number of division.
    /// </param>
    /// <param name="closed">
    /// Is the circle closed?
    /// If true, the last point will be in the same position as the first one.
    /// </param>
    /// <returns>
    /// A circle.
    /// </returns>
    public static IList<Vector2> GetCircle(int thetaDiv, bool closed = false)
    {
        Dictionary<int, IList<Vector2>>? cache = null;
        IList<Vector2>? circle;
        if (!IsCacheExists(ref cache, thetaDiv, closed, out circle))
        {
            circle = new Vector2Collection(closed ? thetaDiv : thetaDiv + 1);
            cache!.Add(thetaDiv, circle);
            // Determine the angle steps
            float angle = (float)Math.PI * 2f / thetaDiv;
            for (var i = 0; i < thetaDiv; i++)
            {
                circle.Add(new Vector2((float)Math.Cos(i * angle), -(float)Math.Sin(i * angle)));
            }
            if (closed)
            {
                circle.Add(circle[0]);
            }
        }
        // Since Vector2Collection is not Freezable,
        // return new IList<Vector> to avoid manipulation of the Cached Values
        if (circle is not null)
        {
            return new Vector2Collection(circle);
        }
        return new Vector2Collection();

        static bool IsCacheExists(ref Dictionary<int, IList<Vector2>>? cache, int thetaDiv, bool closed, out IList<Vector2>? circle)
        {
            if (closed)
            {
                cache = ClosedCircleCache.Value;
            }
            else
            {
                cache = CircleCache.Value;
            }
            return cache!.TryGetValue(thetaDiv, out circle);
        }
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
        IList<Vector2> circleSegment = new Vector2Collection();
        for (var i = 0; i < thetaDiv; i++)
        {
            var theta = totalAngle * ((float)i / (thetaDiv - 1)) + angleOffset;
            circleSegment.Add(new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)));
        }

        return circleSegment;
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
        if (UnitSphereCache.Value!.ContainsKey(subdivisions))
        {
            return UnitSphereCache.Value[subdivisions];
        }

        var mb = new MeshBuilder(false, false);
        mb.AddRegularIcosahedron(Vector3.Zero, 1, false);
        for (var i = 0; i < subdivisions; i++)
        {
            mb.SubdivideLinear();
        }

        for (var i = 0; i < mb.Positions.Count; i++)
        {
            mb.Positions[i] = Vector3.Normalize(mb.Positions[i]);
        }
        var mesh = mb.ToMesh();
        UnitSphereCache.Value[subdivisions] = mesh;
        return mesh;
    }

    /// <summary>
    /// Calculate the Mesh's Normals
    /// </summary>
    /// <param name="positions">The Positions.</param>
    /// <param name="triangleIndices">The TriangleIndices.</param>
    /// <param name="normals">The calcualted Normals.</param>
    private static void ComputeNormals(IList<Vector3> positions, IList<int> triangleIndices, out IList<Vector3> normals)
    {
        normals = new Vector3Collection(positions.Count);
        for (var i = 0; i < positions.Count; i++)
        {
            normals.Add(Vector3.Zero);
        }
        for (var t = 0; t < triangleIndices.Count; t += 3)
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
            p1 = Vector3.Normalize(p1);
            p2 = Vector3.Normalize(p2);
            var a = (float)Math.Acos(Vector3.Dot(p1, p2));
            n = Vector3.Normalize(n);
            normals[i1] += (a * n);
            normals[i2] += (a * n);
            normals[i3] += (a * n);
        }
        for (var i = 0; i < normals.Count; i++)
        {
            //Cannot use normals[i].normalize() if using Media3D.Vector3DCollection. Does not change the internal value in Vector3DCollection.
            normals[i] = Vector3.Normalize(normals[i]);
        }
    }

    /// <summary>
    /// Calculate the Mesh's Tangents
    /// </summary>
    /// <param name="meshFaces">The Faces of the Mesh</param>
    public void ComputeTangents(MeshFaces meshFaces)
    {
        switch (meshFaces)
        {
            case MeshFaces.Default:
                if (this.Positions != null && this.TriangleIndices != null && this.Normals != null && this.TextureCoordinates != null)
                {
                    ComputeTangents(this.Positions, this.Normals, this.TextureCoordinates, this.TriangleIndices,
                        out var t1, out var t2);
                    Debug.Assert(t1 is Vector3Collection);
                    Debug.Assert(t2 is Vector3Collection);
                    this.Tangents = t1 as Vector3Collection;
                    this.BiTangents = t2 as Vector3Collection;
                }
                break;
            case MeshFaces.QuadPatches:
                if (this.Positions != null && this.TriangleIndices != null && this.Normals != null && this.TextureCoordinates != null)
                {
                    ComputeTangentsQuads(this.Positions, this.Normals, this.TextureCoordinates, this.TriangleIndices,
                        out var t1, out var t2);
                    Debug.Assert(t1 is Vector3Collection);
                    Debug.Assert(t2 is Vector3Collection);
                    this.Tangents = t1 as Vector3Collection;
                    this.BiTangents = t2 as Vector3Collection;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Tangent Space computation for IndexedTriangle meshes
    /// Based on:
    /// http://www.terathon.com/code/tangent.html
    /// </summary>
    public static void ComputeTangents(IList<Vector3>? positions, IList<Vector3>? normals, IList<Vector2>? textureCoordinates, IList<int> triangleIndices,
        out IList<Vector3> tangents, out IList<Vector3> bitangents)
    {
        positions ??= new Vector3Collection();
        normals ??= new Vector3Collection();
        textureCoordinates ??= new Vector2Collection();

        var tan1 = new Vector3[positions.Count];
        for (var t = 0; t < triangleIndices.Count; t += 3)
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
            var x1 = v2.X - v1.X;
            var x2 = v3.X - v1.X;
            var y1 = v2.Y - v1.Y;
            var y2 = v3.Y - v1.Y;
            var z1 = v2.Z - v1.Z;
            var z2 = v3.Z - v1.Z;
            var s1 = w2.X - w1.X;
            var s2 = w3.X - w1.X;
            var t1 = w2.Y - w1.Y;
            var t2 = w3.Y - w1.Y;
            var r = 1.0f / (s1 * t2 - s2 * t1);
            var udir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            tan1[i1] += udir;
            tan1[i2] += udir;
            tan1[i3] += udir;
        }
        tangents = new Vector3Collection(positions.Count);
        bitangents = new Vector3Collection(positions.Count);
        for (var i = 0; i < positions.Count; i++)
        {
            var n = normals[i];
            var t = tan1[i];
            t = Vector3.Normalize(t - n * Vector3.Dot(n, t));
            var b = Vector3.Cross(n, t);
            tangents.Add(t);
            bitangents.Add(b);
        }
    }

    /// <summary>
    /// Calculate the Tangents for a Quad.
    /// </summary>
    /// <param name="positions">The Positions.</param>
    /// <param name="normals">The Normals.</param>
    /// <param name="textureCoordinates">The TextureCoordinates.</param>
    /// <param name="indices">The Indices.</param>
    /// <param name="tangents">The calculated Tangens.</param>
    /// <param name="bitangents">The calculated Bi-Tangens.</param>
    public static void ComputeTangentsQuads(IList<Vector3>? positions, IList<Vector3>? normals, IList<Vector2>? textureCoordinates, IList<int> indices,
        out IList<Vector3> tangents, out IList<Vector3> bitangents)
    {
        positions ??= new Vector3Collection();
        normals ??= new Vector3Collection();
        textureCoordinates ??= new Vector2Collection();

        var tan1 = new Vector3[positions.Count];
        for (var t = 0; t < indices.Count; t += 4)
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
            var x1 = v2.X - v1.X;
            var x2 = v4.X - v1.X;
            var y1 = v2.Y - v1.Y;
            var y2 = v4.Y - v1.Y;
            var z1 = v2.Z - v1.Z;
            var z2 = v4.Z - v1.Z;
            var s1 = w2.X - w1.X;
            var s2 = w4.X - w1.X;
            var t1 = w2.Y - w1.Y;
            var t2 = w4.Y - w1.Y;
            var r = 1.0f / (s1 * t2 - s2 * t1);
            var udir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            tan1[i1] += udir;
            tan1[i2] += udir;
            tan1[i3] += udir;
            tan1[i4] += udir;
        }
        tangents = new Vector3Collection(positions.Count);
        bitangents = new Vector3Collection(positions.Count);
        for (var i = 0; i < positions.Count; i++)
        {
            var n = normals[i];
            var t = tan1[i];
            t = Vector3.Normalize(t - n * Vector3.Dot(n, t));
            var b = Vector3.Cross(n, t);
            tangents.Add(t);
            bitangents.Add(b);
        }
    }

    /// <summary>
    /// Calculate the Tangents for a MeshGeometry3D.
    /// </summary>
    /// <param name="meshGeometry">The MeshGeometry3D.</param>
    public static void ComputeTangents(MeshGeometry3D meshGeometry)
    {
        if (meshGeometry.Positions is not null && meshGeometry.Normals is not null && meshGeometry.TextureCoordinates is not null && meshGeometry.TriangleIndices is not null)
        {
            ComputeTangents(meshGeometry.Positions, meshGeometry.Normals, meshGeometry.TextureCoordinates, meshGeometry.TriangleIndices,
                out var t1, out var t2);

            meshGeometry.Tangents = t1 as Vector3Collection;
            meshGeometry.BiTangents = t2 as Vector3Collection;
        }
    }

    /// <summary>
    /// Calculate the Normals and Tangents for all MeshFaces.
    /// </summary>
    /// <param name="meshFaces">The MeshFaces.</param>
    /// <param name="tangents">Also calculate the Tangents or not.</param>
    public void ComputeNormalsAndTangents(MeshFaces meshFaces, bool tangents = false)
    {
        if (!this.HasNormals && this.Positions != null && this.TriangleIndices != null)
        {
            ComputeNormals(this.Positions, this.TriangleIndices, out var normals);
            Debug.Assert(normals is Vector3Collection);
            this.Normals = normals as Vector3Collection;
        }
        switch (meshFaces)
        {
            case MeshFaces.Default:
                if (tangents && this.HasNormals && this.Positions != null && this.Normals != null && this.TextureCoordinates != null && this.TriangleIndices != null)
                {
                    ComputeTangents(this.Positions, this.Normals, this.TextureCoordinates, this.TriangleIndices,
                        out var t1, out var t2);
                    Debug.Assert(t1 is Vector3Collection);
                    Debug.Assert(t2 is Vector3Collection);
                    this.Tangents = t1 as Vector3Collection;
                    this.BiTangents = t2 as Vector3Collection;
                }
                break;
            case MeshFaces.QuadPatches:
                if (tangents && this.HasNormals && this.Positions != null && this.Normals != null && this.TextureCoordinates != null && this.TriangleIndices != null)
                {
                    ComputeTangentsQuads(this.Positions, this.Normals, this.TextureCoordinates, this.TriangleIndices,
                        out var t1, out var t2);
                    Debug.Assert(t1 is Vector3Collection);
                    Debug.Assert(t2 is Vector3Collection);
                    this.Tangents = t1 as Vector3Collection;
                    this.BiTangents = t2 as Vector3Collection;
                }
                break;
            default:
                break;
        }
    }
    #endregion Geometric Base Functions


    #region Add Geometry
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
    public void AddArrow(Vector3 point1, Vector3 point2, float diameter, float headLength = 3, int thetaDiv = 18)
    {
        var dir = point2 - point1;
        var length = dir.Length();
        var r = diameter / 2;

        var pc = new Vector2Collection
                {
                    new Vector2(0, 0),
                    new Vector2(0, r),
                    new Vector2(length - (diameter * headLength), r),
                    new Vector2(length - (diameter * headLength), r * 2),
                    new Vector2(length, 0)
                };

        this.AddRevolvedGeometry(pc, null, point1, dir, thetaDiv);
    }

    /// <summary>
    /// Adds the edges of a bounding box as pipes.
    /// </summary>
    /// <param name="boundingBox">
    /// The bounding box.
    /// </param>
    /// <param name="diameter">
    /// The diameter of the cylinders.
    /// </param>
    public void AddBoundingBox(BoundingBox boundingBox, float diameter)
    {
        Vector3[] corners = boundingBox.GetCorners();
        void addEdge(Vector3 c1, Vector3 c2) => this.AddPipe(c1, c2, 0, diameter, 10);

        addEdge(corners[0], corners[1]);
        addEdge(corners[1], corners[2]);
        addEdge(corners[2], corners[3]);
        addEdge(corners[3], corners[0]);

        addEdge(corners[4], corners[5]);
        addEdge(corners[5], corners[6]);
        addEdge(corners[6], corners[7]);
        addEdge(corners[7], corners[4]);

        addEdge(corners[0], corners[4]);
        addEdge(corners[3], corners[7]);
        addEdge(corners[1], corners[5]);
        addEdge(corners[2], corners[6]);
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
    public void AddBox(Vector3 center, float xlength, float ylength, float zlength)
    {
        this.AddBox(center, xlength, ylength, zlength, BoxFaces.All);
    }

    /// <summary>
    /// Adds a box aligned with the X, Y and Z axes.
    /// </summary>
    /// <param name="boundingBox">
    /// The bounding box.
    /// </param>
    /// <param name="faces">The faces to include.</param>
    public void AddBox(BoundingBox boundingBox, BoxFaces faces = BoxFaces.All)
    {
        this.AddBox(boundingBox.Center, boundingBox.Width, boundingBox.Height, boundingBox.Depth, faces);
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
    public void AddBox(Vector3 center, float xlength, float ylength, float zlength, BoxFaces faces)
    {
        this.AddBox(center, Vector3.UnitX, Vector3.UnitY, xlength, ylength, zlength, faces);
    }

    /// <summary>
    /// Adds a box with the specified faces, aligned with the specified axes.
    /// </summary>
    /// <param name="center">The center point of the box.</param>
    /// <param name="x">The x axis.</param>
    /// <param name="y">The y axis.</param>
    /// <param name="xlength">The length of the box along the X axis.</param>
    /// <param name="ylength">The length of the box along the Y axis.</param>
    /// <param name="zlength">The length of the box along the Z axis.</param>
    /// <param name="faces">The faces to include.</param>
    public void AddBox(Vector3 center, Vector3 x, Vector3 y, float xlength, float ylength, float zlength, BoxFaces faces = BoxFaces.All)
    {
        var z = Vector3.Cross(x, y);
        if ((faces & BoxFaces.Front) == BoxFaces.Front)
        {
            this.AddCubeFace(center, x, z, xlength, ylength, zlength);
        }

        if ((faces & BoxFaces.Back) == BoxFaces.Back)
        {
            this.AddCubeFace(center, -x, z, xlength, ylength, zlength);
        }

        if ((faces & BoxFaces.Left) == BoxFaces.Left)
        {
            this.AddCubeFace(center, -y, z, ylength, xlength, zlength);
        }

        if ((faces & BoxFaces.Right) == BoxFaces.Right)
        {
            this.AddCubeFace(center, y, z, ylength, xlength, zlength);
        }

        if ((faces & BoxFaces.Top) == BoxFaces.Top)
        {
            this.AddCubeFace(center, z, y, zlength, xlength, ylength);
        }

        if ((faces & BoxFaces.Bottom) == BoxFaces.Bottom)
        {
            this.AddCubeFace(center, -z, y, zlength, xlength, ylength);
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
    public void AddCone(Vector3 origin, Vector3 direction,
        float baseRadius, float topRadius, float height,
        bool baseCap, bool topCap, int thetaDiv)
    {
        var pc = new Vector2Collection();
        var tc = new List<float>();
        if (baseCap)
        {
            pc.Add(Vector2.Zero);
            tc.Add(0);
        }

        pc.Add(new Vector2(0, baseRadius));
        tc.Add(1);
        pc.Add(new Vector2(height, topRadius));
        tc.Add(0);
        if (topCap)
        {
            pc.Add(new Vector2(height, 0));
            tc.Add(1);
        }

        this.AddRevolvedGeometry(pc, tc, origin, direction, thetaDiv);
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
    public void AddCone(Vector3 origin, Vector3 apex, float baseRadius, bool baseCap, int thetaDiv)
    {
        var dir = apex - origin;
        this.AddCone(origin, dir, baseRadius, 0, dir.Length(), baseCap, false, thetaDiv);
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
    public void AddCubeFace(Vector3 center, Vector3 normal, Vector3 up, float dist, float width, float height)
    {
        var right = Vector3.Cross(normal, up);
        var n = normal * dist / 2;
        up *= height / 2;
        right *= width / 2;
        var p1 = center + n - up - right;
        var p2 = center + n - up + right;
        var p3 = center + n + up + right;
        var p4 = center + n + up - right;

        var i0 = this.Positions.Count;
        this.Positions.Add(p1);
        this.Positions.Add(p2);
        this.Positions.Add(p3);
        this.Positions.Add(p4);

        if (this.Normals != null)
        {
            this.Normals.Add(normal);
            this.Normals.Add(normal);
            this.Normals.Add(normal);
            this.Normals.Add(normal);
        }

        if (this.TextureCoordinates != null)
        {
            this.TextureCoordinates.Add(new Vector2(1, 1));
            this.TextureCoordinates.Add(new Vector2(0, 1));
            this.TextureCoordinates.Add(new Vector2(0, 0));
            this.TextureCoordinates.Add(new Vector2(1, 0));
        }

        this.TriangleIndices.Add(i0 + 2);
        this.TriangleIndices.Add(i0 + 1);
        this.TriangleIndices.Add(i0 + 0);
        this.TriangleIndices.Add(i0 + 0);
        this.TriangleIndices.Add(i0 + 3);
        this.TriangleIndices.Add(i0 + 2);
    }

    /// <summary>
    /// Add a Cube, only with specified Faces.
    /// </summary>
    /// <param name="faces">The Faces to create (default all Faces)</param>
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
    public void AddCylinder(Vector3 p1, Vector3 p2, float diameter, int thetaDiv)
    {
        var n = p2 - p1;
        var l = n.Length();
        n = Vector3.Normalize(n);
        this.AddCone(p1, n, diameter / 2, diameter / 2, l, false, false, thetaDiv);
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
    /// <param name="cap1">
    /// The first Cap.
    /// </param>
    /// <param name="cap2">
    /// The second Cap.
    /// </param>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Cylinder_(geometry).
    /// </remarks>
    public void AddCylinder(Vector3 p1, Vector3 p2, float radius = 1, int thetaDiv = 32, bool cap1 = true, bool cap2 = true)
    {
        var n = p2 - p1;
        var l = n.Length();
        n = Vector3.Normalize(n);
        this.AddCone(p1, n, radius, radius, l, cap1, cap2, thetaDiv);
    }

    /// <summary>
    /// Generate a Dodecahedron
    /// </summary>
    /// <param name="center">The Center of the Dodecahedron</param>
    /// <param name="forward">The Direction to the first Point (normalized).</param>
    /// <param name="up">The Up-Dirextion (normalized, perpendicular to the forward Direction)</param>
    /// <param name="sideLength">Length of the Edges of the Dodecahedron</param>
    /// <remarks>
    /// See:
    /// https://en.wikipedia.org/wiki/Dodecahedron
    /// https://en.wikipedia.org/wiki/Pentagon
    /// https://en.wikipedia.org/wiki/Isosceles_triangle
    /// </remarks>
    public void AddDodecahedron(Vector3 center, Vector3 forward, Vector3 up, float sideLength)
    {
        // If points already exist in the MeshBuilder
        var positionsCount = this.Positions.Count;

        var right = Vector3.Cross(up, forward);
        // Distance from the Center to the Dodekaeder-Points
        var radiusSphere = 0.25f * (float)Math.Sqrt(3) * (1 + (float)Math.Sqrt(5)) * sideLength;
        var radiusFace = 0.1f * (float)Math.Sqrt(50 + 10 * (float)Math.Sqrt(5)) * sideLength;
        var vectorDown = (float)Math.Sqrt(radiusSphere * radiusSphere - radiusFace * radiusFace);

        // Add Points
        var baseCenter = center - up * vectorDown;
        var pentagonPoints = GetCircle(5, false);
        // Base Points
        var basePoints = new Vector3Collection();
        foreach (var point in pentagonPoints)
        {
            var newPoint = baseCenter + forward * point.X * radiusFace + right * point.Y * radiusFace;
            basePoints.Add(newPoint);
            this.Positions.Add(newPoint);
        }
        // Angle of Projected Isosceles triangle
        var gamma = (float)Math.Acos(1 - (sideLength * sideLength / (2 * radiusSphere * radiusSphere)));
        // Base Upper Points
        foreach (var point in basePoints)
        {
            var baseCenterToPoint = Vector3.Normalize(point - baseCenter);
            var centerToPoint = Vector3.Normalize(point - center);
            var tempRight = Vector3.Cross(up, baseCenterToPoint);
            var newPoint = new Vector3(radiusSphere * (float)Math.Cos(gamma), 0, radiusSphere * (float)Math.Sin(gamma));
            var tempUp = Vector3.Cross(centerToPoint, tempRight);
            this.Positions.Add(center + centerToPoint * newPoint.X + tempUp * newPoint.Z);
        }

        // Top Points
        var topCenter = center + up * vectorDown;
        var topPoints = new Vector3Collection();
        foreach (var point in pentagonPoints)
        {
            var newPoint = topCenter - forward * point.X * radiusFace + right * point.Y * radiusFace;
            topPoints.Add(newPoint);
        }
        // Top Lower Points
        foreach (var point in topPoints)
        {
            var topCenterToPoint = Vector3.Normalize(point - topCenter);
            var centerToPoint = Vector3.Normalize(point - center);
            var tempRight = Vector3.Cross(up, topCenterToPoint);
            var newPoint = new Vector3(radiusSphere * (float)Math.Cos(gamma), 0, radiusSphere * (float)Math.Sin(gamma));
            var tempUp = Vector3.Cross(tempRight, centerToPoint);
            this.Positions.Add(center + centerToPoint * newPoint.X + tempUp * newPoint.Z);
        }
        // Add top Points at last
        foreach (var point in topPoints)
        {
            this.Positions.Add(point);
        }

        // Add Normals if wanted
        if (this.Normals != null)
        {
            for (var i = positionsCount; i < this.Positions.Count; i++)
            {
                var centerToPoint = Vector3.Normalize(this.Positions[i] - center);
                this.Normals.Add(centerToPoint);
            }
        }

        // Add Texture Coordinates
        if (this.TextureCoordinates != null)
        {
            for (var i = positionsCount; i < this.Positions.Count; i++)
            {
                var centerToPoint = Vector3.Normalize(this.Positions[i] - center);
                var cTPUpValue = Vector3.Dot(centerToPoint, up);
                var planeCTP = Vector3.Normalize(centerToPoint - up * cTPUpValue);
                var u = (float)Math.Atan2(Vector3.Dot(planeCTP, forward), Vector3.Dot(planeCTP, right));
                var v = cTPUpValue * 0.5f + 0.5f;
                this.TextureCoordinates.Add(new Vector2(u, v));
            }
        }

        // Add Faces
        // Base Polygon
        this.AddPolygonByTriangulation(this.Positions.Skip(positionsCount).Take(5).Select((p, i) => i).ToList());
        // Top Polygon
        this.AddPolygonByTriangulation(this.Positions.Skip(positionsCount + 15).Select((p, i) => 15 + i).ToList());
        // SidePolygons
        for (var i = 0; i < 5; i++)
        {
            // Polygon one
            var pIndices = new IntCollection {
                    (i + 1) % 5 + positionsCount,
                    i, i + 5 + positionsCount,
                    (5 - i + 2) % 5 + 10 + positionsCount,
                    (i + 1) % 5 + 5 + positionsCount
                };
            this.AddPolygonByTriangulation(pIndices);

            // Polygon two
            pIndices = new IntCollection {
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
    public void AddEdges(IList<Vector3> points, IList<int> edges, float diameter, int thetaDiv)
    {
        for (var i = 0; i < edges.Count - 1; i += 2)
        {
            this.AddCylinder(points[edges[i]], points[edges[i + 1]], diameter, thetaDiv);
        }
    }

    /// <summary>
    /// Adds an ellipsoid.
    /// </summary>
    /// <param name="center">
    /// The center of the ellipsoid.
    /// </param>
    /// <param name="radiusx">
    /// The x radius of the ellipsoid.
    /// </param>
    /// <param name="radiusy">
    /// The y radius of the ellipsoid.
    /// </param>
    /// <param name="radiusz">
    /// The z radius of the ellipsoid.
    /// </param>
    /// <param name="thetaDiv">
    /// The number of divisions around the ellipsoid.
    /// </param>
    /// <param name="phiDiv">
    /// The number of divisions from top to bottom of the ellipsoid.
    /// </param>
    public void AddEllipsoid(Vector3 center, float radiusx, float radiusy, float radiusz, int thetaDiv = 20, int phiDiv = 10)
    {
        var index0 = this.Positions.Count;
        var dt = 2 * (float)Math.PI / thetaDiv;
        var dp = (float)Math.PI / phiDiv;

        for (var pi = 0; pi <= phiDiv; pi++)
        {
            var phi = pi * dp;

            for (var ti = 0; ti <= thetaDiv; ti++)
            {
                // we want to start the mesh on the x axis
                var theta = ti * dt;

                // Spherical coordinates
                // http://mathworld.wolfram.com/SphericalCoordinates.html
                var x = (float)Math.Cos(theta) * (float)Math.Sin(phi);
                var y = (float)Math.Sin(theta) * (float)Math.Sin(phi);
                var z = (float)Math.Cos(phi);

                var p = new Vector3(center.X + (radiusx * x), center.Y + (radiusy * y), center.Z + (radiusz * z));
                this.Positions.Add(p);

                if (this.Normals != null)
                {
                    var n = new Vector3(x, y, z);
                    this.Normals.Add(n);
                }

                if (this.TextureCoordinates != null)
                {
                    var uv = new Vector2(theta / (2 * (float)Math.PI), phi / (float)Math.PI);
                    this.TextureCoordinates.Add(uv);
                }
            }
        }

        this.AddRectangularMeshTriangleIndices(index0, phiDiv + 1, thetaDiv + 1, true);
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
    /// The y-axis is determined by the cross product between the specified x-axis and the p1-origin vector.
    /// </remarks>
    public void AddExtrudedGeometry(IList<Vector2> points, Vector3 xaxis, Vector3 p0, Vector3 p1)
    {
        var p10 = p1 - p0;
        var ydirection = Vector3.Cross(xaxis, p10);
        ydirection = Vector3.Normalize(ydirection);
        xaxis = Vector3.Normalize(xaxis);

        var index0 = this.Positions.Count;
        var np = 2 * points.Count;
        foreach (var p in points)
        {
            var v = (xaxis * p.X) + (ydirection * p.Y);
            this.Positions.Add(p0 + v);
            this.Positions.Add(p1 + v);

            v = Vector3.Normalize(v);
            if (this.Normals != null)
            {
                this.Normals.Add(v);
                this.Normals.Add(v);
            }

            if (this.TextureCoordinates != null)
            {
                this.TextureCoordinates.Add(new Vector2(0, 0));
                this.TextureCoordinates.Add(new Vector2(1, 0));
            }

            var i1 = index0 + 1;
            var i2 = (index0 + 2) % np;
            var i3 = ((index0 + 2) % np) + 1;

            this.TriangleIndices.Add(i1);
            this.TriangleIndices.Add(i2);
            this.TriangleIndices.Add(index0);

            this.TriangleIndices.Add(i1);
            this.TriangleIndices.Add(i3);
            this.TriangleIndices.Add(i2);
        }

        ComputeNormals(this.Positions, this.TriangleIndices, out var normals);
        Debug.Assert(normals is Vector3Collection);
        this.Normals = normals as Vector3Collection;
    }

    /// <summary>
    /// Add a Face in positive Z-Direction.
    /// </summary>
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
                new Vector3(0,0,1),
                new Vector3(0,0,1),
                new Vector3(0,0,1),
                new Vector3(0,0,1),
        };
        var i0 = this.Positions.Count;
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

        foreach (var position in positions)
        {
            this.Positions.Add(position);
        }
        if (this.Normals is not null)
        {
            foreach (var normal in normals)
            {
                this.Normals.Add(normal);
            }
        }
        foreach (var index in indices)
        {
            this.TriangleIndices.Add(index);
        }
        if (this.TextureCoordinates is not null)
        {
            foreach (var texCoord in texcoords)
            {
                this.TextureCoordinates.Add(texCoord);
            }
        }
    }

    /// <summary>
    /// Add a Face in negative Z-Direction.
    /// </summary>
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
                -new Vector3(0,0,1),
                -new Vector3(0,0,1),
                -new Vector3(0,0,1),
                -new Vector3(0,0,1),
        };

        var i0 = this.Positions.Count;
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

        foreach (var position in positions)
        {
            this.Positions.Add(position);
        }
        if (this.Normals is not null)
        {
            foreach (var normal in normals)
            {
                this.Normals.Add(normal);
            }
        }
        foreach (var index in indices)
        {
            this.TriangleIndices.Add(index);
        }
        if (this.TextureCoordinates is not null)
        {
            foreach (var texCoord in texcoords)
            {
                this.TextureCoordinates.Add(texCoord);
            }
        }
    }
    /// <summary>
    /// Add a Face in positive X-Direction.
    /// </summary>
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
                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,0),
        };

        var i0 = this.Positions.Count;
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

        foreach (var position in positions)
        {
            this.Positions.Add(position);
        }
        if (this.Normals is not null)
        {
            foreach (var normal in normals)
            {
                this.Normals.Add(normal);
            }
        }
        foreach (var index in indices)
        {
            this.TriangleIndices.Add(index);
        }
        if (this.TextureCoordinates is not null)
        {
            foreach (var texCoord in texcoords)
            {
                this.TextureCoordinates.Add(texCoord);
            }
        }
    }
    /// <summary>
    /// Add a Face in negative X-Direction.
    /// </summary>
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
                -new Vector3(1,0,0),
                -new Vector3(1,0,0),
                -new Vector3(1,0,0),
                -new Vector3(1,0,0),
        };

        var i0 = this.Positions.Count;
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

        foreach (var position in positions)
        {
            this.Positions.Add(position);
        }
        if (this.Normals is not null)
        {
            foreach (var normal in normals)
            {
                this.Normals.Add(normal);
            }
        }
        foreach (var index in indices)
        {
            this.TriangleIndices.Add(index);
        }
        if (this.TextureCoordinates is not null)
        {
            foreach (var texCoord in texcoords)
            {
                this.TextureCoordinates.Add(texCoord);
            }
        }
    }

    /// <summary>
    /// Add a Face in positive Y-Direction.
    /// </summary>
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
                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0),
        };

        var i0 = this.Positions.Count;
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

        foreach (var position in positions)
        {
            this.Positions.Add(position);
        }
        if (this.Normals is not null)
        {
            foreach (var normal in normals)
            {
                this.Normals.Add(normal);
            }
        }
        foreach (var index in indices)
        {
            this.TriangleIndices.Add(index);
        }
        if (this.TextureCoordinates is not null)
        {
            foreach (var texCoord in texcoords)
            {
                this.TextureCoordinates.Add(texCoord);
            }
        }
    }

    /// <summary>
    /// Add a Face in negative Y-Direction.
    /// </summary>
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
                -new Vector3(0,1,0),
                -new Vector3(0,1,0),
                -new Vector3(0,1,0),
                -new Vector3(0,1,0),
        };

        var i0 = this.Positions.Count;
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

        foreach (var position in positions)
        {
            this.Positions.Add(position);
        }
        if (this.Normals is not null)
        {
            foreach (var normal in normals)
            {
                this.Normals.Add(normal);
            }
        }
        foreach (var index in indices)
        {
            this.TriangleIndices.Add(index);
        }
        if (this.TextureCoordinates is not null)
        {
            foreach (var texCoord in texcoords)
            {
                this.TextureCoordinates.Add(texCoord);
            }
        }
    }

    /// <summary>
    /// Adds an extruded surface of the specified line segments.
    /// </summary>
    /// <param name="points">The 2D points describing the line segments to extrude. The number of points must be even.</param>
    /// <param name="axisX">The x-axis.</param>
    /// <param name="p0">The start origin of the extruded surface.</param>
    /// <param name="p1">The end origin of the extruded surface.</param>
    /// <remarks>The y-axis is determined by the cross product between the specified x-axis and the p1-origin vector.</remarks>
    public void AddExtrudedSegments(IList<Vector2> points, Vector3 axisX, Vector3 p0, Vector3 p1)
    {
        if (points.Count % 2 != 0)
        {
            ThrowHelper.ThrowInvalidOperationException("The number of points should be even.");
        }

        var p10 = p1 - p0;
        var axisY = Vector3.Cross(axisX, p10);
        axisY = Vector3.Normalize(axisY);
        axisX = Vector3.Normalize(axisX);
        var index0 = this.Positions.Count;

        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i];
            var d = (axisX * p.X) + (axisY * p.Y);
            this.Positions.Add(p0 + d);
            this.Positions.Add(p1 + d);

            if (this.Normals != null)
            {
                d = Vector3.Normalize(d);
                this.Normals.Add(d);
                this.Normals.Add(d);
            }

            if (this.TextureCoordinates != null)
            {
                var v = (float)i / (points.Count - 1);
                this.TextureCoordinates.Add(new Vector2(0, v));
                this.TextureCoordinates.Add(new Vector2(1, v));
            }
        }

        var n = points.Count - 1;
        for (var i = 0; i < n; i++)
        {
            var i0 = index0 + (i * 2);
            var i1 = i0 + 1;
            var i2 = i0 + 3;
            var i3 = i0 + 2;

            this.TriangleIndices.Add(i0);
            this.TriangleIndices.Add(i1);
            this.TriangleIndices.Add(i2);

            this.TriangleIndices.Add(i2);
            this.TriangleIndices.Add(i3);
            this.TriangleIndices.Add(i0);
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
        IList<IList<Vector3>> positionsList,
        IList<IList<Vector3>> normalList,
        IList<IList<Vector2>> textureCoordinateList)
    {
        var index0 = this.Positions.Count;
        var n = -1;
        for (var i = 0; i < positionsList.Count; i++)
        {
            var pc = positionsList[i];

            // check that all curves have same number of points
            if (n == -1)
            {
                n = pc.Count;
            }

            if (pc.Count != n)
            {
                ThrowHelper.ThrowInvalidOperationException(AllCurvesShouldHaveTheSameNumberOfPoints);
            }

            // add the points
            foreach (var p in pc)
            {
                this.Positions.Add(p);
            }

            // add normals
            if (this.Normals != null && normalList != null)
            {
                var nc = normalList[i];
                foreach (var normal in nc)
                {
                    this.Normals.Add(normal);
                }
            }

            // add texcoords
            if (this.TextureCoordinates != null && textureCoordinateList != null)
            {
                var tc = textureCoordinateList[i];
                foreach (var t in tc)
                {
                    this.TextureCoordinates.Add(t);
                }
            }
        }

        for (var i = 0; i + 1 < positionsList.Count; i++)
        {
            for (var j = 0; j + 1 < n; j++)
            {
                var i0 = index0 + (i * n) + j;
                var i1 = i0 + n;
                var i2 = i1 + 1;
                var i3 = i0 + 1;
                this.TriangleIndices.Add(i0);
                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(i2);

                this.TriangleIndices.Add(i2);
                this.TriangleIndices.Add(i3);
                this.TriangleIndices.Add(i0);
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
        this.Positions.Add(position);
        this.Normals?.Add(normal);
        this.TextureCoordinates?.Add(textureCoordinate);
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
    public void AddOctahedron(Vector3 center, Vector3 forward, Vector3 up, float sideLength, float height)
    {
        var right = Vector3.Cross(forward, up);
        var n = forward * sideLength / 2;
        up *= height / 2;
        right *= sideLength / 2;

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
    public void AddPipe(Vector3 point1, Vector3 point2, float innerDiameter, float diameter, int thetaDiv)
    {
        var dir = point2 - point1;

        var height = dir.Length();
        dir = Vector3.Normalize(dir);

        var pc = new Vector2Collection
                {
                    new Vector2(0, innerDiameter / 2),
                    new Vector2(0, diameter / 2),
                    new Vector2(height, diameter / 2),
                    new Vector2(height, innerDiameter / 2)
                };

        var tc = new List<float> { 1, 0, 1, 0 };

        if (innerDiameter > 0)
        {
            // Add the inner surface
            pc.Add(new Vector2(0, innerDiameter / 2));
            tc.Add(1);
        }

        this.AddRevolvedGeometry(pc, tc, point1, dir, thetaDiv);
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
    public void AddPipes(IList<Vector3> points, IList<int> edges, float diameter = 1, int thetaDiv = 32)
    {
        for (var i = 0; i < edges.Count - 1; i += 2)
        {
            this.AddCylinder(points[edges[i]], points[edges[i + 1]], diameter, thetaDiv);
        }
    }

    /// <summary>
    /// Adds a polygon.
    /// </summary>
    /// <param name="points">The 2D points defining the polygon.</param>
    /// <param name="axisX">The x axis.</param>
    /// <param name="axisY">The y axis.</param>
    /// <param name="origin">The origin.</param>
    public void AddPolygon(IList<Vector2> points, Vector3 axisX, Vector3 axisY, Vector3 origin)
    {
        var indices = SweepLinePolygonTriangulator.Triangulate(points);
        if (indices != null)
        {
            var index0 = this.Positions.Count;
            foreach (var p in points)
            {
                this.Positions.Add(origin + (axisX * p.X) + (axisY * p.Y));
            }

            foreach (var i in indices)
            {
                this.TriangleIndices.Add(index0 + i);
            }
        }
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
    /// Adds a polygon specified by vertex index (uses a triangle fan).
    /// </summary>
    /// <param name="vertexIndices">The vertex indices.</param>
    public void AddPolygon(IList<int> vertexIndices)
    {
        var n = vertexIndices.Count;
        for (var i = 0; i + 2 < n; i++)
        {
            this.TriangleIndices.Add(vertexIndices[0]);
            this.TriangleIndices.Add(vertexIndices[i + 1]);
            this.TriangleIndices.Add(vertexIndices[i + 2]);
        }
    }

    /// <summary>
    /// Adds a polygon defined by vertex indices (uses the cutting ears algorithm).
    /// </summary>
    /// <param name="vertexIndices">The vertex indices.</param>
    [Obsolete("Please use the faster version AddPolygon instead")]
    public void AddPolygonByCuttingEars(IList<int> vertexIndices)
    {
        var points = vertexIndices.Select(vi => this.Positions[vi]).ToList();
        var poly3D = new Polygon3D(points);
        // Transform the polygon to 2D
        var poly2D = poly3D.Flatten();

        // Triangulate
        var triangulatedIndices = CuttingEarsTriangulator.Triangulate(poly2D.Points);
        if (triangulatedIndices != null)
        {
            foreach (var i in triangulatedIndices)
            {
                this.TriangleIndices.Add(vertexIndices[i]);
            }
        }
    }

    /// <summary>
    /// Adds a polygon defined by vertex indices (uses the sweep line algorithm).
    /// </summary>
    /// <param name="vertexIndices">The vertex indices.</param>
    public void AddPolygonByTriangulation(IList<int> vertexIndices)
    {
        var points = vertexIndices.Select(vi => this.Positions[vi]).ToList();
        var poly3D = new Polygon3D(points);
        // Transform the polygon to 2D
        var poly2D = poly3D.Flatten();

        // Triangulate
        var triangulatedIndices = poly2D.Triangulate();
        if (triangulatedIndices != null)
        {
            foreach (var i in triangulatedIndices)
            {
                this.TriangleIndices.Add(vertexIndices[i]);
            }
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
    public void AddPyramid(Vector3 center, float sideLength, float height, bool closeBase = false)
    {
        this.AddPyramid(center, new Vector3(1, 0, 0), new Vector3(0, 0, 1), sideLength, height, closeBase);
    }

    /// <summary>
    /// Adds a pyramid.
    /// </summary>
    /// <param name="center">The center.</param>
    /// <param name="forward">The normal vector (normalized).</param>
    /// <param name="up">The 'up' vector (normalized).</param>
    /// <param name="sideLength">Length of the sides of the pyramid.</param>
    /// <param name="height">The height of the pyramid.</param>
    /// <param name="closeBase">Add triangles to the base of the pyramid or not.</param>
    public void AddPyramid(Vector3 center, Vector3 forward, Vector3 up, float sideLength, float height, bool closeBase = false)
    {
        var right = Vector3.Cross(forward, up);
        var n = forward * sideLength / 2;
        up *= height;
        right *= sideLength / 2;

        var down = -up * 1f / 3;
        var realup = up * 2f / 3;

        var p1 = center - n - right + down;
        var p2 = center - n + right + down;
        var p3 = center + n + right + down;
        var p4 = center + n - right + down;
        var p5 = center + realup;

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
    /// Adds a quad (exactely 4 indices)
    /// </summary>
    /// <param name="vertexIndices">The vertex indices.</param>
    public void AddQuad(IList<int> vertexIndices)
    {
        for (var i = 0; i < 4; i++)
        {
            this.TriangleIndices.Add(vertexIndices[i]);
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
        //// origin               p1
        var uv0 = new Vector2(0, 0);
        var uv1 = new Vector2(1, 0);
        var uv2 = new Vector2(1, 1);
        var uv3 = new Vector2(0, 1);
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
        //// origin               p1
        var i0 = this.Positions.Count;

        this.Positions.Add(p0);
        this.Positions.Add(p1);
        this.Positions.Add(p2);
        this.Positions.Add(p3);

        if (this.TextureCoordinates != null)
        {
            this.TextureCoordinates.Add(uv0);
            this.TextureCoordinates.Add(uv1);
            this.TextureCoordinates.Add(uv2);
            this.TextureCoordinates.Add(uv3);
        }

        if (this.Normals != null)
        {
            var p10 = p1 - p0;
            var p30 = p3 - p0;
            var w = Vector3.Cross(p10, p30);
            w = Vector3.Normalize(w);
            this.Normals.Add(w);
            this.Normals.Add(w);
            this.Normals.Add(w);
            this.Normals.Add(w);
        }

        this.TriangleIndices.Add(i0 + 0);
        this.TriangleIndices.Add(i0 + 1);
        this.TriangleIndices.Add(i0 + 2);

        this.TriangleIndices.Add(i0 + 2);
        this.TriangleIndices.Add(i0 + 3);
        this.TriangleIndices.Add(i0 + 0);
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
        IList<Vector3> quadPositions, IList<Vector3>? quadNormals, IList<Vector2>? quadTextureCoordinates)
    {
        Guard.IsNotNull(quadPositions);

        if (this.Normals != null)
        {
            Guard.IsNotNull(quadNormals);
        }

        if (this.TextureCoordinates != null)
        {
            Guard.IsNotNull(quadTextureCoordinates);
        }

        if (quadNormals != null && quadNormals.Count != quadPositions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfNormals);
        }

        if (quadTextureCoordinates != null && quadTextureCoordinates.Count != quadPositions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        Debug.Assert(quadPositions.Count > 0 && quadPositions.Count % 4 == 0, "Wrong number of positions.");

        var index0 = this.Positions.Count;
        foreach (var p in quadPositions)
        {
            this.Positions.Add(p);
        }

        if (this.TextureCoordinates != null && quadTextureCoordinates != null)
        {
            foreach (var tc in quadTextureCoordinates)
            {
                this.TextureCoordinates.Add(tc);
            }
        }

        if (this.Normals != null && quadNormals != null)
        {
            foreach (var n in quadNormals)
            {
                this.Normals.Add(n);
            }
        }

        var indexEnd = this.Positions.Count;
        for (var i = index0; i + 3 < indexEnd; i++)
        {
            this.TriangleIndices.Add(i);
            this.TriangleIndices.Add(i + 1);
            this.TriangleIndices.Add(i + 2);

            this.TriangleIndices.Add(i + 2);
            this.TriangleIndices.Add(i + 3);
            this.TriangleIndices.Add(i);
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
    public void AddRectangularMesh(IList<Vector3> points, int columns)
    {
        Guard.IsNotNull(points);

        var index0 = this.Positions.Count;

        foreach (var pt in points)
        {
            this.Positions.Add(pt);
        }

        var rows = points.Count / columns;

        this.AddRectangularMeshTriangleIndices(index0, rows, columns);
        if (this.Normals != null)
        {
            this.AddRectangularMeshNormals(index0, rows, columns);
        }

        if (this.TextureCoordinates != null)
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
        Vector3[,] points, Vector2[,]? texCoords = null, bool closed0 = false, bool closed1 = false)
    {
        Guard.IsNotNull(points);

        var rows = points.GetUpperBound(0) + 1;
        var columns = points.GetUpperBound(1) + 1;
        var index0 = this.Positions.Count;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                this.Positions.Add(points[i, j]);
            }
        }

        this.AddRectangularMeshTriangleIndices(index0, rows, columns, closed0, closed1);

        if (this.Normals != null)
        {
            this.AddRectangularMeshNormals(index0, rows, columns);
        }

        if (this.TextureCoordinates != null)
        {
            if (texCoords != null)
            {
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < columns; j++)
                    {
                        this.TextureCoordinates.Add(texCoords[i, j]);
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
    /// Adds a rectangular mesh (m x n points).
    /// </summary>
    /// <param name="points">
    /// The one-dimensional array of points. The points are stored row-by-row.
    /// </param>
    /// <param name="columns">
    /// The number of columns in the rectangular mesh.
    /// </param>
    /// <param name="flipTriangles">
    /// Flip the Triangles.
    /// </param>
    public void AddRectangularMesh(IList<Vector3> points, int columns, bool flipTriangles = false)
    {
        Guard.IsNotNull(points);

        var index0 = this.Positions.Count;

        foreach (var pt in points)
        {
            this.Positions.Add(pt);
        }

        var rows = points.Count / columns;

        if (flipTriangles)
        {
            this.AddRectangularMeshTriangleIndicesFlipped(index0, rows, columns);
        }
        else
        {
            this.AddRectangularMeshTriangleIndices(index0, rows, columns);
        }

        if (this.Normals != null)
        {
            this.AddRectangularMeshNormals(index0, rows, columns);
        }

        if (this.TextureCoordinates != null)
        {
            this.AddRectangularMeshTextureCoordinates(rows, columns);
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
            ThrowHelper.ThrowInvalidOperationException("columns or rows too small");
        }

        if (width <= 0 || height <= 0)
        {
            ThrowHelper.ThrowInvalidOperationException("width or height too small");
        }

        // index0
        var index0 = this.Positions.Count;

        // positions
        var stepy = height / (rows - 1);
        var stepx = width / (columns - 1);
        //rows++;
        //columns++;
        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < columns; x++)
            {
                this.Positions.Add(new Vector3(x * stepx, y * stepy, 0));
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
        if (this.Normals != null)
        {
            this.AddRectangularMeshNormals(index0, rows, columns);
        }

        // texcoords
        if (this.TextureCoordinates != null)
        {
            this.AddRectangularMeshTextureCoordinates(rows, columns, flipTexCoordsVAxis, flipTexCoordsUAxis);
        }
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
        if (this.Normals is null)
        {
            return;
        }

        for (var i = 0; i < rows; i++)
        {
            var i1 = i + 1;
            if (i1 == rows)
            {
                i1--;
            }

            var i0 = i1 - 1;
            for (var j = 0; j < columns; j++)
            {
                var j1 = j + 1;
                if (j1 == columns)
                {
                    j1--;
                }

                var j0 = j1 - 1;
                var u = Vector3.Subtract(
                    this.Positions[index0 + (i1 * columns) + j0], this.Positions[index0 + (i0 * columns) + j0]);
                var v = Vector3.Subtract(
                    this.Positions[index0 + (i0 * columns) + j1], this.Positions[index0 + (i0 * columns) + j0]);
                var normal = Vector3.Cross(u, v);
                normal = Vector3.Normalize(normal);
                this.Normals.Add(normal);
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
    /// <param name="flipRowsAxis">
    /// Flip the Rows.
    /// </param>
    /// <param name="flipColumnsAxis">
    /// Flip the Columns.
    /// </param>
    private void AddRectangularMeshTextureCoordinates(int rows, int columns, bool flipRowsAxis = false, bool flipColumnsAxis = false)
    {
        if (this.TextureCoordinates is null)
        {
            return;
        }

        for (var i = 0; i < rows; i++)
        {
            var v = flipRowsAxis ? (1 - (float)i / (rows - 1)) : (float)i / (rows - 1);

            for (var j = 0; j < columns; j++)
            {
                var u = flipColumnsAxis ? (1 - (float)j / (columns - 1)) : (float)j / (columns - 1);
                this.TextureCoordinates.Add(new Vector2(u, v));
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
    public void AddRectangularMeshTriangleIndices(int index0, int rows, int columns, bool isSpherical = false)
    {
        for (var i = 0; i < rows - 1; i++)
        {
            for (var j = 0; j < columns - 1; j++)
            {
                var ij = (i * columns) + j;
                if (!isSpherical || i > 0)
                {
                    this.TriangleIndices.Add(index0 + ij);
                    this.TriangleIndices.Add(index0 + ij + 1 + columns);
                    this.TriangleIndices.Add(index0 + ij + 1);
                }

                if (!isSpherical || i < rows - 2)
                {
                    this.TriangleIndices.Add(index0 + ij + 1 + columns);
                    this.TriangleIndices.Add(index0 + ij);
                    this.TriangleIndices.Add(index0 + ij + columns);
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
    public void AddRectangularMeshTriangleIndices(
        int index0, int rows, int columns, bool rowsClosed, bool columnsClosed)
    {
        var m2 = rows - 1;
        var n2 = columns - 1;
        if (columnsClosed)
        {
            m2++;
        }

        if (rowsClosed)
        {
            n2++;
        }

        for (var i = 0; i < m2; i++)
        {
            for (var j = 0; j < n2; j++)
            {
                var i00 = index0 + (i * columns) + j;
                var i01 = index0 + (i * columns) + ((j + 1) % columns);
                var i10 = index0 + (((i + 1) % rows) * columns) + j;
                var i11 = index0 + (((i + 1) % rows) * columns) + ((j + 1) % columns);
                this.TriangleIndices.Add(i00);
                this.TriangleIndices.Add(i11);
                this.TriangleIndices.Add(i01);

                this.TriangleIndices.Add(i11);
                this.TriangleIndices.Add(i00);
                this.TriangleIndices.Add(i10);
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
        for (var i = 0; i < rows - 1; i++)
        {
            for (var j = 0; j < columns - 1; j++)
            {
                var ij = (i * columns) + j;
                if (!isSpherical || i > 0)
                {
                    this.TriangleIndices.Add(index0 + ij);
                    this.TriangleIndices.Add(index0 + ij + 1);
                    this.TriangleIndices.Add(index0 + ij + 1 + columns);
                }

                if (!isSpherical || i < rows - 2)
                {
                    this.TriangleIndices.Add(index0 + ij + 1 + columns);
                    this.TriangleIndices.Add(index0 + ij + columns);
                    this.TriangleIndices.Add(index0 + ij);
                }
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
    public void AddRegularIcosahedron(Vector3 center, float radius, bool shareVertices)
    {
        var a = (float)Math.Sqrt(2.0 / (5.0 + Math.Sqrt(5.0)));
        var b = (float)Math.Sqrt(2.0 / (5.0 - Math.Sqrt(5.0)));

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
            var index0 = this.Positions.Count;
            foreach (var v in icosahedronVertices)
            {
                this.Positions.Add(center + (v * radius));
            }

            foreach (var i in icosahedronIndices)
            {
                this.TriangleIndices.Add(index0 + i);
            }
        }
        else
        {
            for (var i = 0; i + 2 < icosahedronIndices.Length; i += 3)
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
    /// <param name="points">The points (x coordinates are distance from the origin along the axis of revolution, y coordinates are radius, )</param>
    /// <param name="textureValues">The v texture coordinates, one for each point in the <paramref name="points" /> list.</param>
    /// <param name="origin">The origin of the revolution axis.</param>
    /// <param name="direction">The direction of the revolution axis.</param>
    /// <param name="thetaDiv">The number of divisions around the mesh.</param>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Surface_of_revolution.
    /// </remarks>
    public void AddRevolvedGeometry(IList<Vector2> points, IList<float>? textureValues, Vector3 origin, Vector3 direction, int thetaDiv)
    {
        direction = Vector3.Normalize(direction);

        // Find two unit vectors orthogonal to the specified direction
        var u = direction.FindAnyPerpendicular();
        var v = Vector3.Cross(direction, u);
        u = Vector3.Normalize(u);
        v = Vector3.Normalize(v);

        var circle = GetCircle(thetaDiv);

        var index0 = this.Positions.Count;
        var n = points.Count;

        var totalNodes = (points.Count - 1) * 2 * thetaDiv;
        var rowNodes = (points.Count - 1) * 2;

        for (var i = 0; i < thetaDiv; i++)
        {
            var w = (v * circle[i].X) + (u * circle[i].Y);

            for (var j = 0; j + 1 < n; j++)
            {
                // Add segment
                var q1 = origin + (direction * points[j].X) + (w * points[j].Y);
                var q2 = origin + (direction * points[j + 1].X) + (w * points[j + 1].Y);

                // TODO: should not add segment if q1==q2 (corner point)
                // const double eps = 1e-6;
                // if (Point3D.Subtract(q1, q2).LengthSquared < eps)
                // continue;
                this.Positions.Add(q1);
                this.Positions.Add(q2);

                if (this.Normals != null)
                {
                    var tx = points[j + 1].X - points[j].X;
                    var ty = points[j + 1].Y - points[j].Y;
                    var normal = Vector3.Normalize((-direction * ty) + (w * tx));
                    this.Normals.Add(normal);
                    this.Normals.Add(normal);
                }

                if (this.TextureCoordinates != null)
                {
                    this.TextureCoordinates.Add(new Vector2((float)i / (thetaDiv - 1), textureValues == null ? (float)j / (n - 1) : textureValues[j]));
                    this.TextureCoordinates.Add(new Vector2((float)i / (thetaDiv - 1), textureValues == null ? (float)(j + 1) / (n - 1) : textureValues[j + 1]));
                }

                var i0 = index0 + (i * rowNodes) + (j * 2);
                var i1 = i0 + 1;
                var i2 = index0 + ((((i + 1) * rowNodes) + (j * 2)) % totalNodes);
                var i3 = i2 + 1;

                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(i0);
                this.TriangleIndices.Add(i2);

                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(i2);
                this.TriangleIndices.Add(i3);
            }
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
    public void AddSphere(Vector3 center, float radius = 1, int thetaDiv = 32, int phiDiv = 32)
    {
        this.AddEllipsoid(center, radius, radius, radius, thetaDiv, phiDiv);
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
    public void AddSubdivisionSphere(Vector3 center, float radius, int subdivisions)
    {
        var p0 = this.Positions.Count;
        this.Append(GetUnitSphere(subdivisions));
        var p1 = this.Positions.Count;
        for (var i = p0; i < p1; i++)
        {
            var pVec = this.Positions[i];
            this.Positions[i] = center + (radius * pVec);
        }
    }

    /// <summary>
    /// Adds a surface of revolution.
    /// </summary>
    /// <param name="origin">The origin.</param>
    /// <param name="axis">The axis.</param>
    /// <param name="section">The points defining the curve to revolve.</param>
    /// <param name="sectionIndices">The indices of the line segments of the section.</param>
    /// <param name="thetaDiv">The number of divisions.</param>
    /// <param name="textureValues">The texture values.</param>
    public void AddSurfaceOfRevolution(
        Vector3 origin, Vector3 axis, IList<Vector2> section, IList<int> sectionIndices,
        int thetaDiv = 36, IList<float>? textureValues = null)
    {
        if (this.TextureCoordinates != null)
        {
            Guard.IsNotNull(textureValues);
        }

        if (textureValues != null && textureValues.Count != section.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        axis = Vector3.Normalize(axis);

        // Find two unit vectors orthogonal to the specified direction
        var u = axis.FindAnyPerpendicular();
        var v = Vector3.Cross(axis, u);
        var circle = GetCircle(thetaDiv);
        var n = section.Count;
        var index0 = this.Positions.Count;
        for (var i = 0; i < thetaDiv; i++)
        {
            var w = (v * circle[i].X) + (u * circle[i].Y);
            for (var j = 0; j < n; j++)
            {
                var q1 = origin + (axis * section[j].Y) + (w * section[j].X);
                this.Positions.Add(q1);
                if (this.Normals != null)
                {
                    var tx = section[j + 1].X - section[j].X;
                    var ty = section[j + 1].Y - section[j].Y;
                    var normal = (-axis * ty) + (w * tx);
                    normal = Vector3.Normalize(normal);
                    this.Normals.Add(normal);
                }

                this.TextureCoordinates?.Add(new Vector2((float)i / (thetaDiv - 1), textureValues == null ? (float)j / (n - 1) : textureValues[j]));
            }
        }
        for (var i = 0; i < thetaDiv; i++)
        {
            var ii = (i + 1) % thetaDiv;
            for (var j = 0; j + 1 < sectionIndices.Count; j += 2)
            {
                var j0 = sectionIndices[j];
                var j1 = sectionIndices[j + 1];

                var i0 = index0 + (i * n) + j0;
                var i1 = index0 + (ii * n) + j0;
                var i2 = index0 + (i * n) + j1;
                var i3 = index0 + (ii * n) + j1;

                this.TriangleIndices.Add(i0);
                this.TriangleIndices.Add(i1);
                this.TriangleIndices.Add(i3);

                this.TriangleIndices.Add(i3);
                this.TriangleIndices.Add(i2);
                this.TriangleIndices.Add(i0);
            }
        }
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
    public void AddTetrahedron(Vector3 center, Vector3 forward, Vector3 up, float sideLength)
    {
        // Helper Variables
        var right = Vector3.Cross(up, forward);
        var heightSphere = (float)Math.Sqrt(6) / 3 * sideLength;
        var radiusSphere = (float)Math.Sqrt(6) / 4 * sideLength;
        var heightFace = (float)Math.Sqrt(3) / 2 * sideLength;
        var radiusFace = (float)Math.Sqrt(3) / 3 * sideLength;
        var smallHeightSphere = heightSphere - radiusSphere;
        var smallHeightFace = heightFace - radiusFace;
        var halfLength = sideLength * 0.5f;

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
    /// Adds a torus.
    /// </summary>
    /// <param name="torusDiameter">The diameter of the torus.</param>
    /// <param name="tubeDiameter">The diameter of the torus "tube".</param>
    /// <param name="thetaDiv">The number of subdivisions around the torus.</param>
    /// <param name="phiDiv">The number of subdividions of the torus' "tube.</param>
    public void AddTorus(float torusDiameter, float tubeDiameter, int thetaDiv = 36, int phiDiv = 24)
    {
        var positionsCount = this.Positions.Count;
        // No Torus Diameter means we treat the Visual3D like a Sphere
        if (torusDiameter == 0.0)
        {
            this.AddSphere(new Vector3(), tubeDiameter, thetaDiv, phiDiv);
        }
        // If the second Diameter is zero, we can't build out torus
        else if (tubeDiameter == 0.0)
        {
            HelixToolkitException.Throw("Torus must have a Diameter bigger than 0");
        }
        // Values result in a Torus
        else
        {
            // Points of the Cross-Section of the torus "tube"
            IList<Vector2> crossSectionPoints;
            // Self-intersecting Torus, if the "Tube" Diameter is bigger than the Torus Diameter
            var selfIntersecting = tubeDiameter > torusDiameter;
            if (selfIntersecting)
            {
                // Angle-Calculations for Circle Segment https://de.wikipedia.org/wiki/Gleichschenkliges_Dreieck
                var angleIcoTriangle = (float)Math.Acos(1 - ((torusDiameter * torusDiameter) / (2 * (tubeDiameter * tubeDiameter * .25))));
                var circleAngle = (float)Math.PI + angleIcoTriangle;
                var offset = -circleAngle / 2;
                // The Cross-Section is defined by only a Segment of a Circle
                crossSectionPoints = GetCircleSegment(phiDiv, circleAngle, offset);
            }
            // "normal" Torus (with a Circle as Cross-Section of the Torus
            else
            {
                crossSectionPoints = GetCircle(phiDiv, false);
            }
            // Transform Crosssection to real Size
            crossSectionPoints = crossSectionPoints.Select(p => new Vector2(p.X * tubeDiameter * .5f, p.Y * tubeDiameter * .5f)).ToList();
            // Transform the Cross-Section Points to 3D Space
            var crossSection3DPoints = crossSectionPoints.Select(p => new Vector3(p.X, 0, p.Y)).ToList();

            // Add the needed Vertex-Positions of the Torus
            for (var i = 0; i < thetaDiv; i++)
            {
                // Angle of the current Cross-Section in the XY-Plane
                var angle = (float)Math.PI * 2 * ((float)i / thetaDiv);
                // Rotate the Cross-Section around the Origin by using the angle and the defined torusDiameter
                var rotatedPoints = crossSection3DPoints.Select(p3D => new Vector3((float)Math.Cos(angle) * (p3D.X + torusDiameter * .5f), (float)Math.Sin(angle) * (p3D.X + torusDiameter * .5f), p3D.Z)).ToList();
                for (var j = 0; j < phiDiv; j++)
                {
                    // If selfintersecting Torus, skip the first and last Point of the Cross-Sections, when not the first Cross Section.
                    // We only need the first and last Point of the first Cross-Section once!
                    if (selfIntersecting && i > 0 && (j == 0 || j == (phiDiv - 1)))
                        continue;
                    // Add the Position
                    this.Positions.Add(rotatedPoints[j]);
                }
            }
            // Add all Normals, if they need to be calculated
            if (this.Normals != null)
            {
                for (var i = 0; i < thetaDiv; i++)
                {
                    // Transform the Cross-Section as well as the Origin of the Cross-Section
                    var angle = (float)Math.PI * 2 * ((float)i / thetaDiv);
                    var rotatedPoints = crossSection3DPoints.Select(p3D => new Vector3((float)Math.Cos(angle) * (p3D.X + torusDiameter * .5f), (float)Math.Sin(angle) * (p3D.X + torusDiameter * .5f), p3D.Z)).ToList();
                    // We don't need the first and last Point of the rotated Points, if we are not in the first Cross-Section
                    if (selfIntersecting && i > 0)
                    {
                        rotatedPoints.RemoveAt(0);
                        rotatedPoints.RemoveAt(rotatedPoints.Count - 1);
                    }
                    // Transform the Center of the Cross-Section
                    var rotatedOrigin = new Vector3((float)Math.Cos(angle) * torusDiameter * .5f, (float)Math.Sin(angle) * torusDiameter * .5f, 0);
                    // Add the Normal of the Vertex
                    for (var j = 0; j < rotatedPoints.Count; j++)
                    {
                        // The default Normal has the same Direction as the Vector from the Center to the Vertex
                        var normal = rotatedPoints[j] - rotatedOrigin;
                        normal = Vector3.Normalize(normal);
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
                        this.Normals.Add(normal);
                    }
                }
            }
            // Add all Texture Coordinates, if they need to be calculated
            if (this.TextureCoordinates != null)
            {
                // For all Points, calculate a simple uv Coordinate
                for (var i = 0; i < thetaDiv; i++)
                {
                    // Determine the Number of Vertices of this Cross-Section present in the positions Collection
                    var numCS = (selfIntersecting && i > 0) ? phiDiv - 2 : phiDiv;
                    for (var j = 0; j < numCS; j++)
                    {
                        // Calculate u- and v- Coordinates for the Points
                        var u = (float)i / thetaDiv;
                        float v = 0;
                        if (i > 0 && selfIntersecting)
                            v = (float)(j + 1) / phiDiv;
                        else
                            v = (float)j / phiDiv;
                        // Add the Texture-Coordinate
                        this.TextureCoordinates.Add(new Vector2(u, v));
                    }
                }
            }
            // Add Triangle-Indices
            for (var i = 0; i < thetaDiv; i++)
            {
                // Normal non-selfintersecting Torus
                // Just add Triangle-Strips between all neighboring Cross-Sections
                if (!selfIntersecting)
                {
                    var firstPointIdx = i * phiDiv;
                    var firstPointIdxNextCircle = ((i + 1) % thetaDiv) * phiDiv;
                    for (var j = 0; j < phiDiv; j++)
                    {
                        var jNext = (j + 1) % phiDiv;
                        this.TriangleIndices.Add(firstPointIdx + j + positionsCount);
                        this.TriangleIndices.Add(firstPointIdx + jNext + positionsCount);
                        this.TriangleIndices.Add(firstPointIdxNextCircle + j + positionsCount);

                        this.TriangleIndices.Add(firstPointIdxNextCircle + j + positionsCount);
                        this.TriangleIndices.Add(firstPointIdx + jNext + positionsCount);
                        this.TriangleIndices.Add(firstPointIdxNextCircle + jNext + positionsCount);
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
                    if (firstPointIdxNextCircle >= this.Positions.Count)
                    {
                        firstPointIdxNextCircle %= this.Positions.Count;
                        firstPointIdxNextCircle++;
                    }
                    // Add Triangles between the "middle" Parts of the neighboring Cross-Sections
                    for (var j = 1; j < phiDiv - 2; j++)
                    {
                        this.TriangleIndices.Add(firstPointIdx + j - 1 + positionsCount);
                        this.TriangleIndices.Add(firstPointIdxNextCircle + j - 1 + positionsCount);
                        this.TriangleIndices.Add(firstPointIdx + j + positionsCount);

                        this.TriangleIndices.Add(firstPointIdxNextCircle + j - 1 + positionsCount);
                        this.TriangleIndices.Add(firstPointIdxNextCircle + j + positionsCount);
                        this.TriangleIndices.Add(firstPointIdx + j + positionsCount);
                    }
                }
            }
            // For selfintersecting Tori
            if (selfIntersecting)
            {
                // Add bottom Cap by creating a List of Vertex-Indices
                // and using them to create a Triangle-Fan
                var verts = new FastList<int>
                {
                    0
                };
                for (var i = 0; i < thetaDiv; i++)
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
                verts = new FastList<int>
                {
                    phiDiv - 1 + positionsCount
                };
                for (var i = 0; i < thetaDiv; i++)
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

    /// <summary>
    /// Adds a triangle (exactely 3 indices)
    /// </summary>
    /// <param name="vertexIndices">The vertex indices.</param>
    public void AddTriangle(IList<int> vertexIndices)
    {
        for (var i = 0; i < 3; i++)
        {
            this.TriangleIndices.Add(vertexIndices[i]);
        }
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
        var i0 = this.Positions.Count;

        this.Positions.Add(p0);
        this.Positions.Add(p1);
        this.Positions.Add(p2);

        if (this.TextureCoordinates != null)
        {
            this.TextureCoordinates.Add(uv0);
            this.TextureCoordinates.Add(uv1);
            this.TextureCoordinates.Add(uv2);
        }

        if (this.Normals != null)
        {
            var p10 = p1 - p0;
            var p20 = p2 - p0;
            var w = Vector3.Cross(p10, p20);
            w = Vector3.Normalize(w);
            this.Normals.Add(w);
            this.Normals.Add(w);
            this.Normals.Add(w);
        }

        this.TriangleIndices.Add(i0 + 0);
        this.TriangleIndices.Add(i0 + 1);
        this.TriangleIndices.Add(i0 + 2);
    }

    /// <summary>
    /// Adds a triangle fan.
    /// </summary>
    /// <param name="vertices">
    /// The vertex indices of the triangle fan.
    /// </param>
    public void AddTriangleFan(IList<int> vertices)
    {
        for (var i = 0; i + 2 < vertices.Count; i++)
        {
            this.TriangleIndices.Add(vertices[0]);
            this.TriangleIndices.Add(vertices[i + 1]);
            this.TriangleIndices.Add(vertices[i + 2]);
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
    public void AddTriangleFan(IList<Vector3> fanPositions, IList<Vector3>? fanNormals = null, IList<Vector2>? fanTextureCoordinates = null)
    {
        Guard.IsNotNull(fanPositions);

        if (this.Normals != null)
        {
            Guard.IsNotNull(fanNormals);
        }

        if (this.TextureCoordinates != null)
        {
            Guard.IsNotNull(fanTextureCoordinates);
        }

        if (fanPositions.Count < 3)
            return;

        var index0 = this.Positions.Count;
        foreach (var p in fanPositions)
        {
            this.Positions.Add(p);
        }

        if (this.TextureCoordinates != null && fanTextureCoordinates != null)
        {
            foreach (var tc in fanTextureCoordinates)
            {
                this.TextureCoordinates.Add(tc);
            }
        }

        if (this.Normals != null && fanNormals != null)
        {
            foreach (var n in fanNormals)
            {
                this.Normals.Add(n);
            }
        }

        var indexEnd = this.Positions.Count;
        for (var i = index0; i + 2 < indexEnd; i++)
        {
            this.TriangleIndices.Add(index0);
            this.TriangleIndices.Add(i + 1);
            this.TriangleIndices.Add(i + 2);
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
    public void AddTriangles(IList<Vector3> trianglePositions, IList<Vector3>? triangleNormals = null, IList<Vector2>? triangleTextureCoordinates = null)
    {
        Guard.IsNotNull(trianglePositions);

        if (this.Normals != null)
        {
            Guard.IsNotNull(triangleNormals);
        }

        if (this.TextureCoordinates != null)
        {
            Guard.IsNotNull(triangleTextureCoordinates);
        }

        if (trianglePositions.Count % 3 != 0)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfPositions);
        }

        if (triangleNormals != null && triangleNormals.Count != trianglePositions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfNormals);
        }

        if (triangleTextureCoordinates != null && triangleTextureCoordinates.Count != trianglePositions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        var index0 = this.Positions.Count;
        foreach (var p in trianglePositions)
        {
            this.Positions.Add(p);
        }

        if (this.TextureCoordinates != null && triangleTextureCoordinates != null)
        {
            foreach (var tc in triangleTextureCoordinates)
            {
                this.TextureCoordinates.Add(tc);
            }
        }

        if (this.Normals != null && triangleNormals != null)
        {
            foreach (var n in triangleNormals)
            {
                this.Normals.Add(n);
            }
        }

        var indexEnd = this.Positions.Count;
        for (var i = index0; i < indexEnd; i++)
        {
            this.TriangleIndices.Add(i);
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
    public void AddTriangleStrip(IList<Vector3> stripPositions, IList<Vector3>? stripNormals = null, IList<Vector2>? stripTextureCoordinates = null)
    {
        Guard.IsNotNull(stripPositions);

        if (this.Normals != null)
        {
            Guard.IsNotNull(stripNormals);
        }

        if (this.TextureCoordinates != null)
        {
            Guard.IsNotNull(stripTextureCoordinates);
        }

        if (stripNormals != null && stripNormals.Count != stripPositions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfNormals);
        }

        if (stripTextureCoordinates != null && stripTextureCoordinates.Count != stripPositions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        var index0 = this.Positions.Count;
        for (var i = 0; i < stripPositions.Count; i++)
        {
            this.Positions.Add(stripPositions[i]);
            if (this.Normals != null && stripNormals != null)
            {
                this.Normals.Add(stripNormals[i]);
            }

            if (this.TextureCoordinates != null && stripTextureCoordinates != null)
            {
                this.TextureCoordinates.Add(stripTextureCoordinates[i]);
            }
        }

        var indexEnd = this.Positions.Count;
        for (var i = index0; i + 2 < indexEnd; i += 2)
        {
            this.TriangleIndices.Add(i);
            this.TriangleIndices.Add(i + 1);
            this.TriangleIndices.Add(i + 2);

            if (i + 3 < indexEnd)
            {
                this.TriangleIndices.Add(i + 1);
                this.TriangleIndices.Add(i + 3);
                this.TriangleIndices.Add(i + 2);
            }
        }
    }

    /// <summary>
    /// Adds a tube.
    /// </summary>
    /// <param name="path">
    /// A list of points defining the centers of the tube.
    /// </param>
    /// <param name="values">
    /// The texture coordinate X-values.
    /// </param>
    /// <param name="diameters">
    /// The diameters.
    /// </param>
    /// <param name="thetaDiv">
    /// The number of divisions around the tube.
    /// </param>
    /// <param name="isTubeClosed">
    /// Set to true if the tube path is closed.
    /// </param>
    /// <param name="frontCap">
    /// Create a front Cap or not.
    /// </param>
    /// <param name="backCap">
    /// Create a back Cap or not.
    /// </param>
    public void AddTube(IList<Vector3> path, float[]? values, float[]? diameters, int thetaDiv, bool isTubeClosed, bool frontCap = false, bool backCap = false)
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
    /// Generate front Cap.
    /// </param>
    /// <param name="backCap">
    /// Generate back Cap.
    /// </param>
    public void AddTube(IList<Vector3> path, float diameter, int thetaDiv, bool isTubeClosed, bool frontCap = false, bool backCap = false)
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
    /// Create a front Cap or not.
    /// </param>
    /// <param name="backCap">
    /// Create a back Cap or not.
    /// </param>
    public void AddTube(
        IList<Vector3> path, IList<float>? values, IList<float>? diameters,
        IList<Vector2> section, bool isTubeClosed, bool isSectionClosed, bool frontCap = false, bool backCap = false)
    {
        if (values != null && values.Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        if (diameters != null && diameters.Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfDiameters);
        }

        var index0 = this.Positions.Count;
        var pathLength = path.Count;
        var sectionLength = section.Count;
        if (pathLength < 2 || sectionLength < 2)
        {
            return;
        }

        var up = (path[1] - path[0]).FindAnyPerpendicular();

        var diametersCount = diameters != null ? diameters.Count : 0;
        var valuesCount = values != null ? values.Count : 0;

        //*******************************
        //*** PROPOSED SOLUTION *********
        var lastUp = new Vector3();
        var lastForward = new Vector3();
        //*** PROPOSED SOLUTION *********
        //*******************************

        for (var i = 0; i < pathLength; i++)
        {
            var r = diameters != null ? diameters[i % diametersCount] / 2 : 1;
            var i0 = i > 0 ? i - 1 : i;
            var i1 = i + 1 < pathLength ? i + 1 : i;
            var forward = path[i1] - path[i0];
            var right = Vector3.Cross(up, forward);

            up = Vector3.Cross(forward, right);
            up = Vector3.Normalize(up);
            right = Vector3.Normalize(right);
            var u = right;
            var v = up;

            //*******************************
            //*** PROPOSED SOLUTION *********
            // ** I think this will work because if path[n-1] is same point, 
            // ** it is always a reflection of the current move
            // ** so reversing the last move vector should work?
            //*******************************
            if (u.AnyUndefined() || v.AnyUndefined())
            {
                forward = lastForward;
                forward *= -1;
                up = lastUp;
                //** Please verify that negation of "up" is correct here
                up *= -1;
                right = Vector3.Cross(up, forward);
                up = Vector3.Normalize(up);
                right = Vector3.Normalize(right);
                u = right;
                v = up;
            }
            lastForward = forward;
            lastUp = up;

            //*** PROPOSED SOLUTION *********
            //*******************************
            for (var j = 0; j < sectionLength; j++)
            {
                var w = (section[j].X * u * r) + (section[j].Y * v * r);
                var q = path[i] + w;
                this.Positions.Add(q);
                if (this.Normals != null)
                {
                    w = Vector3.Normalize(w);
                    this.Normals.Add(w);
                }

                this.TextureCoordinates?.Add(
                        values != null
                            ? new Vector2(values[i % valuesCount], (float)j / (sectionLength - 1))
                            : new Vector2());
            }
        }

        this.AddRectangularMeshTriangleIndices(index0, pathLength, sectionLength, isSectionClosed, isTubeClosed);

        if (frontCap || backCap)
        {
            var normals = new Vector3[section.Count];
            var fanTextures = new Vector2[section.Count];
            var count = path.Count;
            if (backCap)
            {
                var circleBack = Positions.Skip(Positions.Count - section.Count).Take(section.Count).Reverse().ToArray();
                var normal = path[count - 1] - path[count - 2];
                normal = Vector3.Normalize(normal);
                for (var i = 0; i < normals.Length; ++i)
                {
                    normals[i] = normal;
                }
                this.AddTriangleFan(circleBack, normals, fanTextures);
            }
            if (frontCap)
            {
                var circleFront = Positions.Take(section.Count).ToArray();
                var normal = path[0] - path[1];
                normal = Vector3.Normalize(normal);

                for (var i = 0; i < normals.Length; ++i)
                {
                    normals[i] = normal;
                }
                this.AddTriangleFan(circleFront, normals, fanTextures);
            }
        }
    }

    /// <summary>
    /// Adds a tube with a custom section.
    /// </summary>
    /// <param name="path">A list of points defining the centers of the tube.</param>
    /// <param name="angles">The rotation of the section as it moves along the path</param>
    /// <param name="values">The texture coordinate X values (optional).</param>
    /// <param name="diameters">The diameters (optional).</param>
    /// <param name="section">The section to extrude along the tube path.</param>
    /// <param name="sectionXAxis">The initial alignment of the x-axis of the section into the
    /// 3D viewport</param>
    /// <param name="isTubeClosed">If the tube is closed set to <c>true</c> .</param>
    /// <param name="isSectionClosed">if set to <c>true</c> [is section closed].</param>
    /// <param name="frontCap">
    /// Create a front Cap or not.
    /// </param>
    /// <param name="backCap">
    /// Create a back Cap or not.
    /// </param>
    public void AddTube(
        IList<Vector3> path, IList<float>? angles, IList<float>? values, IList<float>? diameters,
        IList<Vector2>? section, Vector3 sectionXAxis, bool isTubeClosed, bool isSectionClosed, bool frontCap = false, bool backCap = false)
    {
        if (values != null && values.Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        if (diameters != null && diameters.Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfDiameters);
        }

        if (angles != null && angles.Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfAngles);
        }

        if (section is null)
        {
            return;
        }

        var index0 = this.Positions.Count;
        var pathLength = path.Count;
        var sectionLength = section.Count;
        if (pathLength < 2 || sectionLength < 2)
        {
            return;
        }

        var forward = path[1] - path[0];
        var right = sectionXAxis;
        var up = Vector3.Cross(forward, right);
        up = Vector3.Normalize(up);
        right = Vector3.Normalize(right);

        var diametersCount = diameters != null ? diameters.Count : 0;
        var valuesCount = values != null ? values.Count : 0;
        var anglesCount = angles != null ? angles.Count : 0;

        for (var i = 0; i < pathLength; i++)
        {
            var radius = diameters != null ? diameters[i % diametersCount] / 2 : 1;
            var theta = angles != null ? angles[i % anglesCount] : 0.0;

            var ct = (float)Math.Cos(theta);
            var st = (float)Math.Sin(theta);

            var i0 = i > 0 ? i - 1 : i;
            var i1 = i + 1 < pathLength ? i + 1 : i;

            forward = path[i1] - path[i0];
            right = Vector3.Cross(up, forward);
            if (right.LengthSquared() > 1e-6f)
            {
                up = Vector3.Cross(forward, right);
            }

            up = Vector3.Normalize(up);
            right = Vector3.Normalize(right);
            for (var j = 0; j < sectionLength; j++)
            {
                var x = (section[j].X * ct) - (section[j].Y * st);
                var y = (section[j].X * st) + (section[j].Y * ct);

                var w = (x * right * radius) + (y * up * radius);
                var q = path[i] + w;
                this.Positions.Add(q);
                if (this.Normals != null)
                {
                    w = Vector3.Normalize(w);
                    this.Normals.Add(w);
                }

                this.TextureCoordinates?.Add(
                        values != null
                            ? new Vector2(values[i % valuesCount], (float)j / (sectionLength - 1))
                            : new Vector2());
            }
        }

        this.AddRectangularMeshTriangleIndices(index0, pathLength, sectionLength, isSectionClosed, isTubeClosed);
        if (frontCap || backCap && path.Count > 1)
        {
            var normals = new Vector3[section.Count];
            var fanTextures = new Vector2[section.Count];
            var count = path.Count;
            if (backCap)
            {
                var circleBack = Positions.Skip(Positions.Count - section.Count).Take(section.Count).Reverse().ToArray();
                var normal = path[count - 1] - path[count - 2];
                normal = Vector3.Normalize(normal);
                for (var i = 0; i < normals.Length; ++i)
                {
                    normals[i] = normal;
                }
                this.AddTriangleFan(circleBack, normals, fanTextures);
            }
            if (frontCap)
            {
                var circleFront = Positions.Take(section.Count).ToArray();
                var normal = path[0] - path[1];
                normal = Vector3.Normalize(normal);

                for (var i = 0; i < normals.Length; ++i)
                {
                    normals[i] = normal;
                }
                this.AddTriangleFan(circleFront, normals, fanTextures);
            }
        }
    }
    #endregion Add Geometry


    #region Helper Functions
    /// <summary>
    /// Appends the specified mesh.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public void Append(MeshBuilder? mesh)
    {
        Guard.IsNotNull(mesh);

        this.Append(mesh.Positions, mesh.TriangleIndices, mesh.Normals, mesh.TextureCoordinates);
    }

    /// <summary>
    /// Appends the specified mesh.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public void Append(MeshGeometry3D? mesh)
    {
        Guard.IsNotNull(mesh);

        this.Append(mesh.Positions, mesh.TriangleIndices, this.Normals is not null ? mesh.Normals : null, this.TextureCoordinates is not null ? mesh.TextureCoordinates : null);
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
        IList<Vector3> positionsToAppend, IList<int> triangleIndicesToAppend,
        IList<Vector3>? normalsToAppend = null, IList<Vector2>? textureCoordinatesToAppend = null)
    {
        Guard.IsNotNull(positionsToAppend);

        //if (this.Normals is not null && normalsToAppend is null)
        //{
        //    ThrowHelper.ThrowInvalidOperationException(SourceMeshNormalsShouldNotBeNull);
        //}

        //if (this.TextureCoordinates is not null && textureCoordinatesToAppend is null)
        //{
        //    ThrowHelper.ThrowInvalidOperationException(SourceMeshTextureCoordinatesShouldNotBeNull);
        //}

        if (normalsToAppend is not null && normalsToAppend.Count != positionsToAppend.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfNormals);
        }

        if (textureCoordinatesToAppend is not null && textureCoordinatesToAppend.Count != positionsToAppend.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        var index0 = this.Positions.Count;
        foreach (var p in positionsToAppend)
        {
            this.Positions.Add(p);
        }

        if (this.Normals is not null && normalsToAppend is not null)
        {
            foreach (var n in normalsToAppend)
            {
                this.Normals.Add(n);
            }
        }

        if (this.TextureCoordinates is not null && textureCoordinatesToAppend is not null)
        {
            foreach (var t in textureCoordinatesToAppend)
            {
                this.TextureCoordinates.Add(t);
            }
        }

        foreach (var i in triangleIndicesToAppend)
        {
            this.TriangleIndices.Add(index0 + i);
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
    public void ChamferCorner(Vector3 p, float d, float eps = 1e-6f, IList<Vector3>? chamferPoints = null)
    {
        this.NoSharedVertices();

        this.Normals = null;
        this.TextureCoordinates = null;

        var cornerNormal = this.FindCornerNormal(p, eps);

        var newCornerPoint = p - (cornerNormal * d);
        var index0 = this.Positions.Count;
        this.Positions.Add(newCornerPoint);

        var plane = PlaneHelper.Create(newCornerPoint, cornerNormal);

        var ntri = this.TriangleIndices.Count;

        for (var i = 0; i < ntri; i += 3)
        {
            var i0 = i;
            var i1 = i + 1;
            var i2 = i + 2;
            var p0 = this.Positions[this.TriangleIndices[i0]];
            var p1 = this.Positions[this.TriangleIndices[i1]];
            var p2 = this.Positions[this.TriangleIndices[i2]];
            var pp0 = p - p0;
            var pp1 = p - p1;
            var pp2 = p - p2;
            var d0 = pp0.LengthSquared();
            var d1 = pp1.LengthSquared();
            var d2 = pp2.LengthSquared();
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

            p0 = this.Positions[this.TriangleIndices[i0]];
            p1 = this.Positions[this.TriangleIndices[i1]];
            p2 = this.Positions[this.TriangleIndices[i2]];

            // origin is the corner vertex (at index i0)
            // find the intersections between the chamfer plane and the two edges connected to the corner

            if (!plane.IntersectsLine(ref p0, ref p1, out Vector3 p01))
            {
                continue;
            }

            if (!plane.IntersectsLine(ref p0, ref p2, out Vector3 p02))
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

            var i01 = i0;

            // change the original triangle to use the first chamfer point
            this.Positions[this.TriangleIndices[i01]] = p01;

            var i02 = this.Positions.Count;
            this.Positions.Add(p02);

            // add a new triangle for the other chamfer point
            this.TriangleIndices.Add(i01);
            this.TriangleIndices.Add(i2);
            this.TriangleIndices.Add(i02);

            // add a triangle connecting the chamfer points and the new corner point
            this.TriangleIndices.Add(index0);
            this.TriangleIndices.Add(i01);
            this.TriangleIndices.Add(i02);
        }

        this.NoSharedVertices();
    }

    /// <summary>
    /// Checks the performance limits.
    /// </summary>
    /// <remarks>
    /// See <a href="https://msdn.microsoft.com/en-us/library/bb613553(v=vs.100).aspx">MSDN</a>.
    /// Try to keep mesh sizes under these limits:
    /// Positions : 20,001 point instances
    /// TriangleIndices : 60,003 integer instances
    /// </remarks>
    public void CheckPerformanceLimits()
    {
        if (this.Positions.Count > 20000)
        {
            Trace.WriteLine(string.Format("Too many positions ({0}).", this.Positions.Count));
        }

        if (this.TriangleIndices.Count > 60002)
        {
            Trace.WriteLine(string.Format("Too many triangle indices ({0}).", this.TriangleIndices.Count));
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
    private Vector3 FindCornerNormal(Vector3 p, float eps)
    {
        var sum = new Vector3();
        var count = 0;
        var addedNormals = new HashSet<Vector3>();
        for (var i = 0; i < this.TriangleIndices.Count; i += 3)
        {
            var i0 = i;
            var i1 = i + 1;
            var i2 = i + 2;
            var p0 = this.Positions[this.TriangleIndices[i0]];
            var p1 = this.Positions[this.TriangleIndices[i1]];
            var p2 = this.Positions[this.TriangleIndices[i2]];

            // check if any of the vertices are on the corner
            var pp0 = p - p0;
            var pp1 = p - p1;
            var pp2 = p - p2;
            var d0 = pp0.LengthSquared();
            var d1 = pp1.LengthSquared();
            var d2 = pp2.LengthSquared();
            var mind = Math.Min(d0, Math.Min(d1, d2));
            if (mind > eps)
            {
                continue;
            }

            // calculate the triangle normal and check if this face is already added
            var p10 = p1 - p0;
            var p20 = p2 - p0;
            var normal = Vector3.Normalize(Vector3.Cross(p10, p20));

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
            return new Vector3();
        }

        return sum * (1f / count);
    }

    /// <summary>
    /// Makes sure no triangles share the same vertex.
    /// </summary>
    private void NoSharedVertices()
    {
        var p = new Vector3Collection();
        var ti = new IntCollection();
        Vector3Collection? n = this.Normals is not null ? new() : null;
        Vector2Collection? tc = this.TextureCoordinates is not null ? new() : null;

        for (var i = 0; i < this.TriangleIndices.Count; i += 3)
        {
            var i0 = i;
            var i1 = i + 1;
            var i2 = i + 2;
            var index0 = this.TriangleIndices[i0];
            var index1 = this.TriangleIndices[i1];
            var index2 = this.TriangleIndices[i2];
            var p0 = this.Positions[index0];
            var p1 = this.Positions[index1];
            var p2 = this.Positions[index2];
            p.Add(p0);
            p.Add(p1);
            p.Add(p2);
            ti.Add(i0);
            ti.Add(i1);
            ti.Add(i2);
            if (n != null)
            {
                n.Add(this.Normals![index0]);
                n.Add(this.Normals![index1]);
                n.Add(this.Normals![index2]);
            }

            if (tc != null)
            {
                tc.Add(this.TextureCoordinates![index0]);
                tc.Add(this.TextureCoordinates![index1]);
                tc.Add(this.TextureCoordinates![index2]);
            }
        }

        this.Positions = p;
        this.TriangleIndices = ti;
        this.Normals = n;
        this.TextureCoordinates = tc;
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
    public void Scale(float scaleX, float scaleY, float scaleZ)
    {
        for (var i = 0; i < this.Positions.Count; i++)
        {
            this.Positions[i] = new Vector3(this.Positions[i].X * scaleX, this.Positions[i].Y * scaleY, this.Positions[i].Z * scaleZ);
        }

        if (this.Normals != null)
        {
            for (var i = 0; i < this.Normals.Count; i++)
            {
                var v = new Vector3(this.Normals[i].X * scaleX, this.Normals[i].Y * scaleY, this.Normals[i].Z * scaleZ);
                v = Vector3.Normalize(v);
                this.Normals[i] = v;
            }
        }
    }

    /// <summary>
    /// Subdivides each triangle into four sub-triangles.
    /// </summary>
    private void Subdivide4()
    {
        // Each triangle is divided into four subtriangles, adding new vertices in the middle of each edge.
        var ip = this.Positions.Count;
        var ntri = this.TriangleIndices.Count;
        for (var i = 0; i < ntri; i += 3)
        {
            var i0 = this.TriangleIndices[i];
            var i1 = this.TriangleIndices[i + 1];
            var i2 = this.TriangleIndices[i + 2];
            var p0 = this.Positions[i0];
            var p1 = this.Positions[i1];
            var p2 = this.Positions[i2];
            var v01 = p1 - p0;
            var v12 = p2 - p1;
            var v20 = p0 - p2;
            var p01 = p0 + (v01 * 0.5f);
            var p12 = p1 + (v12 * 0.5f);
            var p20 = p2 + (v20 * 0.5f);

            var i01 = ip++;
            var i12 = ip++;
            var i20 = ip++;

            this.Positions.Add(p01);
            this.Positions.Add(p12);
            this.Positions.Add(p20);

            if (this.Normals != null)
            {
                var n = this.Normals[i0];
                this.Normals.Add(n);
                this.Normals.Add(n);
                this.Normals.Add(n);
            }

            if (this.TextureCoordinates != null)
            {
                var uv0 = this.TextureCoordinates[i0];
                var uv1 = this.TextureCoordinates[i0 + 1];
                var uv2 = this.TextureCoordinates[i0 + 2];
                var t01 = uv1 - uv0;
                var t12 = uv2 - uv1;
                var t20 = uv0 - uv2;
                var u01 = uv0 + (t01 * 0.5f);
                var u12 = uv1 + (t12 * 0.5f);
                var u20 = uv2 + (t20 * 0.5f);
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
    /// See <a href="http://en.wikipedia.org/wiki/Barycentric_subdivision">wikipedia</a>.
    /// </remarks>
    private void SubdivideBarycentric()
    {
        // The BCS of a triangle S divides it into six triangles; each part has one vertex v2 at the
        // barycenter of S, another one v1 at the midpoint of some side, and the last one v0 at one
        // of the original vertices.
        var im = this.Positions.Count;
        var ntri = this.TriangleIndices.Count;
        for (var i = 0; i < ntri; i += 3)
        {
            var i0 = this.TriangleIndices[i];
            var i1 = this.TriangleIndices[i + 1];
            var i2 = this.TriangleIndices[i + 2];
            var p0 = this.Positions[i0];
            var p1 = this.Positions[i1];
            var p2 = this.Positions[i2];
            var v01 = p1 - p0;
            var v12 = p2 - p1;
            var v20 = p0 - p2;
            var p01 = p0 + (v01 * 0.5f);
            var p12 = p1 + (v12 * 0.5f);
            var p20 = p2 + (v20 * 0.5f);
            var m = new Vector3((p0.X + p1.X + p2.X) / 3, (p0.Y + p1.Y + p2.Y) / 3, (p0.Z + p1.Z + p2.Z) / 3);

            var i01 = im + 1;
            var i12 = im + 2;
            var i20 = im + 3;

            this.Positions.Add(m);
            this.Positions.Add(p01);
            this.Positions.Add(p12);
            this.Positions.Add(p20);

            if (this.Normals != null)
            {
                var n = this.Normals[i0];
                this.Normals.Add(n);
                this.Normals.Add(n);
                this.Normals.Add(n);
                this.Normals.Add(n);
            }

            if (this.TextureCoordinates != null)
            {
                var uv0 = this.TextureCoordinates[i0];
                var uv1 = this.TextureCoordinates[i0 + 1];
                var uv2 = this.TextureCoordinates[i0 + 2];
                var t01 = uv1 - uv0;
                var t12 = uv2 - uv1;
                var t20 = uv0 - uv2;
                var u01 = uv0 + (t01 * 0.5f);
                var u12 = uv1 + (t12 * 0.5f);
                var u20 = uv2 + (t20 * 0.5f);
                var uvm = new Vector2((uv0.X + uv1.X) * 0.5f, (uv0.Y + uv1.Y) * 0.5f);
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

    /// <summary>
    /// Performs a linear subdivision of the mesh.
    /// </summary>
    /// <param name="barycentric">
    /// Add a vertex in the center if set to <c>true</c> .
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

    public void Reset()
    {
        Positions = new();
        Normals = null;
        TextureCoordinates = null;
        Tangents = null;
        BiTangents = null;
        TriangleIndices = new();
    }
    #endregion Helper Functions


    #region Exporter Functions
    /// <summary>
    /// Converts the geometry to a <see cref="MeshGeometry3D"/>.
    /// All internal mesh builder data are directly assigned to the <see cref="MeshGeometry3D"/> without copying.
    /// User must call <see cref="Reset"/> to reset and reuse the mesh builder object to create new meshes.
    /// </summary>
    /// <returns>
    /// A mesh geometry.
    /// </returns>
    public MeshGeometry3D ToMesh()
    {
        if (this.TriangleIndices.Count == 0)
        {
            var emptyGeometry = new MeshGeometry3D();
            return emptyGeometry;
        }

        if (this.Normals is not null && this.Normals.Count != this.Positions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfNormals);
        }

        if (this.TextureCoordinates is not null && this.TextureCoordinates.Count != this.Positions.Count)
        {
            ThrowHelper.ThrowInvalidOperationException(WrongNumberOfTextureCoordinates);
        }

        if (this.HasTangents && this.Tangents!.Count == 0 && this.Normals is not null && this.TextureCoordinates is not null)
        {
            ComputeTangents(this.Positions, this.Normals, this.TextureCoordinates, this.TriangleIndices,
                out var tan, out var bitan);
            Debug.Assert(tan is Vector3Collection);
            Debug.Assert(bitan is Vector3Collection);
            this.Tangents = tan as Vector3Collection;
            this.BiTangents = bitan as Vector3Collection;
        }

        return new MeshGeometry3D()
        {
            TriangleIndices = this.TriangleIndices,
            Positions = this.Positions,
            Normals = this.HasNormals ? this.Normals : null,
            TextureCoordinates = this.HasTexCoords ? this.TextureCoordinates : null,
            Tangents = this.HasTangents ? this.Tangents : null,
            BiTangents = this.HasTangents ? this.BiTangents : null,
        };
    }

    private static Vector3 GetPosition(float theta, float phi, float radius)
    {
#if NET6_0_OR_GREATER
        float x = radius * MathF.Sin(theta) * MathF.Sin(phi);
        float y = radius * MathF.Cos(phi);
        float z = radius * MathF.Cos(theta) * MathF.Sin(phi);
#else
        float x = radius * (float)Math.Sin(theta) * (float)Math.Sin(phi);
        float y = radius * (float)Math.Cos(phi);
        float z = radius * (float)Math.Cos(theta) * (float)Math.Sin(phi);
#endif
        return new Vector3(x, y, z);
    }

    private static Vector3 GetNormal(float theta, float phi)
    {
        return GetPosition(theta, phi, 1.0f);
    }

    private static float DegToRad(float degrees)
    {
#if NET6_0_OR_GREATER
        return (degrees / 180.0f) * MathF.PI;
#else
        return (degrees / 180.0f) * (float)Math.PI;
#endif
    }

    private static Vector2 GetTextureCoordinate(float theta, float phi)
    {
#if NET6_0_OR_GREATER
        return new Vector2(theta / (2 * MathF.PI), phi / MathF.PI);
#else
        return new Vector2(theta / (2 * (float)Math.PI), phi / (float)Math.PI);
#endif
    }

    /// <summary>
    /// Tesselates the element and returns a MeshGeometry3D representing the 
    /// tessellation based on the parameters given 
    /// </summary>        
    public void AppendSphere(Vector3 center, float radius = 1, int thetaSteps = 64, int phiSteps = 64)
    {
        AppendSphere(center, radius, thetaSteps, phiSteps,
            out var pos, out var nor, out var tcoord, out List<int> tind);

        int i0 = this.Positions.Count;
        this.Positions.AddRange(pos);
        this.Normals?.AddRange(nor);
        this.TextureCoordinates?.AddRange(tcoord);
        this.TriangleIndices.AddRange(tind.Select(x => x + i0));
    }

    private static void AppendSphere(Vector3 center, float radius, int thetaSteps, int phiSteps,
        out IList<Vector3> positions, out IList<Vector3> normals, out IList<Vector2> textureCoordinates, out List<int> triangleIndices)
    {
        positions = new Vector3Collection();
        normals = new Vector3Collection();
        textureCoordinates = new Vector2Collection();
        triangleIndices = new();

        float dt = DegToRad(360.0f) / thetaSteps;
        float dp = DegToRad(180.0f) / phiSteps;

        for (int pi = 0; pi <= phiSteps; pi++)
        {
            float phi = pi * dp;
            for (int ti = 0; ti <= thetaSteps; ti++)
            {
                // we want to start the mesh on the x axis
                float theta = ti * dt;

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
    #endregion Exporter Functions
}
