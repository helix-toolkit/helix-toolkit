using HelixToolkit.Wpf.SharpDX.Cameras;
using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class CameraModel3D : CompositeModel3D
    {
        public static readonly DependencyProperty CameraProperty =
           DependencyProperty.Register("Camera", typeof(ProjectionCamera), typeof(CameraModel3D), new PropertyMetadata(null, (d, e) =>
           {
               (d as CameraModel3D).camera = e.NewValue as ProjectionCamera;
           }));

        /// <summary>
        /// Distance of the directional light from origin
        /// </summary>
        public ProjectionCamera Camera
        {
            get { return (ProjectionCamera)this.GetValue(CameraProperty); }
            set { this.SetValue(CameraProperty, value); }
        }

        protected bool isCaptured;
        protected Viewport3DX viewport;
        protected CameraCore viewportCamera;
        protected Vector3 lastHitPos;

        private ProjectionCamera _camera;
        protected ProjectionCamera camera
        {
            private set
            {
                if(_camera == value) { return; }
                _camera = value;
                this.Transform = new MatrixTransform3D(_camera.GetInversedViewMatrix());
            }
            get
            {
                return _camera;
            }
        }

        public CameraModel3D()
        {
            var b1 = new MeshBuilder();
            b1.AddBox(new Vector3(), 1f, 1f, 1.2f, BoxFaces.All);           
            var body = new MeshGeometryModel3D() { CullMode = CullMode.Back };
            body.Geometry = b1.ToMeshGeometry3D();
            body.Material = new DiffuseMaterial() { DiffuseColor = Color.Gray };
            this.Children.Add(body);
            b1 = new MeshBuilder();
            b1.AddCone(new Vector3(0, 0, -1.2f), new Vector3(0, 0f, 0), 0.4f, true, 12);
            var lens = new MeshGeometryModel3D() { CullMode = CullMode.Back};
            lens.Geometry = b1.ToMeshGeometry3D();
            lens.Material = new DiffuseMaterial() { DiffuseColor = Color.Yellow };
            this.Children.Add(lens);

            var builder = new LineBuilder();
            builder.AddLine(Vector3.Zero, new Vector3(2, 0, 0));
            builder.AddLine(Vector3.Zero, new Vector3(0, 2, 0));
            builder.AddLine(Vector3.Zero, new Vector3(0, 0, -2));

            var mesh = builder.ToLineGeometry3D();
            var arrowMeshModel = new LineGeometryModel3D
            {
                Geometry = mesh,
                Color = System.Windows.Media.Colors.White,
                IsHitTestVisible = false
            };
            int segment = mesh.Positions.Count / 3;
            var colors = new Color4Collection(Enumerable.Repeat<Color4>(Color.Black, mesh.Positions.Count));
            int i = 0;
            for (; i < segment; ++i)
            {
                colors[i] = Color.Red;
            }
            for (; i < segment * 2; ++i)
            {
                colors[i] = Color.Green;
            }
            for (; i < segment * 3; ++i)
            {
                colors[i] = Color.Blue;
            }
            mesh.Colors = colors;
            this.Children.Add(arrowMeshModel);
            SceneNode.OnTransformChanged += SceneNode_OnTransformChanged;
        }

        protected override void OnMouse3DDown(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DDown(sender, e);

            if (!(e is Mouse3DEventArgs args)) return;
            if (args.Viewport == null) return;

            this.isCaptured = true;
            this.viewport = args.Viewport;
            this.viewportCamera = args.Viewport.Camera;
            this.lastHitPos = args.HitTestResult.PointHit;
        }

        protected override void OnMouse3DUp(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DUp(sender, e);
            if (this.isCaptured)
            {
                this.isCaptured = false;
                this.viewportCamera = null;
                this.viewport = null;
            }
        }

        protected override void OnMouse3DMove(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DMove(sender, e);
            if (this.isCaptured)
            {
                var args = e as Mouse3DEventArgs;

                // move dragmodel                         
                var normal = this.viewportCamera.LookDirection;

                // hit position                        
                var newHit = this.viewport.UnProjectOnPlane(args.Position, lastHitPos.ToPoint3D(), normal.ToVector3D());
                if (newHit.HasValue)
                {
                    var offset = (newHit.Value - lastHitPos.ToPoint3D());
                    this.lastHitPos = newHit.Value.ToVector3();
                    if (Transform == null)
                    {
                        Transform = new TranslateTransform3D(offset);
                    }
                    else
                    {
                        this.Transform = new MatrixTransform3D(Transform.AppendTransform(new TranslateTransform3D(offset)).Value);
                    }
                }
            }
        }

        private void SceneNode_OnTransformChanged(object sender, TransformArgs e)
        {
            if(camera != null)
            {
                var m = e.Transform;
                camera.Position = new Point3D(m.M41, m.M42, m.M43);
                camera.LookDirection = new Vector3D(-m.M31, -m.M32, -m.M33);
                camera.UpDirection = new Vector3D(m.M21, m.M22, m.M23);               
            }
        }
    }
}
