using HelixToolkit;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HalfEdgeMesh;

/// <summary>
/// A visual element that shows HalfEdgeMesh structures.
/// </summary>
/// <remarks>
/// This element is not optimized for performance.
/// </remarks>
public class HalfEdgeMeshVisual3D : ModelVisual3D
{
    /// <summary>
    /// The adjacent material property.
    /// </summary>
    public static readonly DependencyProperty AdjacentMaterialProperty =
        DependencyProperty.Register(
            "AdjacentMaterial",
            typeof(Material),
            typeof(HalfEdgeMeshVisual3D),
            new UIPropertyMetadata(Materials.Green));

    /// <summary>
    /// The connected material property.
    /// </summary>
    public static readonly DependencyProperty ConnectedMaterialProperty =
        DependencyProperty.Register(
            "ConnectedMaterial",
            typeof(Material),
            typeof(HalfEdgeMeshVisual3D),
            new UIPropertyMetadata(Materials.Red));

    /// <summary>
    /// The edge diameter property.
    /// </summary>
    public static readonly DependencyProperty EdgeDiameterProperty = DependencyProperty.Register(
        "EdgeDiameter", typeof(double), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(0.03, MeshChanged));

    /// <summary>
    /// The edge material property.
    /// </summary>
    public static readonly DependencyProperty EdgeMaterialProperty = DependencyProperty.Register(
        "EdgeMaterial", typeof(Material), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(Materials.White));

    /// <summary>
    /// The face back material property.
    /// </summary>
    public static readonly DependencyProperty FaceBackMaterialProperty =
        DependencyProperty.Register(
            "FaceBackMaterial",
            typeof(Material),
            typeof(HalfEdgeMeshVisual3D),
            new UIPropertyMetadata(Materials.Gray));

    /// <summary>
    /// The face material property.
    /// </summary>
    public static readonly DependencyProperty FaceMaterialProperty = DependencyProperty.Register(
        "FaceMaterial", typeof(Material), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(Materials.Blue));

    /// <summary>
    /// The mesh property.
    /// </summary>
    public static readonly DependencyProperty MeshProperty = DependencyProperty.Register(
        "Mesh", typeof(HalfEdgeMesh), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(null, MeshChanged));

    /// <summary>
    /// The selected material property.
    /// </summary>
    public static readonly DependencyProperty SelectedMaterialProperty =
        DependencyProperty.Register(
            "SelectedMaterial",
            typeof(Material),
            typeof(HalfEdgeMeshVisual3D),
            new UIPropertyMetadata(Materials.Yellow));

    /// <summary>
    /// The shared vertices property.
    /// </summary>
    public static readonly DependencyProperty SharedVerticesProperty = DependencyProperty.Register(
        "SharedVertices", typeof(bool), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(false, MeshChanged));

    /// <summary>
    /// The shrink factor property.
    /// </summary>
    public static readonly DependencyProperty ShrinkFactorProperty = DependencyProperty.Register(
        "ShrinkFactor", typeof(double), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(0.1, MeshChanged));

    /// <summary>
    /// The unselected material property.
    /// </summary>
    public static readonly DependencyProperty UnselectedMaterialProperty =
        DependencyProperty.Register(
            "UnselectedMaterial",
            typeof(Material),
            typeof(HalfEdgeMeshVisual3D),
            new UIPropertyMetadata(Materials.Gray));

    /// <summary>
    /// The vertex material property.
    /// </summary>
    public static readonly DependencyProperty VertexMaterialProperty = DependencyProperty.Register(
        "VertexMaterial", typeof(Material), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(Materials.Gold));

    /// <summary>
    /// The vertex radius property.
    /// </summary>
    public static readonly DependencyProperty VertexRadiusProperty = DependencyProperty.Register(
        "VertexRadius", typeof(double), typeof(HalfEdgeMeshVisual3D), new UIPropertyMetadata(0.05, MeshChanged));

    /// <summary>
    /// The face to face visual map.
    /// </summary>
    private Dictionary<HalfEdgeMesh.Face, ModelUIElement3D>? faceVisuals;

