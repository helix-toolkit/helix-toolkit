/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Reflection;
#if !NETFX_CORE
using Microsoft.CSharp;
using System.CodeDom.Compiler;
#endif
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
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
#if !NETFX_CORE
    /// <summary>
    /// 
    /// </summary>
    public class DynamicCodeSurface3DNode : ParametricSurface3DNode
    {
        public event EventHandler OnCompileError;
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

        private string source;
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

        private CompilerErrorCollection errors;
        public CompilerErrorCollection Errors
        {
            private set
            {
                if(Set(ref errors, value))
                {
                    OnCompileError?.Invoke(this, EventArgs.Empty);
                }
            }
            get => errors;
        }

        // Type and instance of the dynamic code
        private Type _codeType;
        private object _codeInstance;
        private string sourceCode;

        private void UpdateSource()
        {
            sourceCode = Source;
            _codeType = null;
            _codeInstance = null;
            if (string.IsNullOrEmpty(sourceCode))
                return;

            var provider = new CSharpCodeProvider();
            var options = new CompilerParameters { GenerateInMemory = true };
            string qn = typeof(Vector3).Assembly.Location;
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add(qn);
            string src = GetTemplate().Replace("#code#", sourceCode);
            CompilerResults compilerResults = provider.CompileAssemblyFromSource(options, src);
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
                for (int i = 0; i < Errors.Count; i++)
                    Errors[i].Line -= 17;
            }
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

            object[] parameters = new object[3];
            parameters[0] = u;
            parameters[1] = v;
            parameters[2] = ParameterW;
            object result = _codeType.InvokeMember("Evaluate", BindingFlags.InvokeMethod, null, _codeInstance, parameters);
            var p = (Tuple<double, double, double, double>)result;

            // todo: why doesn't this work??
            //            texCoord = new Point(p.W, 0); // (double)parameters[2], 0);
            texCoord = new Vector2((float)u, (float)v);
            return new Vector3((float)p.Item1, (float)p.Item2, (float)p.Item3);
        }
    }
#endif
    }

}
