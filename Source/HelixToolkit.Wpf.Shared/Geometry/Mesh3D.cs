// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mesh3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a 3D mesh for polygon models containing faces with any number of vertices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a 3D mesh for polygon models containing faces with any number of vertices.
    /// </summary>
    public class Mesh3D : ICloneable
    {
        //// TODO: should implement a better data structure. http://en.wikipedia.org/wiki/Winged_edge

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh3D"/> class.
        /// </summary>
        public Mesh3D()
        {
            this.Vertices = new List<Point3D>();
            this.Faces = new List<int[]>();
            this.Edges = new List<int[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh3D"/> class.
        /// </summary>
        /// <param name="positions">
        /// The positions.
        /// </param>
        /// <param name="triangleIndices">
        /// The triangle indices.
        /// </param>
        public Mesh3D(IEnumerable<Point3D> positions, IEnumerable<int> triangleIndices)
            : this(positions, null, triangleIndices)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh3D"/> class.
        /// </summary>
        /// <param name="positions">
        /// The positions.
        /// </param>
        /// <param name="textureCoordinates">
        /// The texture Coordinates.
        /// </param>
        /// <param name="triangleIndices">
        /// The triangle indices.
        /// </param>
        public Mesh3D(IEnumerable<Point3D> positions, IEnumerable<Point> textureCoordinates, IEnumerable<int> triangleIndices)
        {
            this.Vertices = new List<Point3D>(positions);
            if (textureCoordinates != null)
            {
                this.TextureCoordinates = new List<Point>(textureCoordinates);
            }

            this.Faces = new List<int[]>();
            this.Edges = new List<int[]>();
            var tri = new int[3];
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

            this.UpdateEdges();
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value> The edges. </value>
        public IList<int[]> Edges { get; private set; }

        /// <summary>
        /// Gets the faces.
        /// </summary>
        /// <value> The faces. </value>
        public IList<int[]> Faces { get; private set; }

        /// <summary>
        /// Gets the texture coordinates.
        /// </summary>
        /// <value> The texture coordinates. </value>
        public IList<Point> TextureCoordinates { get; private set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value> The vertices. </value>
        public IList<Point3D> Vertices { get; private set; }

        /// <summary>
        /// Adds a face.
        /// </summary>
        /// <param name="vertexIndices">
        /// The vertex indices of the face.
        /// </param>
        public void AddFace(params int[] vertexIndices)
        {
            this.Faces.Add(vertexIndices);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new Mesh3D
                {
                    Vertices = new List<Point3D>(this.Vertices),
                    Faces = new List<int[]>(this.Faces),
                    Edges = new List<int[]>(this.Edges)
                };
        }

        /// <summary>
        /// Finds the centroid of the specified face.
        /// </summary>
        /// <param name="faceIndex">
        /// Index of the face.
        /// </param>
        /// <returns>
        /// The centroid.
        /// </returns>
        public Point3D FindCentroid(int faceIndex)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            int n = this.Faces[faceIndex].Length;
            for (int i = 0; i < n; i++)
            {
                x += this.Vertices[this.Faces[faceIndex][i]].X;
                y += this.Vertices[this.Faces[faceIndex][i]].Y;
                z += this.Vertices[this.Faces[faceIndex][i]].Z;
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
        /// Finds the face from edge.
        /// </summary>
        /// <param name="v0">
        /// The v0.
        /// </param>
        /// <param name="v1">
        /// The v1.
        /// </param>
        /// <returns>
        /// The face index.
        /// </returns>
        public int FindFaceFromEdge(int v0, int v1)
        {
            for (int i = 0; i < this.Faces.Count; i++)
            {
                int m = this.Faces[i].Length;
                for (int j = 0; j < m; j++)
                {
                    if (this.Faces[i][j] == v0 && this.Faces[i][(j + 1) % m] == v1)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns>
        /// The bounds.
        /// </returns>
        public Rect3D GetBounds()
        {
            var bounds = Rect3D.Empty;
            foreach (var v in this.Vertices)
            {
                bounds.Union(v);
            }

            return bounds;
        }

        /// <summary>
        /// Gets the face normal (averaged).
        /// </summary>
        /// <param name="faceIndex">
        /// Index of the face.
        /// </param>
        /// <returns>
        /// The face normal.
        /// </returns>
        public Vector3D GetFaceNormal(int faceIndex)
        {
            int m = this.Faces[faceIndex].Length;
            double x = 0;
            double y = 0;
            double z = 0;
            for (int i = 0; i + 2 < m; i++)
            {
                var v0 = this.Vertices[this.Faces[faceIndex][i]];
                var v1 = this.Vertices[this.Faces[faceIndex][(i + 1) % m]];
                var v2 = this.Vertices[this.Faces[faceIndex][(i + 2) % m]];
                var n = Vector3D.CrossProduct(v1 - v0, v2 - v0);
                x += n.X;
                y += n.Y;
                z += n.Z;
            }

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Gets the neighbour vertices.
        /// </summary>
        /// <param name="vertexIndex">
        /// Index of the vertex.
        /// </param>
        /// <returns>
        /// The neighbour vertices.
        /// </returns>
        public int[] GetNeighbourVertices(int vertexIndex)
        {
            return this.Edges[vertexIndex];
        }

        /// <summary>
        /// Determines whether the mesh contains quadrilateral faces only.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the mesh is quadrilateral; otherwise, <c>false</c> .
        /// </returns>
        public bool IsQuadrilateralMesh()
        {
            foreach (var f in this.Faces)
            {
                if (f.Length != 4)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the mesh contains triangular faces only.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the mesh is triangular; otherwise, <c>false</c> .
        /// </returns>
        public bool IsTriangularMesh()
        {
            foreach (var f in this.Faces)
            {
                if (f.Length != 3)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the mesh to a mesh of quadrilaterals.
        /// </summary>
        public void Quadrangulate()
        {
            int n = this.Faces.Count;
            for (int i = 0; i < n; i++)
            {
                if (this.Faces[i].Length == 4)
                {
                    continue;
                }

                var c = this.FindCentroid(i);
                int ci = this.Vertices.Count;
                this.Vertices.Add(c);

                int m = this.Faces[i].Length;
                for (int j = 0; j < m; j++)
                {
                    var mp = this.FindMidpoint(this.Faces[i][j], this.Faces[i][(j + 1) % m]);
                    this.Vertices.Add(mp);
                }

                for (int j = 0; j < m; j++)
                {
                    var quad = new int[4];
                    quad[0] = ci + 1 + j;
                    quad[1] = this.Faces[i][(j + 1) % m];
                    quad[2] = ci + 1 + ((j + 1) % m);
                    quad[3] = ci;
                    if (j == m - 1)
                    {
                        this.Faces[i] = quad;
                    }
                    else
                    {
                        this.Faces.Add(quad);
                    }
                }
            }
        }

        /// <summary>
        /// Converts the mesh to a MeshGeometry3D.
        /// </summary>
        /// <param name="sharedVertices">
        /// Allow shared vertices (smooth shading) if set to <c>true</c> .
        /// </param>
        /// <param name="shrinkFactor">
        /// The shrink factor.
        /// </param>
        /// <param name="faceIndices">
        /// The face indices.
        /// </param>
        /// <returns>
        /// A mesh geometry.
        /// </returns>
        public MeshGeometry3D ToMeshGeometry3D(
            bool sharedVertices = true, double shrinkFactor = 0.0, List<int> faceIndices = null)
        {
            bool shrink = Math.Abs(shrinkFactor) > double.Epsilon;

            if (!sharedVertices || shrink)
            {
                // not shared vertices - flat shading
                var tm = new MeshBuilder(false, this.TextureCoordinates != null);
                int faceIndex = 0;
                foreach (var face in this.Faces)
                {
                    var vertices = new int[face.Length];
                    int j = 0;

                    var centroid = this.FindCentroid(faceIndex);

                    // var n = GetFaceNormal(faceIndex);
                    // for (int i = 0; i < face.Length; i++)
                    // tm.Normals.Add(n);
                    foreach (int v in face)
                    {
                        vertices[j++] = tm.Positions.Count;
                        var vertex = this.Vertices[v];
                        if (shrink)
                        {
                            vertex = vertex + (shrinkFactor * (centroid - vertex));
                        }

                        tm.Positions.Add(vertex);
                        if (tm.CreateTextureCoordinates)
                        {
                            tm.TextureCoordinates.Add(this.TextureCoordinates[v]);
                        }
                    }

                    tm.AddTriangleFan(vertices);
                    if (faceIndices != null)
                    {
                        int numberOfTriangles = vertices.Length - 2;
                        for (int i = 0; i < numberOfTriangles; i++)
                        {
                            faceIndices.Add(faceIndex);
                        }
                    }

                    faceIndex++;
                }

                return tm.ToMesh();
            }
            else
            {
                // shared vertices - smooth shading
                var tm = new MeshBuilder(false, this.TextureCoordinates != null);
                foreach (var v in this.Vertices)
                {
                    tm.Positions.Add(v);
                }

                if (this.TextureCoordinates != null)
                {
                    foreach (var uv in this.TextureCoordinates)
                    {
                        tm.TextureCoordinates.Add(uv);
                    }
                }

                int faceIndex = 0;
                foreach (var face in this.Faces)
                {
                    tm.AddTriangleFan(face);
                    if (faceIndices != null)
                    {
                        int numberOfTriangles = face.Length - 2;
                        for (int i = 0; i < numberOfTriangles; i++)
                        {
                            faceIndices.Add(faceIndex);
                        }
                    }

                    faceIndex++;
                }

                return tm.ToMesh();
            }
        }

        /// <summary>
        /// Triangulates the specified mesh.
        /// </summary>
        /// <param name="barycentric">
        /// Use barycentric subdivision if set to <c>true</c> .
        /// </param>
        public void Triangulate(bool barycentric)
        {
            int n = this.Faces.Count;
            for (int i = 0; i < n; i++)
            {
                if (this.Faces[i].Length == 3)
                {
                    continue;
                }

                if (barycentric)
                {
                    var c = this.FindCentroid(i);
                    int ci = this.Vertices.Count;
                    this.Vertices.Add(c);
                    int m = this.Faces[i].Length;
                    for (int j = 0; j < m; j++)
                    {
                        var tri = new int[3];
                        tri[0] = this.Faces[i][j];
                        tri[1] = this.Faces[i][(j + 1) % m];
                        tri[2] = ci;
                        if (j == m - 1)
                        {
                            this.Faces[i] = tri;
                        }
                        else
                        {
                            this.Faces.Add(tri);
                        }
                    }
                }
                else
                {
                    int m = this.Faces[i].Length;
                    for (int j = 1; j + 1 < m; j++)
                    {
                        var tri = new int[3];
                        tri[0] = this.Faces[i][0];
                        tri[1] = this.Faces[i][j % m];
                        tri[2] = this.Faces[i][(j + 1) % m];
                        if (j + 1 == m - 1)
                        {
                            this.Faces[i] = tri;
                        }
                        else
                        {
                            this.Faces.Add(tri);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the edges.
        /// </summary>
        public void UpdateEdges()
        {
            var edges = new List<List<int>>(this.Vertices.Count);
            foreach (var v in this.Vertices)
            {
                edges.Add(new List<int>(5));
            }

            foreach (var f in this.Faces)
            {
                for (int i = 0; i < f.Length; i++)
                {
                    int v0 = f[i];
                    int v1 = f[(i + 1) % f.Length];

                    // int vmin = Math.Min(v0, v1);
                    // int vmax = Math.Max(v0, v1);
                    // edges[vmin].Add(vmax);
                    edges[v0].Add(v1);
                    edges[v1].Add(v0);
                }
            }

            this.Edges.Clear();
            foreach (var e in edges)
            {
                this.Edges.Add(e.ToArray());
            }
        }

        /// <summary>
        /// Finds the midpoint of the specified edge.
        /// </summary>
        /// <param name="v0">
        /// The first vertex index.
        /// </param>
        /// <param name="v1">
        /// The second vertex index.
        /// </param>
        /// <returns>
        /// The midpoint.
        /// </returns>
        private Point3D FindMidpoint(int v0, int v1)
        {
            return new Point3D(
                (this.Vertices[v0].X + this.Vertices[v1].X) * 0.5,
                (this.Vertices[v0].Y + this.Vertices[v1].Y) * 0.5,
                (this.Vertices[v0].Z + this.Vertices[v1].Z) * 0.5);
        }
    }
}