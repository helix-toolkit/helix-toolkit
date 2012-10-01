namespace HelixToolkit.SharpDX
{
    using System.Windows;

    using global::SharpDX;

    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;

    public abstract class ProjectionCamera : Camera
    {
        protected ProjectionCamera()
        {
            this.NearPlaneDistance = 1e-2;
            this.FarPlaneDistance = 1e3;
        }

        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point3D), typeof(ProjectionCamera));

        public Vector3D LookDirection { get; set; }
        public Vector3D UpDirection { get; set; }
        public double NearPlaneDistance { get; set; }
        public double FarPlaneDistance { get; set; }
        public bool CreateLeftHandSystem { get; set; }

        public override Matrix CreateViewMatrix()
        {
            if (this.CreateLeftHandSystem)
            {
                return Matrix.LookAtLH(this.Position, this.Position + this.LookDirection, this.UpDirection);
            }

            return Matrix.LookAtRH(this.Position, this.Position + this.LookDirection, this.UpDirection);
        }

    }
}