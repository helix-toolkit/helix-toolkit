using HelixToolkit.Geometry;
using System.Diagnostics;
using System.Text;
#if NET6_0_OR_GREATER
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
#endif
namespace HelixToolkit.Geometry;

/// <summary>
/// Provides helper methods for mesh geometries.
/// </summary>
public static class MeshGeometryHelper
{
    // Optimizing 3D Collections in WPF
    // http://blogs.msdn.com/timothyc/archive/2006/08/31/734308.aspx
    // - Remember to disconnect collections from the MeshGeometry when changing it

    /// <summary>
    /// Calculates the normal vectors.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    /// <returns>
    /// Collection of normal vectors.
    /// </returns>
    public static IList<Vector3> CalculateNormals(this MeshGeometry3D mesh)
    {
        return CalculateNormals(mesh.Positions, mesh.TriangleIndices);
    }

    /// <summary>
    /// Calculates the normal vectors.
    /// </summary>
    /// <param name="positions">
    /// The positions.
    /// </param>
    /// <param name="triangleIndices">
    /// The triangle indices.
    /// </param>
    /// <returns>
    /// Collection of normal vectors.
    /// </returns>
    public static IList<Vector3> CalculateNormals(IList<Vector3> positions, IList<int> triangleIndices)
    {
        var normals = new Vector3Collection(positions.Count);
        normals.Resize(positions.Count, true);
        for (var i = 0; i < triangleIndices.Count; i += 3)
        {
            var index0 = triangleIndices[i];
            var index1 = triangleIndices[i + 1];
            var index2 = triangleIndices[i + 2];
            var p0 = positions[index0];
            var p1 = positions[index1];
            var p2 = positions[index2];
            var u = p1 - p0;
            var v = p2 - p0;
            var w = Vector3.Cross(u, v);
            w = Vector3.Normalize(w);
            normals[index0] += w;
            normals[index1] += w;
            normals[index2] += w;
        }

        NormalizeInPlace(normals);
        return normals;
    }

    public unsafe static void NormalizeInPlace(Vector3Collection data)
    {
#pragma warning disable CS0219
        var elementCount = 3;
#pragma warning restore CS0219
        var currIndex = 0;
#if NET6_0_OR_GREATER
        if (MathSettings.EnableSIMD)
        {
            if (Sse2.IsSupported)
            {
                // Ref: https://virtuallyrandom.com/part-2-vector3-batch-normalization-fpu-vs-simd/
                Debug.Assert(Vector128<float>.Count == 4);
                var inc = Vector128<int>.Count * elementCount;
                var arrayCount = (data.Count - currIndex) * elementCount;
                int length = arrayCount - arrayCount % inc;
                fixed (void* dataPtr = data.GetInternalArray())
                {
                    float* floatData = (float*)dataPtr;
                    floatData += currIndex * elementCount;
                    for (int i = 0; i < length; i += inc, floatData += inc)
                    {
                        var vx = Sse.LoadVector128(floatData);
                        var vy = Sse.LoadVector128(floatData + 4);
                        var vz = Sse.LoadVector128(floatData + 8);
                        // Compute the reciprocal of magnitude: 1 / sqrt(x^2 + y^2 + z^2)
                        var sqX = Sse.Multiply(vx, vx);
                        var sqY = Sse.Multiply(vy, vy);
                        var sqZ = Sse.Multiply(vz, vz);
                        // first transpose
                        //
                        // 0: x0 y0 z0 x1    x0 y0 y1 z1
                        // 1: y1 z1 x2 y2 => z0 x1 x2 y2
                        // 2: z2 x3 y3 z3    z2 x3 y3 z3
                        var xpose1_0 = Sse.MoveLowToHigh(sqX, sqY);
                        var xpose1_1 = Sse.MoveHighToLow(sqY, sqX);
                        // second transpose
                        //
                        // 0: x0 y0 y1 z1    x0 y0 y1 z1
                        // 1: z0 x1 x2 y2 => z0 x1 z2 x3
                        // 2: z2 x3 y3 z3    x2 y2 y3 z3
                        var xpose2_1 = Sse.MoveLowToHigh(xpose1_1, sqZ);
                        var xpose2_2 = Sse.MoveHighToLow(sqZ, xpose1_1);
                        // third transpose
                        // 0: x0 y0 y1 z1    x0 y1 x2 y3
                        // 1: z0 x1 z2 x3 => z0 x1 z2 x3
                        // 2: x2 y2 y3 z3    y0 z1 y2 z3
                        var xpose3_0 = Sse.Shuffle(xpose1_0, xpose2_2, 0b10001000);
                        var xpose3_2 = Sse.Shuffle(xpose1_0, xpose2_2, 0b11011101);

                        var v = Sse.ReciprocalSqrt(Sse.Add(xpose3_2, Sse.Add(xpose3_0, xpose2_1)));

                        // Normalize with reciprocal of magnitude

                        // to apply it, we have to mangle it around again
                        //               s0, s0, s0, s1
                        // x, y, z, w => s1, s1, s2, s2
                        //               s2, s3, s3, s3
                        var scaleX = Sse.Shuffle(v, v, 0b01_00_00_00);
                        var scaleY = Sse.Shuffle(v, v, 0b10_10_01_01);
                        var scaleZ = Sse.Shuffle(v, v, 0b11_11_11_10);

                        vx = Sse.Multiply(vx, scaleX);
                        vy = Sse.Multiply(vy, scaleY);
                        vz = Sse.Multiply(vz, scaleZ);
                        Sse.Store(floatData, vx);
                        Sse.Store(floatData + 4, vy);
                        Sse.Store(floatData + 8, vz);
                    }
                }
                currIndex = length / elementCount;
            }
        }
#endif
        for (var i = currIndex; i < data.Count; i++)
        {
            data[i] = Vector3.Normalize(data[i]);
        }
    }

