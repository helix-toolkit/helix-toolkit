using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SurfaceDemo;

// http://www.nuclecu.unam.mx/~federico/rt/
public class Torus3D : ParametricSurface3D
{
    const double pi = Math.PI;

    const double R0 = 4;
    const double R1 = 1;
    const double AMP = 0.2;
    const double FREQ = 6;
    const double TURNS = 1;

    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
        double color = u;
        double theta1 = u * 2 * Math.PI;
        double phi1 = v * 2 * Math.PI;

        double r = R1 + AMP * Math.Sin(FREQ * phi1);
        double ang = phi1 + pi * 2 * Math.Sin(TURNS * theta1);

        double tx = (r * Math.Cos(ang)) + R0;
        double z = r * Math.Sin(ang);
        double x = tx * Math.Cos(theta1);
        double y = tx * Math.Sin(theta1);

        texCoord = new Point(color, 0);
        return new Point3D(x, y, z);
    }
}
