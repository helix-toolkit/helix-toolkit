using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Windows.Media.Animation;
using Media3D = System.Windows.Media.Media3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace OctreeDemo;

public partial class SphereModel : DataModel
{
    private static readonly MeshGeometry3D Sphere;
    private static readonly MeshGeometry3D Box;
    private static readonly MeshGeometry3D Pyramid;
    private static readonly MeshGeometry3D Pipe;

    static SphereModel()
    {
        var builder = new MeshBuilder(true, false, false);
        var center = new Vector3();
        builder.AddSphere(center, 1, 12, 12);
        Sphere = builder.ToMeshGeometry3D();
        builder = new MeshBuilder(true, false, false);
        builder.AddBox(center, 1, 1, 1);
        Box = builder.ToMeshGeometry3D();
        builder = new MeshBuilder(true, false, false);
        builder.AddPyramid(center, 1, 1, true);
        Pyramid = builder.ToMeshGeometry3D();
        builder = new MeshBuilder(true, false, false);
        builder.AddPipe(center, (center + new Vector3(0, 1, 0)), 0, 2, 12);
        Pipe = builder.ToMeshGeometry3D();
    }

    private static readonly Random rnd = new();

    public SphereModel(Vector3 center, double radius, bool enableTransform = true)
        : base()
    {
        Center = center;
        Radius = radius;
        CreateModel();
        if (enableTransform)
        {
            CreateAnimatedTransform1(DynamicTransform, center.ToVector3D(), new Media3D.Vector3D(rnd.Next(-1, 1), rnd.Next(-1, 1), rnd.Next(-1, 1)), rnd.Next(10, 100));
        }
        var color = rnd.NextColor();
        Material = new PhongMaterial() { DiffuseColor = color };
    }

    [ObservableProperty]
    private Vector3 center;

    partial void OnCenterChanged(Vector3 value)
    {
        translateTransform.OffsetX = translateTransform.OffsetY = translateTransform.OffsetZ = value.X;
    }

    [ObservableProperty]
    private double radius = 1;

    partial void OnRadiusChanged(double value)
    {
        scaleTransform.ScaleX = scaleTransform.ScaleY = scaleTransform.ScaleZ = value;
    }

    private void CreateModel()
    {
        int type = rnd.Next(0, 3);

        switch (type)
        {
            case 0:
                Model = Sphere;
                break;
            case 1:
                Model = Box;
                break;
            case 2:
                Model = Pyramid;
                break;
            case 3:
                Model = Pipe;
                break;
        }
    }

    private static Media3D.Transform3D CreateAnimatedTransform1(Media3D.Transform3DGroup transformGroup,
        Media3D.Vector3D center, Media3D.Vector3D axis, double speed = 4)
    {

        var rotateAnimation = new Rotation3DAnimation
        {
            RepeatBehavior = RepeatBehavior.Forever,
            By = new Media3D.AxisAngleRotation3D(axis, 90),
            Duration = TimeSpan.FromSeconds(speed / 2),
            IsCumulative = true,
        };

        var rotateTransform = new Media3D.RotateTransform3D();
        rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);

        transformGroup.Children.Add(rotateTransform);

        var rotateAnimation1 = new Rotation3DAnimation
        {
            RepeatBehavior = RepeatBehavior.Forever,
            By = new Media3D.AxisAngleRotation3D(axis, 240),
            Duration = TimeSpan.FromSeconds(speed / 4),
            IsCumulative = true,
        };

        var rotateTransform1 = new Media3D.RotateTransform3D
        {
            CenterX = center.X,
            CenterY = center.Y,
            CenterZ = center.Z
        };
        rotateTransform1.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation1);

        transformGroup.Children.Add(rotateTransform1);

        return transformGroup;
    }
}
