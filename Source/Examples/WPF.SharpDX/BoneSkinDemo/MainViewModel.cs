using DemoCore;
using HelixToolkit.Wpf.SharpDX;
//using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Media3D = System.Windows.Media.Media3D;
using System.Windows.Media;
using Vector3 = global::SharpDX.Vector3;
using Matrix = global::SharpDX.Matrix;
using Vector4 = global::SharpDX.Vector4;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace BoneSkinDemo
{
    public class MainViewModel : BaseViewModel
    {
        public Color Light1Color { get; set; }
        public Color AmbientLightColor { get; set; }

        private FillMode fillMode = FillMode.Solid;
        public FillMode FillMode
        {
            set
            {
                fillMode = value;
                OnPropertyChanged();
            }
            get
            {
                return fillMode;
            }
        }

        public ObservableElement3DCollection Models { get; } = new ObservableElement3DCollection();

        public MeshGeometry3D FloorModel
        {
            private set;get;
        }
        public PhongMaterial Material
        {
            private set;get;
        }

        public PhongMaterial FloorMaterial
        {
            get;
        } = PhongMaterials.Indigo;

        private BoneMatricesStruct bones;
        public BoneMatricesStruct Bones
        {
            set
            {
                SetValue(ref bones, value, nameof(Bones));
            }
            get
            {
                return bones;
            }
        }

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                showWireframe = value;
                OnPropertyChanged();
                if (showWireframe)
                {
                    FillMode = FillMode.Wireframe;
                }
                else
                {
                    FillMode = FillMode.Solid;
                }
            }
            get
            {
                return showWireframe;
            }
        }

        private bool enableAnimation = true;
        public bool EnableAnimation
        {
            set
            {
                enableAnimation = value;
                OnPropertyChanged();
                if (enableAnimation)
                {
                    StartAnimation();
                }
                else
                {
                    cts.Cancel();
                }
            }
            get
            {
                return enableAnimation;
            }
        }

        public IList<Matrix> Instances { get; private set; }

        private const int numBonesInModel = 32;

        private readonly Matrix[] boneInternal = new Matrix[BoneMatricesStruct.NumberOfBones];
        private readonly List<BoneIds> boneParams = new List<BoneIds>();
        private int frame = 0;
        private bool direction = false;

        private const int NumSegments = 100;
        private const int Theta = 24;
        private IList<SharpDX.Vector3> path;
        private int numSegmentPerBone;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SynchronizationContext context = SynchronizationContext.Current;
        public MainViewModel()
        {
            this.Title = "BoneSkin Demo";
            this.SubTitle = "WPF & SharpDX";
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
           
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Media3D.Point3D(5, 5, 5),
                LookDirection = new Media3D.Vector3D(-5, -5, -5),
                UpDirection = new Media3D.Vector3D(0, 1, 0)
            };
            this.Light1Color = Colors.White;
            this.AmbientLightColor = Colors.DarkGray;

            Material = new PhongMaterial()
            {
                DiffuseColor = Colors.SteelBlue.ToColor4(),
                RenderShadowMap=true,
            };
            FloorMaterial.RenderShadowMap = true;
            for(int i=0; i< numBonesInModel; ++i)
            {
                boneInternal[i] = Matrix.Identity;
            }
            Bones = new BoneMatricesStruct()
            {
                Bones = boneInternal.ToArray()
            };

            var builder = new MeshBuilder(true, true, false);
            builder.AddBox(new Vector3(0, -1, 0), 5, 0.1, 5, BoxFaces.All);
            FloorModel = builder.ToMesh();

            LoadFile();
        }
        private void LoadFile()
        {
            var loader = new CMOReader();
            var obj3Ds = loader.Read("Character.cmo");
            foreach(var obj3D in obj3Ds)
            {
                if(obj3D.Geometry is BoneSkinnedMeshGeometry3D)
                {
                    Models.Add(new BoneSkinMeshGeometryModel3D()
                    {
                        Geometry = obj3D.Geometry,
                        FrontCounterClockwise = false,
                        Material = obj3D.Material.ConvertToMaterial(),
                        CullMode = CullMode.Back
                    });
                }
                else if(obj3D.Geometry is MeshGeometry3D)
                {
                    Models.Add(new MeshGeometryModel3D()
                    {
                        Geometry = obj3D.Geometry,
                        Material = obj3D.Material.ConvertToMaterial(),
                        CullMode = CullMode.Back, FrontCounterClockwise=false
                    });
                }
            }
            using(var texFile = File.OpenRead("Character.png"))
            {
                var memory = new MemoryStream();
                texFile.CopyTo(memory);
                foreach(var model in Models)
                {
                    ((model as MaterialGeometryModel3D).Material as PhongMaterial).DiffuseMap = memory;
                }
            }
        }

        private void StartAnimation()
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Timer_Tick();
                    Task.Delay(20).Wait();
                }
            }, token);
        }

        private void Timer_Tick()
        {

        }

        protected override void Dispose(bool disposing)
        {
            cts.Cancel(true);
            base.Dispose(disposing);
        }
    }
}
