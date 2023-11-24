using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SurfaceDemo;

// http://en.wikipedia.org/wiki/Klein_bottle
public class HelicalSurface3D : ParametricSurface3D
{
    public Point3D BasicCurve(double u)
    {
        return new Point3D(4 * u * u * u - 2 * u * u + 5 * u, 2 * u - 5, 10 * u);
    }

    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
        double color = u;
        // u *= 2 * Math.PI;
        v *= 2 * Math.PI;
        v *= 10;

        double cosu = Math.Cos(u);
        double sinu = Math.Sin(u);
        double cosv = Math.Cos(v);
        double sinv = Math.Sin(v);
        double cos2u = Math.Cos(2 * u);
        double d = 1;

        Point3D b = BasicCurve(u);

        double x = b.X * cosv - b.Y * sinv;
        double y = b.X * sinv + b.Y * cosv;
        double z = b.Z + d * v;
        texCoord = new Point(color, 0);
        return new Point3D(x, y, z);
    }
}