    /// <summary>
    /// Finds edges that are only connected to one triangle.
    /// </summary>
    /// <param name="mesh">
    /// A mesh geometry.
    /// </param>
    /// <returns>
    /// The edge indices for the edges that are only used by one triangle.
    /// </returns>
    public static IList<int> FindBorderEdges(this MeshGeometry3D mesh)
    {
        var dict = new Dictionary<ulong, int>();

        for (var i = 0; i < mesh.TriangleIndices.Count / 3; i++)
        {
            var i0 = i * 3;
            for (var j = 0; j < 3; j++)
            {
                var index0 = mesh.TriangleIndices[i0 + j];
                var index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                var minIndex = Math.Min(index0, index1);
                var maxIndex = Math.Max(index1, index0);
                var key = CreateKey((uint)minIndex, (uint)maxIndex);
                if (dict.ContainsKey(key))
                {
                    dict[key] = dict[key] + 1;
                }
                else
                {
                    dict.Add(key, 1);
                }
            }
        }

        var edges = new IntCollection();
        foreach (var kvp in dict)
        {
            // find edges only used by 1 triangle
            if (kvp.Value == 1)
            {
                ReverseKey(kvp.Key, out uint i0, out uint i1);
                edges.Add((int)i0);
                edges.Add((int)i1);
            }
        }

        return edges;
    }

    /// <summary>
    /// Finds all edges in the mesh (each edge is only included once).
    /// </summary>
    /// <param name="mesh">
    /// A mesh geometry.
    /// </param>
    /// <returns>
    /// The edge indices (minimum index first).
    /// </returns>
    public static IList<int> FindEdges(this MeshGeometry3D mesh)
    {
        var edges = new IntCollection();
        var dict = new HashSet<ulong>();

        for (var i = 0; i < mesh.TriangleIndices.Count / 3; i++)
        {
            var i0 = i * 3;
            for (var j = 0; j < 3; j++)
            {
                var index0 = mesh.TriangleIndices[i0 + j];
                var index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                var minIndex = Math.Min(index0, index1);
                var maxIndex = Math.Max(index1, index0);
                var key = CreateKey((uint)minIndex, (uint)maxIndex);
                if (!dict.Contains(key))
                {
                    edges.Add(minIndex);
                    edges.Add(maxIndex);
                    dict.Add(key);
                }
            }
        }

        return edges;
    }

#pragma warning disable IDE0052
    private readonly struct EdgeKey
    {
        private readonly Vector3 position0;
        private readonly Vector3 position1;

        public EdgeKey(Vector3 position0, Vector3 position1)
        {
            this.position0 = position0;
            this.position1 = position1;
        }
    }
#pragma warning restore IDE0052

