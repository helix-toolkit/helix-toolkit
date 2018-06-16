using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DynamicCodeSurfaceDemo
{
    public class MainViewModel : BaseViewModel
    {
        private double parameterW = 1;
        public double ParameterW
        {
            set => SetValue(ref parameterW, value);
            get => parameterW;
        }
        private int meshSizeU = 120;
        public int MeshSizeU
        {
            set => SetValue(ref meshSizeU, value);
            get => meshSizeU;
        }

        private int meshSizeV = 120;
        public int MeshSizeV
        {
            set => SetValue(ref meshSizeV, value);
            get => meshSizeV;
        }

        private Material material = new NormalMaterial();
        public Material Material
        {
            set => SetValue(ref material, value);
            get => material;
        }

        public string[] Models { private set; get; }

        private List<Uri> sourceCodeUri { get; } = new List<Uri>();
        private Dictionary<string, string> fileDict = new Dictionary<string, string>();

        private string selectedModel;
        public string SelectedModel
        {
            set
            {
                if (SetValue(ref selectedModel, value))
                {
                    LoadSourceCode();
                }
            }
            get => selectedModel;
        }

        private string sourceCode;
        public string SourceCode
        {
            private set
            {
                SetValue(ref sourceCode, value);
            }
            get => sourceCode;
        }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera() { Position = new System.Windows.Media.Media3D.Point3D(0, 0, -10),
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, 10),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = 2000, NearPlaneDistance = 1
            };
            //foreach (string m in Models)
            //{
            //    var uri = new Uri(String.Format("pack://application:,,/Expressions/{0}.txt", m));
            //    sourceCodeUri.Add(uri);
            //}

            var dir = "Expressions";
            if (!Directory.Exists(dir))
                return;
            string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                fileDict.Add(Path.GetFileNameWithoutExtension(file), Path.GetFullPath(file));
            }
            Models = fileDict.Keys.ToArray();
        }

        void LoadSourceCode()
        {
            if(fileDict.TryGetValue(selectedModel, out string filePath))
            {
                using (var reader = File.OpenRead(filePath))
                {
                    using(var strReader = new StreamReader(reader))
                    {
                        SourceCode = strReader.ReadToEnd();
                    }
                }
            }
        }
    }
}
