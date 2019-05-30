// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoopSubdivision.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Subdivision scheme.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Subdivision scheme.
    /// </summary>
    public enum SubdivisionScheme
    {
        /// <summary>
        /// Loop's original scheme
        /// </summary>
        Loop,

        /// <summary>
        /// Loop subdivision with Warren's weights
        /// </summary>
        Warren
    }

    /// <summary>
    /// Builds a subdivision surface from a triangular mesh.
    /// </summary>
    /// <remarks>
    /// <para>
    /// http:///en.wikipedia.org/wiki/Subdivision_surface
    /// http:///en.wikipedia.org/wiki/Loop_subdivision_surface
    /// http:///research.microsoft.com/~cloop/thesis.pdf
    /// http:///www.dgp.toronto.edu/people/stam/reality/Research/pdf/loop.pdf
    /// http:///research.microsoft.com/en-us/um/people/cloop/
    /// </para>
    /// <para>
    /// This code is based on a matlab program "loopSubdivision.m" by Jesús P. Mena-Chalco.
    /// http:///www.mathworks.com.au/matlabcentral/fileexchange/24942-loop-subdivision
    /// </para>
    /// <para>
    /// Copyright (c) 2009, Jesus Mena
    /// All rights reserved.
    /// </para>
    /// <para>
    /// Redistribution and use in source and binary forms, with or without
    /// modification, are permitted provided that the following conditions are
    /// met:
    /// </para>
    /// <para>
    /// * Redistributions of source code must retain the above copyright
    /// notice, this list of conditions and the following disclaimer.
    /// * Redistributions in binary form must reproduce the above copyright
    /// notice, this list of conditions and the following disclaimer in
    /// the documentation and/or other materials provided with the distribution
    /// </para>
    /// <para>
    /// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    /// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    /// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
    /// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    /// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
    /// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
    /// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
    /// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
    /// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    /// POSSIBILITY OF SUCH DAMAGE.
    /// </para>
    /// </remarks>
    public class LoopSubdivision
    {
        /// <summary>
        /// The vertices.
        /// </summary>
        private IList<Vector3D> vertices;

        /// <summary>
        /// The triangle indices.
        /// </summary>
        private IList<int> triangleIndices;

        /// <summary>
        /// The new vertices (temoporary list).
        /// </summary>
        private IList<Vector3D> newVertices;

        /// <summary>
        /// The new triangle indices (temporary list).
        /// </summary>
        private IList<int> newTriangleIndices;

        /// <summary>
        /// Gets or sets the subdivision scheme.
        /// </summary>
        /// <value>The scheme.</value>
        public SubdivisionScheme Scheme { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopSubdivision"/> class.
        /// </summary>
        /// <param name="vertices">
        /// The vertices.
        /// </param>
        /// <param name="triangleIndices">
        /// The triangle indices.
        /// </param>
        public LoopSubdivision(IList<Point3D> vertices, IList<int> triangleIndices)
        {
            this.Scheme = SubdivisionScheme.Loop;

            // Convert points to vectors
            this.vertices = new List<Vector3D>(vertices.Select(v => v.ToVector3D()));

            this.triangleIndices = triangleIndices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopSubdivision"/> class.
        /// </summary>
        /// <param name="meshGeometry">
        /// The mesh geometry.
        /// </param>
        public LoopSubdivision(MeshGeometry3D meshGeometry)
            : this(meshGeometry.Positions, meshGeometry.TriangleIndices)
        {
        }

        /// <summary>
        /// Creates a mesh geometry.
        /// </summary>
        /// <returns>
        /// A MeshGeometry3D instance.
        /// </returns>
        public MeshGeometry3D ToMeshGeometry3D()
        {
            return new MeshGeometry3D
                {
                    Positions = new Point3DCollection(this.Positions),
                    TriangleIndices = new Int32Collection(this.triangleIndices)
                };
        }

        /// <summary>
        /// Gets the positions.
        /// </summary>
        /// <value>The positions.</value>
        public IList<Point3D> Positions
        {
            get
            {
                // Converts vectors to points
                return new List<Point3D>(this.vertices.Select(v => v.ToPoint3D()));
            }
        }

        /// <summary>
        /// Creates a mesh.
        /// </summary>
        /// <returns>
        /// A Mesh3D instance.
        /// </returns>
        public Mesh3D ToMesh3D()
        {
            return new Mesh3D(this.Positions, this.triangleIndices);
        }

        // Original vertices: va, vb, vc, vd.
        // New vertices: vp, vq, vr.
        // vb                  vb
        // |  \                 |  \
        // |    \              vp----vq
        // |      \             | \ / \
        // va ---- vc   ->     va--vr--vc
        // |      /             |     /
        // |    /               |   /
        // |  /                 | /
        // vd                  vd

        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="v0">
        /// The first vertex index.
        /// </param>
        /// <param name="v1">
        /// The second vertex index.
        /// </param>
        /// <param name="v2">
        /// The third vertex index.
        /// </param>
        public void Add(int v0, int v1, int v2)
        {
            this.newTriangleIndices.Add(v0);
            this.newTriangleIndices.Add(v1);
            this.newTriangleIndices.Add(v2);
        }

#if METHOD1
        int[,,] edgeVertice;

        int getEdgeVertice(int v0, int v1, int i)
        {
            return edgeVertice[v0, v1, i];
        }

        void setEdgeVertice(int v0, int v1, int i, int value)
        {
            edgeVertice[v0, v1, i]=value;
        }
#endif
#if !METHOD2

        /// <summary>
        /// The edge vertice.
        /// </summary>
        private Dictionary<int, int[]>[] edgeVertice;

        /// <summary>
        /// Gets an edge vertice.
        /// </summary>
        /// <param name="v0">
        /// The first vertex index.
        /// </param>
        /// <param name="v1">
        /// The second vertex index.
        /// </param>
        /// <param name="i">
        /// 0: index of the new vertex between (x,y)
        /// 1: index of the first opposite vertex between (x,y)
        /// 2: index of the second opposite vertex between (x,y)
        /// </param>
        /// <returns>
        /// The vertex index.
        /// </returns>
        private int GetEdgeVertice(int v0, int v1, int i)
        {
            if (this.edgeVertice[v0].ContainsKey(v1))
            {
                return this.edgeVertice[v0][v1][i];
            }

            return 0;
        }

        /// <summary>
        /// Set an edge vertice.
        /// </summary>
        /// <param name="v0">
        /// The first vertex index.
        /// </param>
        /// <param name="v1">
        /// The second vertex index.
        /// </param>
        /// <param name="i">
        /// 0: index of the new vertex between (x,y)
        /// 1: index of the first opposite vertex between (x,y)
        /// 2: index of the second opposite vertex between (x,y)
        /// </param>
        /// <param name="value">
        /// The vertex index.
        /// </param>
        private void SetEdgeVertice(int v0, int v1, int i, int value)
        {
            if (!this.edgeVertice[v0].ContainsKey(v1))
            {
                this.edgeVertice[v0][v1] = new int[3];
            }

            this.edgeVertice[v0][v1][i] = value;
        }

#endif

#if METHOD3
        Dictionary<long, int[]> edgeVertice = new Dictionary<long, int[]>();

        int getEdgeVertice(int v0, int v1, int i)
        {
            long l = v0;
            l = l << 32;
            l += v1;
            if (!edgeVertice.ContainsKey(l))
                return 0;
            return edgeVertice[l][i];
        }

        void setEdgeVertice(int v0, int v1, int i, int value)
        {
            long l = v0;
            l = l << 32;
            l += v1;
            if (!edgeVertice.ContainsKey(l))
                edgeVertice[l] = new int[3];
            edgeVertice[l][i] = value;
        }
#endif
#if METHOD4
        DoubleKeyDictionary<int, int, int[]> edgeVertice = new DoubleKeyDictionary<int, int, int[]>();
        int getEdgeVertice(int v0, int v1, int i)
        {
            if (!edgeVertice.ContainsKey(v0, v1))
                return 0;
            return edgeVertice[v0, v1][i];
        }

        void setEdgeVertice(int v0, int v1, int i, int value)
        {
            if (!edgeVertice.ContainsKey(v0, v1))
                edgeVertice[v0, v1] = new int[3];
            edgeVertice[v0, v1][i] = value;
        }
#endif

        /// <summary>
        /// Sums the specified vectors.
        /// </summary>
        /// <param name="indices">
        /// The indices of the vectors.
        /// </param>
        /// <returns>
        /// The sum.
        /// </returns>
        private Vector3D Sum(IEnumerable<int> indices)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            foreach (var i in indices)
            {
                x += this.vertices[i].X;
                y += this.vertices[i].Y;
                z += this.vertices[i].Z;
            }

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Squares the specified double.
        /// </summary>
        /// <param name="d">
        /// The double.
        /// </param>
        /// <returns>
        /// The square.
        /// </returns>
        public double Sqr(double d)
        {
            return d * d;
        }

        /// <summary>
        /// Subdivides this instance n times.
        /// </summary>
        /// <param name="n">
        /// The number of subdivisions.
        /// </param>
        public void Subdivide(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.Subdivide();
            }
        }

        /// <summary>
        /// Subdivides this instance.
        /// </summary>
        public void Subdivide()
        {
            // TODO: check the following implementation - faster?
            // http:///www.mathworks.com/matlabcentral/fileexchange/32727-fast-loop-mesh-subdivision

            this.newVertices = new List<Vector3D>(this.vertices);
            this.newTriangleIndices = new List<int>();
            int nVertices = this.vertices.Count;
            int nFaces = this.triangleIndices.Count / 3;

#if !METHOD2
            this.edgeVertice = new Dictionary<int, int[]>[nVertices];
            for (int i = 0; i < nVertices; i++)
            {
                this.edgeVertice[i] = new Dictionary<int, int[]>();
            }

#endif
#if METHODx
            edgeVertice = new int[nVertices, nVertices, 3];

            // edgeVertice.Clear();
#endif

            for (int i = 0; i < nFaces; i++)
            {
                int vaIndex = this.triangleIndices[i * 3];
                int vbIndex = this.triangleIndices[i * 3 + 1];
                int vcIndex = this.triangleIndices[i * 3 + 2];

                int vpIndex = this.AddEdgeVertice(vaIndex, vbIndex, vcIndex);
                int vqIndex = this.AddEdgeVertice(vbIndex, vcIndex, vaIndex);
                int vrIndex = this.AddEdgeVertice(vaIndex, vcIndex, vbIndex);

                this.Add(vaIndex, vpIndex, vrIndex);
                this.Add(vpIndex, vbIndex, vqIndex);
                this.Add(vrIndex, vqIndex, vcIndex);
                this.Add(vrIndex, vpIndex, vqIndex);
            }

            // positions of the new vertices
            for (int v1 = 0; v1 < nVertices - 1; v1++)
            {
                for (int v2 = v1; v2 < nVertices; v2++)
                {
                    int vNIndex = this.GetEdgeVertice(v1, v2, 0);
                    if (vNIndex != 0)
                    {
                        int vNOpposite1Index = this.GetEdgeVertice(v1, v2, 1);
                        int vNOpposite2Index = this.GetEdgeVertice(v1, v2, 2);

                        if (vNOpposite2Index == 0)
                        {
                            // boundary case
                            this.newVertices[vNIndex] = 0.5 * (this.vertices[v1] + this.vertices[v2]);
                        }
                        else
                        {
                            this.newVertices[vNIndex] = 3.0 / 8 * (this.vertices[v1] + this.vertices[v2])
                                                        +
                                                        (1.0 / 8)
                                                        *
                                                        (this.vertices[vNOpposite1Index]
                                                         + this.vertices[vNOpposite2Index]);
                        }
                    }
                }
            }

            // adjacent vertices
            var adjVertice = new List<int>[nVertices];

            for (int v = 0; v < nVertices; v++)
            {
                adjVertice[v] = new List<int>();
                for (int vTmp = 0; vTmp < nVertices; vTmp++)
                {
                    if ((v < vTmp && this.GetEdgeVertice(v, vTmp, 0) != 0)
                        || (v > vTmp && this.GetEdgeVertice(vTmp, v, 0) != 0))
                    {
                        adjVertice[v].Add(vTmp);
                    }
                }
            }

            // new positions of the original vertices
            for (int v = 0; v < nVertices; v++)
            {
                int k = adjVertice[v].Count;

                var adjBoundaryVertices = new List<int>();
                for (int i = 0; i < k; i++)
                {
                    int vi = adjVertice[v][i];
                    if ((vi > v) && (this.GetEdgeVertice(v, vi, 2) == 0)
                        || (vi < v) && (this.GetEdgeVertice(vi, v, 2) == 0))
                    {
                        adjBoundaryVertices.Add(vi);
                    }
                }

                if (adjBoundaryVertices.Count == 2)
                {
                    // boundary case
                    this.newVertices[v] = 6.0 / 8 * this.vertices[v] + 1.0 / 8 * this.Sum(adjBoundaryVertices);
                }
                else
                {
                    double beta;

                    switch (this.Scheme)
                    {
                        case SubdivisionScheme.Warren:
                            beta = k > 3 ? 3.0 / 8 / k : 3.0 / 16;
                            break;

                        default:
                            beta = 1.0 / k * (5.0 / 8 - this.Sqr(3.0 / 8 + 1 / 4 * Math.Cos(2 * Math.PI / k)));
                            break;
                    }

                    this.newVertices[v] = (1 - k * beta) * this.vertices[v] + beta * this.Sum(adjVertice[v]);
                }
            }

            this.vertices = this.newVertices;
            this.triangleIndices = this.newTriangleIndices;
        }

        /// <summary>
        /// Adds an edge vertex.
        /// </summary>
        /// <param name="v1Index">
        /// The first vertex index.
        /// </param>
        /// <param name="v2Index">
        /// The second vertex index.
        /// </param>
        /// <param name="v3Index">
        /// The third vertex index.
        /// </param>
        /// <returns>
        /// The added edge vertex index.
        /// </returns>
        private int AddEdgeVertice(int v1Index, int v2Index, int v3Index)
        {
            if (v1Index > v2Index)
            {
                int vTmp = v1Index;
                v1Index = v2Index;
                v2Index = vTmp;
            }

            if (this.GetEdgeVertice(v1Index, v2Index, 0) == 0)
            {
                this.SetEdgeVertice(v1Index, v2Index, 0, this.newVertices.Count);
                this.SetEdgeVertice(v1Index, v2Index, 1, v3Index);
                this.newVertices.Add(new Vector3D());
            }
            else
            {
                this.SetEdgeVertice(v1Index, v2Index, 2, v3Index);
            }

            return this.GetEdgeVertice(v1Index, v2Index, 0);
        }
    }
}