using HelixToolkit;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Fractal.Fractals;

public sealed class MengerSponge : FractalBase
{
    public override GeometryModel3D? Generate()
    {
        // Create the initial unit cube
        var cubeCenters = new List<Point3D>();
        double L = 1;
        cubeCenters.Add(new Point3D(0, 0, 0));

        for (int i = 0; i < Level; i++)
        {
            L /= 3;
            cubeCenters = SubDivide(cubeCenters, L);
        }
        return AddGeometry(cubeCenters, L);
    }

    // Add all cubes to a ModelVisual3D, reuse geometry but create new visual for each cube - this is slow
    /*   GeometryModel3D AddGeometrySeparate(IEnumerable<Point3D> centers, double L)
       {
           var mv = new ModelVisual3D();

           var cubit = new CubeVisual3D { SideLength = L * 0.95, Fill = Brushes.Gold };
           var cuboidGeometry = cubit.Model.Geometry as MeshGeometry3D;
           var r = new Random();

           foreach (var center in centers)
           {
               var tg = new Transform3DGroup();
               tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), (r.NextDouble() - 0.5) * 10)));
               tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), (r.NextDouble() - 0.5) * 10)));
               tg.Children.Add(new TranslateTransform3D(center.ToVector3D()));

               var c = new ModelVisual3D
                           {
                               Content =
                                   new GeometryModel3D
                                       {
                                           Geometry = cuboidGeometry,
                                           Material = cubit.Material,
                                           Transform = tg
                                       }
                           };
               mv.Children.Add(c);
           }
          return mv;
       }*/

    // All cubes in one GeometryModel - much faster
    GeometryModel3D AddGeometry(IEnumerable<Point3D> centers, double L)
    {
        var w = new Stopwatch();
        w.Start();
        /*            var geometry = new MeshGeometry3D();
                    foreach (var center in centers)
                    {
                        MeshGeometryHelper.AddBox(geometry,center, L, L, L);
                    }
                    */

        var builder = new MeshBuilder();
        foreach (var center in centers)
        {
            builder.AddBox(center.ToVector(), (float)L, (float)L, (float)L);
        }
        var geometry = builder.ToMesh().ToMeshGeometry3D();
        geometry.Freeze();

        Trace.WriteLine(Level + ": " + w.ElapsedMilliseconds + " ms");

        var mv = new GeometryModel3D
        {
            Geometry = geometry,
            Material = MaterialHelper.CreateMaterial(Brushes.Gold)
        };
        TriangleCount = geometry.TriangleIndices.Count / 3;

        return mv;
    }

    // SubDivide all cubes with the Menger sponge pattern
    private static List<Point3D> SubDivide(IEnumerable<Point3D> centers, double L)
    {
        var newCenters = new List<Point3D>();

        foreach (var center in centers)
        {
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    for (int k = -1; k < 2; k++)
                    {
                        int c = 0;
                        if (i == 0) c++;
                        if (j == 0) c++;
                        if (k == 0) c++;
                        if (c < 2)
                        {
                            newCenters.Add(new Point3D(center.X + i * L, center.Y + j * L, center.Z + k * L));
                        }
                    }

        }
        return newCenters;
    }
}
