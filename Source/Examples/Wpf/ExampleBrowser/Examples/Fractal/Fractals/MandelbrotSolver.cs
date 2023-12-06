using System.Diagnostics;
using System.Threading.Tasks;

namespace Fractal.Fractals;

/// <summary>
/// The MandelbrotSolver contains the coordinates of the image and the resulting ImageBuffer
/// </summary>
public sealed class MandelbrotSolver
{
    // Frame coordinates : Rectangle(X0,Y0,X1,Y1)
    public double X0 { get; set; }
    public double Y0 { get; set; }
    public double X1 { get; set; }
    public double Y1 { get; set; }

    // Number of pixels
    public int Width { get; set; }
    public int Height { get; set; }

    // Image buffer
    public int[]? ImageBuffer { get; private set; }
    public double[]? Xvalues;
    public double[]? Yvalues;

    public int MaxIterations { get; set; }

    public MandelbrotSolver()
    {
        X0 = -2; Y0 = -1.5;
        X1 = 1; Y1 = 1.5;
        MaxIterations = 40;
    }

    private static int Solve(double x0, double y0, int maxIterations)
    {
        int iteration = 0;
        double x = 0;
        double y = 0;
        while (x * x + y * y <= 4 && iteration < maxIterations)
        {
            double xtemp = x * x - y * y + x0;
            y = 2 * x * y + y0;
            x = xtemp;
            iteration++;
        }
        return iteration;
    }

    private static int[]? Solve(double[]? x, double[]? y, int maxit)
    {
        if (x is null || y is null)
        {
            return null;
        }

        int n = x.Length;
        var result = new int[n];
        Parallel.For(0, n, i => result[i] = Solve(x[i], y[i], maxit));
        return result;
    }

    public double Update()
    {
        var w = new Stopwatch();
        w.Start();
        Initialize();
        //long timeInit = w.ElapsedTicks;
        w.Reset();
        w.Start();
        ImageBuffer = Solve(Xvalues, Yvalues, MaxIterations);
        //long timeSolve = w.ElapsedTicks;
        return w.ElapsedMilliseconds * 0.001;
    }

    /// <summary>
    /// Initialize the x/y arrays (using a Parallel loop)
    /// </summary>
    private void Initialize()
    {
        Xvalues = new double[Width * Height];
        Yvalues = new double[Width * Height];

        double dx = (X1 - X0) / (Width - 1);
        double dy = (Y1 - Y0) / (Height - 1);

        double x0 = X0;
        double y0 = Y0;
        int w = Width;

        Parallel.For(0, Height, j =>
        {
            int ij = j * w;
            for (int i = 0; i < w; i++)
            {
                Xvalues[ij] = x0 + dx * i;
                Yvalues[ij++] = y0 + dy * j;
            }
        });
    }

    public void Zoom(double u, double v, double f)
    {
        double x = X0 + (X1 - X0) * u;
        double y = Y0 + (Y1 - Y0) * v;
        X0 += (x - X0) * f;
        X1 += (x - X1) * f;
        Y0 += (y - Y0) * f;
        Y1 += (y - Y1) * f;
    }

    public void Pan(double du, double dv)
    {
        double w = X1 - X0;
        double h = Y1 - Y0;
        X0 += w * du;
        X1 += w * du;
        Y0 += h * dv;
        Y1 += h * dv;
    }
}
