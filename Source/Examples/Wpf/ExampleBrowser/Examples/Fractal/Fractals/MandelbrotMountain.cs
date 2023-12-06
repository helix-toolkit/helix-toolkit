using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Fractal.Fractals;

// http://www.skytopia.com/project/fractal/mandelbrot.html
// http://www.relativitybook.com/CoolStuff/erkfractals.html
public sealed class MandelbrotMountain : FractalBase
{
    private int _maxIterations;
    public int MaxIterations
    {
        get { return _maxIterations; }
        set { _maxIterations = value; Generate(); }
    }

    public double HeightFactor { get; set; }
    private MandelbrotSolver solver;

    private Material gradientMaterial;

    public MandelbrotMountain()
    {
        solver = new MandelbrotSolver();

        var brush = new LinearGradientBrush();
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(1, 0);
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 7, 100), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(32, 107, 203), 0.15));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(237, 255, 255), 0.42));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 170, 0), 0.64));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 0, 0), 0.854));
        //      brush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 7, 100), 1.0));
        gradientMaterial = MaterialHelper.CreateMaterial(brush, null, Brushes.Gray, 1.0, 200);

        MaxIterations = 32;
        HeightFactor = 0.3;

        Generate();
    }

    public override GeometryModel3D? Generate()
    {
        if (solver == null)
            return null;

        solver.Width = 75 * Level;
        solver.Height = 70 * Level;
        solver.MaxIterations = MaxIterations;
        solver.Update();

        return CreateModel();
    }

    private MeshGeometry3D CreateMesh()
    {
        var geometry = new MeshGeometry3D();
        int nx = solver.Width;
        int ny = solver.Height;
        int ij = 0;
        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                double z = (double)solver.ImageBuffer![ij] / solver.MaxIterations;
                geometry.Positions.Add(new Point3D(solver.Xvalues![ij], solver.Yvalues![ij], z * HeightFactor));
                double u = z * 0.854;
                geometry.TextureCoordinates.Add(new Point(u, 0));
                ij++;
            }
        }

        ij = 0;
        for (int i = 0; i < ny - 1; i++)
        {
            for (int j = 0; j < nx - 1; j++)
            {
                geometry.TriangleIndices.Add(ij);
                geometry.TriangleIndices.Add(ij + 1);
                geometry.TriangleIndices.Add(ij + nx);
                geometry.TriangleIndices.Add(ij + 1);
                geometry.TriangleIndices.Add(ij + 1 + nx);
                geometry.TriangleIndices.Add(ij + nx);
                ij++;
            }
            ij++;
        }
        return geometry;
    }

    GeometryModel3D CreateModel()
    {
        var geometry = CreateMesh();
        var mv = new GeometryModel3D
        {
            Geometry = geometry,
            Material = gradientMaterial,
            BackMaterial = gradientMaterial
        };

        TriangleCount = geometry.TriangleIndices.Count / 3;

        return mv;
    }

    private static List<Point3D> SubDivide(IEnumerable<Point3D> centers, double L)
    {
        var newCenters = new List<Point3D>();

        foreach (var center in centers)
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                {
                    newCenters.Add(new Point3D(center.X + (i - 0.5) * L, center.Y + (j - 0.5) * L, center.Z));
                }
            newCenters.Add(new Point3D(center.X, center.Y, center.Z + L));

        }
        return newCenters;
    }
}
