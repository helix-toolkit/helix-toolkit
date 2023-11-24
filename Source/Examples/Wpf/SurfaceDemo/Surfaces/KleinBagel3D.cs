using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SurfaceDemo;

// http://en.wikipedia.org/wiki/Klein_bottle
public class KleinBagel3D : ParametricSurface3D
{
    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
        double color = u;
        u *= 2 * Math.PI;
        v *= 2 * Math.PI;
        double r = 2;

        double x = (r + Math.Cos(0.5 * u) * Math.Sin(v) - Math.Sin(0.5 * u) * Math.Sin(2 * v)) * Math.Cos(u);
        double y = (r + Math.Cos(0.5 * u) * Math.Sin(v) - Math.Sin(0.5 * u) * Math.Sin(2 * v)) * Math.Sin(u);
        double z = Math.Sin(0.5 * u) * Math.Sin(v) + Math.Cos(0.5 * u) * Math.Sin(2 * v);
        texCoord = new Point(color, 0);
        return new Point3D(x, y, z);
    }
}
