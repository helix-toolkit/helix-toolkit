// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that can be bound to a MeshGeometry3D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that can be bound to a <see cref="MeshGeometry3D"/>.
    /// </summary>
    public class MeshGeometryVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="MeshGeometry"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("MeshGeometry", typeof(MeshGeometry3D), typeof(MeshGeometryVisual3D), new PropertyMetadata(null, GeometryChanged));

        /// <summary>
        /// Gets or sets the <see cref="MeshGeometry3D"/> defining the shape of the visual.
        /// </summary>
        public MeshGeometry3D MeshGeometry
        {
            get { return (MeshGeometry3D)this.GetValue(GeometryProperty); }
            set { this.SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>
        /// A triangular mesh geometry.
        /// </returns>
        protected override MeshGeometry3D Tessellate()
        {
            return this.MeshGeometry;
        }
    }
}