// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides helper methods for mesh geometries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
#if SHARPDX
    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;
    using Point = global::SharpDX.Vector2;
    using Int32Collection = IntCollection;
    using Vector3DCollection = Vector3Collection;
    using Point3DCollection = Vector3Collection;
    using PointCollection = Vector2Collection;
    using DoubleOrSingle = System.Single;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using DoubleOrSingle = System.Double;
#endif

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
        public static Vector3DCollection CalculateNormals(this MeshGeometry3D mesh)
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
        public static Vector3DCollection CalculateNormals(IList<Point3D> positions, IList<int> triangleIndices)
        {
            var normals = new Vector3DCollection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                normals.Add(new Vector3D());
            }

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int index0 = triangleIndices[i];
                int index1 = triangleIndices[i + 1];
                int index2 = triangleIndices[i + 2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                Vector3D u = p1 - p0;
                Vector3D v = p2 - p0;
                Vector3D w = SharedFunctions.CrossProduct(ref u, ref v);
                w.Normalize();
                normals[index0] += w;
                normals[index1] += w;
                normals[index2] += w;
            }

            for (int i = 0; i < normals.Count; i++)
            {
                var n = normals[i];
                n.Normalize();
                normals[i] = n;
            }

            return normals;
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
        public static Int32Collection FindBorderEdges(this MeshGeometry3D mesh)
        {
            var dict = new Dictionary<ulong, int>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index1, index0);
                    ulong key = CreateKey((uint)minIndex, (uint)maxIndex);
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

            var edges = new Int32Collection();
            foreach (var kvp in dict)
            {
                // find edges only used by 1 triangle
                if (kvp.Value == 1)
                {
                    uint i0, i1;
                    ReverseKey(kvp.Key, out i0, out i1);
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
        public static Int32Collection FindEdges(this MeshGeometry3D mesh)
        {
            var edges = new Int32Collection();
            var dict = new HashSet<ulong>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index1, index0);
                    ulong key = CreateKey((uint)minIndex, (uint)maxIndex);
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
        public static Int32Collection FindSharpEdges(this MeshGeometry3D mesh, double minimumAngle)
        {
            var edgeIndices = new Int32Collection();

            // the keys of the dictionary are created from the triangle indices of the edge
            var edgeNormals = new Dictionary<ulong, Vector3D>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                var p0 = mesh.Positions[mesh.TriangleIndices[i0]];
                var p1 = mesh.Positions[mesh.TriangleIndices[i0 + 1]];
                var p2 = mesh.Positions[mesh.TriangleIndices[i0 + 2]];
                var p10 = p1 - p0;
                var p20 = p2 - p0;
                var n = SharedFunctions.CrossProduct(ref p10, ref p20);
                n.Normalize();
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + ((j + 1) % 3)];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index0, index1);
                    ulong key = CreateKey((uint)minIndex, (uint)maxIndex);
                    Vector3D value;
                    if (edgeNormals.TryGetValue(key, out value))
                    {
                        var n2 = value;
                        n2.Normalize();
                        var angle = 180 / (DoubleOrSingle)Math.PI * (DoubleOrSingle)Math.Acos(SharedFunctions.DotProduct(ref n, ref n2));
                        if (angle > minimumAngle)
                        {
                            edgeIndices.Add(minIndex);
                            edgeIndices.Add(maxIndex);
                        }
                    }
                    else
                    {
                        edgeNormals.Add(key, n);
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
            var p = new Point3DCollection();
            var ti = new Int32Collection();
            Vector3DCollection n = null;
            if (input.Normals != null)
            {
                n = new Vector3DCollection();
            }

            PointCollection tc = null;
            if (input.TextureCoordinates != null)
            {
                tc = new PointCollection();
            }

            for (int i = 0; i < input.TriangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                int index0 = input.TriangleIndices[i0];
                int index1 = input.TriangleIndices[i1];
                int index2 = input.TriangleIndices[i2];
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
                    n.Add(input.Normals[index0]);
                    n.Add(input.Normals[index1]);
                    n.Add(input.Normals[index2]);
                }

                if (tc != null)
                {
                    tc.Add(input.TextureCoordinates[index0]);
                    tc.Add(input.TextureCoordinates[index1]);
                    tc.Add(input.TextureCoordinates[index2]);
                }
            }

#if SHARPDX
            return new MeshGeometry3D { Positions = p, TriangleIndices = new IntCollection(ti), Normals = n, TextureCoordinates = tc };
#else
            return new MeshGeometry3D { Positions = p, TriangleIndices = ti, Normals = n, TextureCoordinates = tc };
#endif
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
        public static MeshGeometry3D Simplify(this MeshGeometry3D mesh, DoubleOrSingle eps)
        {
            // Find common positions
            var dict = new Dictionary<int, int>(); // map position index to first occurence of same position
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                for (int j = i + 1; j < mesh.Positions.Count; j++)
                {
                    if (dict.ContainsKey(j))
                    {
                        continue;
                    }
                    var v = mesh.Positions[i] - mesh.Positions[j];
                    var l2 = SharedFunctions.LengthSquared(ref v);
                    if (l2 < eps)
                    {
                        dict.Add(j, i);
                    }
                }
            }

            var p = new Point3DCollection();
            var ti = new Int32Collection();

            // create new positions array
            var newIndex = new Dictionary<int, int>(); // map old index to new index
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                if (!dict.ContainsKey(i))
                {
                    newIndex.Add(i, p.Count);
                    p.Add(mesh.Positions[i]);
                }
            }

            // Update triangle indices
            foreach (int index in mesh.TriangleIndices)
            {
                int j;
                ti.Add(dict.TryGetValue(index, out j) ? newIndex[j] : newIndex[index]);
            }
