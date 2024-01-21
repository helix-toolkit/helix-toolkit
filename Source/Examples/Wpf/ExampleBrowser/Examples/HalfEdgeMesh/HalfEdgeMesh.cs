using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System;
using HelixToolkit;

namespace HalfEdgeMesh;

/// <summary>
/// Represent a manifold mesh by a halfedge data structure.
/// </summary>
/// <remarks>
/// See http://en.wikipedia.org/wiki/Polygon_mesh http://www.dgp.toronto.edu/~alexk/lydos.html http://openmesh.org http://sharp3d.codeplex.com http://www.cs.sunysb.edu/~gu/software/MeshLib/index.html http://www.flipcode.com/archives/The_Half-Edge_Data_Structure.shtml http://www.cgal.org/Manual/latest/doc_html/cgal_manual/HalfedgeDS/Chapter_main.html http://algorithmicbotany.org/papers/smithco.dis2006.pdf http://www.cs.mtu.edu/~shene/COURSES/cs3621/SLIDES/Mesh.pdf http://mrl.nyu.edu/~dzorin/ig04/lecture24/meshes.pdf http://www.hao-li.com/teaching/surfaceRepresentationAndGeometricModeling/OpenMeshTutorial.pdf http://www.cs.rpi.edu/~cutler/classes/advancedgraphics/S09/lectures/02_Adjacency_Data_Structures.pdf
/// </remarks>
public sealed class HalfEdgeMesh
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HalfEdgeMesh" /> class.
    /// </summary>
    public HalfEdgeMesh()
    {
        this.Vertices = new List<Vertex>();
        this.Edges = new List<HalfEdge>();
        this.Faces = new List<Face>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HalfEdgeMesh"/> class.
    /// </summary>
    /// <param name="vertices">
    /// The vertices.
    /// </param>
    /// <param name="triangleIndices">
    /// The triangle indices.
    /// </param>
    public HalfEdgeMesh(IList<Point3D> vertices, IList<int>? triangleIndices = null)
        : this()
    {
        // Add each vertex to the Vertices collection
        for (int i = 0; i < vertices.Count; i++)
        {
            this.Vertices.Add(new Vertex { Position = vertices[i], Index = i });
        }

        if (triangleIndices != null)
        {
            // Add each triangle face and update the halfedge structures
            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                this.AddFace(triangleIndices[i], triangleIndices[i + 1], triangleIndices[i + 2]);
            }
        }
    }

    /// <summary>
    /// Gets or sets the edges.
    /// </summary>
    /// <value> The edges. </value>
    public IList<HalfEdge> Edges { get; set; }

    /// <summary>
    /// Gets or sets the faces.
    /// </summary>
    /// <value> The faces. </value>
    public IList<Face> Faces { get; set; }

    /// <summary>
    /// Gets or sets the vertices.
    /// </summary>
    /// <value> The vertices. </value>
    public IList<Vertex> Vertices { get; set; }

    /// <summary>
    /// Adds the face.
    /// </summary>
    /// <param name="indices">
    /// The indices.
    /// </param>
    /// <returns>
    /// The face.
    /// </returns>
    public Face AddFace(params int[] indices)
    {
        int n = indices.Length;
        var faceVertices = indices.Select(i => this.Vertices[i]).ToList();
        var face = new Face { Index = this.Faces.Count };
        var faceEdges = new HalfEdge[n];

        // Create the halfedges for the face
        for (int j = 0; j < n; j++)
        {
            faceEdges[j] = new HalfEdge
            {
                StartVertex = faceVertices[j],
                EndVertex = faceVertices[(j + 1) % n],
                Face = face,
                Index = this.Edges.Count
            };
            this.Edges.Add(faceEdges[j]);
        }

        // Set the NextEdge properties
        for (int j = 0; j < n; j++)
        {
            faceEdges[j].NextEdge = faceEdges[(j + 1) % n];
        }

        for (int j = 0; j < n; j++)
        {
            var startVertex = faceVertices[j];
            var endVertex = faceVertices[(j + 1) % n];
            if (endVertex.FirstIncomingEdge == null)
            {
                // This is the first incoming edge to this vertex
                endVertex.FirstIncomingEdge = faceEdges[j];
            }
            else
            {
                // todo: this needs to be fixed - I have just been trying to get the right structure in this first prototype
                // The vertex has been used by before, check if any of the edges are adjacent
                foreach (var e in this.Edges)
                {
                    if (e == faceEdges[j])
                    {
                        continue;
                    }

                    if (e.StartVertex == startVertex && e.EndVertex == endVertex)
                    {
                        throw new InvalidOperationException("Edge already used.");
                    }

                    if (e.StartVertex == endVertex && e.EndVertex == startVertex)
                    {
                        e.AdjacentEdge = faceEdges[j];
                        faceEdges[j].AdjacentEdge = e;
                        break;
                    }
                }

                // for (int k = 0; k < n; k++)
                // {
                // var v0 = faceVertices[(j + k) % n];
                // var v1 = faceVertices[(j + k + 1) % n];
                // if (startVertex == v0 && endVertex == v1)
                // {
                // throw new InvalidOperationException("Edge already defined.");
                // }

                // if (endVertex == v0 && startVertex == v1)
                // {
                // // Set the AdjacentEdge property
                // endVertex.FirstIncomingEdge.AdjacentEdge = faceEdges[(j + k) % n];
                // faceEdges[(j + k) % n].AdjacentEdge = endVertex.FirstIncomingEdge;
                // }
                // }
            }
        }

        // Add the first edge to the face
        face.Edge = faceEdges[0];

        // Add the face to the faces collection
        this.Faces.Add(face);

        return face;
    }

    /// <summary>
    /// Gets the faces.
    /// </summary>
    /// <returns>
    /// The faces.
    /// </returns>
    public IEnumerable<HalfEdge> GetFaces()
    {
        var isEdgeVisited = new bool[this.Edges.Count];
        for (int i = 0; i < this.Edges.Count; i++)
        {
            if (!isEdgeVisited[i])
            {
                yield return this.Edges[i];
            }

            foreach (var e in this.Edges[i].Face.Edges)
            {
                int j = this.Edges.IndexOf(e);
                isEdgeVisited[j] = true;
            }
        }
    }

    /// <summary>
    /// Create a MeshGeometry3D.
    /// </summary>
    /// <returns>
    /// A MeshGeometry3D.
    /// </returns>
    public MeshGeometry3D ToWndMeshGeometry3D()
    {
        return new MeshGeometry3D
        {
            Positions = new Point3DCollection(this.Vertices.Select(v => v.Position)),
            TriangleIndices = new Int32Collection(this.Triangulate())
        };
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        this.Vertices.Select((v, i) => v.Index = i).ToList();
        this.Edges.Select((e, i) => e.Index = i).ToList();
        this.Faces.Select((f, i) => f.Index = i).ToList();
        var builder = new StringBuilder();
        foreach (var v in this.Vertices)
        {
            builder.AppendLine(v.ToString());
        }

        foreach (var v in this.Edges)
        {
            builder.AppendLine(v.ToString());
        }

        foreach (var v in this.Faces)
        {
            builder.AppendLine(v.ToString());
        }

        return builder.ToString();
    }

    /// <summary>
    /// Gets the triangle indices.
    /// </summary>
    /// <returns>
    /// The triangle indices.
    /// </returns>
    public IEnumerable<int> Triangulate()
    {
        return from face in this.Faces from v in face.Triangulate() select v.Index;
    }

    /// <summary>
    /// Represents a face.
    /// </summary>
    public class Face
    {
        /// <summary>
        /// Gets the adjacent faces.
        /// </summary>
        /// <value> The adjacent faces. </value>
        public IEnumerable<Face> AdjacentFaces
        {
            get
            {
                return this.Edges.Select(edge => edge.AdjacentFace).Where(adjacentFace => adjacentFace != null)!;
            }
        }

        /// <summary>
        /// Gets or sets the first edge of the face.
        /// </summary>
        /// <value> The edge. </value>
        public HalfEdge? Edge { get; set; }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value> The edges. </value>
        public IEnumerable<HalfEdge> Edges
        {
            get
            {
                var edge = this.Edge;
                do
                {
                    if (edge is not null)
                    {
                        yield return edge;
                    }

                    edge = edge?.NextEdge;
                }
                while (edge != this.Edge);
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value> The index. </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value> The tag. </value>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value> The vertices. </value>
        public IEnumerable<Vertex> Vertices
        {
            get
            {
                return this.Edges.Select(e => e.EndVertex);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "f{0}: {1} | {2}",
                this.Index,
                this.Vertices.Select(v => v.Index).EnumerateToString("v"),
                this.Edges.Select(e => e.Index).EnumerateToString("e"));
        }

        /// <summary>
        /// Triangulates this face.
        /// </summary>
        /// <returns>
        /// Triangulated vertices.
        /// </returns>
        public IEnumerable<Vertex> Triangulate()
        {
            var v = this.Vertices.ToList();
            for (int i = 1; i + 1 < v.Count; i++)
            {
                yield return v[0];
                yield return v[i];
                yield return v[i + 1];
            }
        }

    }

    /// <summary>
    /// Represents a half edge.
    /// </summary>
    public class HalfEdge
    {
        /// <summary>
        /// Gets or sets the adjacent edge.
        /// </summary>
        /// <value> The adjacent edge. </value>
        public HalfEdge? AdjacentEdge { get; set; }

        /// <summary>
        /// Gets the adjacent face.
        /// </summary>
        /// <value> The adjacent face. </value>
        public Face? AdjacentFace
        {
            get
            {
                return this.AdjacentEdge?.Face;
            }
        }

        /// <summary>
        /// Gets or sets the end vertex.
        /// </summary>
        /// <value> The end vertex. </value>
        public required Vertex EndVertex { get; set; }

        /// <summary>
        /// Gets or sets the face.
        /// </summary>
        /// <value> The face. </value>
        public required Face Face { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value> The index. </value>
        public required int Index { get; set; }

        /// <summary>
        /// Gets or sets the next edge.
        /// </summary>
        /// <value> The next edge. </value>
        public HalfEdge? NextEdge { get; set; }

        /// <summary>
        /// Gets or sets the start vertex.
        /// </summary>
        /// <value> The start vertex. </value>
        public required Vertex StartVertex { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value> The tag. </value>
        public object? Tag { get; set; }

        // public Vertex StartVertex
        // {
        // get
        // {
        // if (AdjacentEdge != null) return AdjacentEdge.EndVertex;

        // var edge = this;
        // do
        // {
        // if (edge.NextEdge == this) return edge.EndVertex;
        // edge = edge.NextEdge;
        // } while (edge != this);

        // return null;
        // }
        // }
        /// <summary>
        /// Checks if the halfedge is on the boundary of the mesh.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the halfedge is on the boundary; otherwise, <c>false</c> .
        /// </returns>
        public bool IsOnBoundary()
        {
            return this.AdjacentEdge == null;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "e{0}: v{1}->v{2} ae{3} f{4} af{5}",
                this.Index,
                this.StartVertex.Index,
                this.EndVertex.Index,
                this.AdjacentEdge != null ? this.AdjacentEdge.Index.ToString(CultureInfo.InvariantCulture) : "-",
                this.Face.Index,
                this.AdjacentFace != null ? this.AdjacentFace.Index.ToString(CultureInfo.InvariantCulture) : "-");
        }

    }

    /// <summary>
    /// Represents a vertex.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Gets the adjacent faces.
        /// </summary>
        /// <value> The adjacent faces. </value>
        public IEnumerable<Face> AdjacentFaces => this.OutgoingEdges.Select(e => e.Face);

        /// <summary>
        /// Gets or sets the first incoming edge.
        /// </summary>
        /// <value> The first incoming edge. </value>
        public HalfEdge? FirstIncomingEdge { get; set; }

        /// <summary>
        /// Gets the incoming halfedges.
        /// </summary>
        /// <value> The incoming edges. </value>
        public IEnumerable<HalfEdge> IncomingEdges
        {
            get
            {
                var edge = this.FirstIncomingEdge;
                do
                {
                    if (edge is not null)
                    {
                        yield return edge;
                    }

                    edge = edge?.NextEdge?.AdjacentEdge;
                }
                while (edge != this.FirstIncomingEdge && edge != null);
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value> The index. </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets the halfedges originating from the vertex.
        /// </summary>
        public IEnumerable<HalfEdge> OutgoingEdges => this.IncomingEdges.Where(e => e.AdjacentEdge != null).Select(e => e.AdjacentEdge!);

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value> The position. </value>
        public Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value> The tag. </value>
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value> The value. </value>
        public double Value { get; set; }

        /// <summary>
        /// Gets the vertices in the one ring neighborhood.
        /// </summary>
        public IEnumerable<Vertex> Vertices => this.OutgoingEdges.Select(h => h.EndVertex);

        /// <summary>
        /// Determines whether the vertex is on the boundary.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the vertex is on the boundary; otherwise, <c>false</c> .
        /// </returns>
        public bool IsOnBoundary()
        {
            if (this.FirstIncomingEdge == null)
            {
                return true;
            }

            return this.OutgoingEdges.Any(edge => edge.IsOnBoundary());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "v{0}: {1} | {2} | {3} | {4}",
                this.Index,
                this.FirstIncomingEdge?.Face.Edges.Select(e => e.Index).EnumerateToString("e"),
                this.IncomingEdges.Select(e => e.Index).EnumerateToString("ie"),
                this.OutgoingEdges.Select(e => e.Index).EnumerateToString("oe"),
                this.AdjacentFaces.Select(f => f.Index).EnumerateToString("af"));
        }
    }
}
