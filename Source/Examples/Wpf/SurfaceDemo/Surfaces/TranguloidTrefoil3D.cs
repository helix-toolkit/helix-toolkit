using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SurfaceDemo;

// http://root.cern.ch/root/html/tutorials/gl/glparametric.C.html
// http://local.wasp.uwa.edu.au/~pbourke/geometry/tranguloid/
public class TranguloidTrefoil3D : ParametricSurface3D
{
    const double pi = Math.PI;

    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
        double color = u;
        u = (u - 0.5) * 2 * Math.PI;
        v = (v - 0.5) * 2 * Math.PI;

        double cosu = Math.Cos(u);
        double sinu = Math.Sin(u);
        double cosv = Math.Cos(v);
        double sinv = Math.Sin(v);
        double sin2u = Math.Sin(2 * u);
        double cos2u = Math.Cos(2 * u);

        double x = 2 * Math.Sin(3 * u) / (2 + cosv);
        double y = 2 * (sinu + 2 * sin2u) / (2 + Math.Cos(v + 2 * pi / 3));
        double z = (cosu - 2 * cos2u) * (2 + cosv) * (2 + Math.Cos(v + 2 * pi / 3)) / 4;

        texCoord = new Point(color, 0);
        return new Point3D(x, y, z);
    }
}
