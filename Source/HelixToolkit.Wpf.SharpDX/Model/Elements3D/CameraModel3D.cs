using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Cameras;
using SharpDX;
using SharpDX.Direct3D11;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.SharpDX;

public class CameraModel3D : CompositeModel3D
{
    public static readonly DependencyProperty CameraProperty =
       DependencyProperty.Register("Camera", typeof(ProjectionCamera), typeof(CameraModel3D), new PropertyMetadata(null, (d, e) =>
       {
           if (d is CameraModel3D model)
           {
               model.camera = e.NewValue as ProjectionCamera;
           }
       }));

    /// <summary>
    /// Distance of the directional light from origin
    /// </summary>
    public ProjectionCamera? Camera
    {
        get
        {
            return (ProjectionCamera?)this.GetValue(CameraProperty);
        }
        set
        {
            this.SetValue(CameraProperty, value);
        }
    }

    protected bool isCaptured;
    protected Viewport3DX? viewport;
    protected CameraCore? viewportCamera;
    protected Vector3 lastHitPos;

    private ProjectionCamera? _camera;

#pragma warning disable IDE1006
    protected ProjectionCamera? camera
#pragma warning restore IDE1006
    {
        private set
        {
            if (_camera == value)
            {
                return;
            }
            _camera = value;
            this.Transform = _camera is null ? Transform3D.Identity : new MatrixTransform3D(_camera.GetInversedViewMatrix());
        }
        get
        {
            return _camera;
        }
    }

    public CameraModel3D()
    {
        var b1 = new MeshBuilder();
        b1.AddBox(new Vector3().ToVector(), 1f, 1f, 1.2f, BoxFaces.All);
        var body = new MeshGeometryModel3D
        {
            CullMode = CullMode.Back,
            Geometry = b1.ToMesh().ToMeshGeometry3D(),
            Material = new DiffuseMaterial() { DiffuseColor = Color.Gray }
        };
        this.Children.Add(body);
        b1 = new MeshBuilder();
        b1.AddCone(new Vector3(0, 0, -1.2f).ToVector(), new Vector3(0, 0f, 0).ToVector(), 0.4f, true, 12);
        var lens = new MeshGeometryModel3D
        {
            CullMode = CullMode.Back,
            Geometry = b1.ToMesh().ToMeshGeometry3D(),
            Material = new DiffuseMaterial() { DiffuseColor = Color.Yellow }
        };
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
        var segment = mesh.Positions is null ? 0 : mesh.Positions.Count / 3;
        var colors = new Color4Collection(Enumerable.Repeat<Color4>(Color.Black, mesh.Positions is null ? 0 : mesh.Positions.Count));
        var i = 0;
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
        SceneNode.TransformChanged += SceneNode_OnTransformChanged;
    }

    protected override void OnMouse3DDown(object? sender, RoutedEventArgs e)
    {
        base.OnMouse3DDown(sender, e);

        if (e is not Mouse3DEventArgs args)
            return;

        if (args.Viewport == null)
            return;

        this.isCaptured = true;
        this.viewport = args.Viewport;
        this.viewportCamera = args.Viewport?.Camera;

        if (args.HitTestResult is not null)
        {
            this.lastHitPos = args.HitTestResult.PointHit;
        }
    }

    protected override void OnMouse3DUp(object? sender, RoutedEventArgs e)
    {
        base.OnMouse3DUp(sender, e);
        if (this.isCaptured)
        {
            this.isCaptured = false;
            this.viewportCamera = null;
            this.viewport = null;
        }
    }

    protected override void OnMouse3DMove(object? sender, RoutedEventArgs e)
    {
        base.OnMouse3DMove(sender, e);
        if (this.isCaptured)
        {
            if (e is not Mouse3DEventArgs args || this.viewportCamera is null || this.viewport is null)
            {
                return;
            }

            // move dragmodel                         
            var normal = this.viewportCamera.LookDirection;

            // hit position                        
            var newHit = this.viewport.UnProjectOnPlane(args.Position.ToVector2(), lastHitPos, normal);
            if (newHit.HasValue)
            {
                var offset = (newHit.Value - lastHitPos).ToVector3D();
                this.lastHitPos = newHit.Value;
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

    private void SceneNode_OnTransformChanged(object? sender, TransformArgs e)
    {
        if (camera != null)
        {
            var m = e.Transform;
            camera.Position = new Point3D(m.M41, m.M42, m.M43);
            camera.LookDirection = new Vector3D(-m.M31, -m.M32, -m.M33);
            camera.UpDirection = new Vector3D(m.M21, m.M22, m.M23);
        }
    }
}
