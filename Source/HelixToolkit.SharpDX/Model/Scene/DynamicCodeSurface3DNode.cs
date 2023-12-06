using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using SharpDX;
using System.CodeDom.Compiler;
using System.Reflection;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class DynamicCodeSurface3DNode : ParametricSurface3DNode
{
#if NET48
    public event EventHandler? OnCompileError;
#endif

    private float parameterW = 1f;
    public float ParameterW
    {
        set
        {
            if (Set(ref parameterW, value))
            {
                UpdateSource();
            }
        }
        get => parameterW;
    }

    private string source = string.Empty;
    /// <summary>
    /// Gets or sets the source code
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    public string Source
    {
        set
        {
            if (Set(ref source, value))
            {
                UpdateSource();
            }
        }
        get => source;
    }

#if NET48
    private CompilerErrorCollection? errors;

    public CompilerErrorCollection? Errors
    {
        private set
        {
            if (Set(ref errors, value))
            {
                OnCompileError?.Invoke(this, EventArgs.Empty);
            }
        }
        get => errors;
    }
#endif

    // Type and instance of the dynamic code
    private Type? _codeType;
    private object? _codeInstance;
    private string sourceCode = string.Empty;

    private void UpdateSource()
    {
        sourceCode = Source;
        _codeType = null;
        _codeInstance = null;
        if (string.IsNullOrEmpty(sourceCode))
            return;

        var src = GetTemplate().Replace("#code#", sourceCode);

#if NET48
        var provider = new CSharpCodeProvider();
        var options = new CompilerParameters { GenerateInMemory = true };
        var qn = typeof(Vector3).Assembly.Location;
        options.ReferencedAssemblies.Add("System.dll");
        options.ReferencedAssemblies.Add(qn);
        var compilerResults = provider.CompileAssemblyFromSource(options, src);
        if (!compilerResults.Errors.HasErrors)
        {
            Errors = null;
            var assembly = compilerResults.CompiledAssembly;
            _codeInstance = assembly.CreateInstance("MyNamespace.MyEvaluator");
            _codeType = _codeInstance.GetType();
            TessellateAsync();
        }
        else
        {
            // correct line numbers
            Errors = compilerResults.Errors;
            for (var i = 0; i < Errors.Count; i++)
                Errors[i].Line -= 17;
        }
#else
        var syntaxTree = SyntaxFactory.ParseSyntaxTree(src);
        string baseDir = Path.GetDirectoryName(typeof(object).Assembly.Location) + "\\";
        var compilation = CSharpCompilation.Create(Path.GetRandomFileName())
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(baseDir + "System.dll"))
            .AddReferences(MetadataReference.CreateFromFile(baseDir + "System.Runtime.dll"))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Vector3).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        if (!result.Success)
        {
            return;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        _codeInstance = assembly.CreateInstance("MyNamespace.MyEvaluator");

        if (_codeInstance is null)
        {
            return;
        }

        _codeType = _codeInstance.GetType();

        TessellateAsync();
#endif
    }

    protected virtual string GetTemplate()
    {
        return DynamicCodeSurfaceTemplate.template;
    }

    protected override Vector3 Evaluate(double u, double v, out Vector2 texCoord)
    {
        if (_codeType == null)
        {
            texCoord = new Point();
            return Vector3.Zero;
        }

        var parameters = new object[3];
        parameters[0] = u;
        parameters[1] = v;
        parameters[2] = ParameterW;
        var result = _codeType.InvokeMember("Evaluate", BindingFlags.InvokeMethod, null, _codeInstance, parameters);
        var p = (Tuple<double, double, double, double>)result!;

        // todo: why doesn't this work??
        //            texCoord = new Point(p.W, 0); // (double)parameters[2], 0);
        texCoord = new Vector2((float)u, (float)v);
        return new Vector3((float)p.Item1, (float)p.Item2, (float)p.Item3);
    }
}
