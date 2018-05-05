// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatrixCamera.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Camera which specifies the view and projection transforms as Matrix3D objects
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Cameras;

    /// <summary>
    /// Camera which specifies the view and projection transforms as Matrix3D objects
    /// </summary>
    public class MatrixCamera : Camera
    {
        /// <summary>
        /// The projection matrix property
        /// </summary>
        public static readonly DependencyProperty ProjectionMatrixProperty =
            DependencyProperty.Register(
                "ProjectionMatrix", typeof(Matrix3D), typeof(MatrixCamera), new PropertyMetadata(Matrix3D.Identity, 
                    (d,e)=> 
                    {
                        ((d as Camera).CameraInternal as MatrixCameraCore).ProjectionMatrix = ((Matrix3D)e.NewValue).ToMatrix();
                    }));

        /// <summary>
        /// The view matrix property
        /// </summary>
        public static readonly DependencyProperty ViewMatrixProperty = DependencyProperty.Register(
            "ViewMatrix", typeof(Matrix3D), typeof(MatrixCamera), new PropertyMetadata(Matrix3D.Identity,
                (d, e) =>
                {
                    ((d as Camera).CameraInternal as MatrixCameraCore).ViewMatrix = ((Matrix3D)e.NewValue).ToMatrix();
                }));

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixCamera" /> class.
        /// </summary>
        public MatrixCamera()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixCamera"/> class.
        /// </summary>
        /// <param name="viewMatrix">
        /// The view matrix.
        /// </param>
        /// <param name="projectionMatrix">
        /// The projection matrix.
        /// </param>
        public MatrixCamera(Matrix3D viewMatrix, Matrix3D projectionMatrix)
        {
            this.ViewMatrix = viewMatrix;
            this.ProjectionMatrix = projectionMatrix;
        }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>
        /// The projection matrix.
        /// </value>
        public Matrix3D ProjectionMatrix
        {
            get { return (Matrix3D)this.GetValue(ProjectionMatrixProperty); }
            set { this.SetValue(ProjectionMatrixProperty, value); }
        }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        /// <value>
        /// The view matrix.
        /// </value>
        public Matrix3D ViewMatrix
        {
            get { return (Matrix3D)this.GetValue(ViewMatrixProperty); }
            set { this.SetValue(ViewMatrixProperty, value); }
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new MatrixCamera();
        }

        protected override void OnChanged()
        {
            base.OnChanged();
        }

        protected override CameraCore CreatePortableCameraCore()
        {
            return new MatrixCameraCore() { ProjectionMatrix = this.ProjectionMatrix.ToMatrix(), ViewMatrix = this.ViewMatrix.ToMatrix() };
        }

        public override Point3D Position
        {
            set;get;
        }

        public override Vector3D UpDirection
        {
            set;get;
        }

        public override Vector3D LookDirection
        {
            set;get;
        }
    }
}