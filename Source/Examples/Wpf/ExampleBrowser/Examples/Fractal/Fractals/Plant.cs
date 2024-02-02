using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Geometry;

namespace Fractal.Fractals;

// http://en.wikipedia.org/wiki/Barnsley_fern
// http://www.mathworks.com/moler/exm/chapters/fern.pdf
// pythagoras tree...
// http://www.andrew-hoyer.com/experiments/fractals/

public sealed class Plant : FractalBase
{
    private Material mat = MaterialHelper.CreateMaterial(Brushes.Green);

    public override GeometryModel3D Generate()
    {
        var mesh = new MeshBuilder();

        AddBranch(mesh, new Point3D(0, 0, 0), UpVector, 0);

        var mv = new GeometryModel3D
        {
            Geometry = mesh.ToMesh().ToWndMeshGeometry3D(),
            Material = mat,
            BackMaterial = mat
        };

        TriangleCount = mesh.TriangleIndices.Count / 3;

        return mv;
    }
    /*
            private static readonly Matrix3D T1 = new Matrix3D(
                0.85, -0.04, 0, 0,
                0.04, 0.85, 0, 0,
                0.00, 0.00, 1, 0,
                0.00, 1.60, 0, 1);
            private static readonly Matrix3D T2 = new Matrix3D(
                0.20, 0.23, 0, 0,
               -0.26, 0.22, 0, 0,
                0.00, 0.00, 1, 0,
                0.00, 1.60, 0, 1);
            private static readonly Matrix3D T3 = new Matrix3D(
               -0.15, 0.26, 0, 0,
                0.28, 0.24, 0, 0,
                0.00, 0.00, 1, 0,
                0.00, 0.44, 0, 1);
            */

    private Transform3D T1, T2, T3;

    public Plant()
    {
        var x = new Vector3D(1, 0, 0);
        var r1 = new RotateTransform3D(new AxisAngleRotation3D(x, 80));
        var r2 = new RotateTransform3D(new AxisAngleRotation3D(x, -70));
        var r3 = new RotateTransform3D(new AxisAngleRotation3D(x, -10));
        var t1 = new TranslateTransform3D(0, 0, 0.5);
        var t2 = new TranslateTransform3D(0, 0, 0.7);
        var t3 = new TranslateTransform3D(0, 0, 1.0);
        var s1 = new ScaleTransform3D(0.5, 0.5, 0.5);
        var s2 = new ScaleTransform3D(0.3, 0.3, 0.3);
        var s3 = new ScaleTransform3D(0.8, 0.8, 0.8);
        var m1 = new Transform3DGroup();
        m1.Children.Add(r1);
        m1.Children.Add(s1);
        m1.Children.Add(t1);
        var m2 = new Transform3DGroup();
        m2.Children.Add(r2);
        m2.Children.Add(s2);
        m2.Children.Add(t2);
        var m3 = new Transform3DGroup();
        m3.Children.Add(r3);
        m3.Children.Add(s3);
        m3.Children.Add(t3);
        T1 = m1;
        T2 = m2;
        T3 = m3;
    }

    private static readonly Vector3D UpVector = new Vector3D(0, 0, 1);

    private void AddBranch(MeshBuilder mesh, Point3D p0, Vector3D direction, int p)
    {
        double angle = GetAngleBetween(direction, UpVector);
        bool isStem = angle < 10;

        double h = isStem ? 2.5 : 2;
        double r = (Level + 1 - p) * 0.1;

        mesh.AddCone(p0.ToVector3(), direction.ToVector3(), (float)r, (float)(r * 0.8), (float)h, false, false, 12);
        var p1 = p0 + direction * h;

        if (p == Level)
            return;

        if (isStem)
        {
            var rightVector = direction.FindAnyPerpendicular();
            var t0 = new RotateTransform3D(new AxisAngleRotation3D(rightVector, GetRandom(3)));
            AddBranch(mesh, p1, t0.Transform(direction), p + 1);

            var t1 = new RotateTransform3D(new AxisAngleRotation3D(rightVector, 95 + GetRandom(5)));
            var d1 = t1.Transform(direction);
            int nBranches = 5 + GetRandom(2);
            for (int i = 0; i < nBranches; i++)
            {
                double a = 360.0 * i / nBranches + GetRandom(25);
                var t2 = new RotateTransform3D(new AxisAngleRotation3D(UpVector, a));
                AddBranch(mesh, p1, t2.Transform(d1), p + 1);
            }
        }
        else
        {
            var rightVector = Vector3D.CrossProduct(direction, UpVector);
            var t1 = new RotateTransform3D(new AxisAngleRotation3D(rightVector, -5 + GetRandom(5)));
            var t2 = new RotateTransform3D(new AxisAngleRotation3D(UpVector, 45 + GetRandom(10)));
            var t3 = new RotateTransform3D(new AxisAngleRotation3D(UpVector, -45 + GetRandom(10)));
            var d1 = t1.Transform(direction);
            AddBranch(mesh, p1, d1, p + 1);
            AddBranch(mesh, p1, t2.Transform(d1), p + 1);
            AddBranch(mesh, p1, t3.Transform(d1), p + 1);
        }
    }

    private Random r = new();
    public double GetRandom(double plusMinus)
    {
        return (r.NextDouble() * 2 - 1) * plusMinus;
    }
    public int GetRandom(int plusMinus)
    {
        return r.Next(plusMinus * 2 + 1) - plusMinus;
    }

    private double GetAngleBetween(Vector3D v1, Vector3D v2)
    {
        v1.Normalize();
        v2.Normalize();
        var dp = Vector3D.DotProduct(v1, v2);
        return Math.Acos(dp) * 180 / Math.PI;
    }
}
