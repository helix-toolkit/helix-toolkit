using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SurfaceDemo;

// http://en.wikipedia.org/wiki/Klein_bottle
public class KleinBottle3D : ParametricSurface3D
{
    double sqrt2 = Math.Sqrt(2);
    const double pi = Math.PI;

    public double Cos(double x) { return Math.Cos(x); }
    public double Sin(double x) { return Math.Sin(x); }
    public double Abs(double x) { return Math.Abs(x); }
    public double Sign(double x) { return Math.Sign(x); }
    public double Pow(double x, double y) { return Math.Pow(x, y); }

    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
        double color = u;
        u *= 2 * Math.PI;
        v *= 2 * Math.PI;

        double cosu = Math.Cos(u);
        double sinu = Math.Sin(u);
        double cosv = Math.Cos(v);
        double sinv = Math.Sin(v);
        double cos2u = Math.Cos(2 * u);

        /*
           double fu = f(u);
           double gu = g(u);
           double x = (sqrt2 * fu * cosu * cosv * (3 * cosu * cosu - 1) - 2 * cos2u) / (80 * pi * pi * pi * gu) - (3 * cosu - 3) / 4;
           double y = -fu * sinv / (60 * pi * pi * pi);
           double z = -(sqrt2 * fu * sinu * cosv) / (15 * pi * pi * pi * gu) + (sinu * cosu * cosu + sinu) / 4 - (sinu * cosu) / 2;
        */

        double c1 = Math.Sign(u - pi);
        double c3 = 4 * (1 - cosu / 2);
        double c4 = Math.Max(-c1, 0);
        double c5 = Math.Max(c1, 0);
        double x = 6 * cosu * (1 + sinu) + c4 * c3 * cosu * cosv + c5 * c3 * Math.Cos(v + pi);
        double y = 16 * sinu + c4 * c3 * sinu * cosv;
        double z = c3 * sinv;
        texCoord = new Point(color, 0);
        return new Point3D(x, y, z);
    }

    double F(double u)
    {
        return 20 * u * u * u - 65 * pi * u * u + 50 * pi * pi * u - 16 * pi * pi * pi;
    }

    double G(double u)
    {
        double cosu = Math.Cos(u);
        double sinu = Math.Sin(u);
        double cos2u = Math.Cos(2 * u);
        return Math.Sqrt(8 * cos2u * cos2u - cos2u * (24 * cosu * cosu * cosu - 8 * cosu + 15) * 6 * cosu * cosu * cosu * cosu * (1 - 3 * sinu * sinu) + 17);
    }
}
