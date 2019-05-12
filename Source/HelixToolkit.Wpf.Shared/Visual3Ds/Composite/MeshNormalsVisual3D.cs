// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshNormalsVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows the normals of the specified mesh geometry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows the normals of the specified mesh geometry.
    /// </summary>
    public class MeshNormalsVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(MeshNormalsVisual3D), new UIPropertyMetadata(Colors.Blue, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(MeshNormalsVisual3D), new UIPropertyMetadata(0.1, MeshChanged));

        /// <summary>
        /// Identifies the <see cref="Mesh"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MeshProperty = DependencyProperty.Register(
            "Mesh", typeof(MeshGeometry3D), typeof(MeshNormalsVisual3D), new UIPropertyMetadata(null, MeshChanged));

        /// <summary>
        /// Gets or sets the color of the normals.
        /// </summary>
        /// <value> The color. </value>
        public Color Color
        {
            get
            {
                return (Color)this.GetValue(ColorProperty);
            }

            set
            {
                this.SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the diameter of the normal arrows.
        /// </summary>
        /// <value> The diameter. </value>
        public double Diameter
        {
            get
            {
                return (double)this.GetValue(DiameterProperty);
            }

            set
            {
                this.SetValue(DiameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mesh.
        /// </summary>
        /// <value> The mesh. </value>
        public MeshGeometry3D Mesh
        {
            get
            {
                return (MeshGeometry3D)this.GetValue(MeshProperty);
            }

            set
            {
                this.SetValue(MeshProperty, value);
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
            ((MeshNormalsVisual3D)obj).OnMeshChanged();
        }

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        protected virtual void OnMeshChanged()
        {
            this.Children.Clear();

            var builder = new MeshBuilder();
            for (int i = 0; i < this.Mesh.Positions.Count; i++)
            {
                builder.AddArrow(
                    this.Mesh.Positions[i], this.Mesh.Positions[i] + this.Mesh.Normals[i], this.Diameter, 3, 10);
            }

            this.Content = new GeometryModel3D
                {
                   Geometry = builder.ToMesh(true), Material = MaterialHelper.CreateMaterial(this.Color)
                };
        }

    }
}