    /// <summary>
    /// The half edge to half edge visual map.
    /// </summary>
    private Dictionary<HalfEdgeMesh.HalfEdge, ModelUIElement3D>? halfEdgeVisuals;

    /// <summary>
    /// The vertex to vertex visual map.
    /// </summary>
    private Dictionary<HalfEdgeMesh.Vertex, ModelUIElement3D>? vertexVisuals;

    /// <summary>
    /// Gets or sets AdjacentMaterial.
    /// </summary>
    public Material AdjacentMaterial
    {
        get
        {
            return (Material)this.GetValue(AdjacentMaterialProperty);
        }

        set
        {
            this.SetValue(AdjacentMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets ConnectedMaterial.
    /// </summary>
    public Material ConnectedMaterial
    {
        get
        {
            return (Material)this.GetValue(ConnectedMaterialProperty);
        }

        set
        {
            this.SetValue(ConnectedMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the edge diameter.
    /// </summary>
    /// <value> The edge diameter. </value>
    public double EdgeDiameter
    {
        get
        {
            return (double)this.GetValue(EdgeDiameterProperty);
        }

        set
        {
            this.SetValue(EdgeDiameterProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the edge material.
    /// </summary>
    /// <value> The edge material. </value>
    public Material EdgeMaterial
    {
        get
        {
            return (Material)this.GetValue(EdgeMaterialProperty);
        }

        set
        {
            this.SetValue(EdgeMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the face back material.
    /// </summary>
    /// <value> The face back material. </value>
    public Material FaceBackMaterial
    {
        get
        {
            return (Material)this.GetValue(FaceBackMaterialProperty);
        }

        set
        {
            this.SetValue(FaceBackMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the face material.
    /// </summary>
    /// <value> The face material. </value>
    public Material FaceMaterial
    {
        get
        {
            return (Material)this.GetValue(FaceMaterialProperty);
        }

        set
        {
            this.SetValue(FaceMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the mesh.
    /// </summary>
    /// <value> The mesh. </value>
    public HalfEdgeMesh Mesh
    {
        get
        {
            return (HalfEdgeMesh)this.GetValue(MeshProperty);
        }

        set
        {
            this.SetValue(MeshProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets SelectedMaterial.
    /// </summary>
    public Material SelectedMaterial
    {
        get
        {
            return (Material)this.GetValue(SelectedMaterialProperty);
        }

        set
        {
            this.SetValue(SelectedMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to share vertices (smooth shading).
    /// </summary>
    /// <value> <c>true</c> if vertices are shared; otherwise, <c>false</c> . </value>
    public bool SharedVertices
    {
        get
        {
            return (bool)this.GetValue(SharedVerticesProperty);
        }

        set
        {
            this.SetValue(SharedVerticesProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the shrink factor.
    /// </summary>
    /// <value> The shrink factor. </value>
    public double ShrinkFactor
    {
        get
        {
            return (double)this.GetValue(ShrinkFactorProperty);
        }

        set
        {
            this.SetValue(ShrinkFactorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets UnselectedMaterial.
    /// </summary>
    public Material UnselectedMaterial
    {
        get
        {
            return (Material)this.GetValue(UnselectedMaterialProperty);
        }

        set
        {
            this.SetValue(UnselectedMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the vertex material.
    /// </summary>
    /// <value> The vertex material. </value>
    public Material VertexMaterial
    {
        get
        {
            return (Material)this.GetValue(VertexMaterialProperty);
        }

        set
        {
            this.SetValue(VertexMaterialProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the vertex radius.
    /// </summary>
    /// <value> The vertex radius. </value>
    public double VertexRadius
    {
        get
        {
            return (double)this.GetValue(VertexRadiusProperty);
        }

        set
        {
            this.SetValue(VertexRadiusProperty, value);
        }
    }

    /// <summary>
    /// Finds the centroid of the specified face.
    /// </summary>
    /// <param name="vertices">
    /// The vertices.
    /// </param>
    /// <returns>
    /// The centroid.
    /// </returns>
    public Point3D FindCentroid(IList<Point3D> vertices)
    {
        double x = 0;
        double y = 0;
        double z = 0;
        int n = vertices.Count;
        for (int i = 0; i < n; i++)
        {
            x += vertices[i].X;
            y += vertices[i].Y;
            z += vertices[i].Z;
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
    /// The mesh changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void MeshChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((HalfEdgeMeshVisual3D)obj).UpdateVisuals();
    }

    /// <summary>
    /// Updates the visuals.
    /// </summary>
    protected void UpdateVisuals()
    {
        this.vertexVisuals = new Dictionary<HalfEdgeMesh.Vertex, ModelUIElement3D>();
        this.halfEdgeVisuals = new Dictionary<HalfEdgeMesh.HalfEdge, ModelUIElement3D>();
        this.faceVisuals = new Dictionary<HalfEdgeMesh.Face, ModelUIElement3D>();
        this.Children.Clear();
        if (this.Mesh == null)
        {
            return;
        }

        if (this.VertexRadius > 0)
        {
            // Add the vertices
            foreach (var vertex in this.Mesh.Vertices)
            {
                var gm = new MeshBuilder(false, false);
                gm.AddSubdivisionSphere(vertex.Position.ToVector(), (float)this.VertexRadius, 4);
                var vertexElement = new ModelUIElement3D
                {
                    Model = new GeometryModel3D(gm.ToMesh().ToMeshGeometry3D(), this.VertexMaterial)
                };
                var currentVertex = vertex;
                vertexElement.MouseLeftButtonDown += (s, e) => this.HighlightVertex(currentVertex);
                this.vertexVisuals.Add(vertex, vertexElement);
                this.Add(vertexElement);
            }
        }

        var faceCenter = new Dictionary<HalfEdgeMesh.Face, Point3D>();

        foreach (var face in this.Mesh.Faces)
        {
            var faceVertices = face.Vertices.Select(v => v.Position).ToList();

            // Find the face centroid
            var center = this.FindCentroid(faceVertices);
            faceCenter.Add(face, center);

            if (this.ShrinkFactor < 1)
            {
                // Add the faces
                for (int i = 0; i < faceVertices.Count; i++)
                {
                    faceVertices[i] += (center - faceVertices[i]) * this.ShrinkFactor;
                }

                var gm = new MeshBuilder(false, false);
                gm.AddTriangleFan(faceVertices.Select(t => t.ToVector()).ToList());
                var faceElement = new ModelUIElement3D
                {
                    Model =
                            new GeometryModel3D(gm.ToMesh().ToMeshGeometry3D(), this.FaceMaterial)
                            {
                                BackMaterial = this.FaceBackMaterial
                            }
                };
                var currentFace = face;
                faceElement.MouseLeftButtonDown += (s, e) => this.HighlightFace(currentFace);
                this.faceVisuals.Add(face, faceElement);
                this.Add(faceElement);
            }
        }

        if (this.EdgeDiameter > 0)
        {
            // Add the edges
            foreach (var edge in this.Mesh.Edges)
            {
                var start = edge.StartVertex.Position;
                var end = edge.EndVertex.Position;
                var center = faceCenter[edge.Face];
                start += (center - start) * this.ShrinkFactor;
                end += (center - end) * this.ShrinkFactor;
                var gm = new MeshBuilder(false, false);
                gm.AddArrow(start.ToVector(), end.ToVector(), (float)this.EdgeDiameter);
                var edgeElement = new ModelUIElement3D
                {
                    Model = new GeometryModel3D(gm.ToMesh().ToMeshGeometry3D(), this.EdgeMaterial)
                };
                var currentEdge = edge;
                edgeElement.MouseLeftButtonDown += (s, e) => { this.HighlightEdge(currentEdge); };
                this.halfEdgeVisuals.Add(edge, edgeElement);
                this.Add(edgeElement);
            }
        }
    }

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    private void Add(ModelUIElement3D element)
    {
        // http://social.msdn.microsoft.com/Forums/en/wpf/thread/7ac92a5a-001f-443a-a549-60cd5bb13083
        this.Children.Add(element);
        element.Visibility = Visibility.Hidden;
        this.Dispatcher.BeginInvoke(
            DispatcherPriority.Send, new ThreadStart(delegate { element.Visibility = Visibility.Visible; }));
    }

    /// <summary>
    /// The highlight edge.
    /// </summary>
    /// <param name="edge">
    /// The edge.
    /// </param>
    private void HighlightEdge(HalfEdgeMesh.HalfEdge edge)
    {
        foreach (var f in this.Mesh.Faces)
        {
            var element = this.faceVisuals![f];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (edge.Face == f)
            {
                model.Material = this.ConnectedMaterial;
            }
            else if (edge.AdjacentFace == f)
            {
                model.Material = this.AdjacentMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }

        foreach (var v in this.Mesh.Vertices)
        {
            var element = this.vertexVisuals![v];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (v == edge.StartVertex)
            {
                model.Material = Materials.Black;
            }
            else if (v == edge.EndVertex)
            {
                model.Material = Materials.White;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }

        foreach (var e in this.Mesh.Edges)
        {
            var element = this.halfEdgeVisuals![e];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (e == edge)
            {
                model.Material = this.SelectedMaterial;
            }
            else if (e == edge.AdjacentEdge)
            {
                model.Material = this.AdjacentMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }
    }

    /// <summary>
    /// The highlight face.
    /// </summary>
    /// <param name="face">
    /// The face.
    /// </param>
    private void HighlightFace(HalfEdgeMesh.Face face)
    {
        var adjacentFaces = new HashSet<HalfEdgeMesh.Face>(face.AdjacentFaces);
        var adjacentVertices = new HashSet<HalfEdgeMesh.Vertex>(face.Vertices);
        var edges = new HashSet<HalfEdgeMesh.HalfEdge>(face.Edges);
        foreach (var f in this.Mesh.Faces)
        {
            var element = this.faceVisuals![f];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (f == face)
            {
                model.Material = this.SelectedMaterial;
            }
            else if (adjacentFaces.Contains(f))
            {
                model.Material = this.AdjacentMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }

        foreach (var v in this.Mesh.Vertices)
        {
            var element = this.vertexVisuals![v];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (adjacentVertices.Contains(v))
            {
                model.Material = this.AdjacentMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }

        foreach (var e in this.Mesh.Edges)
        {
            var element = this.halfEdgeVisuals![e];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (edges.Contains(e))
            {
                model.Material = this.ConnectedMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }
    }

    /// <summary>
    /// The highlight vertex.
    /// </summary>
    /// <param name="vertex">
    /// The vertex.
    /// </param>
    private void HighlightVertex(HalfEdgeMesh.Vertex vertex)
    {
        var adjacentFaces = new HashSet<HalfEdgeMesh.Face>(vertex.AdjacentFaces);
        var adjacentVertices = new HashSet<HalfEdgeMesh.Vertex>(vertex.Vertices);
        var incomingEdges = new HashSet<HalfEdgeMesh.HalfEdge>(vertex.IncomingEdges);
        var outgoingEdges = new HashSet<HalfEdgeMesh.HalfEdge>(vertex.OutgoingEdges);

        foreach (var f in this.Mesh.Faces)
        {
            var element = this.faceVisuals![f];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (adjacentFaces.Contains(f))
            {
                model.Material = this.AdjacentMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }

        foreach (var v in this.Mesh.Vertices)
        {
            var element = this.vertexVisuals![v];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (v == vertex)
            {
                model.Material = this.SelectedMaterial;
            }
            else if (adjacentVertices.Contains(v))
            {
                model.Material = this.AdjacentMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }

        foreach (var e in this.Mesh.Edges)
        {
            var element = this.halfEdgeVisuals![e];

            if (element.Model is not GeometryModel3D model)
            {
                continue;
            }

            if (incomingEdges.Contains(e))
            {
                model.Material = this.AdjacentMaterial;
            }
            else if (outgoingEdges.Contains(e))
            {
                model.Material = this.ConnectedMaterial;
            }
            else
            {
                model.Material = this.UnselectedMaterial;
            }
        }
    }
}
