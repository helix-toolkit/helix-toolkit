using DependencyPropertyGenerator;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Cameras;
using HelixToolkit.Wpf.SharpDX;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MatrixTransform3D = System.Windows.Media.Media3D.MatrixTransform3D;

namespace MouseDragDemo;

[DependencyProperty<bool>("DragX", DefaultValue = true)]
[DependencyProperty<bool>("DragY", DefaultValue = true)]
[DependencyProperty<bool>("DragZ", DefaultValue = true)]
[DependencyProperty<bool>("IsSelected", DefaultValue = false)]
[DependencyProperty<Material>("Material")]
public sealed partial class InteractionHandle3D : GroupModel3D, IHitable, ISelectable
{
    // 3 --- 2 
    // |     |
    // 0 --- 1
    private Vector3[] positions = new Vector3[4]
    {
            new(-1,-1,0),
            new(+1,-1,0),
            new(+1,+1,0),
            new(-1,+1,0),
    };
    private readonly DraggableGeometryModel3D[] cornerHandles = new DraggableGeometryModel3D[4];
    private readonly DraggableGeometryModel3D[] midpointHandles = new DraggableGeometryModel3D[4];
    private readonly MeshGeometryModel3D[] edgeHandles = new MeshGeometryModel3D[4];
    private bool isCaptured;
    private Viewport3DX? viewport;
    private CameraCore? camera;
    private System.Windows.Media.Media3D.Point3D lastHitPos;
    private MatrixTransform3D dragTransform;
    //private Material selectionMaterial;

    private static Geometry3D NodeGeometry;
    private static Geometry3D EdgeHGeometry, EdgeVGeometry;
    private static Geometry3D BoxGeometry;

