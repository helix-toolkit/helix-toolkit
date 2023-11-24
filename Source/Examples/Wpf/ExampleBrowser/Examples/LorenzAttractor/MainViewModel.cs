using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace LorenzAttractor;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool directionArrows;

    /// <summary>
    /// Gets or sets the Rayleigh number.
    /// http://en.wikipedia.org/wiki/Rayleigh_number
    /// </summary>
    /// <value>The rho.</value>
    [ObservableProperty]
    private double rho;

    /// <summary>
    /// Gets or sets the Prandtl number.
    /// http://en.wikipedia.org/wiki/Prandtl_number
    /// </summary>
    /// <value>The sigma.</value>
    [ObservableProperty]
    private double sigma;

    [ObservableProperty]
    private double beta;

    [ObservableProperty]
    private Model3D model;

#pragma warning disable CS8618
    public MainViewModel()
#pragma warning restore CS8618
    {
        rho = 28;
        sigma = 10;
        beta = 8.0 / 3;
        directionArrows = true;

        UpdateModel();
    }

    partial void OnDirectionArrowsChanged(bool value)
    {
        UpdateModel();
    }

    partial void OnRhoChanged(double value)
    {
        UpdateModel();
    }

    partial void OnSigmaChanged(double value)
    {
        UpdateModel();
    }

    partial void OnBetaChanged(double value)
    {
        UpdateModel();
    }

    private void UpdateModel()
    {
        // http://en.wikipedia.org/wiki/Lorenz_attractor
        double[] lorenzAttractor(double[] x)
        {
            var dx = new double[3];
            dx[0] = Sigma * (x[1] - x[0]);
            dx[1] = x[0] * (Rho - x[2]) - x[1];
            dx[2] = x[0] * x[1] - Beta * x[2];
            return dx;
        }
        // solve the ODE
        var x0 = new[] { 0, 1, 1.05 };
        IEnumerable<double[]> solution = EulerSolver(lorenzAttractor, x0, 25);
        // todo: should have a better ODE solver (R-K(4,5)? http://www.mathworks.com/help/techdoc/ref/ode45.html)
        List<Point3D> path = solution.Select(x => new Point3D(x[0], x[1], x[2])).ToList();

        // create the WPF3D model
        var m = new Model3DGroup();
        var gm = new MeshBuilder();
        gm.AddTube(path.Select(t => t.ToVector()).ToList(), 0.8f, 10, false);
        if (DirectionArrows)
        {
            // sphere at the initial point
            gm.AddSphere(path[0].ToVector(), 1);
            // arrow heads every 100 point
            for (int i = 100; i + 1 < path.Count; i += 100)
            {
                gm.AddArrow(path[i].ToVector(), path[i + 1].ToVector(), 0.8f);
            }
            // arrow head at the end
            Point3D p0 = path[^2];
            Point3D p1 = path[^1];
            var d = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            d.Normalize();
            Point3D p2 = p1 + d * 2;
            gm.AddArrow(p1.ToVector(), p2.ToVector(), 0.8f);
        }

        m.Children.Add(new GeometryModel3D(gm.ToMesh().ToMeshGeometry3D(), Materials.Gold));

        Model = m;
    }

    private static IEnumerable<double[]> EulerSolver(Func<double[], double[]> dx, double[] x0, double tSpan)
    {
        var results = new List<double[]> { x0 };
        double t = 0;
        const double h = 0.008;
        int j = 0;
        while (t < tSpan)
        {
            t += h;
            double[] xprime = dx(results[j]);
            var x = new double[3];
            for (int i = 0; i < 3; i++)
                x[i] = results[j][i] + xprime[i] * h;
            j++;
            results.Add(x);
        }
        return results;
    }

    private static IEnumerable<double[]> RungeKutta45Solver(Func<double[], double[]> dx, double[] x0, double tSpan)
    {
        // http://math.fullerton.edu/mathews/n2003/RungeKuttaFehlbergMod.html
        // http://www.trentfguidry.net/post/2009/10/09/Runge-Kutta-Fehlberg.aspx

        throw new NotImplementedException();
    }
}