    /// <summary>
    /// Finds all edges where the angle between adjacent triangle normal vectors.
    /// is larger than minimumAngle
    /// </summary>
    /// <param name="mesh">
    /// A mesh geometry.
    /// </param>
    /// <param name="minimumAngle">
    /// The minimum angle between the normal vectors of two adjacent triangles (degrees).
    /// </param>
    /// <returns>
    /// The edge indices.
    /// </returns>
    public static IList<int> FindSharpEdges(this MeshGeometry3D mesh, float minimumAngle)
    {
        var edgeIndices = new IntCollection();
        var edgeNormals = new Dictionary<EdgeKey, Vector3>();
        for (var i = 0; i < mesh.TriangleIndices.Count / 3; i++)
        {
            var i0 = i * 3;
            var p0 = mesh.Positions[mesh.TriangleIndices[i0]];
            var p1 = mesh.Positions[mesh.TriangleIndices[i0 + 1]];
            var p2 = mesh.Positions[mesh.TriangleIndices[i0 + 2]];
            var triangleNormal = Vector3.Cross(p1 - p0, p2 - p0);

            // Handle degenerated triangles.
            if (triangleNormal.LengthSquared() < 0.001f)
                continue;

            triangleNormal = Vector3.Normalize(triangleNormal);
            for (var j = 0; j < 3; j++)
            {
                var index0 = mesh.TriangleIndices[i0 + j];
                var index1 = mesh.TriangleIndices[i0 + (j + 1) % 3];
                var position0 = mesh.Positions[index0];
                var position1 = mesh.Positions[index1];
                var edgeKey = new EdgeKey(position0, position1);
                var reverseEdgeKey = new EdgeKey(position1, position0);
                if (edgeNormals.TryGetValue(edgeKey, out var value) ||
                    edgeNormals.TryGetValue(reverseEdgeKey, out value))
                {
                    var rawDot = Vector3.Dot(triangleNormal, value);

                    // Acos returns NaN if rawDot > 1 or rawDot < -1
                    var dot = Math.Max(-1, Math.Min(rawDot, 1));

                    var angle = 180 / Math.PI * Math.Acos(dot);
                    if (angle > minimumAngle)
                    {
                        edgeIndices.Add(index0);
                        edgeIndices.Add(index1);
                    }
                }
                else
                {
                    edgeNormals.Add(edgeKey, triangleNormal);
                }
            }
        }
        return edgeIndices;
    }

    /// <summary>
    /// Creates a new mesh where no vertices are shared.
    /// </summary>
    /// <param name="input">
    /// The input mesh.
    /// </param>
    /// <returns>
    /// A new mesh.
    /// </returns>
    public static MeshGeometry3D NoSharedVertices(this MeshGeometry3D input)
    {
        var p = new Vector3Collection();
        var ti = new IntCollection();
        Vector3Collection? n = input.Normals != null && input.Normals.Count > 0 ? new() : null;
        Vector2Collection? tc = input.TextureCoordinates != null && input.TextureCoordinates.Count > 0 ? new() : null;

        for (var i = 0; i < input.TriangleIndices.Count; i += 3)
        {
            var i0 = i;
            var i1 = i + 1;
            var i2 = i + 2;
            var index0 = input.TriangleIndices[i0];
            var index1 = input.TriangleIndices[i1];
            var index2 = input.TriangleIndices[i2];
            var p0 = input.Positions[index0];
            var p1 = input.Positions[index1];
            var p2 = input.Positions[index2];
            p.Add(p0);
            p.Add(p1);
            p.Add(p2);
            ti.Add(i0);
            ti.Add(i1);
            ti.Add(i2);
            if (n != null)
            {
                n.Add(input.Normals![index0]);
                n.Add(input.Normals![index1]);
                n.Add(input.Normals![index2]);
            }

            if (tc != null)
            {
                tc.Add(input.TextureCoordinates![index0]);
                tc.Add(input.TextureCoordinates![index1]);
                tc.Add(input.TextureCoordinates![index2]);
            }
        }

        return new MeshGeometry3D
        {
            Positions = p,
            TriangleIndices = ti,
            Normals = n,
            TextureCoordinates = tc
        };
    }

