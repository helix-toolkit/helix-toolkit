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
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
using System.Threading;
using System.Threading.Tasks;

namespace BoneSkinDemo
{
    public class MainViewModel : BaseViewModel
    {
        private Media3D.Vector3D light1Direction = new Media3D.Vector3D();
        public Media3D.Vector3D Light1Direction
        {
            set
            {
                if (light1Direction != value)
                {
                    light1Direction = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return light1Direction;
            }
        }

        public Color Light1Color { get; set; }
        public Color AmbientLightColor { get; set; }

        private Media3D.Vector3D camLookDir = new Media3D.Vector3D(-10, -10, -10);
        public Media3D.Vector3D CamLookDir
        {
            set
            {
                if (camLookDir != value)
                {
                    camLookDir = value;
                    OnPropertyChanged();
                    Light1Direction = value;
                }
            }
            get
            {
                return camLookDir;
            }
        }

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

        public MeshGeometry3D Model
        {
            private set;get;
        }
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

        private IList<BoneIds> vertexBoneParams;
        public IList<BoneIds> VertexBoneParams
        {
            set
            {
                SetValue(ref vertexBoneParams, value, nameof(VertexBoneParams));
            }
            get
            {
                return vertexBoneParams;
            }
        }

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
        private IList<Vector3> path;
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
                Position = new Media3D.Point3D(20, 20, 20),
                LookDirection = new Media3D.Vector3D(-20, -20, -20),
                UpDirection = new Media3D.Vector3D(0, 1, 0)
            };
            this.Light1Color = Colors.White;
            this.Light1Direction = new Media3D.Vector3D(-10, -10, -10);
            this.AmbientLightColor = Colors.DarkGray;
            SetupCameraBindings(this.Camera);

            var builder = new MeshBuilder(true, true, true);
            path = new List<Vector3>();
            for(int i=0; i< NumSegments; ++i)
            {
                path.Add(new Vector3(0, (float)i/10, 0));
            }

            builder.AddTube(path, 2, Theta, false, false, true);
            Model = builder.ToMesh();
            for (int i = 0; i < Model.Positions.Count; ++i)
            {
                Model.Positions[i] = new Vector3(Model.Positions[i].X, 0, Model.Positions[i].Z);
            }
            Material = new PhongMaterial()
            {
                DiffuseColor = Colors.SteelBlue.ToColor4()
            };
            for(int i=0; i< numBonesInModel; ++i)
            {
                boneInternal[i] = Matrix.Identity;
            }
            Bones = new BoneMatricesStruct()
            {
                Bones = boneInternal.ToArray()
            };

            builder = new MeshBuilder(true, true, false);
            builder.AddBox(new Vector3(), 40, 0.5, 40, BoxFaces.All);
            FloorModel = builder.ToMesh();

            int boneId = 0;
            numSegmentPerBone = (int)Math.Max(1, (double)Model.Positions.Count / Theta / (numBonesInModel - 1));
            int count = 0;
            for(int i=0; i < Model.Positions.Count / Theta; ++i)
            {
                boneParams.AddRange(Enumerable.Repeat(new BoneIds()
                {
                    Bone1 = Math.Min(numBonesInModel - 1, boneId),
                    Bone2 = Math.Min(numBonesInModel - 1, boneId-1),
                    Bone3 = Math.Min(numBonesInModel - 1, boneId+1),
                    Weights = new Vector4(0.6f, 0.2f, 0.2f, 0)
                }, Theta));
                ++count;
                if (count == numSegmentPerBone)
                {
                    count = 0;
                    ++boneId;
                }
            }

            VertexBoneParams = boneParams.ToArray();

            Instances = new List<Matrix>();
            for (int i = 0; i < 3; ++i)
            {
                Instances.Add(Matrix.CreateTranslation(new Vector3(-5 + i * 4, 0, -10)));
            }
            for (int i = 0; i < 3; ++i)
            {
                Instances.Add(Matrix.CreateTranslation(new Vector3(-5 + i * 4, 0, 0)));
            }
            for (int i = 0; i < 3; ++i)
            {
                Instances.Add(Matrix.CreateTranslation(new Vector3(-5 + i * 4, 0, 10)));
            }
            StartAnimation();
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
            double angle = (0.05f*frame) * Math.PI / 180;
            var xAxis = new Vector3(1, 0, 0);
            var zAxis = new Vector3(0, 0, 1);
            var yAxis = new Vector3(0, 1, 0);
            var rotation = Matrix.CreateFromAxisAngle(xAxis, 0);
            double angleEach = 0;
            int counter = 0;
            for (int i=0; i< NumSegments && i < numBonesInModel; ++i, counter+= numSegmentPerBone)
            {
                if (i == 0)
                {
                    boneInternal[0] =rotation;
                }
                else
                {
                    var vp = Vector3.Transform(path[counter - numSegmentPerBone], Matrix.CreateFromAxisAngle(xAxis, (float)angleEach));
                    angleEach += angle;
                    var v = Vector3.Transform(path[counter], Matrix.CreateFromAxisAngle(xAxis, (float)angleEach));
                    var rad = Math.Acos(Vector3.Dot(yAxis, Vector3.Normalize(v-vp)));
                    if (angleEach < 0)
                    {
                        rad = -rad;
                    }
                    var rot = Matrix.CreateFromAxisAngle(xAxis, (float)rad);
                    var trans = Matrix.CreateTranslation(v);
                    boneInternal[i] = rot * trans;
                }
            }
            var newBone =  new BoneMatricesStruct() { Bones = boneInternal.ToArray() };
            context.Post((o) =>
            {
                Bones = newBone;
            }, null);

            if (frame > 40 || frame < -40)
            {
                direction = !direction;
            }
            if (direction)
            {
                ++frame;
            }
            else
            {
                --frame;
            }
        }

        public void SetupCameraBindings(Camera camera)
        {
            if (camera is ProjectionCamera)
            {
                SetBinding("CamLookDir", camera, ProjectionCamera.LookDirectionProperty, this);
            }
        }

        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }

        protected override void Dispose(bool disposing)
        {
            cts.Cancel(true);
            base.Dispose(disposing);
        }
    }
}