    static InteractionHandle3D()
    {
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0.0f, 0.0f, 0), 0.135f);
        NodeGeometry = b1.ToMeshGeometry3D();

        var b2 = new MeshBuilder();
        b2.AddCylinder(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 0.05f, 32, true, true);
        EdgeHGeometry = b2.ToMeshGeometry3D();

        var b3 = new MeshBuilder();
        b3.AddCylinder(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0.05f, 32, true, true);
        EdgeVGeometry = b3.ToMeshGeometry3D();

        var b4 = new MeshBuilder();
        b4.AddBox(new Vector3(0, 0, 0), 0.175f, 0.175f, 0.175f);
        BoxGeometry = b4.ToMeshGeometry3D();
    }

    /// <summary>
    /// 
    /// </summary>
    public InteractionHandle3D()
    {
        this.Material = PhongMaterials.Orange;
        //selectionColor.EmissiveColor = Color.Blue;
        //selectionColor.SpecularColor = Color.Black;
        //selectionColor.ReflectiveColor = Color.Black;

        for (int i = 0; i < 4; i++)
        {
            var translate = Matrix.CreateTranslation(positions[i]);
            this.cornerHandles[i] = new DraggableGeometryModel3D()
            {
                DragZ = false,
                Visibility = Visibility.Visible,
                Material = this.Material,
                Geometry = NodeGeometry,
                Transform = new MatrixTransform3D(translate.ToMatrix3D()),
            };
            this.cornerHandles[i].MouseMove3D += OnNodeMouse3DMove;
            this.cornerHandles[i].MouseUp3D += OnNodeMouse3DUp;
            this.cornerHandles[i].MouseDown3D += OnNodeMouse3DDown;

            this.edgeHandles[i] = new MeshGeometryModel3D()
            {
                Geometry = (i % 2 == 0) ? EdgeHGeometry : EdgeVGeometry,
                Material = this.Material,
                Visibility = Visibility.Visible,
                Transform = new MatrixTransform3D(translate.ToMatrix3D()),
            };
            this.edgeHandles[i].MouseMove3D += OnEdgeMouse3DMove;
            this.edgeHandles[i].MouseUp3D += OnEdgeMouse3DUp;
            this.edgeHandles[i].MouseDown3D += OnEdgeMouse3DDown;


            translate = Matrix.CreateTranslation(0.5f * (positions[i] + positions[(i + 1) % 4]));
            this.midpointHandles[i] = new DraggableGeometryModel3D()
            {
                DragZ = false,
                DragX = (i % 2 == 1),
                DragY = (i % 2 == 0),
                Material = this.Material,
                Geometry = BoxGeometry,
                Transform = new MatrixTransform3D(translate.ToMatrix3D()),
            };
            this.midpointHandles[i].MouseMove3D += OnNodeMouse3DMove;
            this.midpointHandles[i].MouseUp3D += OnNodeMouse3DUp;
            this.midpointHandles[i].MouseDown3D += OnNodeMouse3DDown;

            this.Children.Add(cornerHandles[i]);
            this.Children.Add(edgeHandles[i]);
            this.Children.Add(midpointHandles[i]);
        }

        // 3 --- 2 
        // |     |
        // 0 --- 1
        var m0 = Matrix.CreateScale(+2, 1, 1) * Matrix.CreateTranslation(positions[0]);
        this.edgeHandles[0].Transform = new MatrixTransform3D(m0.ToMatrix3D());
        var m2 = Matrix.CreateScale(+2, 1, 1) * Matrix.CreateTranslation(positions[3]);
        this.edgeHandles[2].Transform = new MatrixTransform3D(m2.ToMatrix3D());

        var m1 = Matrix.CreateScale(1, +2, 1) * Matrix.CreateTranslation(positions[1]);
        this.edgeHandles[1].Transform = new MatrixTransform3D(m1.ToMatrix3D());
        var m3 = Matrix.CreateScale(1, +2, 1) * Matrix.CreateTranslation(positions[0]);
        this.edgeHandles[3].Transform = new MatrixTransform3D(m3.ToMatrix3D());

        this.dragTransform = new MatrixTransform3D(this.Transform.Value);
    }

    private void OnNodeMouse3DDown(object? sender, RoutedEventArgs e)
    {
        if (e is not Mouse3DEventArgs args)
        {
            return;
        }

        if (args.Viewport is null)
        {
            return;
        }

        this.isCaptured = true;
    }

    private void OnNodeMouse3DUp(object? sender, RoutedEventArgs e)
    {
        if (this.isCaptured)
        {
            Application.Current.MainWindow.Cursor = Cursors.Arrow;
            //UpdateTransforms(sender);
        }
    }

    private void OnNodeMouse3DMove(object? sender, RoutedEventArgs e)
    {
        if (this.isCaptured)
        {
            UpdateTransforms(sender);
        }
    }

    private void OnEdgeMouse3DDown(object? sender, RoutedEventArgs e)
    {
        if (e is not Mouse3DEventArgs args)
        {
            return;
        }

        if (args.Viewport is null)
        {
            return;
        }

        this.isCaptured = true;
        this.viewport = args.Viewport;
        this.camera = args.Viewport.Camera;
        this.lastHitPos = args.HitTestResult is null ? new() : args.HitTestResult.PointHit.ToPoint3D();
    }

    private void OnEdgeMouse3DUp(object? sender, RoutedEventArgs e)
    {
        if (this.isCaptured)
        {
            Application.Current.MainWindow.Cursor = Cursors.Arrow;
            this.isCaptured = false;
            this.camera = null;
            this.viewport = null;
        }
    }

    private void OnEdgeMouse3DMove(object? sender, RoutedEventArgs e)
    {
        if (this.camera is null || this.viewport is null)
        {
            return;
        }

        if (this.isCaptured && e is Mouse3DEventArgs args)
        {
            Application.Current.MainWindow.Cursor = Cursors.SizeAll;

            // move dragmodel                         
            var normal = this.camera.LookDirection;

            // hit position                        
            var newHit = this.viewport.UnProjectOnPlane(args.Position, lastHitPos, normal.ToVector3D());
            if (newHit.HasValue)
            {
                var offset = (newHit.Value - lastHitPos);
                var trafo = this.Transform.Value;

                if (this.DragX)
                    trafo.OffsetX += offset.X;

                if (this.DragY)
                    trafo.OffsetY += offset.Y;

                if (!this.DragZ)
                    trafo.OffsetZ += offset.Z;

                this.dragTransform.Matrix = trafo;
                this.Transform = this.dragTransform;
                this.lastHitPos = newHit.Value;
            }
        }
    }

    private void UpdateTransforms(object? sender)
    {
        var cornerTrafos = this.cornerHandles.Select(x => x.Transform as MatrixTransform3D).ToArray();
        var cornerMatrix = cornerTrafos.Select(x => x?.Value ?? System.Windows.Media.Media3D.Matrix3D.Identity).ToArray();
        this.positions = cornerMatrix.Select(x => x.ToMatrix().Translation).ToArray();

        BoundingBox bb;
        if (sender == cornerHandles[0] || sender == cornerHandles[2])
        {
            Application.Current.MainWindow.Cursor = Cursors.SizeNESW;
            bb = BoundingBox.FromPoints(new[] { positions[0], positions[2] });
        }
        else if (sender == cornerHandles[1] || sender == cornerHandles[3])
        {
            Application.Current.MainWindow.Cursor = Cursors.SizeNWSE;
            bb = BoundingBox.FromPoints(new[] { positions[1], positions[3] });
        }
        else
        {
            if (sender == midpointHandles[0] || sender == midpointHandles[2])
            {
                Application.Current.MainWindow.Cursor = Cursors.SizeNS;
            }
            else
            {
                Application.Current.MainWindow.Cursor = Cursors.SizeWE;
            }
            positions = this.midpointHandles.Select(x => x.Transform.Value.ToMatrix().Translation).ToArray();
            bb = BoundingBox.FromPoints(positions);
        }

        // 3 --- 2 
        // |     |
        // 0 --- 1
        positions[0].X = bb.Minimum.X;
        positions[1].X = bb.Maximum.X;
        positions[2].X = bb.Maximum.X;
        positions[3].X = bb.Minimum.X;

        positions[0].Y = bb.Minimum.Y;
        positions[1].Y = bb.Minimum.Y;
        positions[2].Y = bb.Maximum.Y;
        positions[3].Y = bb.Maximum.Y;

        for (int i = 0; i < 4; i++)
        {
            if (sender != cornerHandles[i])
            {
                var corner = cornerTrafos[i];
                if (corner is not null)
                {
                    corner.Matrix = Matrix.CreateTranslation(positions[i]).ToMatrix3D();
                }
            }

            var m = Matrix.CreateTranslation(0.5f * (positions[i] + positions[(i + 1) % 4]));
            ((MatrixTransform3D)this.midpointHandles[i].Transform).Matrix = m.ToMatrix3D();
        }

        // 3 --- 2 
        // |     |
        // 0 --- 1
        var m0 = Matrix.CreateScale(positions[1].X - positions[0].X, 1, 1) * Matrix.CreateTranslation(positions[0]);
        ((MatrixTransform3D)this.edgeHandles[0].Transform).Matrix = (m0.ToMatrix3D());
        var m2 = Matrix.CreateScale(positions[1].X - positions[0].X, 1, 1) * Matrix.CreateTranslation(positions[3]);
        ((MatrixTransform3D)this.edgeHandles[2].Transform).Matrix = (m2.ToMatrix3D());

        var m1 = Matrix.CreateScale(1, positions[2].Y - positions[1].Y, 1) * Matrix.CreateTranslation(positions[1]);
        ((MatrixTransform3D)this.edgeHandles[1].Transform).Matrix = (m1.ToMatrix3D());
        var m3 = Matrix.CreateScale(1, positions[2].Y - positions[1].Y, 1) * Matrix.CreateTranslation(positions[0]);
        ((MatrixTransform3D)this.edgeHandles[3].Transform).Matrix = (m3.ToMatrix3D());
    }

    partial void OnMaterialChanged(Material? newValue)
    {
        if (newValue is PhongMaterial phongMaterial)
        {
            foreach (var item in this.Children)
            {
                if (item is MaterialGeometryModel3D model)
                {
                    model.Material = phongMaterial;
                }
            }
        }
    }
}
