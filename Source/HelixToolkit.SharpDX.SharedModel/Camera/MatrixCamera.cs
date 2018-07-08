/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.UI.Xaml;
using Vector3D = System.Numerics.Vector3;
using Point3D = System.Numerics.Vector3;
using Matrix3D = System.Numerics.Matrix4x4;
namespace HelixToolkit.UWP
#else
using System.Windows;
using System.Windows.Media.Media3D;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Cameras;
    public interface IMatrixCameraModel : ICameraModel
    {
        Matrix3D ProjectionMatrix { set; get; }
        Matrix3D ViewMatrix { set; get; }
    }
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
                    (d, e) =>
                    {
#if NETFX_CORE
                        ((d as Camera).CameraInternal as MatrixCameraCore).ProjectionMatrix = ((Matrix3D)e.NewValue);
#else
                        ((d as Camera).CameraInternal as MatrixCameraCore).ProjectionMatrix = ((Matrix3D)e.NewValue).ToMatrix();
#endif
                    }));

        /// <summary>
        /// The view matrix property
        /// </summary>
        public static readonly DependencyProperty ViewMatrixProperty = DependencyProperty.Register(
            "ViewMatrix", typeof(Matrix3D), typeof(MatrixCamera), new PropertyMetadata(Matrix3D.Identity,
                (d, e) =>
                {
#if NETFX_CORE
                    ((d as Camera).CameraInternal as MatrixCameraCore).ViewMatrix = ((Matrix3D)e.NewValue);
#else
                    ((d as Camera).CameraInternal as MatrixCameraCore).ViewMatrix = ((Matrix3D)e.NewValue).ToMatrix();
#endif
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

        protected override CameraCore CreatePortableCameraCore()
        {
            return new MatrixCameraCore()
            {
#if NETFX_CORE
                ProjectionMatrix = this.ProjectionMatrix,
                ViewMatrix = this.ViewMatrix
#else
                ProjectionMatrix = this.ProjectionMatrix.ToMatrix(),
                ViewMatrix = this.ViewMatrix.ToMatrix()
#endif
            };
        }

        public override Point3D Position
        {
            set; get;
        }

        public override Vector3D UpDirection
        {
            set; get;
        }

        public override Vector3D LookDirection
        {
            set; get;
        }

        public override bool CreateLeftHandSystem { set; get; }
    }
}