    /// <summary>
    /// Simplifies the specified mesh.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    /// <param name="eps">
    /// The tolerance.
    /// </param>
    /// <returns>
    /// A simplified mesh.
    /// </returns>
    public static MeshGeometry3D Simplify(this MeshGeometry3D mesh, float eps)
    {
        // Find common positions
        var dict = new Dictionary<int, int>(); // map position index to first occurence of same position
        for (var i = 0; i < mesh.Positions.Count; i++)
        {
            for (var j = i + 1; j < mesh.Positions.Count; j++)
            {
                if (dict.ContainsKey(j))
                {
                    continue;
                }
                var v = mesh.Positions[i] - mesh.Positions[j];
                var l2 = v.LengthSquared();
                if (l2 < eps)
                {
                    dict.Add(j, i);
                }
            }
        }

        var p = new Vector3Collection();
        var ti = new IntCollection();

        // create new positions array
        var newIndex = new Dictionary<int, int>(); // map old index to new index
        for (var i = 0; i < mesh.Positions.Count; i++)
        {
            if (!dict.ContainsKey(i))
            {
                newIndex.Add(i, p.Count);
                p.Add(mesh.Positions[i]);
            }
        }

        // Update triangle indices
        foreach (var index in mesh.TriangleIndices)
        {
            ti.Add(dict.TryGetValue(index, out int j) ? newIndex[j] : newIndex[index]);
        }

        var result = new MeshGeometry3D
        {
            Positions = p,
            TriangleIndices = ti
        };

        return result;
    }

    /// <summary>
    /// Validates the specified mesh.
    /// </summary>
    /// <param name="mesh">The mesh.</param>
    /// <returns>Validation report or null if no issues were found.</returns>
    public static string? Validate(this MeshGeometry3D mesh)
    {
        var sb = new StringBuilder();
        if (mesh.Normals != null && mesh.Normals.Count != 0 && mesh.Normals.Count != mesh.Positions.Count)
        {
            sb.AppendLine("Wrong number of normal vectors");
        }

        if (mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count != 0
            && mesh.TextureCoordinates.Count != mesh.Positions.Count)
        {
            sb.AppendLine("Wrong number of TextureCoordinates");
        }

        if (mesh.TriangleIndices.Count % 3 != 0)
        {
            sb.AppendLine("TriangleIndices not complete");
        }

        for (var i = 0; i < mesh.TriangleIndices.Count; i++)
        {
            var index = mesh.TriangleIndices[i];
            if (index < 0 || index >= mesh.Positions.Count)
            {
                sb.AppendFormat("Wrong index {0} in triangle {1} vertex {2}", index, i / 3, i % 3);
                sb.AppendLine();
            }
        }

        return sb.Length > 0 ? sb.ToString() : null;
    }


    /// <summary>
    /// Cuts the mesh with the specified plane.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    /// <param name="plane">
    /// The plane origin.
    /// </param>
    /// <param name="normal">
    /// The plane normal.
    /// </param>
    /// <returns>
    /// The <see cref="MeshGeometry3D"/>.
    /// </returns>
    public static MeshGeometry3D Cut(this MeshGeometry3D mesh, Vector3 plane, Vector3 normal)
    {
        var hasTextureCoordinates = mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0;
        var hasNormals = mesh.Normals != null && mesh.Normals.Count > 0;
        var meshBuilder = new MeshBuilder(hasNormals, hasTextureCoordinates);
        var contourHelper = new ContourHelper(plane, normal, mesh);
        foreach (var position in mesh.Positions)
        {
            meshBuilder.Positions.Add(position);
        }

        if (hasTextureCoordinates)
        {
            foreach (var textureCoordinate in mesh.TextureCoordinates!)
            {
                meshBuilder.TextureCoordinates!.Add(textureCoordinate);
            }
        }

        if (hasNormals)
        {
            foreach (var n in mesh.Normals!)
            {
                meshBuilder.Normals!.Add(n);
            }
        }

        for (var i = 0; i < mesh.TriangleIndices.Count; i += 3)
        {
            var index0 = mesh.TriangleIndices[i];
            var index1 = mesh.TriangleIndices[i + 1];
            var index2 = mesh.TriangleIndices[i + 2];


            contourHelper.ContourFacet(index0, index1, index2,
                out Vector3[] positions, out Vector3[] normals, out Vector2[] textureCoordinates, out int[] triangleIndices);

            foreach (var p in positions)
            {
                meshBuilder.Positions.Add(p);
            }

            if (meshBuilder.TextureCoordinates is not null)
            {
                foreach (var tc in textureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(tc);
                }
            }

            if (meshBuilder.Normals is not null)
            {
                foreach (var n in normals)
                {
                    meshBuilder.Normals.Add(n);
                }
            }

            foreach (var ti in triangleIndices)
            {
                meshBuilder.TriangleIndices.Add(ti);
            }
        }

        return meshBuilder.ToMesh();
    }

