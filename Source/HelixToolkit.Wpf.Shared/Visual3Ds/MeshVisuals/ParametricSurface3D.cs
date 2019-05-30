// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParametricSurface3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a base class for parametric surfaces evaluated on a rectangular mesh.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a base class for parametric surfaces evaluated on a rectangular mesh.
    /// </summary>
    /// <remarks>
    /// Override the Evaluate method to define the points.
    /// </remarks>
    public abstract class ParametricSurface3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="MeshSizeU"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MeshSizeUProperty = DependencyProperty.Register(
            "MeshSizeU", typeof(int), typeof(ParametricSurface3D), new UIPropertyMetadata(120, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="MeshSizeV"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MeshSizeVProperty = DependencyProperty.Register(
            "MeshSizeV", typeof(int), typeof(ParametricSurface3D), new UIPropertyMetadata(120, GeometryChanged));

        /// <summary>
        /// Gets or sets the mesh size in u-direction.
        /// </summary>
        /// <value>The mesh size U.</value>
        public int MeshSizeU
        {
            get
            {
                return (int)this.GetValue(MeshSizeUProperty);
            }

            set
            {
                this.SetValue(MeshSizeUProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mesh size in v-direction.
        /// </summary>
        /// <value>The mesh size V.</value>
        public int MeshSizeV
        {
            get
            {
                return (int)this.GetValue(MeshSizeVProperty);
            }

            set
            {
                this.SetValue(MeshSizeVProperty, value);
            }
        }

        /// <summary>
        /// Evaluates the surface at the specified u,v parameters.
        /// </summary>
        /// <param name="u">
        /// The u parameter.
        /// </param>
        /// <param name="v">
        /// The v parameter.
        /// </param>
        /// <param name="textureCoord">
        /// The texture coordinates.
        /// </param>
        /// <returns>
        /// The evaluated <see cref="Point3D"/>.
        /// </returns>
        protected abstract Point3D Evaluate(double u, double v, out Point textureCoord);

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>A triangular mesh geometry.</returns>
        protected override MeshGeometry3D Tessellate()
        {
            var mesh = new MeshGeometry3D();

            int n = this.MeshSizeU;
            int m = this.MeshSizeV;
            var p = new Point3D[m * n];
            var tc = new Point[m * n];

            // todo: use MeshBuilder

            // todo: parallel execution...
            // Parallel.For(0, n, (i) =>
            for (int i = 0; i < n; i++)
            {
                double u = 1.0 * i / (n - 1);

                for (int j = 0; j < m; j++)
                {
                    double v = 1.0 * j / (m - 1);
                    int ij = (i * m) + j;
                    p[ij] = this.Evaluate(u, v, out tc[ij]);
                }
            }

            // );
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    mesh.Positions.Add(p[idx]);
                    mesh.TextureCoordinates.Add(tc[idx]);
                    idx++;
                }
            }

            for (int i = 0; i + 1 < n; i++)
            {
                for (int j = 0; j + 1 < m; j++)
                {
                    int x0 = i * m;
                    int x1 = (i + 1) * m;
                    int y0 = j;
                    int y1 = j + 1;
                    AddTriangle(mesh, x0 + y0, x0 + y1, x1 + y0);
                    AddTriangle(mesh, x1 + y0, x0 + y1, x1 + y1);
                }
            }

            return mesh;
        }

        /// <summary>
        /// The add triangle.
        /// </summary>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        /// <param name="i1">
        /// The i 1.
        /// </param>
        /// <param name="i2">
        /// The i 2.
        /// </param>
        /// <param name="i3">
        /// The i 3.
        /// </param>
        private static void AddTriangle(MeshGeometry3D mesh, int i1, int i2, int i3)
        {
            var p1 = mesh.Positions[i1];
            if (!IsDefined(p1))
            {
                return;
            }

            var p2 = mesh.Positions[i2];
            if (!IsDefined(p2))
            {
                return;
            }

            var p3 = mesh.Positions[i3];
            if (!IsDefined(p3))
            {
                return;
            }

            mesh.TriangleIndices.Add(i1);
            mesh.TriangleIndices.Add(i2);
            mesh.TriangleIndices.Add(i3);
        }

        /// <summary>
        /// Determines whether the specified point is defined.
        /// </summary>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <returns>
        /// The is defined.
        /// </returns>
        private static bool IsDefined(Point3D point)
        {
            return !double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z);
        }
    }
}