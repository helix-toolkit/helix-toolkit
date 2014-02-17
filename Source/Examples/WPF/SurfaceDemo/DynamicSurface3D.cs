// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicSurface3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows.Media.Media3D;
using System.Windows;
using HelixToolkit.Wpf;

namespace SurfaceDemo
{
    public class DynamicCodeSurface3D : ParametricSurface3D
    {

        public double ParameterW
        {
            get { return (double)GetValue(ParameterWProperty); }
            set { SetValue(ParameterWProperty, value); }
        }

        public static readonly DependencyProperty ParameterWProperty =
            DependencyProperty.Register("ParameterW", typeof(double), typeof(DynamicCodeSurface3D), new UIPropertyMetadata(1.0, SourceChanged));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(DynamicCodeSurface3D),
            new UIPropertyMetadata(null, SourceChanged));

        protected static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicCodeSurface3D)d).UpdateSource();
        }

        // Type and instance of the dynamic code
        Type _codeType;
        object _codeInstance;

        public bool HasErrors()
        {
            return _codeType == null;
        }
        public CompilerErrorCollection Errors { get; private set; }

        public void UpdateSource()
        {
            _codeType = null;
            _codeInstance = null;
            if (Source == null)
                return;

            var provider = new CSharpCodeProvider();
            var options = new CompilerParameters { GenerateInMemory = true };
            string qn = typeof(Point3D).Assembly.Location;
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add(qn);
            string src = template.Replace("#code#", Source);
            CompilerResults compilerResults = provider.CompileAssemblyFromSource(options, src);
            if (!compilerResults.Errors.HasErrors)
            {
                Errors = null;
                var assembly = compilerResults.CompiledAssembly;
                _codeInstance = assembly.CreateInstance("MyNamespace.MyEvaluator");
                _codeType = _codeInstance.GetType();
            }
            else
            {
                // correct line numbers
                Errors = compilerResults.Errors;
                for (int i = 0; i < Errors.Count; i++)
                    Errors[i].Line -= 17;
            }

            _w = ParameterW;
            UpdateModel();
        }

        double _w;

        protected override Point3D Evaluate(double u, double v, out Point texCoord)
        {
            if (_codeType == null)
            {
                texCoord = new Point();
                return new Point3D(0, 0, 0);
            }

            object[] parameters = new object[3];
            parameters[0] = u;
            parameters[1] = v;
            parameters[2] = _w;
            object result = _codeType.InvokeMember("Evaluate", BindingFlags.InvokeMethod, null, _codeInstance, parameters);
            var p = (Point4D)result;

            // todo: why doesn't this work??
            //            texCoord = new Point(p.W, 0); // (double)parameters[2], 0);
            texCoord = new Point(u, v);
            return new Point3D(p.X, p.Y, p.Z);
        }

        const string template =
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
public System.Windows.Media.Media3D.Point4D Evaluate(double u, double v, double w) {
double x=0,y=0,z=0;
double color=u;
#code#
return new System.Windows.Media.Media3D.Point4D(x,y,z,color);
}
}
}";

    }
}