using System.Windows.Media.Media3D;
using System.Windows;
using System;
using HelixToolkit.Wpf;
using DependencyPropertyGenerator;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace SurfaceDemo;

[DependencyProperty<double>("ParameterW", DefaultValue = 1.0, OnChanged = nameof(SourceChanged))]
[DependencyProperty<string>("Source", OnChanged = nameof(SourceChanged))]
public partial class DynamicCodeSurface3D : ParametricSurface3D
{
    protected void SourceChanged()
    {
        UpdateSource();
    }

    // Type and instance of the dynamic code
    Type? _codeType;
    object? _codeInstance;

    public bool HasErrors()
    {
        return _codeType == null;
    }

    public System.CodeDom.Compiler.CompilerErrorCollection? Errors { get; private set; }

#if NET6_0_OR_GREATER
    private static readonly List<Microsoft.CodeAnalysis.PortableExecutableReference> References =
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
            .Select(_ => Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(_.Location))
            .ToList();

    private static int _assemblyIndex;
#endif

    public void UpdateSource()
    {
        _codeType = null;
        _codeInstance = null;
        if (Source == null)
            return;

        string src = template.Replace("#code#", Source);

#if NET6_0_OR_GREATER
        var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(src);

        Microsoft.CodeAnalysis.CSharp.CSharpCompilation compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
            "MyAssembly" + _assemblyIndex++,
            new[] { syntaxTree },
            References,
            new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));

        using var dllStream = new MemoryStream();
        using var pdbStream = new MemoryStream();

        var emitResult = compilation.Emit(dllStream, pdbStream);
        if (!emitResult.Success)
        {
            Errors = null;
            // correct line numbers
            //Errors = compilerResults.Errors;
            //for (int i = 0; i < Errors.Count; i++)
            //    Errors[i].Line -= 17;
        }
        else
        {
            Errors = null;

            dllStream.Seek(0, SeekOrigin.Begin);
            pdbStream.Seek(0, SeekOrigin.Begin);

            var _codeAssembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(dllStream, pdbStream);
            _codeInstance = _codeAssembly.CreateInstance("MyNamespace.MyEvaluator");
            _codeType = _codeInstance?.GetType();
        }
#else
        System.CodeDom.Compiler.CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

        var options = new System.CodeDom.Compiler.CompilerParameters { GenerateInMemory = true };
        string qn = typeof(Point3D).Assembly.Location;
        options.ReferencedAssemblies.Add("System.dll");
        options.ReferencedAssemblies.Add(qn);
        System.CodeDom.Compiler.CompilerResults compilerResults = provider.CompileAssemblyFromSource(options, src);
        if (!compilerResults.Errors.HasErrors)
        {
            Errors = null;
            var assembly = compilerResults.CompiledAssembly;
            _codeInstance = assembly?.CreateInstance("MyNamespace.MyEvaluator");
            _codeType = _codeInstance?.GetType();
        }
        else
        {
            // correct line numbers
            Errors = compilerResults.Errors;
            for (int i = 0; i < Errors.Count; i++)
                Errors[i].Line -= 17;
        }
#endif

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
        object? result = _codeType?.InvokeMember("Evaluate", BindingFlags.InvokeMethod, null, _codeInstance, parameters);

        if (result is null)
        {
            texCoord = default;
            return default;
        }

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
