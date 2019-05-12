// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows Mesh3D meshes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows Mesh3D meshes.
    /// </summary>
    public class MeshVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="EdgeDiameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgeDiameterProperty = DependencyProperty.Register(
            "EdgeDiameter", typeof(double), typeof(MeshVisual3D), new UIPropertyMetadata(0.03, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="EdgeMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgeMaterialProperty = DependencyProperty.Register(
            "EdgeMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Gray));

        /// <summary>
        /// Identifies the <see cref="FaceBackMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FaceBackMaterialProperty =
            DependencyProperty.Register(
                "FaceBackMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Gray));

        /// <summary>
        /// Identifies the <see cref="FaceMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FaceMaterialProperty = DependencyProperty.Register(
            "FaceMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Blue));

        /// <summary>
        /// Identifies the <see cref="Mesh"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MeshProperty = DependencyProperty.Register(
            "Mesh", typeof(Mesh3D), typeof(MeshVisual3D), new UIPropertyMetadata(null, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="SharedVertices"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SharedVerticesProperty = DependencyProperty.Register(
            "SharedVertices", typeof(bool), typeof(MeshVisual3D), new UIPropertyMetadata(false, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="ShrinkFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShrinkFactorProperty = DependencyProperty.Register(
            "ShrinkFactor", typeof(double), typeof(MeshVisual3D), new UIPropertyMetadata(0.0, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="VertexMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VertexMaterialProperty = DependencyProperty.Register(
            "VertexMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Gold));

        /// <summary>
        /// Identifies the <see cref="VertexRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VertexRadiusProperty = DependencyProperty.Register(
            "VertexRadius", typeof(double), typeof(MeshVisual3D), new UIPropertyMetadata(0.05, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="VertexResolution"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VertexResolutionProperty =
            DependencyProperty.Register(
                "VertexResolution", typeof(int), typeof(MeshVisual3D), new UIPropertyMetadata(2));

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
        public Mesh3D Mesh
        {
            get
            {
                return (Mesh3D)this.GetValue(MeshProperty);
            }

            set
            {
                this.SetValue(MeshProperty, value);
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
        /// Gets or sets the mapping from triangle index to face index.
        /// </summary>
        /// <value> The index mapping. </value>
        public List<int> TriangleIndexToFaceIndex { get; set; }

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
        /// Gets or sets the vertex resolution (number of subdivisions).
        /// </summary>
        /// <value> The vertex resolution. </value>
        public int VertexResolution
        {
            get
            {
                return (int)this.GetValue(VertexResolutionProperty);
            }

            set
            {
                this.SetValue(VertexResolutionProperty, value);
            }
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
            ((MeshVisual3D)obj).UpdateVisuals();
        }

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        protected void UpdateVisuals()
        {
            if (this.Mesh == null)
            {
                this.Content = null;
                return;
            }

            var m = new Model3DGroup();

            this.TriangleIndexToFaceIndex = new List<int>();
            var faceGeometry = this.Mesh.ToMeshGeometry3D(
                this.SharedVertices, this.ShrinkFactor, this.TriangleIndexToFaceIndex);
            m.Children.Add(
                new GeometryModel3D(faceGeometry, this.FaceMaterial) { BackMaterial = this.FaceBackMaterial });

            // Add the nodes
            if (this.VertexRadius > 0)
            {
                var gm = new MeshBuilder(false, false);
                foreach (var p in this.Mesh.Vertices)
                {
                    gm.AddSubdivisionSphere(p, this.VertexRadius, this.VertexResolution);

                    // gm.AddBox(p, VertexRadius, VertexRadius, VertexRadius);
                }

                m.Children.Add(new GeometryModel3D(gm.ToMesh(), this.VertexMaterial));
            }

            // Add the edges
            if (this.EdgeDiameter > 0)
            {
                var em = new MeshBuilder(false, false);
                //// int fi = 0;
                foreach (var p in this.Mesh.Faces)
                {
                    //// var n = this.Mesh.GetFaceNormal(fi++);
                    for (int i = 0; i < p.Length; i += 1)
                    {
                        var p0 = this.Mesh.Vertices[p[i]];
                        var p1 = this.Mesh.Vertices[p[(i + 1) % p.Length]];
                        em.AddCylinder(p0, p1, this.EdgeDiameter, 4);
                    }
                }

                m.Children.Add(new GeometryModel3D(em.ToMesh(), this.EdgeMaterial));
            }

            this.Content = m;
        }

    }
}