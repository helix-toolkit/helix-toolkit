using HelixToolkit;
using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Export;

// http://www.elfwood.com/farp/figure/williamlibodyconstruction.html
// http://en.wikipedia.org/wiki/Body_proportions
// http://en.wikipedia.org/wiki/Human_skeleton
// http://figure-drawings.com/How-to-Draw-Proportions.html
// http://hypertextbook.com/facts/2006/bodyproportions.shtml

public sealed class StickFigure : ModelVisual3D
{
    private readonly Material material = MaterialHelper.CreateMaterial(Brushes.Sienna);

    public StickFigure()
    {
        double h = 1.85;

        // the following properties could be dependency properties (bindable and animatable...)

        double ShoulderWidth = 0.5;
        double PelvisWidth = 0.4;

        double BodyAngle = 15;
        double NeckAngle = -40;
        double NeckSideAngle = 9;

        double LeftArmAngle = 0;
        double LeftArmOutAngle = -45;
        double LeftForeArmAngle = 20;
        double LeftHandAngle = -50;

        double RightArmAngle = 0;
        double RightArmOutAngle = -45;
        double RightForeArmAngle = 20;
        double RightHandAngle = -50;

        double LeftThighAngle = 45;
        double LeftThighOutAngle = 5;
        double LeftLegAngle = 45;
        double LeftFootAngle = -10;

        double RightThighAngle = -45;
        double RightThighOutAngle = 5;
        double RightLegAngle = 45;
        double RightFootAngle = 20;

        var figure = new Model3DGroup();

        Model3DGroup body = Group("body", new Vector3D(0, 0, 0), BodyAngle, 0, 0);
        figure.Children.Add(body);
        body.Children.Add(Box(PelvisWidth, 0.2, h * 0.25));

        Model3DGroup upperBody = Group("upperBody", new Vector3D(0, 0, h * 0.25), 0, 0, 0);
        body.Children.Add(upperBody);
        upperBody.Children.Add(Box(ShoulderWidth, 0.2, h * 0.25));

        Model3DGroup neck = Group("neck", new Vector3D(0, 0, h * 0.5), NeckAngle, NeckSideAngle, 0);
        body.Children.Add(neck);

        neck.Children.Add(Box(0.1, 0.1, h * 0.05));

        Model3DGroup head = Group("head", new Vector3D(0, 0, h * 0.05), 0, 0, 0);
        neck.Children.Add(head);
        head.Children.Add(Box(0.2, 0.25, 0.35));

        Model3DGroup nose = Group("nose", new Vector3D(0, 0.1, 0.175), 0, 0, 0);
        head.Children.Add(nose);
        nose.Children.Add(Box(0.05, 0.05, 0.05));

        Model3DGroup leftArm = Group("leftArm", new Vector3D(-ShoulderWidth / 2, 0, h * 0.5), LeftArmAngle,
                                     LeftArmOutAngle, 0);
        body.Children.Add(leftArm);
        leftArm.Children.Add(Box(0.1, 0.1, h * 0.25));

        Model3DGroup leftForeArm = Group("leftForeArm", new Vector3D(0, 0, h * 0.25), 0, LeftForeArmAngle, 0);
        leftArm.Children.Add(leftForeArm);
        leftForeArm.Children.Add(Box(0.08, 0.08, h * 0.2));

        Model3DGroup leftHand = Group("leftHand", new Vector3D(0, 0, h * 0.2), LeftHandAngle, 0, 0);
        leftForeArm.Children.Add(leftHand);
        leftHand.Children.Add(Box(0.03, 0.08, h * 0.05));

        Model3DGroup rightArm = Group("rightArm", new Vector3D(ShoulderWidth / 2, 0, h * 0.5), RightArmAngle,
                                      -RightArmOutAngle, 0);
        body.Children.Add(rightArm);
        rightArm.Children.Add(Box(0.1, 0.1, h * 0.25));

        Model3DGroup rightForeArm = Group("rightForeArm", new Vector3D(0, 0, h * 0.25), 0, -RightForeArmAngle, 0);
        rightArm.Children.Add(rightForeArm);
        rightForeArm.Children.Add(Box(0.08, 0.08, h * 0.2));

        Model3DGroup rightHand = Group("rightHand", new Vector3D(0, 0, h * 0.2), RightHandAngle, 0, 0);
        rightForeArm.Children.Add(rightHand);
        rightHand.Children.Add(Box(0.03, 0.08, h * 0.05));

        Model3DGroup leftThigh = Group("leftThigh", new Vector3D(-PelvisWidth / 2, 0, 0), 180 - LeftThighAngle,
                                       -LeftThighOutAngle, 0);
        body.Children.Add(leftThigh);
        leftThigh.Children.Add(Box(0.18, 0.18, h * 0.25));

        Model3DGroup leftLeg = Group("leftLeg", new Vector3D(0, 0, h * 0.25), LeftLegAngle, 0, 0);
        leftThigh.Children.Add(leftLeg);
        leftLeg.Children.Add(Box(0.12, 0.12, h * 0.2));

        Model3DGroup leftFoot = Group("leftFoot", new Vector3D(0, 0, h * 0.2), -90 + LeftFootAngle, 0, 0);
        leftLeg.Children.Add(leftFoot);
        leftFoot.Children.Add(Box(0.1, 0.1, 0.25));

        Model3DGroup rightThigh = Group("rightThigh", new Vector3D(PelvisWidth / 2, 0, 0), 180 - RightThighAngle,
                                        RightThighOutAngle, 0);
        body.Children.Add(rightThigh);
        rightThigh.Children.Add(Box(0.18, 0.18, h * 0.25));

        Model3DGroup rightLeg = Group("rightLeg", new Vector3D(0, 0, h * 0.25), RightLegAngle, 0, 0);
        rightThigh.Children.Add(rightLeg);
        rightLeg.Children.Add(Box(0.12, 0.12, h * 0.2));

        Model3DGroup rightFoot = Group("rightFoot", new Vector3D(0, 0, h * 0.2), -90 + RightFootAngle, 0, 0);
        rightLeg.Children.Add(rightFoot);
        rightFoot.Children.Add(Box(0.1, 0.1, 0.25));

        Content = figure;
    }

    private static Model3DGroup Group(string name, Vector3D origin, double xAngle, double yAngle, double zAngle)
    {
        var part = new Model3DGroup();
        part.SetValue(FrameworkElement.NameProperty, name);
        var tg = new Transform3DGroup();
        tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), zAngle)));
        tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), yAngle)));
        tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(-1, 0, 0), xAngle)));
        tg.Children.Add(new TranslateTransform3D(origin));
        part.Transform = tg;
        return part;
    }

    private GeometryModel3D Box(double width, double length, double height)
    {
        var model = new GeometryModel3D();
        model.SetValue(FrameworkElement.NameProperty, "box");
        var mb = new MeshBuilder(false, false);
        mb.AddBox(new Point3D(0, 0, height * 0.5).ToVector(), (float)width, (float)length, (float)height);
        model.Geometry = mb.ToMesh().ToMeshGeometry3D();
        model.Material = material;
        return model;
    }
}
