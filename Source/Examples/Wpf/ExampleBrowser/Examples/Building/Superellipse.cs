using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System;

namespace Building;

public sealed class Superellipse : MarkupExtension
{
    private readonly double a;

    private readonly double b;

    private readonly double n;

    public Superellipse(double a, double b, double n)
    {
        this.a = a;
        this.b = b;
        this.n = n;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var c = new Point3DCollection();
        int m = 400;
        for (int i = 0; i + 1 < m; i++)
        {
            double t = 2 * Math.PI * i / (m - 2);
            c.Add(new Point3D(
                this.a * Math.Sign(Math.Cos(t)) * Math.Pow(Math.Abs(Math.Cos(t)), 2 / this.n),
                this.b * Math.Sign(Math.Sin(t)) * Math.Pow(Math.Abs(Math.Sin(t)), 2 / this.n),
                0));
        }

        c.Add(c[0]);
        return c;
    }
}
