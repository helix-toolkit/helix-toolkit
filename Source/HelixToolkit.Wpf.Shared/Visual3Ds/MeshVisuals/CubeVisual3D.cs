// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CubeVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that displays a cube.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that displays a cube.
    /// </summary>
    public class CubeVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(CubeVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="SideLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SideLengthProperty = DependencyProperty.Register(
            "SideLength", typeof(double), typeof(CubeVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Gets or sets the center of the cube.
        /// </summary>
        /// <value>The center.</value>
        public Point3D Center
        {
            get
            {
                return (Point3D)this.GetValue(CenterProperty);
            }

            set
            {
                this.SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the cube sides.
        /// </summary>
        /// <value>The length of the sides.</value>
        public double SideLength
        {
            get
            {
                return (double)this.GetValue(SideLengthProperty);
            }

            set
            {
                this.SetValue(SideLengthProperty, value);
            }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>The mesh geometry.</returns>
        protected override MeshGeometry3D Tessellate()
        {
            var b = new MeshBuilder(false, true);
            b.AddCubeFace(
                this.Center, 
                new Vector3D(-1, 0, 0), 
                new Vector3D(0, 0, 1), 
                this.SideLength, 
                this.SideLength, 
                this.SideLength);
            b.AddCubeFace(
                this.Center, 
                new Vector3D(1, 0, 0), 
                new Vector3D(0, 0, -1), 
                this.SideLength, 
                this.SideLength, 
                this.SideLength);
            b.AddCubeFace(
                this.Center, 
                new Vector3D(0, -1, 0), 
                new Vector3D(0, 0, 1), 
                this.SideLength, 
                this.SideLength, 
                this.SideLength);
            b.AddCubeFace(
                this.Center, 
                new Vector3D(0, 1, 0), 
                new Vector3D(0, 0, -1), 
                this.SideLength, 
                this.SideLength, 
                this.SideLength);
            b.AddCubeFace(
                this.Center, 
                new Vector3D(0, 0, 1), 
                new Vector3D(0, -1, 0), 
                this.SideLength, 
                this.SideLength, 
                this.SideLength);
            b.AddCubeFace(
                this.Center, 
                new Vector3D(0, 0, -1), 
                new Vector3D(0, 1, 0), 
                this.SideLength, 
                this.SideLength, 
                this.SideLength);

            return b.ToMesh();
        }
    }
}