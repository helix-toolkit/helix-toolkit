using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Matrix = global::SharpDX.Matrix;
using Media3D = System.Windows.Media.Media3D;
using Vector3 = global::SharpDX.Vector3;

namespace BoneSkinDemo
{
    public class MainViewModel : BaseViewModel
    {
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

        private BoneMatricesStruct boneStruct;
        public BoneMatricesStruct BoneStruct
        {
            set
            {
                if(SetValue(ref boneStruct, value))
                {
                    Model.BoneMatrices = value;
                }
            }
            get
            {
                return boneStruct;
            }
        }

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                showWireframe = value;
                OnPropertyChanged();
                 
                foreach(var model in Models)
                {
                    if(model is MeshGeometryModel3D m)
                    {
                        m.RenderWireframe = value;
                    }
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

        public BoneSkinnedMeshGeometry3D BoneMesh { private set; get; }

        public Material BoneMaterial { get; } = DiffuseMaterials.Red;

        private BoneMatricesStruct boneSkeletonStruct;
        public BoneMatricesStruct BoneSkeletonStruct
        {
            set
            {
                SetValue(ref boneSkeletonStruct, value);
            }
            get
            {
                return boneSkeletonStruct;
            }
        }

        private const int numBonesInModel = 32;

        private readonly Matrix[] boneInternal = new Matrix[BoneMatricesStruct.NumberOfBones];
        private readonly List<BoneIds> boneParams = new List<BoneIds>();

        private const int NumSegments = 100;
        private const int Theta = 24;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SynchronizationContext context = SynchronizationContext.Current;
        private BoneSkinnedMeshGeometry3D Mesh;
        private BoneSkinMeshGeometryModel3D Model;
        private Animation? CurrentAnimation;

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

            Material = new PhongMaterial()
            {
                DiffuseColor = Colors.SteelBlue.ToColor4(),
                RenderShadowMap=true,
            };
            FloorMaterial.RenderShadowMap = true;
            var builder = new MeshBuilder(true, true, false);
            builder.AddBox(new Vector3(0, -1, 0), 5, 0.1, 5, BoxFaces.All);
            FloorModel = builder.ToMesh();

            LoadFile();
            StartAnimation();
        }
        private void LoadFile()
        {
            var loader = new CMOReader();
            var obj3Ds = loader.Read("Character.cmo");
            foreach(var obj3D in obj3Ds)
            {
                if(obj3D.Geometry is BoneSkinnedMeshGeometry3D)
                {
                    Model = new BoneSkinMeshGeometryModel3D()
                    {
                        Geometry = obj3D.Geometry,
                        FrontCounterClockwise = false,
                        Material = obj3D.Material.ConvertToMaterial(),
                        CullMode = CullMode.Back
                    };
                    Models.Add(Model);
                    Mesh = obj3D.Geometry as BoneSkinnedMeshGeometry3D;
                    BoneMesh = Mesh.CreateSkeletonMesh();
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
            CurrentAnimation = Mesh.Animations.Values.First();
            Task.Run(() =>
            {
                lastTime = Stopwatch.GetTimestamp();
                while (!token.IsCancellationRequested)
                {
                    Timer_Tick();
                    Task.Delay(16).Wait();
                }
            }, token);
        }
        long lastTime;
        private void Timer_Tick()
        {
            var curr = Stopwatch.GetTimestamp();
            var time = (float)(curr - lastTime) / Stopwatch.Frequency;

            if (Mesh.Bones != null)
            {
                // Retrieve each bone's local transform
                for (var i = 0; i < Mesh.Bones.Count; i++)
                {
                    boneInternal[i] = Mesh.Bones[i].BoneLocalTransform;
                }

                // Load bone transforms from animation frames
                if (CurrentAnimation.HasValue)
                {
                    // Keep track of the last key-frame used for each bone
                    Keyframe?[] lastKeyForBones = new Keyframe?[Mesh.Bones.Count];
                    // Keep track of whether a bone has been interpolated
                    bool[] lerpedBones = new bool[Mesh.Bones.Count];
                    for (var i = 0; i < CurrentAnimation.Value.Keyframes.Count; i++)
                    {
                        // Retrieve current key-frame
                        var frame = CurrentAnimation.Value.Keyframes[i];

                        // If the current frame is not in the future
                        if (frame.Time <= time)
                        {
                            // Keep track of last key-frame for bone
                            lastKeyForBones[frame.BoneIndex] = frame;
                            // Retrieve transform from current key-frame
                            boneInternal[frame.BoneIndex] = frame.Transform;
                        }
                        // Frame is in the future, check if we should interpolate
                        else
                        {
                            // Only interpolate a bone's key-frames ONCE
                            if (!lerpedBones[frame.BoneIndex])
                            {
                                // Retrieve the previous key-frame if exists
                                Keyframe prevFrame;
                                if (lastKeyForBones[frame.BoneIndex] != null)
                                    prevFrame = lastKeyForBones[frame.BoneIndex].Value;
                                else
                                    continue; // nothing to interpolate
                                // Make sure we only interpolate with 
                                // one future frame for this bone
                                lerpedBones[frame.BoneIndex] = true;

                                // Calculate time difference between frames
                                var frameLength = frame.Time - prevFrame.Time;
                                var timeDiff = time - prevFrame.Time;
                                var amount = timeDiff / frameLength;

                                // Interpolation using Lerp on scale and translation, and Slerp on Rotation (Quaternion)
                                Vector3 t1, t2;   // Translation
                                Quaternion q1, q2;// Rotation
                                float s1, s2;     // Scale
                                // Decompose the previous key-frame's transform
                                prevFrame.Transform.DecomposeUniformScale(out s1, out q1, out t1);
                                // Decompose the current key-frame's transform
                                frame.Transform.DecomposeUniformScale(out s2, out q2, out t2);

                                // Perform interpolation and reconstitute matrix
                                boneInternal[frame.BoneIndex] =
                                    Matrix.Scaling(MathUtil.Lerp(s1, s2, amount)) *
                                    Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, amount)) *
                                    Matrix.Translation(Vector3.Lerp(t1, t2, amount));
                            }
                        }

                    }
                }

                // Apply parent bone transforms
                // We assume here that the first bone has no parent
                // and that each parent bone appears before children
                for (var i = 1; i < Mesh.Bones.Count; i++)
                {
                    var bone = Mesh.Bones[i];
                    if (bone.ParentIndex > -1)
                    {
                        var parentTransform = boneInternal[bone.ParentIndex];
                        boneInternal[i] = (boneInternal[i] * parentTransform);
                    }
                }

                // Change the bone transform from rest pose space into bone space (using the inverse of the bind/rest pose)
                var newBones = boneInternal.ToArray();
                for (var i = 0; i < Mesh.Bones.Count; i++)
                {
                    newBones[i] = Mesh.Bones[i].InvBindPose * boneInternal[i];
                }
                var skeleton = boneInternal.ToArray();
                context.Post((o) => 
                {
                    BoneStruct = new BoneMatricesStruct() { Bones = newBones };
                    BoneSkeletonStruct = BoneStruct;
                }, null);
                // Check need to loop animation
                if (CurrentAnimation.HasValue && CurrentAnimation.Value.EndTime <= time)
                {
                    lastTime = curr;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            cts.Cancel(true);
            base.Dispose(disposing);
        }
    }
}
