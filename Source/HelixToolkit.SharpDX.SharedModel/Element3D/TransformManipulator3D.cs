/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
#if NETFX_CORE
using Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public class TransformManipulator3D : GroupElement3D
    {
        private static readonly Geometry3D TranslationXGeometry;
        private static readonly Geometry3D TranslationYGeometry;
        private static readonly Geometry3D TranslationZGeometry;
        private static readonly Geometry3D RotationXGeometry;
        private static readonly Geometry3D RotationYGeometry;
        private static readonly Geometry3D RotationZGeometry;


        static TransformManipulator3D()
        {
            var bd = new MeshBuilder();
            bd.AddArrow(Vector3.Zero, new Vector3(1, 0, 0), 0.1, 0.2, 12);
            TranslationXGeometry = bd.ToMesh();
            bd = new MeshBuilder();
            bd.AddArrow(Vector3.Zero, new Vector3(0, 1, 0), 0.1, 0.2, 12);
            TranslationYGeometry = bd.ToMesh();
            bd = new MeshBuilder();
            bd.AddArrow(Vector3.Zero, new Vector3(0, 0, 1), 0.1, 0.2, 12);
            TranslationZGeometry = bd.ToMesh();

            bd = new MeshBuilder();
            var p0 = new Vector3(1, 0, 0);
            var p1 = p0 - (0.1f * 0.5f);
            var p2 = p0 + (0.1f * 0.5f);
            bd.AddPipe(p1, p2, 0.8f, 1, 32);
            RotationXGeometry = bd.ToMesh();

            bd = new MeshBuilder();
            p0 = new Vector3(0, 1, 0);
            p1 = p0 - (0.1f * 0.5f);
            p2 = p0 + (0.1f * 0.5f);
            bd.AddPipe(p1, p2, 0.8f, 1, 32);
            RotationYGeometry = bd.ToMesh();

            bd = new MeshBuilder();
            p0 = new Vector3(0, 0, 1);
            p1 = p0 - (0.1f * 0.5f);
            p2 = p0 + (0.1f * 0.5f);
            bd.AddPipe(p1, p2, 0.8f, 1, 32);
            RotationZGeometry = bd.ToMesh();


            TranslationXGeometry.OctreeParameter.MinimumOctantSize = 0.1f;
            TranslationXGeometry.UpdateOctree();
            TranslationYGeometry.OctreeParameter.MinimumOctantSize = 0.1f;
            TranslationYGeometry.UpdateOctree();
            TranslationZGeometry.OctreeParameter.MinimumOctantSize = 0.1f;
            TranslationZGeometry.UpdateOctree();

            RotationXGeometry.OctreeParameter.MinimumOctantSize = 0.1f;
            RotationXGeometry.UpdateOctree();
            RotationYGeometry.OctreeParameter.MinimumOctantSize = 0.1f;
            RotationYGeometry.UpdateOctree();
            RotationZGeometry.OctreeParameter.MinimumOctantSize = 0.1f;
            RotationZGeometry.UpdateOctree();
        }

        public Element3D Target
        {
            get { return (Element3D)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }


        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(Element3D), typeof(TransformManipulator3D), new PropertyMetadata(null, (d,e)=>
            {
                (d as TransformManipulator3D).OnTargetChanged(e.NewValue as Element3D);
            }));



        private readonly MeshGeometryModel3D translationX;
        private readonly MeshGeometryModel3D translationY;
        private readonly MeshGeometryModel3D translationZ;

        private Matrix translationMatrix = Matrix.Identity;
        private Matrix rotationMatrix = Matrix.Identity;
        private Matrix scaleMatrix = Matrix.Identity;

        private Viewport3DX currentViewport;
        private Vector3 lastHitPosWS;
        private Vector3 normal;
        private Vector3 direction;
        private Vector3? currentHit;

        private enum ManipulationType
        {
            None, TranslationX, TranslationY, TranslationZ, RotationX, RotationY, RotationZ, ScaleX, ScaleY, ScaleZ
        }

        private ManipulationType manipulationType = ManipulationType.None;

        public TransformManipulator3D()
        {
            translationX = new MeshGeometryModel3D() { Geometry = TranslationXGeometry, Material = DiffuseMaterials.Red, CullMode = CullMode.Back };
            translationY = new MeshGeometryModel3D() { Geometry = TranslationYGeometry, Material = DiffuseMaterials.Green, CullMode = CullMode.Back };
            translationZ = new MeshGeometryModel3D() { Geometry = TranslationZGeometry, Material = DiffuseMaterials.Blue, CullMode = CullMode.Back };
            translationX.Mouse3DDown += Translation_Mouse3DDown;
            translationY.Mouse3DDown += Translation_Mouse3DDown;
            translationZ.Mouse3DDown += Translation_Mouse3DDown;
            translationX.Mouse3DMove += Translation_Mouse3DMove;
            translationY.Mouse3DMove += Translation_Mouse3DMove;
            translationZ.Mouse3DMove += Translation_Mouse3DMove;

            Children.Add(translationX);
            Children.Add(translationY);
            Children.Add(translationZ);
        }

        private void Translation_Mouse3DDown(object sender, MouseDown3DEventArgs e)
        {
            if(e.HitTestResult.ModelHit == translationX)
            {
                manipulationType = ManipulationType.TranslationX;
                direction = new Vector3(1, 0, 0);
            }
            else if(e.HitTestResult.ModelHit == translationY)
            {
                manipulationType = ManipulationType.TranslationY;
                direction = new Vector3(0, 1, 0);
            }
            else
            {
                manipulationType = ManipulationType.TranslationZ;
                direction = new Vector3(0, 0, 1);
            }
            currentViewport = e.Viewport;
            var cameraNormal = Vector3.Normalize(e.Viewport.Camera.CameraInternal.LookDirection);
            this.lastHitPosWS = e.HitTestResult.PointHit;
            var up = Vector3.Cross(cameraNormal, direction);
            normal = Vector3.Cross(up, direction);
            currentHit = currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal);
        }

        private void Translation_Mouse3DMove(object sender, MouseMove3DEventArgs e)
        {
            var hit = currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal);
            var moveDir = hit - currentHit;
            currentHit = hit;
        }

        private void Translation_Mouse3DUp(object sender, MouseUp3DEventArgs e)
        {
            manipulationType = ManipulationType.None;
        }

        private void OnTargetChanged(Element3D target)
        {

        }

        private void OnUpdateMatrix()
        {

        }
    }
}
