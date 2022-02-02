using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using Point = System.Windows.Point;
using MatrixTransform3D = System.Windows.Media.Media3D.MatrixTransform3D;
using System;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.Collections.Generic;
using System.Diagnostics;

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
                        model.currentTranslation = Matrix.Translation(plane.Normal * plane.D);
                        var v1 = plane.Normal.FindAnyPerpendicular();
                        var v2 = Vector3.Cross(plane.Normal, v1);
                        model.currentRotation = new Matrix(v2.X, v2.Y, v2.Z, 0, v1.X, v1.Y, v1.Z, 0, -plane.Normal.X, -plane.Normal.Y, -plane.Normal.Z, 0, 0, 0, 0, 1);
                        model.UpdateTransform(false);
                    }
                }));



        /// <summary>
        /// This is used to demonstrate the code to constraint the rotation against a fixed axis instead 
        /// of a free gimble. If left null a free trakcball approach is used, otherwise the rotation
        /// is determined around the specified axis.
        /// </summary>
        public Vector3? ConstrainAxis
        {
            get { return (Vector3?)GetValue(ConstrainAxisProperty); }
            set { SetValue(ConstrainAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConstrainAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConstrainAxisProperty =
            DependencyProperty.Register("ConstrainAxis", typeof(Vector3?), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(null));


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
            DependencyProperty.Register("CornerMaterial", typeof(Material), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(null, (d, e) =>
            {
                (d as CrossSectionPlaneManipulator3D).UpdateCornerMaterial(e.NewValue as Material);
            }));



        public Material EdgeMaterial
        {
            get { return (Material)GetValue(EdgeMaterialProperty); }
            set { SetValue(EdgeMaterialProperty, value); }
        }

        public static readonly DependencyProperty EdgeMaterialProperty =
            DependencyProperty.Register("EdgeMaterial", typeof(Material), typeof(CrossSectionPlaneManipulator3D), new PropertyMetadata(null, (d, e) =>
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
        private readonly static Geometry3D EdgeHGeometry;
        private readonly MeshGeometryModel3D cornerHandle;
        private readonly MeshGeometryModel3D edgeHandle;
        private Point startPoint;
        private Vector3 startHitPoint;
        private Viewport3DX viewport;
        private Camera camera;
        private bool isCaptured = false;
        private Matrix currentTranslation = Matrix.Identity;
        private Matrix currentRotation = Matrix.Identity;
        private Matrix totalTransform = Matrix.Identity;
        private bool internalUpdate = false;
        private Color4 orgColor;

        static CrossSectionPlaneManipulator3D()
        {
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(), 0.125, 12, 12);
            b1.AddPyramid(new Vector3(0, 0, -0.15f), new Vector3(1, 0, 0), new Vector3(0, 0, -1), 0.125, 0.25);
            NodeGeometry = b1.ToMeshGeometry3D();
            NodeGeometry.OctreeParameter.MinimumOctantSize = 0.01f;
            NodeGeometry.UpdateOctree();
            var b2 = new MeshBuilder();
            b2.AddCylinder(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 0.05, 12, true, true);
            EdgeHGeometry = b2.ToMeshGeometry3D();
            EdgeHGeometry.OctreeParameter.MinimumOctantSize = 0.01f;
            EdgeHGeometry.UpdateOctree();
        }

        public static ICommand Cmd
        {
            get
            {
                if (cmd == null)
                {
                    cmd = new myCmd();    
                }
                return cmd;
            }
        }

        private static ICommand cmd = null;

        private class myCmd : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                
            }
        }



        public CrossSectionPlaneManipulator3D()
        {
            // corner manipulation is used to move the plane along its normal
            this.cornerHandle = new MeshGeometryModel3D()
            {
                Geometry = NodeGeometry,
                CullMode = SharpDX.Direct3D11.CullMode.Back,
            };
            this.cornerHandle.MouseMove3D += OnNodeMouse3DMove;
            this.cornerHandle.MouseUp3D += OnNodeMouse3DUp;
            this.cornerHandle.MouseDown3D += OnNodeMouse3DDown;

            // edge manipulation is used to rotate the plane
            this.edgeHandle = new MeshGeometryModel3D()
            {
                Geometry = EdgeHGeometry,
                CullMode = SharpDX.Direct3D11.CullMode.Back,
            };
#if TRUE
            this.edgeHandle.MouseMove3D += OnEdgeMouse3DMove;
            this.edgeHandle.MouseUp3D += OnEdgeMouse3DUp;
            this.edgeHandle.MouseDown3D += OnEdgeMouse3DDown;
#else

            MouseGesture leftMouseGesture = new MouseGesture(MouseAction.LeftClick);
            MouseBinding theBinding = new MouseBinding(
                Cmd,
                leftMouseGesture
                );
            if (edgeHandle.InputBindings != null)
            {
                edgeHandle.InputBindings.Add(theBinding);
            }
#endif

            // completing setup
            this.Children.Add(cornerHandle);
            this.Children.Add(edgeHandle);

            CornerMaterial = DiffuseMaterials.Orange;
            EdgeMaterial = DiffuseMaterials.Blue;

            UpdateScaling((float)CornerScale, (float)EdgeThicknessScale, (float)SizeScale);
            this.SceneNode.VisibleChanged += SceneNode_OnVisibleChanged;
        }

        private void UpdateScaling(float cornerScale, float edgeThicknessScale, float sizeScale)
        {
            // 3 --- 2 
            // |     |
            // 0 --- 1
            edgeHandle.Instances = new Matrix[4]
            {
                Matrix.Scaling(2 * sizeScale, edgeThicknessScale, edgeThicknessScale) * Matrix.Translation(positions[0] * sizeScale),
                Matrix.Scaling(2 * sizeScale, edgeThicknessScale, edgeThicknessScale) * Matrix.Translation(positions[3] * sizeScale),
                Matrix.Scaling(2 * sizeScale, edgeThicknessScale, edgeThicknessScale) * Matrix.RotationAxis(new Vector3(0, 0, 1), (float)(Math.PI / 2)) * Matrix.Translation(positions[1] * sizeScale),
                Matrix.Scaling(2 * sizeScale, edgeThicknessScale, edgeThicknessScale) * Matrix.RotationAxis(new Vector3(0, 0, 1), (float)(Math.PI / 2)) * Matrix.Translation(positions[0] * sizeScale)
            };
            cornerHandle.Instances = new Matrix[4]
            {
                Matrix.Scaling(cornerScale) * Matrix.Translation(positions[0] * sizeScale),
                Matrix.Scaling(cornerScale) * Matrix.Translation(positions[1] * sizeScale),
                Matrix.Scaling(cornerScale) * Matrix.Translation(positions[2] * sizeScale),
                Matrix.Scaling(cornerScale) * Matrix.Translation(positions[3] * sizeScale),
            };
        }

        private void UpdateCornerMaterial(Material material)
        {
            cornerHandle.Material = material;
        }
        private void UpdateEdgeMaterial(Material material)
        {
            edgeHandle.Material = material;
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
                arg.OriginalInputEventArgs.Handled = true;
                viewport = arg.Viewport;
                camera = viewport.Camera;
                startPoint = arg.Position;
                isCaptured = true;
                e.Handled = true;
                if(EdgeMaterial is DiffuseMaterial m)
                {
                    orgColor = m.DiffuseColor;
                    m.DiffuseColor = Color.Yellow;
                }
            }
        }

        private void OnEdgeMouse3DUp(object sender, RoutedEventArgs e)
        {
            if (e is Mouse3DEventArgs me)
            {
                me.OriginalInputEventArgs.Handled = true;
            }
            if (isCaptured && EdgeMaterial is DiffuseMaterial m)
            {
                m.DiffuseColor = orgColor;
            }
            isCaptured = false;
            viewport = null;
            camera = null;
            e.Handled = true;
        }

        private void OnEdgeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (isCaptured && e is Mouse3DEventArgs arg && arg.Viewport == viewport)
            {
                arg.OriginalInputEventArgs.Handled = true;
                RotateTrackball(startPoint, arg.Position, currentTranslation.TranslationVector);
                startPoint = arg.Position;
                e.Handled=true;
            }
        }

        private void OnNodeMouse3DDown(object sender, RoutedEventArgs e)
        {
            if (e is Mouse3DEventArgs arg)
            {
                arg.OriginalInputEventArgs.Handled = true;
                viewport = arg.Viewport;
                camera = viewport.Camera;
                startHitPoint = arg.HitTestResult.PointHit;
                isCaptured = true;
                e.Handled = true;
                if (CornerMaterial is DiffuseMaterial m)
                {
                    orgColor = m.DiffuseColor;
                    m.DiffuseColor = Color.Yellow;
                }
            }
        }

        private void OnNodeMouse3DUp(object sender, RoutedEventArgs e)
        {
            if (e is Mouse3DEventArgs me)
            {
                me.OriginalInputEventArgs.Handled = true;
            }
            if (isCaptured && CornerMaterial is DiffuseMaterial m)
            {
                m.DiffuseColor = orgColor;
            }
            isCaptured = false;
            viewport = null;
            camera = null;
        }

        private void OnNodeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (isCaptured && e is Mouse3DEventArgs arg && arg.Viewport == viewport)
            {
                arg.OriginalInputEventArgs.Handled = true;
                var newHit = viewport.UnProjectOnPlane(arg.Position, startHitPoint.ToPoint3D(), camera.LookDirection);
                if (newHit.HasValue)
                {
                    var newPos = newHit.Value.ToVector3();
                    newPos = new Vector3(newPos.X, startHitPoint.Y, newPos.Z); // trying to constraint elevation
                    var offset = newPos - startHitPoint;
                    startHitPoint = newPos;
                    currentTranslation.TranslationVector += offset;
                    UpdateTransform();
                    e.Handled = true;
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

            Vector3 v1, v2;
            if (ConstrainAxis.HasValue)
            {
                // preparing to compute the ratio to the screen
                var diag = Math.Sqrt(
                    viewport.ActualWidth * viewport.ActualWidth +
                    viewport.ActualHeight * viewport.ActualHeight
                    );

                // can we project the constraintAxis onto the view?
                var t3 = Vector3.TransformCoordinate(ConstrainAxis.Value, camera.CameraInternal.GetViewMatrix());
                var dir = new Vector2(t3.X, t3.Y); // axis of Constraint in view coordinates
                
                var pp1 = p1.ToVector2(); // computing distance perpendicular to axis in view coordinates
                var r1 =  pp1 - (pp1 * dir);

                var pp2 = p2.ToVector2(); // computing distance perpendicular to axis in view coordinates
                var r2 = pp2 - (pp2 * dir);

                var angle = (r2.Length() - r1.Length()) / diag * 4;
                // Create the transform
                currentRotation *= Matrix.RotationAxis(Vector3.Normalize(ConstrainAxis.Value), (float)(angle * this.RotationSensitivity * 5));
                UpdateTransform();
            }
            else
            {
                v1 = ProjectToTrackball(p1, viewport.ActualWidth, viewport.ActualHeight);
                v2 = ProjectToTrackball(p2, viewport.ActualWidth, viewport.ActualHeight);

                // transform the trackball coordinates to view space
                var viewZ = camera.CameraInternal.LookDirection;
                var viewX = Vector3.Cross(camera.CameraInternal.UpDirection, viewZ);
                var viewY = Vector3.Cross(viewX, viewZ);
                viewX.Normalize();
                viewY.Normalize();
                viewZ.Normalize();
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
                currentRotation *= Matrix.RotationAxis(Vector3.Normalize(axis), (float)(angle * this.RotationSensitivity * 5));
                UpdateTransform();
            }
            

           
        }

        private void UpdateCutPlane()
        {
            var planeNormal = Vector3.TransformNormal(new Vector3(0, 0, -1), currentRotation);
            CutPlane = new Plane(-currentTranslation.TranslationVector, planeNormal);
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
        protected override SceneNode OnCreateSceneNode()
        {
            return new AlwaysHitGroupNode(this);
        }

        private sealed class AlwaysHitGroupNode : GroupNode
        {
            private readonly object edgeHandle;
            private readonly object cornerHandle;

            public AlwaysHitGroupNode(CrossSectionPlaneManipulator3D manipulator)
            {
                this.edgeHandle = manipulator.edgeHandle;
                this.cornerHandle = manipulator.cornerHandle;
            }
            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                //Set hit distance to 0 so event manipulator is inside the model, hit test still works
                if (base.OnHitTest(context, totalModelMatrix, ref hits))
                {
                    if (hits.Count > 0)
                    {
                        HitTestResult res = new HitTestResult() { Distance = int.MaxValue };
                        foreach (var hit in hits)
                        {
                            if (hit.ModelHit == cornerHandle || hit.ModelHit == edgeHandle)
                            {
                                if(res.Distance > hit.Distance)
                                {
                                    res = hit;
                                }
                            }
                        }
                        res.Distance = 0;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
