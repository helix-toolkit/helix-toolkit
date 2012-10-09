namespace HelixToolkit.SharpDX
{
    using System.Windows;
    using System.Windows.Media.Media3D;
    using global::SharpDX;

    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;
    

    public abstract class ProjectionCamera : Camera
    {
        protected ProjectionCamera()
        {
            this.NearPlaneDistance = 1e-2;
            this.FarPlaneDistance = 1e3;
            this.CreateLeftHandSystem = true;
        }

        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point3D), typeof(ProjectionCamera), new PropertyMetadata(new Point3D(0, 0, -5), PositionChanged));

        public Vector3D LookDirection { get; set; }
        public Vector3D UpDirection { get; set; }
        public double NearPlaneDistance { get; set; }
        public double FarPlaneDistance { get; set; }
        public bool CreateLeftHandSystem { get; set; }
        
        private ScaleTransform3D m_scale = new ScaleTransform3D();
        private RotateTransform3D m_rotation = new RotateTransform3D();
        private TranslateTransform3D m_translate = new TranslateTransform3D();
        private Transform3DGroup m_transform = new Transform3DGroup();

        public override Matrix CreateViewMatrix()
        {
            if (this.CreateLeftHandSystem)
            {
                //return Matrix.LookAtLH(this.Position, this.Position + this.LookDirection, this.UpDirection);
                var m = m_transform.Value;
                return new Matrix(
                    (float)m.M11, (float)m.M12, (float)m.M13, (float)m.M14,
                    (float)m.M21, (float)m.M22, (float)m.M23, (float)m.M24,
                    (float)m.M31, (float)m.M32, (float)m.M33, (float)m.M34,
                    -(float)m.OffsetX, -(float)m.OffsetY, +5f, (float)m.M44);  
            }
            else
            {
                //return Matrix.LookAtRH(this.Position, this.Position + this.LookDirection, this.UpDirection);
                var m = m_transform.Value;
                return new Matrix(
                    (float)m.M11, (float)m.M12, (float)m.M13, (float)m.M14,
                    (float)m.M21, (float)m.M22, (float)m.M23, (float)m.M24,
                    (float)m.M31, (float)m.M32, (float)m.M33, (float)m.M34,
                    (float)m.OffsetX, (float)m.OffsetY, (float)m.OffsetZ, (float)m.M44);  
            }              
        }

        protected override void OnTransformChanged(DependencyPropertyChangedEventArgs args)
        {
            m_transform = args.NewValue as Transform3DGroup;
            var trafo = args.NewValue as Transform3DGroup;
            if (m_transform != null)
            {
                m_scale = trafo.Children[0] as ScaleTransform3D;
                m_rotation = trafo.Children[1] as RotateTransform3D;
                m_translate = trafo.Children[2] as TranslateTransform3D;
            }           
        }

        protected static void PositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ProjectionCamera)obj).OnPositionChanged(args);  
        }
        protected void OnPositionChanged(DependencyPropertyChangedEventArgs args)
        {
            var pos = (Point3D)args.NewValue;
            m_transform.Children[2] = new TranslateTransform3D(pos.X, pos.Y, pos.Z);
        }

    }
}