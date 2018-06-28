using HelixToolkit.Wpf.SharpDX;
using System.Numerics;
using System.Windows;
using Point = System.Windows.Point;
using MatrixTransform3D = System.Windows.Media.Media3D.MatrixTransform3D;
using System;
using Matrix = System.Numerics.Matrix4x4;
using HelixToolkit.Mathematics;

namespace CrossSectionDemo
{
    public class CrossSectionPlaneManipulator3D : GroupModel3D
    {
        public Plane CutPlane
        {
            get { return (Plane)GetValue(CutPlaneProperty); }
            set { SetValue(CutPlaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CutPlane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CutPlaneProperty =
            DependencyProperty.Register("CutPlane", typeof(Plane), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(new Plane(new Vector3(0, 0, -1), 0),
                (d, e) =>
                {
                    var model = d as CrossSectionPlaneManipulator3D;
                    if (!model.internalUpdate)
                    {
                        var plane = (Plane)e.NewValue;
                        model.currentTranslation = Matrix.CreateTranslation(plane.Normal * plane.D);
                        var v1 = plane.Normal.FindAnyPerpendicular();
                        var v2 = Vector3.Cross(plane.Normal, v1);
                        model.currentRotation = new Matrix(v2.X, v2.Y, v2.Z, 0, v1.X, v1.Y, v1.Z, 0, -plane.Normal.X, -plane.Normal.Y, -plane.Normal.Z, 0, 0, 0, 0, 1);
                        model.UpdateTransform(false);
                    }
                }));

        public double SizeScale
        {
            get { return (double)GetValue(SizeScaleProperty); }
            set { SetValue(SizeScaleProperty, value); }
        }

        public static readonly DependencyProperty SizeScaleProperty =
            DependencyProperty.Register("SizeScale", typeof(double), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    var model = d as CrossSectionPlaneManipulator3D;
                    model.UpdateScaling((float)model.CornerScale, (float)model.EdgeThicknessScale, (float)(double)e.NewValue);
                }));



        public double CornerScale
        {
            get { return (double)GetValue(CornerScaleProperty); }
            set { SetValue(CornerScaleProperty, value); }
        }

        public static readonly DependencyProperty CornerScaleProperty =
            DependencyProperty.Register("CornerScale", typeof(double), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    var model = d as CrossSectionPlaneManipulator3D;
                    model.UpdateScaling((float)(double)e.NewValue, (float)model.EdgeThicknessScale, (float)model.SizeScale);
                }));

        public double EdgeThicknessScale
        {
            get { return (double)GetValue(EdgeThicknessScaleProperty); }
            set { SetValue(EdgeThicknessScaleProperty, value); }
        }

