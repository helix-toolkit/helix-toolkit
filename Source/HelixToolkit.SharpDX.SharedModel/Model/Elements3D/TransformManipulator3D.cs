﻿using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using System.Diagnostics;

#if false
#elif WINUI
#elif WPF
using HelixToolkit.Wpf.SharpDX.Utilities;
using System.ComponentModel;
using Media3D = System.Windows.Media.Media3D;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class TransformManipulator3D : GroupElement3D
{
    private static readonly Geometry3D TranslationXGeometry;
    private static readonly Geometry3D RotationXGeometry;
    private static readonly Geometry3D ScalingGeometry;

    static TransformManipulator3D()
    {
        var bd = new MeshBuilder();
        var arrowLength = 1.5f;
        bd.AddArrow((Vector3.UnitX * arrowLength), new Vector3(1.2f * arrowLength, 0, 0), 0.08f, 4, 12);
        bd.AddCylinder(Vector3.Zero, (Vector3.UnitX * arrowLength), 0.04f, 12);
        TranslationXGeometry = bd.ToMeshGeometry3D();

        bd = new MeshBuilder();
        var circle = MeshBuilder.GetCircle(32, false);
        var path = circle.Select(x => new Vector3(0, x.X, x.Y)).ToArray();
        bd.AddTube(path, 0.06f, 8, true);
        RotationXGeometry = bd.ToMeshGeometry3D();

        bd = new MeshBuilder();
        bd.AddBox((Vector3.UnitX * 0.8f), 0.15f, 0.15f, 0.15f);
        bd.AddCylinder(Vector3.Zero, (Vector3.UnitX * 0.8f), 0.02f, 4);
        ScalingGeometry = bd.ToMeshGeometry3D();

        TranslationXGeometry.OctreeParameter.MinimumOctantSize = 0.01f;
        TranslationXGeometry.UpdateOctree();

        RotationXGeometry.OctreeParameter.MinimumOctantSize = 0.01f;
        RotationXGeometry.UpdateOctree();

        ScalingGeometry.OctreeParameter.MinimumOctantSize = 0.01f;
        ScalingGeometry.UpdateOctree();
    }

    #region Dependency Properties
    public Element3D Target
    {
        get
        {
            return (Element3D)GetValue(TargetProperty);
        }
        set
        {
            SetValue(TargetProperty, value);
        }
    }

    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register("Target", typeof(Element3D), typeof(TransformManipulator3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.OnTargetChanged(e.NewValue as Element3D);
            }
        }));

    public bool EnableScaling
    {
        get
        {
            return (bool)GetValue(EnableScalingProperty);
        }
        set
        {
            SetValue(EnableScalingProperty, value);
        }
    }

    public static readonly DependencyProperty EnableScalingProperty =
        DependencyProperty.Register("EnableScaling", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.scaleX.IsRendering = (bool)e.NewValue && m.EnableScalingX;
                m.scaleY.IsRendering = (bool)e.NewValue && m.EnableScalingY;
                m.scaleZ.IsRendering = (bool)e.NewValue && m.EnableScalingZ;
            }
        }));

    public bool EnableScalingX
    {
        get
        {
            return (bool)GetValue(EnableScalingXProperty);
        }
        set
        {
            SetValue(EnableScalingXProperty, value);
        }
    }

    public static readonly DependencyProperty EnableScalingXProperty =
        DependencyProperty.Register("EnableScalingX", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.scaleX.IsRendering = (bool)e.NewValue && m.EnableScalingX;
            }
        }));

    public bool EnableScalingY
    {
        get
        {
            return (bool)GetValue(EnableScalingYProperty);
        }
        set
        {
            SetValue(EnableScalingYProperty, value);
        }
    }

    public static readonly DependencyProperty EnableScalingYProperty =
        DependencyProperty.Register("EnableScalingY", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.scaleY.IsRendering = (bool)e.NewValue && m.EnableScaling;
            }
        }));

    public bool EnableScalingZ
    {
        get
        {
            return (bool)GetValue(EnableScalingZProperty);
        }
        set
        {
            SetValue(EnableScalingZProperty, value);
        }
    }

    public static readonly DependencyProperty EnableScalingZProperty =
        DependencyProperty.Register("EnableScalingZ", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.scaleZ.IsRendering = (bool)e.NewValue && m.EnableScaling;
            }
        }));

    public bool EnableTranslation
    {
        get
        {
            return (bool)GetValue(EnableTranslationProperty);
        }
        set
        {
            SetValue(EnableTranslationProperty, value);
        }
    }

    public static readonly DependencyProperty EnableTranslationProperty =
        DependencyProperty.Register("EnableTranslation", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.translationX.IsRendering = (bool)e.NewValue && m.EnableTranslationX;
                m.translationY.IsRendering = (bool)e.NewValue && m.EnableTranslationY;
                m.translationZ.IsRendering = (bool)e.NewValue && m.EnableTranslationZ;
            }
        }));

    public bool EnableTranslationX
    {
        get
        {
            return (bool)GetValue(EnableTranslationXProperty);
        }
        set
        {
            SetValue(EnableTranslationXProperty, value);
        }
    }

    public static readonly DependencyProperty EnableTranslationXProperty =
        DependencyProperty.Register("EnableTranslationX", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.translationX.IsRendering = (bool)e.NewValue && m.EnableTranslation;
            }
        }));

    public bool EnableTranslationY
    {
        get
        {
            return (bool)GetValue(EnableTranslationYProperty);
        }
        set
        {
            SetValue(EnableTranslationYProperty, value);
        }
    }

    public static readonly DependencyProperty EnableTranslationYProperty =
        DependencyProperty.Register("EnableTranslationY", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.translationY.IsRendering = (bool)e.NewValue && m.EnableTranslation;
            }
        }));

    public bool EnableTranslationZ
    {
        get
        {
            return (bool)GetValue(EnableTranslationZProperty);
        }
        set
        {
            SetValue(EnableTranslationZProperty, value);
        }
    }

    public static readonly DependencyProperty EnableTranslationZProperty =
        DependencyProperty.Register("EnableTranslationZ", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.translationZ.IsRendering = (bool)e.NewValue && m.EnableTranslation;
            }
        }));

    public bool EnableRotation
    {
        get
        {
            return (bool)GetValue(EnableRotationProperty);
        }
        set
        {
            SetValue(EnableRotationProperty, value);
        }
    }

    public static readonly DependencyProperty EnableRotationProperty =
        DependencyProperty.Register("EnableRotation", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.rotationX.IsRendering = (bool)e.NewValue && m.EnableRotationX;
                m.rotationY.IsRendering = (bool)e.NewValue && m.EnableRotationY;
                m.rotationZ.IsRendering = (bool)e.NewValue && m.EnableRotationZ;
            }
        }));

    public bool EnableRotationX
    {
        get
        {
            return (bool)GetValue(EnableRotationXProperty);
        }
        set
        {
            SetValue(EnableRotationXProperty, value);
        }
    }

    public static readonly DependencyProperty EnableRotationXProperty =
        DependencyProperty.Register("EnableRotationX", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.rotationX.IsRendering = (bool)e.NewValue && m.EnableRotation;
            }
        }));

    public bool EnableRotationY
    {
        get
        {
            return (bool)GetValue(EnableRotationYProperty);
        }
        set
        {
            SetValue(EnableRotationYProperty, value);
        }
    }

    public static readonly DependencyProperty EnableRotationYProperty =
        DependencyProperty.Register("EnableRotationY", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.rotationY.IsRendering = (bool)e.NewValue && m.EnableRotation;
            }
        }));

    public bool EnableRotationZ
    {
        get
        {
            return (bool)GetValue(EnableRotationZProperty);
        }
        set
        {
            SetValue(EnableRotationZProperty, value);
        }
    }

    public static readonly DependencyProperty EnableRotationZProperty =
        DependencyProperty.Register("EnableRotationZ", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.rotationZ.IsRendering = (bool)e.NewValue && m.EnableRotation;
            }
        }));

    public bool EnableXRayGrid
    {
        get
        {
            return (bool)GetValue(EnableXRayGridProperty);
        }
        set
        {
            SetValue(EnableXRayGridProperty, value);
        }
    }

    public static readonly DependencyProperty EnableXRayGridProperty =
        DependencyProperty.Register("EnableXRayGrid", typeof(bool), typeof(TransformManipulator3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.xrayEffect.IsRendering = (bool)e.NewValue;
            }
        }));