    /// <summary>
    /// Gets the contour segments.
    /// </summary>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    /// <param name="plane">
    /// The plane origin.
    /// </param>
    /// <param name="normal">
    /// The plane normal.
    /// </param>
    /// <returns>
    /// The segments of the contour.
    /// </returns>
    public static IList<Vector3> GetContourSegments(this MeshGeometry3D mesh, Vector3 plane, Vector3 normal)
    {
        var segments = new Vector3Collection();
        var contourHelper = new ContourHelper(plane, normal, mesh);
        for (var i = 0; i < mesh.TriangleIndices.Count; i += 3)
        {
            contourHelper.ContourFacet(
                mesh.TriangleIndices[i],
                mesh.TriangleIndices[i + 1],
                mesh.TriangleIndices[i + 2],
                out Vector3[] positions,
                out _,
                out _,
                out _);

            segments.AddRange(positions);
        }

        return segments;
    }


    /// <summary>
    /// Combines the segments.
    /// </summary>
    /// <param name="segments">
    /// The segments.
    /// </param>
    /// <param name="eps">
    /// The tolerance.
    /// </param>
    /// <returns>
    /// Enumerated connected contour curves.
    /// </returns>
    public static IEnumerable<IList<Vector3>> CombineSegments(IList<Vector3> segments, float eps)
    {
        // This is a simple, slow, naïve method - should be improved:
        // http://stackoverflow.com/questions/1436091/joining-unordered-line-segments
        var curve = new Vector3Collection();
        var curveCount = 0;

        var segmentCount = segments.Count;
        int segment1 = -1, segment2 = -1;
        while (segmentCount > 0)
        {
            if (curveCount > 0)
            {
                // Find a segment that is connected to the head of the contour
                segment1 = FindConnectedSegment(segments, curve[0], eps);
                if (segment1 >= 0)
                {
                    if (segment1 % 2 == 1)
                    {
                        curve.Insert(0, segments[segment1 - 1]);
                        segments.RemoveAt(segment1 - 1);
                        segments.RemoveAt(segment1 - 1);
                    }
                    else
                    {
                        curve.Insert(0, segments[segment1 + 1]);
                        segments.RemoveAt(segment1);
                        segments.RemoveAt(segment1);
                    }

                    curveCount++;
                    segmentCount -= 2;
                }

                // Find a segment that is connected to the tail of the contour
                segment2 = FindConnectedSegment(segments, curve[curveCount - 1], eps);
                if (segment2 >= 0)
                {
                    if (segment2 % 2 == 1)
                    {
                        curve.Add(segments[segment2 - 1]);
                        segments.RemoveAt(segment2 - 1);
                        segments.RemoveAt(segment2 - 1);
                    }
                    else
                    {
                        curve.Add(segments[segment2 + 1]);
                        segments.RemoveAt(segment2);
                        segments.RemoveAt(segment2);
                    }

                    curveCount++;
                    segmentCount -= 2;
                }
            }

            if ((segment1 < 0 && segment2 < 0) || segmentCount == 0)
            {
                if (curveCount > 0)
                {
                    yield return curve;
                    curve = new Vector3Collection();
                    curveCount = 0;
                }

                if (segmentCount > 0)
                {
                    curve.Add(segments[0]);
                    curve.Add(segments[1]);
                    curveCount += 2;
                    segments.RemoveAt(0);
                    segments.RemoveAt(0);
                    segmentCount -= 2;
                }
            }
        }
    }

    /// <summary>
    /// Create a 64-bit key from two 32-bit indices
    /// </summary>
    /// <param name="i0">
    /// The i 0.
    /// </param>
    /// <param name="i1">
    /// The i 1.
    /// </param>
    /// <returns>
    /// The create key.
    /// </returns>
    private static ulong CreateKey(uint i0, uint i1)
    {
        return ((ulong)i0 << 32) + i1;
    }

    /// <summary>
    /// Extract two 32-bit indices from the 64-bit key
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="i0">
    /// The i 0.
    /// </param>
    /// <param name="i1">
    /// The i 1.
    /// </param>
    private static void ReverseKey(ulong key, out uint i0, out uint i1)
    {
        i0 = (uint)(key >> 32);
        i1 = (uint)((key << 32) >> 32);
    }

    /// <summary>
    /// Finds the nearest connected segment to the specified point.
    /// </summary>
    /// <param name="segments">
    /// The segments.
    /// </param>
    /// <param name="point">
    /// The point.
    /// </param>
    /// <param name="eps">
    /// The tolerance.
    /// </param>
    /// <returns>
    /// The index of the nearest point.
    /// </returns>
    private static int FindConnectedSegment(IList<Vector3> segments, Vector3 point, float eps)
    {
        var best = eps;
        var result = -1;
        for (var i = 0; i < segments.Count; i++)
        {
            var v = point - segments[i];
            var ls0 = v.LengthSquared();
            if (ls0 < best)
            {
                result = i;
                best = ls0;
            }
        }

        return result;
    }