#if SHARPDX
            var result = new MeshGeometry3D { Positions = p, TriangleIndices = new IntCollection(ti), };
#else
            var result = new MeshGeometry3D { Positions = p, TriangleIndices = ti };
#endif
            return result;
        }

        /// <summary>
        /// Validates the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <returns>Validation report or null if no issues were found.</returns>
        public static string Validate(this MeshGeometry3D mesh)
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

            for (int i = 0; i < mesh.TriangleIndices.Count; i++)
            {
                int index = mesh.TriangleIndices[i];
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
        public static MeshGeometry3D Cut(this MeshGeometry3D mesh, Point3D plane, Vector3D normal)
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
                foreach (var textureCoordinate in mesh.TextureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(textureCoordinate);
                }
            }

            if (hasNormals)
            {
                foreach (var n in mesh.Normals)
                {
                    meshBuilder.Normals.Add(n);
                }
            }

            for (var i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                var index0 = mesh.TriangleIndices[i];
                var index1 = mesh.TriangleIndices[i + 1];
                var index2 = mesh.TriangleIndices[i + 2];

                Point3D[] positions;
                Vector3D[] normals;
                Point[] textureCoordinates;
                int[] triangleIndices;

                contourHelper.ContourFacet(index0, index1, index2, out positions, out normals, out textureCoordinates, out triangleIndices);

                foreach (var p in positions)
                {
                    meshBuilder.Positions.Add(p);
                }

                foreach (var tc in textureCoordinates)
                {
                    meshBuilder.TextureCoordinates.Add(tc);
                }

                foreach (var n in normals)
                {
                    meshBuilder.Normals.Add(n);
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
        public static IList<Point3D> GetContourSegments(this MeshGeometry3D mesh, Point3D plane, Vector3D normal)
        {
            var segments = new List<Point3D>();
            var contourHelper = new ContourHelper(plane, normal, mesh);
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                Point3D[] positions;
                Vector3D[] normals;
                Point[] textureCoordinates;
                int[] triangleIndices;

                contourHelper.ContourFacet(
                    mesh.TriangleIndices[i],
                    mesh.TriangleIndices[i + 1],
                    mesh.TriangleIndices[i + 2],
                    out positions,
                    out normals,
                    out textureCoordinates,
                    out triangleIndices);
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
        public static IEnumerable<IList<Point3D>> CombineSegments(IList<Point3D> segments, DoubleOrSingle eps)
        {
            // This is a simple, slow, naïve method - should be improved:
            // http://stackoverflow.com/questions/1436091/joining-unordered-line-segments
            var curve = new List<Point3D>();
            int curveCount = 0;

            int segmentCount = segments.Count;
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
                        curve = new List<Point3D>();
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
        private static int FindConnectedSegment(IList<Point3D> segments, Point3D point, DoubleOrSingle eps)
        {
            var best = eps;
            int result = -1;
            for (int i = 0; i < segments.Count; i++)
            {
                var v = point - segments[i];
                var ls0 = SharedFunctions.LengthSquared(ref v);
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
            Point3DCollection vertNew;
            Int32Collection triNew;
            PointCollection textureNew;
            Vector3DCollection normalNew;
            RemoveIsolatedVertices(mesh.Positions, mesh.TriangleIndices, mesh.TextureCoordinates, mesh.Normals, out vertNew, out triNew, out textureNew, out normalNew);
            var newMesh = new MeshGeometry3D() { Positions = vertNew, TriangleIndices = triNew, TextureCoordinates = textureNew, Normals = normalNew };
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
        public static void RemoveIsolatedVertices(IList<Point3D> vertices, IList<int> triangles, IList<Point> texture, IList<Vector3D> normals,
            out Point3DCollection verticesOut, out Int32Collection trianglesOut, out PointCollection textureOut, out Vector3DCollection normalOut)
        {
            verticesOut = null;
            trianglesOut = null;
            textureOut = null;
            normalOut = null;
            List<List<int>> tracking = new List<List<int>>(vertices.Count);
            Debug.WriteLine(string.Format("NumVert:{0}; NumTriangle:{1};", vertices.Count, triangles.Count));
            for (int i = 0; i < vertices.Count; ++i)
            {
                tracking.Add(new List<int>());
            }
            for (int i = 0; i < triangles.Count; ++i)
            {
                tracking[triangles[i]].Add(i);
            }

            List<int> vertToRemove = new List<int>(vertices.Count);
            for (int i = 0; i < vertices.Count; ++i)
            {
                if (tracking[i].Count == 0)
                {
                    vertToRemove.Add(i);
                }
            }
            verticesOut = new Point3DCollection(vertices.Count - vertToRemove.Count);
            trianglesOut = new Int32Collection(triangles);
            if (texture != null)
            {
                textureOut = new PointCollection(vertices.Count - vertToRemove.Count);
            }
            if (normals != null)
            {
                normalOut = new Vector3DCollection(vertices.Count - vertToRemove.Count);
            }
            if (vertices.Count == vertToRemove.Count)
            {
                return;
            }
            int counter = 0;
            for (int i = 0; i < vertices.Count; ++i)
            {
                if (counter == vertToRemove.Count || i < vertToRemove[counter])
                {
                    verticesOut.Add(vertices[i]);
                    if (texture != null)
                    {
                        textureOut.Add(texture[i]);
                    }
                    if (normals != null)
                    {
                        normalOut.Add(normals[i]);
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
            var removeOutOfRangeTriangles = new List<int>();
            for (int i = 0; i < triangles.Count; i += 3)
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
    }
}