#if false
#elif WINUI
#elif WPF
    [TypeConverter(typeof(Vector3Converter))]
#else
#error Unknown framework
#endif
    public Vector3 CenterOffset
    {
        get
        {
            return (Vector3)GetValue(CenterOffsetProperty);
        }
        set
        {
            SetValue(CenterOffsetProperty, value);
        }
    }

    public static readonly DependencyProperty CenterOffsetProperty =
        DependencyProperty.Register("CenterOffset", typeof(Vector3), typeof(TransformManipulator3D), new PropertyMetadata(Vector3.Zero, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.centerOffset = (Vector3)e.NewValue;
                m.OnUpdateSelfTransform();
            }
        }));

    public double SizeScale
    {
        get
        {
            return (double)GetValue(SizeScaleProperty);
        }
        set
        {
            SetValue(SizeScaleProperty, value);
        }
    }

    public static readonly DependencyProperty SizeScaleProperty =
        DependencyProperty.Register("SizeScale", typeof(double), typeof(TransformManipulator3D), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is TransformManipulator3D m)
            {
                m.sizeScale = (double)e.NewValue;
            }
        }));

#endregion
    #region Variables
    private readonly MeshGeometryModel3D translationX, translationY, translationZ;
    private readonly MeshGeometryModel3D rotationX, rotationY, rotationZ;
    private readonly MeshGeometryModel3D scaleX, scaleY, scaleZ;
    private readonly GroupModel3D translationGroup, rotationGroup, scaleGroup, ctrlGroup;
    private readonly Element3D xrayEffect;
    private Vector3 centerOffset = Vector3.Zero;
    private Vector3 translationVector = Vector3.Zero;
    private Matrix rotationMatrix = Matrix.Identity;
    private Matrix scaleMatrix = Matrix.Identity;
    private Matrix targetMatrix = Matrix.Identity;

    private Element3D? target;
    private Viewport3DX? currentViewport;
    private Vector3 lastHitPosWS;
    private Vector3 normal;

    private Vector3 direction;
    private Vector3 currentHit;
    private bool isCaptured = false;
    private double sizeScale = 1;
    private Color4 currentColor;
    #endregion
    private enum ManipulationType
    {
        None, TranslationX, TranslationY, TranslationZ, RotationX, RotationY, RotationZ, ScaleX, ScaleY, ScaleZ
    }

    private ManipulationType manipulationType = ManipulationType.None;

    public TransformManipulator3D()
    {
        var rotationYMatrix = Matrix.CreateRotationZ((float)Math.PI / 2);
        var rotationZMatrix = Matrix.CreateRotationY(-(float)Math.PI / 2);
        ctrlGroup = new GroupModel3D();
        #region Translation Models
        translationX = new MeshGeometryModel3D() { Geometry = TranslationXGeometry, Material = DiffuseMaterials.Red, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };
        translationY = new MeshGeometryModel3D() { Geometry = TranslationXGeometry, Material = DiffuseMaterials.Green, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };
        translationZ = new MeshGeometryModel3D() { Geometry = TranslationXGeometry, Material = DiffuseMaterials.Blue, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };

#if false
#elif WINUI
        translationY.HxTransform3D = rotationYMatrix;
        translationZ.HxTransform3D = rotationZMatrix;
#elif WPF
        translationY.Transform = new Media3D.MatrixTransform3D(rotationYMatrix.ToMatrix3D());
        translationZ.Transform = new Media3D.MatrixTransform3D(rotationZMatrix.ToMatrix3D());
#else
#error Unknown framework
#endif

        translationX.Mouse3DDown += Translation_Mouse3DDown;
        translationY.Mouse3DDown += Translation_Mouse3DDown;
        translationZ.Mouse3DDown += Translation_Mouse3DDown;
        translationX.Mouse3DMove += Translation_Mouse3DMove;
        translationY.Mouse3DMove += Translation_Mouse3DMove;
        translationZ.Mouse3DMove += Translation_Mouse3DMove;
        translationX.Mouse3DUp += Manipulation_Mouse3DUp;
        translationY.Mouse3DUp += Manipulation_Mouse3DUp;
        translationZ.Mouse3DUp += Manipulation_Mouse3DUp;

        translationGroup = new GroupModel3D();
        translationGroup.Children.Add(translationX);
        translationGroup.Children.Add(translationY);
        translationGroup.Children.Add(translationZ);
        ctrlGroup.Children.Add(translationGroup);
#endregion
        #region Rotation Models
        rotationX = new MeshGeometryModel3D() { Geometry = RotationXGeometry, Material = DiffuseMaterials.Red, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };
        rotationY = new MeshGeometryModel3D() { Geometry = RotationXGeometry, Material = DiffuseMaterials.Green, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };
        rotationZ = new MeshGeometryModel3D() { Geometry = RotationXGeometry, Material = DiffuseMaterials.Blue, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };

#if false
#elif WINUI
        rotationY.HxTransform3D = rotationYMatrix;
        rotationZ.HxTransform3D = rotationZMatrix;
#elif WPF
        rotationY.Transform = new Media3D.MatrixTransform3D(rotationYMatrix.ToMatrix3D());
        rotationZ.Transform = new Media3D.MatrixTransform3D(rotationZMatrix.ToMatrix3D());
#else
#error Unknown framework
#endif

        rotationX.Mouse3DDown += Rotation_Mouse3DDown;
        rotationY.Mouse3DDown += Rotation_Mouse3DDown;
        rotationZ.Mouse3DDown += Rotation_Mouse3DDown;
        rotationX.Mouse3DMove += Rotation_Mouse3DMove;
        rotationY.Mouse3DMove += Rotation_Mouse3DMove;
        rotationZ.Mouse3DMove += Rotation_Mouse3DMove;
        rotationX.Mouse3DUp += Manipulation_Mouse3DUp;
        rotationY.Mouse3DUp += Manipulation_Mouse3DUp;
        rotationZ.Mouse3DUp += Manipulation_Mouse3DUp;

        rotationGroup = new GroupModel3D();
        rotationGroup.Children.Add(rotationX);
        rotationGroup.Children.Add(rotationY);
        rotationGroup.Children.Add(rotationZ);
        ctrlGroup.Children.Add(rotationGroup);
        #endregion
        #region Scaling Models
        scaleX = new MeshGeometryModel3D() { Geometry = ScalingGeometry, Material = DiffuseMaterials.Red, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };
        scaleY = new MeshGeometryModel3D() { Geometry = ScalingGeometry, Material = DiffuseMaterials.Green, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };
        scaleZ = new MeshGeometryModel3D() { Geometry = ScalingGeometry, Material = DiffuseMaterials.Blue, CullMode = CullMode.Back, PostEffects = "ManipulatorXRayGrid" };

#if false
#elif WINUI
        scaleY.HxTransform3D = rotationYMatrix;
        scaleZ.HxTransform3D = rotationZMatrix;
#elif WPF
        scaleY.Transform = new Media3D.MatrixTransform3D(rotationYMatrix.ToMatrix3D());
        scaleZ.Transform = new Media3D.MatrixTransform3D(rotationZMatrix.ToMatrix3D());
#else
#error Unknown framework
#endif

        scaleX.Mouse3DDown += Scaling_Mouse3DDown;
        scaleY.Mouse3DDown += Scaling_Mouse3DDown;
        scaleZ.Mouse3DDown += Scaling_Mouse3DDown;
        scaleX.Mouse3DMove += Scaling_Mouse3DMove;
        scaleY.Mouse3DMove += Scaling_Mouse3DMove;
        scaleZ.Mouse3DMove += Scaling_Mouse3DMove;
        scaleX.Mouse3DUp += Manipulation_Mouse3DUp;
        scaleY.Mouse3DUp += Manipulation_Mouse3DUp;
        scaleZ.Mouse3DUp += Manipulation_Mouse3DUp;

        scaleGroup = new GroupModel3D();
        scaleGroup.Children.Add(scaleX);
        scaleGroup.Children.Add(scaleY);
        scaleGroup.Children.Add(scaleZ);
        ctrlGroup.Children.Add(scaleGroup);
        #endregion
        Children.Add(ctrlGroup);
        xrayEffect = new PostEffectMeshXRayGrid()
        {
            EffectName = "ManipulatorXRayGrid",
            DimmingFactor = 0.5,
            BlendingFactor = 0.8,
            GridDensity = 4,
            GridColor = UIColors.Gray
        };

        if (xrayEffect.SceneNode is NodePostEffectXRayGrid node)
        {
            node.XRayDrawingPassName = DefaultPassNames.EffectMeshDiffuseXRayGridP3;
        }

        Children.Add(xrayEffect);
        SceneNode.Attached += SceneNode_OnAttached;
        SceneNode.Detached += SceneNode_OnDetached;
    }

    private void SceneNode_OnDetached(object? sender, EventArgs e)
    {
        //if (target != null)
        //{
        //    target.SceneNode.OnTransformChanged -= SceneNode_OnTransformChanged;
        //}
    }

    private void SceneNode_OnAttached(object? sender, EventArgs e)
    {
        OnTargetChanged(target);
    }

    protected virtual bool CanBeginTransform(MouseDown3DEventArgs? e)
    {
        return true;
    }

    #region Handle Translation
    private void Translation_Mouse3DDown(object? sender, MouseDown3DEventArgs? e)
    {
        if (target == null || !CanBeginTransform(e))
        {
            return;
        }
        if (e?.HitTestResult?.ModelHit is not Element3D elem)
        {
            manipulationType = ManipulationType.None;
            isCaptured = false;
            return;
        }
        if (elem == translationX)
        {
            manipulationType = ManipulationType.TranslationX;
            direction = Vector3.UnitX;
        }
        else if (elem == translationY)
        {
            manipulationType = ManipulationType.TranslationY;
            direction = Vector3.UnitY;
        }
        else if (elem == translationZ)
        {
            manipulationType = ManipulationType.TranslationZ;
            direction = Vector3.UnitZ;
        }
        else
        {
            manipulationType = ManipulationType.None;
            isCaptured = false;
            return;
        }
        var material = (e.HitTestResult.ModelHit as MeshGeometryModel3D)?.Material as DiffuseMaterial;

        if (material is not null)
        {
            currentColor = material.DiffuseColor;
            material.DiffuseColor = Color.Yellow;
        }

        currentViewport = e.Viewport;
        if (currentViewport?.Camera is null)
        {
            return;
        }

        var cameraNormal = Vector3.Normalize(currentViewport.Camera.CameraInternal.LookDirection);
        this.lastHitPosWS = e.HitTestResult.PointHit;
        var up = Vector3.Cross(cameraNormal, direction);
        normal = Vector3.Cross(up, direction);
        if (currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal, out var hit))
        {
            currentHit = hit;
            isCaptured = true;
        }
    }

    private void Translation_Mouse3DMove(object? sender, MouseMove3DEventArgs? e)
    {
        if (!isCaptured || currentViewport is null || e is null)
        {
            return;
        }

        if (currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal, out var hit))
        {
            var moveDir = hit - currentHit;
            currentHit = hit;
            switch (manipulationType)
            {
                case ManipulationType.TranslationX:
                    translationVector += new Vector3(moveDir.X, 0, 0);
                    break;
                case ManipulationType.TranslationY:
                    translationVector += new Vector3(0, moveDir.Y, 0);
                    break;
                case ManipulationType.TranslationZ:
                    translationVector += new Vector3(0, 0, moveDir.Z);
                    break;
            }
            OnUpdateSelfTransform();
            OnUpdateTargetMatrix();
        }
    }
    #endregion

    #region Handle Rotation
    private void Rotation_Mouse3DDown(object? sender, MouseDown3DEventArgs? e)
    {
        if (target == null || !CanBeginTransform(e))
        {
            return;
        }
        if (e?.HitTestResult?.ModelHit is not Element3D elem)
        {
            manipulationType = ManipulationType.None;
            isCaptured = false;
            return;
        }
        if (elem == rotationX)
        {
            manipulationType = ManipulationType.RotationX;
            direction = new Vector3(1, 0, 0);
        }
        else if (elem == rotationY)
        {
            manipulationType = ManipulationType.RotationY;
            direction = new Vector3(0, 1, 0);
        }
        else if (elem == rotationZ)
        {
            manipulationType = ManipulationType.RotationZ;
            direction = new Vector3(0, 0, 1);
        }
        else
        {
            manipulationType = ManipulationType.None;
            isCaptured = false;
            return;
        }
        var material = (e.HitTestResult.ModelHit as MeshGeometryModel3D)?.Material as DiffuseMaterial;
        if (material is not null)
        {
            currentColor = material.DiffuseColor;
            material.DiffuseColor = Color.Yellow;
        }
        currentViewport = e.Viewport;
        if (currentViewport?.Camera is null)
        {
            return;
        }
        normal = Vector3.Normalize(currentViewport.Camera.CameraInternal.LookDirection);
        this.lastHitPosWS = e.HitTestResult.PointHit;
        //var up = Vector3.Cross(cameraNormal, direction);
        //normal = Vector3.Cross(up, direction);
        if (currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal, out var hit))
        {
            currentHit = hit;
            isCaptured = true;
        }
    }

    private void Rotation_Mouse3DMove(object? sender, MouseMove3DEventArgs? e)
    {
        if (!isCaptured || currentViewport is null || e is null)
        {
            return;
        }
        if (currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal, out var hit))
        {
            var position = this.translationVector + centerOffset;
            var v = Vector3.Normalize(currentHit - position);
            var u = Vector3.Normalize(hit - position);
            var currentAxis = Vector3.Cross(u, v);
            var axis = Vector3.UnitX;
            currentHit = hit;
            switch (manipulationType)
            {
                case ManipulationType.RotationX:
                    axis = Vector3.UnitX;
                    break;
                case ManipulationType.RotationY:
                    axis = Vector3.UnitY;
                    break;
                case ManipulationType.RotationZ:
                    axis = Vector3.UnitZ;
                    break;
            }
            var sign = -Vector3.Dot(axis, currentAxis);
            var theta = (float)(Math.Sign(sign) * Math.Asin(currentAxis.Length()));
            switch (manipulationType)
            {
                case ManipulationType.RotationX:
                    rotationMatrix *= Matrix.CreateRotationX(theta);
                    break;
                case ManipulationType.RotationY:
                    rotationMatrix *= Matrix.CreateRotationY(theta);
                    break;
                case ManipulationType.RotationZ:
                    rotationMatrix *= Matrix.CreateRotationZ(theta);
                    break;
            }
            OnUpdateTargetMatrix();
        }
    }
    #endregion

    #region Handle Scaling
    private void Scaling_Mouse3DDown(object? sender, MouseDown3DEventArgs? e)
    {
        if (target == null || !CanBeginTransform(e))
        {
            return;
        }
        if (e?.HitTestResult?.ModelHit is not Element3D elem)
        {
            manipulationType = ManipulationType.None;
            isCaptured = false;
            return;
        }
        if (elem == scaleX)
        {
            manipulationType = ManipulationType.ScaleX;
            direction = Vector3.UnitX;
        }
        else if (elem == scaleY)
        {
            manipulationType = ManipulationType.ScaleY;
            direction = Vector3.UnitY;
        }
        else if (elem == scaleZ)
        {
            manipulationType = ManipulationType.ScaleZ;
            direction = Vector3.UnitZ;
        }
        else
        {
            manipulationType = ManipulationType.None;
            isCaptured = false;
            return;
        }
        var material = (e.HitTestResult.ModelHit as MeshGeometryModel3D)?.Material as DiffuseMaterial;
        if (material is not null)
        {
            currentColor = material.DiffuseColor;
            material.DiffuseColor = Color.Yellow;
        }
        currentViewport = e.Viewport;
        if (currentViewport?.Camera is null)
        {
            return;
        }
        var cameraNormal = Vector3.Normalize(currentViewport.Camera.CameraInternal.LookDirection);
        this.lastHitPosWS = e.HitTestResult.PointHit;
        var up = Vector3.Cross(cameraNormal, direction);
        normal = Vector3.Cross(up, direction);
        if (currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal, out var hit))
        {
            currentHit = hit;
            isCaptured = true;
        }
    }

    private void Scaling_Mouse3DMove(object? sender, MouseMove3DEventArgs? e)
    {
        if (!isCaptured || currentViewport is null || e is null)
        {
            return;
        }
        if (currentViewport.UnProjectOnPlane(e.Position.ToVector2(), lastHitPosWS, normal, out var hit))
        {
            var moveDir = hit - currentHit;
            currentHit = hit;
            var orgAxis = Vector3.Zero;
            float scale = 1;
            switch (manipulationType)
            {
                case ManipulationType.ScaleX:
                    orgAxis = Vector3.UnitX;
                    scale = moveDir.X;
                    break;
                case ManipulationType.ScaleY:
                    orgAxis = Vector3.UnitY;
                    scale = moveDir.Y;
                    break;
                case ManipulationType.ScaleZ:
                    orgAxis = Vector3.UnitZ;
                    scale = moveDir.Z;
                    break;
            }
            var axisX = Vector3.TransformNormal(Vector3.UnitX, rotationMatrix);
            var axisY = Vector3.TransformNormal(Vector3.UnitY, rotationMatrix);
            var axisZ = Vector3.TransformNormal(Vector3.UnitZ, rotationMatrix);
            var dotX = Vector3.Dot(axisX, orgAxis);
            var dotY = Vector3.Dot(axisY, orgAxis);
            var dotZ = Vector3.Dot(axisZ, orgAxis);
            scaleMatrix.M11 += scale * Math.Abs(dotX);
            scaleMatrix.M22 += scale * Math.Abs(dotY);
            scaleMatrix.M33 += scale * Math.Abs(dotZ);
            OnUpdateTargetMatrix();
        }
    }
    #endregion

    private void Manipulation_Mouse3DUp(object? sender, MouseUp3DEventArgs? e)
    {
        if (isCaptured)
        {
            var material = (e?.HitTestResult?.ModelHit as MeshGeometryModel3D)?.Material as DiffuseMaterial;
            if (material is not null)
            {
                material.DiffuseColor = currentColor;
            }
        }
        manipulationType = ManipulationType.None;
        isCaptured = false;
    }

    private void ResetTransforms()
    {
        scaleMatrix = rotationMatrix = targetMatrix = Matrix.Identity;
        translationVector = Vector3.Zero;
        OnUpdateSelfTransform();
    }

    /// <summary>
    /// Called when [target changed]. Use target boundingbox center as Manipulator center
    /// </summary>
    /// <param name="target">The target.</param>
    private void OnTargetChanged(Element3D? target)
    {
        Debug.WriteLine("OnTargetChanged");
        //if(target != null)
        //{
        //    target.SceneNode.OnTransformChanged -= SceneNode_OnTransformChanged;
        //}
        this.target = target;
        if (target == null)
        {
            ResetTransforms();
        }
        else
        {
            //target.SceneNode.OnTransformChanged += SceneNode_OnTransformChanged;
            SceneNode_OnTransformChanged(target.SceneNode, new TransformArgs(target.SceneNode.ModelMatrix));
        }
    }

    private void SceneNode_OnTransformChanged(object? sender, TransformArgs e)
    {
        var m = e.Transform;
        Matrix.Decompose(m, out var scale, out var rotation, out var translation);
        scaleMatrix = Matrix.CreateScale(scale);
        rotationMatrix = rotation.ToMatrix();
        if (centerOffset != Vector3.Zero)
        {
            var org = Matrix.CreateTranslation(-centerOffset) * scaleMatrix * rotationMatrix * Matrix.CreateTranslation(centerOffset);
            translationVector = translation - org.Translation;
        }
        else
        {
            translationVector = m.Translation;
        }
        OnUpdateSelfTransform();
        //OnUpdateTargetMatrix();
    }

    private void OnUpdateTargetMatrix()
    {
        if (target == null)
        {
            return;
        }
        targetMatrix = Matrix.CreateTranslation(-centerOffset) * scaleMatrix * rotationMatrix * Matrix.CreateTranslation(centerOffset) * Matrix.CreateTranslation(translationVector);
#if false
#elif WINUI
        target.HxTransform3D = targetMatrix;
#elif WPF
        target.Transform = new Media3D.MatrixTransform3D(targetMatrix.ToMatrix3D());
#else
#error Unknown framework
#endif
    }

    private void OnUpdateSelfTransform()
    {
        var m = Matrix.CreateTranslation(centerOffset + translationVector);
        m.M11 = m.M22 = m.M33 = (float)sizeScale;
#if false
#elif WINUI
        ctrlGroup.HxTransform3D = m;
#elif WPF
        ctrlGroup.Transform = new Media3D.MatrixTransform3D(m.ToMatrix3D());
#else
#error Unknown framework
#endif
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new AlwaysHitGroupNode(this);
    }

    private sealed class AlwaysHitGroupNode : GroupNode
    {
        private readonly HashSet<object> models = new();
        private readonly TransformManipulator3D manipulator;
        public AlwaysHitGroupNode(TransformManipulator3D manipulator)
        {
            this.manipulator = manipulator;
        }

        protected override bool OnAttach(IEffectsManager effectsManager)
        {
            models.Add(manipulator.translationX);
            models.Add(manipulator.translationY);
            models.Add(manipulator.translationZ);
            models.Add(manipulator.rotationX);
            models.Add(manipulator.rotationY);
            models.Add(manipulator.rotationZ);
            models.Add(manipulator.scaleX);
            models.Add(manipulator.scaleY);
            models.Add(manipulator.scaleZ);
            return base.OnAttach(effectsManager);
        }
        protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
        {
            //Set hit distance to 0 so event manipulator is inside the model, hit test still works
            if (base.OnHitTest(context, totalModelMatrix, ref hits))
            {
                if (hits.Count > 0)
                {
                    var res = new HitTestResult() { Distance = float.MaxValue };
                    foreach (var hit in hits)
                    {
                        if (hit?.ModelHit is null)
                        {
                            continue;
                        }

                        if (models.Contains(hit.ModelHit))
                        {
                            if (hit.Distance < res.Distance)
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
