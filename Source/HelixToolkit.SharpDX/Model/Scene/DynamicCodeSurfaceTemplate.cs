namespace HelixToolkit.SharpDX.Model.Scene;

public static class DynamicCodeSurfaceTemplate
{
    public const string template =
    @"
            using System;

            namespace MyNamespace {
            public class MyEvaluator {
            const double pi = Math.PI;
            public double cos(double x) { return Math.Cos(x); }
            public double sin(double x) { return Math.Sin(x); }
            public double abs(double x) { return Math.Abs(x); }
            public double sqrt(double x) { return Math.Sqrt(x); }
            public double sign(double x) { return Math.Sign(x); }
            public double sqr(double x) { return x*x; }
            public double log(double x) { return Math.Log(x); }
            public double exp(double x) { return Math.Exp(x); }
            public double pow(double x, double y) { return Math.Pow(x,y); }
            public Tuple<double,double,double,double> Evaluate(double u, double v, double w) {
            double x=0,y=0,z=0;
            double color=u;
            #code#
            return new Tuple<double,double,double,double>(x,y,z,color);
            }
            }
            }";
}
