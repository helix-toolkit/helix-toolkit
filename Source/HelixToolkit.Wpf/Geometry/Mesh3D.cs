// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mesh3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HelixToolkit.Wpf
{
    using System.Collections;

    /// <summary>
    /// Represents a 3D mesh for polygon models containing faces with any number of vertices.
    /// </summary>
    /// <remarks>
    /// Todo: should implement a better data structure.
    /// http://en.wikipedia.org/wiki/Winged_edge
    /// </remarks>
    public class Mesh3D : ICloneable
    {
        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        public IList<Point3D> Vertices { get; private set; }
        /// <summary>
        /// Gets or sets the faces.
        /// </summary>
        /// <value>The faces.</value>
        public IList<int[]> Faces { get; set; }
        /// <summary>
        /// Gets or sets the edges.
        /// </summary>
        /// <value>The edges.</value>
        public IList<int[]> Edges { get; private set; }

        /// <summary>
        /// Converts the mesh to a MeshGeometry3D.
        /// </summary>
        /// <param name="sharedVertices">Allow shared vertices (smooth shading) if set to <c>true</c>.</param>
        /// <param name="shrinkFactor">The shrink factor.</param>
        /// <param name="faceIndices">The face indices.</param>
        /// <returns></returns>
        public MeshGeometry3D ToMeshGeometry3D(bool sharedVertices = true, double shrinkFactor = 0.0, List<int> faceIndices = null)
        {
            if (shrinkFactor != 0)
                sharedVertices = false;

            if (!sharedVertices)
            {
                // not shared vertices - flat shading
                var tm = new MeshBuilder(false, false);
                int faceIndex = 0;
                foreach (var face in Faces)
                {
                    var vertices = new int[face.Length];
                    int j = 0;

                    var centroid = this.FindCentroid(faceIndex);

                    //var n = GetFaceNormal(faceIndex);
                    //for (int i = 0; i < face.Length; i++)
                    //    tm.Normals.Add(n);

                    foreach (int v in face)
                    {
                        vertices[j++] = tm.Positions.Count;
                        var vertex = Vertices[v];
                        if (shrinkFactor != 0)
                        {
                            vertex = vertex + (shrinkFactor * (centroid - vertex));
                        }
                        tm.Positions.Add(vertex);
                    }

                    tm.AddTriangleFan(vertices);
                    if (faceIndices != null)
                    {
                        int nTriangles = vertices.Length - 2;
                        for (int i = 0; i < nTriangles; i++)
                            faceIndices.Add(faceIndex);
                    }
                    faceIndex++;
                }
                return tm.ToMesh();
            }
            else
            {
                // shared vertices - smooth shading
                var tm = new MeshBuilder(false, false);
                foreach (var v in Vertices)
                {
                    tm.Positions.Add(v);
                }

                int faceIndex = 0;
                foreach (var face in Faces)
                {
                    tm.AddTriangleFan(face);
                    if (faceIndices != null)
                    {
                        int nTriangles = face.Length - 2;
                        for (int i = 0; i < nTriangles; i++)
                            faceIndices.Add(faceIndex);
                    }
                    faceIndex++;
                }
                return tm.ToMesh();
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh3D"/> class.
        /// </summary>
        public Mesh3D()
        {
            Vertices = new List<Point3D>();
            Faces = new List<int[]>();
            Edges = new List<int[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh3D"/> class.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <param name="triangleIndices">The triangle indices.</param>
        public Mesh3D(IEnumerable<Point3D> positions, IEnumerable<int> triangleIndices)
        {
            Vertices = new List<Point3D>(positions);
            Faces = new List<int[]>();
            Edges = new List<int[]>();
            int[] tri = new int[3];
            int i = 0;
            foreach (var index in triangleIndices)
            {
                tri[i++] = index;
                if (i == 3)
                {
                    this.AddFace(tri);
                    i = 0;
                    tri = new int[3];
                }
            }
            UpdateEdges();
        }

        /// <summary>
        /// Adds the face.
        /// </summary>
        /// <param name="v">The v.</param>
        public void AddFace(params int[] v)
        {
            Faces.Add(v);
        }

        /// <summary>
        /// Updates the edges.
        /// </summary>
        public void UpdateEdges()
        {
            var edges = new List<List<int>>(Vertices.Count);
            foreach (var v in Vertices)
                edges.Add(new List<int>(5));

            foreach (var f in Faces)
            {
                for (int i = 0; i < f.Length; i++)
                {
                    int v0 = f[i];
                    int v1 = f[(i + 1) % f.Length];
                    //int vmin = Math.Min(v0, v1);
                    //int vmax = Math.Max(v0, v1);
                    //edges[vmin].Add(vmax);
                    edges[v0].Add(v1);
                    edges[v1].Add(v0);
                }
            }

            Edges.Clear();
            foreach (var e in edges)
                Edges.Add(e.ToArray());
        }

        /// <summary>
        /// Gets the neighbour vertices.
        /// </summary>
        /// <param name="vertexIndex">Index of the vertex.</param>
        /// <returns></returns>
        public int[] GetNeighbourVertices(int vertexIndex)
        {
            return Edges[vertexIndex];
        }

        /// <summary>
        /// Gets the face normal (averaged).
        /// </summary>
        /// <param name="faceIndex">Index of the face.</param>
        /// <returns>The face normal.</returns>
        public Vector3D GetFaceNormal(int faceIndex)
        {
            int m = Faces[faceIndex].Length;
            double x = 0;
            double y = 0;
            double z = 0;
            for (int i = 0; i + 2 < m; i++)
            {
                var v0 = Vertices[Faces[faceIndex][i]];
                var v1 = Vertices[Faces[faceIndex][(i + 1) % m]];
                var v2 = Vertices[Faces[faceIndex][(i + 2) % m]];
                var n = Vector3D.CrossProduct(v1 - v0, v2 - v0);
                x += n.X;
                y += n.Y;
                z += n.Z;
            }
            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Finds the face from edge.
        /// </summary>
        /// <param name="v0">The v0.</param>
        /// <param name="v1">The v1.</param>
        /// <returns>The face index.</returns>
        public int FindFaceFromEdge(int v0, int v1)
        {
            for (int i = 0; i < Faces.Count; i++)
            {
                int m = Faces[i].Length;
                for (int j = 0; j < m; j++)
                {
                    if (Faces[i][j] == v0 && Faces[i][(j + 1) % m] == v1)
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines whether the mesh contains triangular faces only.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the mesh is triangular; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTriangularMesh()
        {
            foreach (var f in Faces)
                if (f.Length != 3)
                    return false;
            return true;
        }

        /// <summary>
        /// Determines whether the mesh contains quadrilateral faces only.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the mesh is quadrilateral; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQuadrilateralMesh()
        {
            foreach (var f in Faces)
                if (f.Length != 4)
                    return false;
            return true;
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns></returns>
        public Rect3D GetBounds()
        {
            var bounds = Rect3D.Empty;
            foreach (var v in Vertices)
                bounds.Union(v);
            return bounds;
        }

        /// <summary>
        /// Quadrangulates this instance.
        /// </summary>
        public void Quadrangulate()
        {
            int n = Faces.Count;
            for (int i = 0; i < n; i++)
            {
                if (Faces[i].Length == 4) continue;
                var c = FindCentroid(i);
                int ci = Vertices.Count;
                Vertices.Add(c);

                int m = Faces[i].Length;
                for (int j = 0; j < m; j++)
                {
                    var mp = FindMidpoint(Faces[i][j], Faces[i][(j + 1) % m]);
                    Vertices.Add(mp);
                }

                for (int j = 0; j < m; j++)
                {
                    var quad = new int[4];
                    quad[0] = ci + 1 + j;
                    quad[1] = Faces[i][(j + 1) % m];
                    quad[2] = ci + 1 + (j + 1) % m;
                    quad[3] = ci;
                    if (j == m - 1)
                    {
                        Faces[i] = quad;
                    }
                    else
                    {
                        Faces.Add(quad);
                    }
                }
            }
        }

        /// <summary>
        /// Triangulates the specified mesh.
        /// </summary>
        /// <param name="barycentric">Use barycentric subdivision if set to <c>true</c>.</param>
        public void Triangulate(bool barycentric)
        {
            int n = Faces.Count;
            for (int i = 0; i < n; i++)
            {
                if (Faces[i].Length == 3) continue;
                if (barycentric)
                {
                    var c = FindCentroid(i);
                    int ci = Vertices.Count;
                    Vertices.Add(c);
                    int m = Faces[i].Length;
                    for (int j = 0; j < m; j++)
                    {
                        var tri = new int[3];
                        tri[0] = Faces[i][j];
                        tri[1] = Faces[i][(j + 1) % m];
                        tri[2] = ci;
                        if (j == m - 1)
                        {
                            Faces[i] = tri;
                        }
                        else
                        {
                            Faces.Add(tri);
                        }
                    }
                }
                else
                {
                    int m = Faces[i].Length;
                    for (int j = 1; j + 1 < m; j++)
                    {
                        var tri = new int[3];
                        tri[0] = Faces[i][0];
                        tri[1] = Faces[i][(j) % m];
                        tri[2] = Faces[i][(j + 1) % m];
                        if (j + 1 == m - 1)
                        {
                            Faces[i] = tri;
                        }
                        else
                        {
                            Faces.Add(tri);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Finds the centroid of the specified face.
        /// </summary>
        /// <param name="faceIndex">Index of the face.</param>
        /// <returns>The centroid.</returns>
        public Point3D FindCentroid(int faceIndex)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            int n = Faces[faceIndex].Length;
            for (int i = 0; i < n; i++)
            {
                x += Vertices[Faces[faceIndex][i]].X;
                y += Vertices[Faces[faceIndex][i]].Y;
                z += Vertices[Faces[faceIndex][i]].Z;
            }
            if (n > 0)
            {
                x /= n;
                y /= n;
                z /= n;
            }
            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Finds the midpoint of the specified edge.
        /// </summary>
        /// <param name="v0">The first vertex index.</param>
        /// <param name="v1">The second vertex index.</param>
        /// <returns>The midpoint.</returns>
        private Point3D FindMidpoint(int v0, int v1)
        {
            return new Point3D(
                (Vertices[v0].X + Vertices[v1].X) * 0.5,
                (Vertices[v0].Y + Vertices[v1].Y) * 0.5,
                (Vertices[v0].Z + Vertices[v1].Z) * 0.5);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new Mesh3D();
            clone.Vertices = new List<Point3D>(this.Vertices);
            clone.Faces = new List<int[]>(this.Faces);
            clone.Edges = new List<int[]>(this.Edges);
            return clone;
        }
    }
}