    /// <summary>
    /// Remove isolated(not connected to any triangles) vertices
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static MeshGeometry3D RemoveIsolatedVertices(this MeshGeometry3D mesh)
    {
        RemoveIsolatedVertices(mesh.Positions, mesh.TriangleIndices, mesh.TextureCoordinates, mesh.Normals,
            out var vertNew, out var triNew, out var textureNew, out var normalNew);

        var newMesh = new MeshGeometry3D()
        {
            Positions = vertNew as Vector3Collection ?? new(),
            TriangleIndices = triNew as IntCollection ?? new(),
            TextureCoordinates = textureNew as Vector2Collection,
            Normals = normalNew as Vector3Collection
        };

        return newMesh;
    }

    /// <summary>
    /// Remove isolated(not connected to any triangles) vertices
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    /// <param name="texture"></param>
    /// <param name="normals"></param>
    /// <param name="verticesOut"></param>
    /// <param name="trianglesOut"></param>
    /// <param name="textureOut"></param>
    /// <param name="normalOut"></param>
    public static void RemoveIsolatedVertices(IList<Vector3> vertices, IList<int> triangles, IList<Vector2>? texture, IList<Vector3>? normals,
        out IList<Vector3> verticesOut, out IList<int> trianglesOut, out IList<Vector2>? textureOut, out IList<Vector3>? normalOut)
    {
        textureOut = null;
        normalOut = null;
        var tracking = new FastList<IntCollection>(vertices.Count);
        Debug.WriteLine(string.Format("NumVert:{0}; NumTriangle:{1};", vertices.Count, triangles.Count));
        for (var i = 0; i < vertices.Count; ++i)
        {
            tracking.Add(new IntCollection());
        }
        for (var i = 0; i < triangles.Count; ++i)
        {
            tracking[triangles[i]].Add(i);
        }

        var vertToRemove = new IntCollection(vertices.Count);
        for (var i = 0; i < vertices.Count; ++i)
        {
            if (tracking[i].Count == 0)
            {
                vertToRemove.Add(i);
            }
        }

        verticesOut = new Vector3Collection(vertices.Count - vertToRemove.Count);
        trianglesOut = new IntCollection(triangles);
        if (texture != null)
        {
            textureOut = new Vector2Collection(vertices.Count - vertToRemove.Count);
        }
        if (normals != null)
        {
            normalOut = new Vector3Collection(vertices.Count - vertToRemove.Count);
        }
        if (vertices.Count == vertToRemove.Count)
        {
            return;
        }
        var counter = 0;
        for (var i = 0; i < vertices.Count; ++i)
        {
            if (counter == vertToRemove.Count || i < vertToRemove[counter])
            {
                verticesOut.Add(vertices[i]);
                if (texture != null)
                {
                    textureOut?.Add(texture[i]);
                }
                if (normals != null)
                {
                    normalOut?.Add(normals[i]);
                }
                foreach (var t in tracking[i])
                {
                    trianglesOut[t] -= counter;
                }
            }
            else
            {
                ++counter;
            }
        }
        Debug.WriteLine(string.Format("Remesh finished. Output NumVert:{0};", verticesOut.Count));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="triangles"></param>
    /// <param name="numVerts"></param>
    public static void RemoveOutOfRangeTriangles(this IList<int> triangles, int numVerts)
    {
        var removeOutOfRangeTriangles = new IntCollection();
        for (var i = 0; i < triangles.Count; i += 3)
        {
            if (triangles[i] >= numVerts || triangles[i + 1] >= numVerts || triangles[i + 2] >= numVerts)
            {
                removeOutOfRangeTriangles.Add(i);
            }
        }
        if (removeOutOfRangeTriangles.Count > 0)
        {
            removeOutOfRangeTriangles.Reverse();
            foreach (var idx in removeOutOfRangeTriangles)
            {
                removeOutOfRangeTriangles.RemoveRange(idx, 3);
            }
        }
    }

    public static Vector3 GetCentroid(this IList<Vector3> vertices)
    {
        if (vertices.Count == 0)
        {
            return default;
        }
        var centroid = vertices[0];
        for (var i = 1; i < vertices.Count; i++)
        {
            centroid += (vertices[i] - centroid) / (i + 1);
        }
        return centroid;
    }
}
