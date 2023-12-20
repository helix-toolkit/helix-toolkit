using HelixToolkit;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Fractal.Fractals;

public sealed class SierpinskiPyramid : FractalBase
{
    public override GeometryModel3D? Generate()
    {
        // Create the initial unit cube
        var centers = new List<Point3D>();
        double l = 1;
        centers.Add(new Point3D(0, 0, -0.5));

        for (int i = 0; i < Level; i++)
        {
            l /= 2;
            centers = SubDivide(centers, l);
        }
        // Count = centers.Count;
        return AddGeometry(centers, l);
    }

    GeometryModel3D AddGeometry(IEnumerable<Point3D> centers, double l)
    {
        var builder = new MeshBuilder();

        foreach (var center in centers)
        {
            builder.AddPyramid(center.ToVector3(), (float)l, (float)l, true);
        }

        var mv = new GeometryModel3D
        {
            Geometry = builder.ToMesh().ToMeshGeometry3D(true),
            Material = MaterialHelper.CreateMaterial(Brushes.Gold)
        };

        TriangleCount = builder.TriangleIndices.Count / 3;

        return mv;
    }

    private static List<Point3D> SubDivide(IEnumerable<Point3D> centers, double l)
    {
        var newCenters = new List<Point3D>();

        foreach (var center in centers)
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                {
                    newCenters.Add(new Point3D(center.X + (i - 0.5) * l, center.Y + (j - 0.5) * l, center.Z));
                }
            newCenters.Add(new Point3D(center.X, center.Y, center.Z + l));

        }
        return newCenters;
    }
}