        public static readonly DependencyProperty EdgeThicknessScaleProperty =
            DependencyProperty.Register("EdgeThicknessScale", typeof(double), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    var model = d as CrossSectionPlaneManipulator3D;
                    model.UpdateScaling((float)model.CornerScale, (float)(double)e.NewValue, (float)model.SizeScale);
                }));



        public Material CornerMaterial
        {
            get { return (Material)GetValue(CornerMaterialProperty); }
            set { SetValue(CornerMaterialProperty, value); }
        }

        public static readonly DependencyProperty CornerMaterialProperty =
            DependencyProperty.Register("CornerMaterial", typeof(Material), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(null, (d,e)=> 
            {
                (d as CrossSectionPlaneManipulator3D).UpdateCornerMaterial(e.NewValue as Material);
            }));



        public Material EdgeMaterial
        {
            get { return (Material)GetValue(EdgeMaterialProperty); }
            set { SetValue(EdgeMaterialProperty, value); }
        }

        public static readonly DependencyProperty EdgeMaterialProperty =
            DependencyProperty.Register("EdgeMaterial", typeof(Material), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(null, (d,e)=> 
            {
                (d as CrossSectionPlaneManipulator3D).UpdateEdgeMaterial(e.NewValue as Material);
            }));



        public float RotationSensitivity { set; get; } = 1;

        // 3 --- 2 
        // |     |
        // 0 --- 1
        private static readonly Vector3[] positions = new Vector3[4]
        {
            new Vector3(-1,-1,0),
            new Vector3(+1,-1,0),
            new Vector3(+1,+1,0),
            new Vector3(-1,+1,0),
        };
        private readonly static Geometry3D NodeGeometry;
        private readonly static Geometry3D EdgeHGeometry, EdgeVGeometry;
        private readonly MeshGeometryModel3D[] cornerHandles = new MeshGeometryModel3D[4];
        private readonly MeshGeometryModel3D[] edgeHandles = new MeshGeometryModel3D[4];
        private Point startPoint;
        private Vector3 startHitPoint;
        private Viewport3DX viewport;
        private Camera camera;
        private bool isCaptured = false;
        private Matrix currentTranslation = Matrix.Identity;
        private Matrix currentRotation = Matrix.Identity;
        private Matrix totalTransform = Matrix.Identity;
        private bool internalUpdate = false;

        static CrossSectionPlaneManipulator3D()
        {
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(), 0.125, 16, 16);
            b1.AddPyramid(new Vector3(0, 0, -0.15f), new Vector3(1, 0, 0), new Vector3(0, 0, -1), 0.125, 0.25);
            NodeGeometry = b1.ToMeshGeometry3D();

            var b2 = new MeshBuilder();
            b2.AddCylinder(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 0.05, 16, true, true);
            EdgeHGeometry = b2.ToMeshGeometry3D();

            var b3 = new MeshBuilder();
            b3.AddCylinder(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0.05, 16, true, true);
            EdgeVGeometry = b3.ToMeshGeometry3D();
        }

        public CrossSectionPlaneManipulator3D()
        {
            for (int i = 0; i < 4; ++i)
            {
                this.cornerHandles[i] = new MeshGeometryModel3D()
                {
                    Geometry = NodeGeometry,
                    CullMode = SharpDX.Direct3D11.CullMode.Back,
                };
                this.cornerHandles[i].MouseMove3D += OnNodeMouse3DMove;
                this.cornerHandles[i].MouseUp3D += OnNodeMouse3DUp;
                this.cornerHandles[i].MouseDown3D += OnNodeMouse3DDown;

                this.edgeHandles[i] = new MeshGeometryModel3D()
                {
                    Geometry = (i % 2 == 0) ? EdgeHGeometry : EdgeVGeometry,
                    CullMode = SharpDX.Direct3D11.CullMode.Back,
                };
                this.edgeHandles[i].MouseMove3D += OnEdgeMouse3DMove;
                this.edgeHandles[i].MouseUp3D += OnEdgeMouse3DUp;
                this.edgeHandles[i].MouseDown3D += OnEdgeMouse3DDown;
                this.Children.Add(cornerHandles[i]);
                this.Children.Add(edgeHandles[i]);
            }

            CornerMaterial = DiffuseMaterials.Orange;
            EdgeMaterial = DiffuseMaterials.Blue;

            UpdateScaling((float)CornerScale, (float)EdgeThicknessScale, (float)SizeScale);
            this.SceneNode.OnVisibleChanged += SceneNode_OnVisibleChanged;
        }

        private void UpdateScaling(float cornerScale, float edgeThicknessScale, float sizeScale)
        {
            // 3 --- 2 
            // |     |
            // 0 --- 1
            var m0 = Matrix.CreateScale(+2 * sizeScale, edgeThicknessScale, edgeThicknessScale) * Matrix.CreateTranslation(positions[0] * sizeScale);
            this.edgeHandles[0].Transform = new MatrixTransform3D(m0.ToMatrix3D());
            var m2 = Matrix.CreateScale(+2 * sizeScale, edgeThicknessScale, edgeThicknessScale) * Matrix.CreateTranslation(positions[3] * sizeScale);
            this.edgeHandles[2].Transform = new MatrixTransform3D(m2.ToMatrix3D());

            var m1 = Matrix.CreateScale(edgeThicknessScale, +2 * sizeScale, edgeThicknessScale) * Matrix.CreateTranslation(positions[1] * sizeScale);
            this.edgeHandles[1].Transform = new MatrixTransform3D(m1.ToMatrix3D());
            var m3 = Matrix.CreateScale(edgeThicknessScale, +2 * sizeScale, edgeThicknessScale) * Matrix.CreateTranslation(positions[0] * sizeScale);
            this.edgeHandles[3].Transform = new MatrixTransform3D(m3.ToMatrix3D());

            for(int i = 0; i < cornerHandles.Length; ++i)
            {
                this.cornerHandles[i].Transform = new MatrixTransform3D((Matrix.CreateScale(cornerScale) * Matrix.CreateTranslation(positions[i] * sizeScale)).ToMatrix3D());
            }
        }

        private void UpdateCornerMaterial(Material material)
        {
            foreach(var model in cornerHandles)
            {
                model.Material = material;
            }
        }
        private void UpdateEdgeMaterial(Material material)
        {
            foreach (var model in edgeHandles)
            {
                model.Material = material;
            }
        }
        private void SceneNode_OnVisibleChanged(object sender, BoolArgs e)
        {
            if (e.Value)
            {
                totalTransform = currentRotation = currentTranslation = Matrix.Identity;
                Transform = new MatrixTransform3D(totalTransform.ToMatrix3D());
                UpdateCutPlane();
            }
        }

        private void OnEdgeMouse3DDown(object sender, RoutedEventArgs e)
        {
            if (e is Mouse3DEventArgs arg)
            {
                viewport = arg.Viewport;
                camera = viewport.Camera;
                startPoint = arg.Position;
                isCaptured = true;
            }
        }

        private void OnEdgeMouse3DUp(object sender, RoutedEventArgs e)
        {
            isCaptured = false;
            viewport = null;
            camera = null;
        }

        private void OnEdgeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (isCaptured && e is Mouse3DEventArgs arg && arg.Viewport == viewport)
            {
                RotateTrackball(startPoint, arg.Position, currentTranslation.Translation);
                startPoint = arg.Position;
            }
        }

        private void OnNodeMouse3DDown(object sender, RoutedEventArgs e)
        {
            if (e is Mouse3DEventArgs arg)
            {
                viewport = arg.Viewport;
                camera = viewport.Camera;
                startHitPoint = arg.HitTestResult.PointHit;
                isCaptured = true;
            }
        }

        private void OnNodeMouse3DUp(object sender, RoutedEventArgs e)
        {
            isCaptured = false;
            viewport = null;
            camera = null;
        }

        private void OnNodeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (isCaptured && e is Mouse3DEventArgs arg && arg.Viewport == viewport)
            {
                var newHit = viewport.UnProjectOnPlane(arg.Position, startHitPoint.ToPoint3D(), camera.LookDirection);
                if (newHit.HasValue)
                {
                    var newPos = newHit.Value.ToVector3();
                    var offset = newPos - startHitPoint;
                    startHitPoint = newPos;
                    currentTranslation.Translation += offset;
                    UpdateTransform();
                }
            }
        }

        private static Vector3 ProjectToTrackball(Point point, double w, double h)
        {
            // Use the diagonal for scaling, making sure that the whole client area is inside the trackball
            double r = Math.Sqrt((w * w) + (h * h)) / 2;
            double x = (point.X - (w / 2)) / r;
            double y = ((h / 2) - point.Y) / r;
            double z2 = 1 - (x * x) - (y * y);
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3((float)x, (float)y, (float)z);
        }

        private void RotateTrackball(Point p1, Point p2, Vector3 rotateAround)
        {
            // http://viewport3d.com/trackball.htm
            // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
            var v1 = ProjectToTrackball(p1, viewport.ActualWidth, viewport.ActualHeight);
            var v2 = ProjectToTrackball(p2, viewport.ActualWidth, viewport.ActualHeight);

            // transform the trackball coordinates to view space
            var viewZ = Vector3.Normalize(camera.CameraInternal.LookDirection);
            var viewX = Vector3.Normalize(Vector3.Cross(camera.CameraInternal.UpDirection, viewZ));
            var viewY = Vector3.Normalize(Vector3.Cross(viewX, viewZ));
            var u1 = (viewZ * v1.Z) + (viewX * v1.X) + (viewY * v1.Y);
            var u2 = (viewZ * v2.Z) + (viewX * v2.X) + (viewY * v2.Y);

            // Could also use the Camera ViewMatrix
            // var vm = Viewport3DHelper.GetViewMatrix(this.ActualCamera);
            // vm.Invert();
            // var ct = new MatrixTransform3D(vm);
            // var u1 = ct.Transform(v1);
            // var u2 = ct.Transform(v2);

            // Find the rotation axis and angle
            var axis = Vector3.Cross(u1, u2);
            if (axis.LengthSquared() < 1e-8)
            {
                return;
            }

            var angle = u1.AngleBetween(u2);
            // Create the transform
            currentRotation *= Matrix.CreateFromAxisAngle(Vector3.Normalize(axis), (float)(angle * this.RotationSensitivity * 5));
            UpdateTransform();
        }

        private void UpdateCutPlane()
        {
            var planeNormal = Vector3.TransformNormal(new Vector3(0, 0, -1), currentRotation);
            CutPlane = PlaneHelper.GetPlane(-currentTranslation.Translation, planeNormal);
        }

        private void UpdateTransform(bool updateCutPlane = true)
        {
            totalTransform = currentRotation * currentTranslation;
            this.Transform = new MatrixTransform3D(totalTransform.ToMatrix3D());
            if (updateCutPlane)
            {
                internalUpdate = true;
                UpdateCutPlane();
                internalUpdate = false;
            }
        }
    }
}
