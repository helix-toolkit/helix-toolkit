using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Media3D = System.Windows.Media.Media3D;

namespace BoneSkinDemo
{
    public class MainViewModel : BaseViewModel
    {
        private Vector3 light1Direction = new Vector3();
        public Vector3 Light1Direction
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

        public Color4 Light1Color { get; set; }
        public Color4 AmbientLightColor { get; set; }

        private Media3D.Vector3D camLookDir = new Media3D.Vector3D(-10, -10, -10);
        public Media3D.Vector3D CamLookDir
        {
            set
            {
                if (camLookDir != value)
                {
                    camLookDir = value;
                    OnPropertyChanged();
                    Light1Direction = value.ToVector3();
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
        public PhongMaterial Material
        {
            private set;get;
        }

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
                    timer.Start();
                }
                else
                {
                    timer.Stop();
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
        private DispatcherTimer timer = new DispatcherTimer();
        private int frame = 0;
        private bool direction = false;

        private const int NumSegments = 100;
        private const int Theta = 24;
        private IList<Vector3> path;
        private int numSegmentPerBone;
        public MainViewModel()
        {
            this.Title = "BoneSkin Demo";
            this.SubTitle = "WPF & SharpDX";
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Media3D.Point3D(20, 20, 20),
                LookDirection = new Media3D.Vector3D(-20, -20, -20),
                UpDirection = new Media3D.Vector3D(0, 1, 0)
            };
            this.Light1Color = (Color4)Color.White;
            this.Light1Direction = new Vector3(-10, -10, -10);
            this.AmbientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
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
                DiffuseColor = Color.MistyRose
            };
            for(int i=0; i< numBonesInModel; ++i)
            {
                boneInternal[i] = Matrix.Identity;
            }
            Bones = new BoneMatricesStruct()
            {
                Bones = boneInternal.ToArray()
            };

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
                Instances.Add(Matrix.Translation(new Vector3(-5 + i * 4, 0, -10)));
            }
            for (int i = 0; i < 3; ++i)
            {
                Instances.Add(Matrix.Translation(new Vector3(-5 + i * 4, 0, 0)));
            }
            for (int i = 0; i < 3; ++i)
            {
                Instances.Add(Matrix.Translation(new Vector3(-5 + i * 4, 0, 10)));
            }

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Start();
        }
    

        private void Timer_Tick(object sender, EventArgs e)
        {
            double angle = (0.05f*frame) * Math.PI / 180;
            var xAxis = new Vector3(1, 0, 0);
            var zAxis = new Vector3(0, 0, 1);
            var yAxis = new Vector3(0, 1, 0);
            var rotation = Matrix.RotationAxis(xAxis, 0);
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
                    var vp = Vector3.Transform(path[counter - numSegmentPerBone], Matrix.RotationAxis(xAxis, (float)angleEach)).ToVector3();
                    angleEach += angle;
                    var v = Vector3.Transform(path[counter], Matrix.RotationAxis(xAxis, (float)angleEach)).ToVector3();
                    var rad = Math.Acos(Vector3.Dot(yAxis, (v-vp).Normalized()));
                    if (angleEach < 0)
                    {
                        rad = -rad;
                    }
                    var rot = Matrix.RotationAxis(xAxis, (float)rad);
                    var trans = Matrix.Translation(v);
                    boneInternal[i] = rot * trans;
                }
            }
            Bones = new BoneMatricesStruct() { Bones = boneInternal.ToArray() };

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
    }
}
