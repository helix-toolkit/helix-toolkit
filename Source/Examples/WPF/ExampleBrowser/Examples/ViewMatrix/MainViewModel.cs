// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewMatrixDemo
{
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    using PropertyTools;

    public class MainViewModel : Observable
    {
        private Matrix3D projectionMatrix;
        private Matrix3D viewMatrix;
        private Matrix3D viewportMatrix;

        public MainViewModel()
        {
            var gm = new MeshBuilder();
            gm.AddBox(new Point3D(0, 0, 0.5), 1, 1, 1);
            gm.AddCylinder(new Point3D(5, 0, 0), new Point3D(5, 0, 5), 1, 36);
            this.Model = new GeometryModel3D(gm.ToMesh(true), Materials.Blue);
            this.Model.Freeze();
        }

        public Model3D Model { get; set; }

        public Transform3D ProjectionTransform
        {
            get
            {
                var m = Matrix3D.Identity;
                m.Append(this.projectionMatrix);
                m.Append(this.viewMatrix);
                return new MatrixTransform3D(m);
            }
        }

        public Transform3D ViewTransform
        {
            get
            {
                var m = Matrix3D.Identity;
                m.Append(this.projectionMatrix);
                return new MatrixTransform3D(m);
            }
        }

        public Transform3D ViewportTransform
        {
            get
            {
                var m = Matrix3D.Identity;
                m.Append(this.viewportMatrix);
                m.Append(this.projectionMatrix);
                m.Append(this.viewMatrix);
                return new MatrixTransform3D(m);
            }
        }

        public Matrix3D ProjectionMatrix
        {
            get
            {
                return this.projectionMatrix;
            }

            set
            {
                this.SetValue(ref this.projectionMatrix, value, nameof(this.ProjectionMatrix));
                this.RaisePropertyChanged(nameof(this.ProjectionTransform));
            }
        }

        public Matrix3D ViewMatrix
        {
            get
            {
                return this.viewMatrix;
            }

            set
            {
                this.SetValue(ref this.viewMatrix, value, nameof(this.ViewMatrix));
                this.RaisePropertyChanged(nameof(this.ViewTransform));
            }
        }

        public Matrix3D ViewportMatrix
        {
            get
            {
                return this.viewportMatrix;
            }

            set
            {
                this.SetValue(ref this.viewportMatrix, value, nameof(this.ViewportMatrix));
                this.RaisePropertyChanged(nameof(this.ViewportTransform));
            }
        }
    }
}