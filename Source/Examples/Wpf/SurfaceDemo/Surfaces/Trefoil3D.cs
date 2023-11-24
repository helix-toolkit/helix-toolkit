using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SurfaceDemo;

// from surfx3d
public class Trefoil3D : ParametricSurface3D
{
    const double pi = Math.PI;

    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
        double color = u;
        u *= 4 * Math.PI;
        v = (v - 0.5) * 2 * Math.PI;

        double cosu = Math.Cos(u);
        double sinu = Math.Sin(u);
        double cosv = Math.Cos(v);
        double sinv = Math.Sin(v);
        double sin2u = Math.Sin(2 * u);
        double cos2u = Math.Cos(2 * u);

        double c1 = 10;
        double c2 = 2;
        double c3 = c1 * (Math.Cos(3 * u / 2) + 3) / 4;
        double c4 = -c3 * sinu - 3 * c1 * Math.Sin(3 * u / 2) * cosu / 8;
        double c5 = c3 * cosu - 3 * c1 * Math.Sin(3 * u / 2) * sinu / 8;
        double c6 = (3 * c3 * Math.Cos(Math.Sin(3 * u / 2)) * Math.Cos(3 * u / 2)) / 2 - (3 * c1 * Math.Sin(Math.Sin(3 * u / 2)) * Math.Sin(3 * u / 2)) / 8;
        double c7 = Math.Sqrt(c4 * c4 + c5 * c5);
        double c8 = Math.Sqrt(c4 * c4 + c5 * c5 + c6 * c6);

        double x = c3 * cosu + (c2 * (c8 * cosv * c5 - sinv * c4 * c6) / (c7 * c8));
        double y = c3 * sinu - (c2 * (c8 * cosv * c4 + sinv * c5 * c6) / (c7 * c8));
        double z = c3 * Math.Sin(Math.Sin(3 * u / 2)) + (c2 * sinv * c7 / c8);

        texCoord = new Point(color, 0);
        return new Point3D(x, y, z);
    }
}
