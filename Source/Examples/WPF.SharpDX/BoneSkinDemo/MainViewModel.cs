/*
Model: Sphere Bot Rusty Version. Author: 3DHaupt. Source : https://sketchfab.com/models/d18753fe3e494ddbbc52a8a2e58be7a4
Model: Character. Source : https://github.com/spazzarama/Direct3D-Rendering-Cookbook
*/
using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
using System.Threading;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;

namespace BoneSkinDemo
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableElement3DCollection Models { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection BoneModels { get; } = new ObservableElement3DCollection();
        public MeshGeometry3D FloorModel
        {
            private set;get;
        }

        public PhongMaterial FloorMaterial
        {
            get;
        } = PhongMaterials.Gray;

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                showWireframe = value;
                OnPropertyChanged();
                 
                foreach(var group in Models)
                {
                    if(group is GroupModel3D g)
                    {
                        foreach(var model in g.Children)
                        {
                            if(model is MeshGeometryModel3D m)
                            {
                                m.RenderWireframe = value;
                            }     
                        }                  
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

        public Material BoneMaterial { get; } = DiffuseMaterials.Red;

        private string selectedAnimation;
        public string SelectedAnimation
        {
            set
            {
                if(SetValue(ref selectedAnimation, value))
                {
                    reset = true;
                    selectedUpdaters = keyframeUpdaters[value];
                }
            }
            get { return selectedAnimation; }
        }

        private AnimationRepeatMode selectedRepeatMode = AnimationRepeatMode.Loop;
        public AnimationRepeatMode SelectedRepeatMode
        {
            set
            {
                if(SetValue(ref selectedRepeatMode, value))
                {
                    reset = true;
                }
            }
            get { return selectedRepeatMode; }
        }

        public Media3D.Transform3D ModelTransform { private set; get; }

        public string[] Animations { private set; get; }

        public GridPattern[] GridTypes { get; } = new GridPattern[] { GridPattern.Tile, GridPattern.Grid };

        public AnimationRepeatMode[] RepeatModes { get; } = new AnimationRepeatMode[] { AnimationRepeatMode.Loop, AnimationRepeatMode.PlayOnce, AnimationRepeatMode.PlayOnceHold };

        private Matrix[] boneInternal = new Matrix[0];
        private readonly List<BoneIds> boneParams = new List<BoneIds>();

        private const int NumSegments = 100;
        private const int Theta = 24;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SynchronizationContext context = SynchronizationContext.Current;
        private Dictionary<Guid, List<BoneGroupModel3D>> BoneGroupsDictionary = new Dictionary<Guid, List<BoneGroupModel3D>>();
        private CMOReader loader;
        private Dictionary<string, List<KeyValuePair<Guid, KeyFrameUpdater>>> keyframeUpdaters = new Dictionary<string, List<KeyValuePair<Guid, KeyFrameUpdater>>>();
        private List<KeyValuePair<Guid, KeyFrameUpdater>> selectedUpdaters;
        private bool reset = true;

        public MainViewModel()
        {
            this.Title = "BoneSkin Demo";
            this.SubTitle = "WPF & SharpDX";
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
           
            this.Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Media3D.Point3D(50, 50, 50),
                LookDirection = new Media3D.Vector3D(-50, -50, -50),
                UpDirection = new Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = 2000
            };

            FloorMaterial.RenderShadowMap = true;
            var builder = new MeshBuilder(true, true, false);
            builder.AddBox(new Vector3(0, 0, 0), 100, 0.1, 100, BoxFaces.All);
            FloorModel = builder.ToMesh();
            LoadFile();
            StartAnimation();
        }

        private void LoadFile()
        {
            loader = new CMOReader();
            var obj3Ds = loader.Read("Sphere_Bot_test.cmo");
            foreach(var group in loader.AnimationHierarchy)
            {
                var boneGroup = new BoneGroupModel3D();
                var skeletonGroup = new BoneGroupModel3D();
                foreach (var obj3D in group.Meshes)
                {
                    if (obj3D.Geometry is BoneSkinnedMeshGeometry3D boneMesh)
                    {
                        var model = new BoneSkinMeshGeometryModel3D()
                        {
                            Geometry = obj3D.Geometry,
                            FrontCounterClockwise = false,
                            CullMode = CullMode.Back,
                            Material = obj3D.Material.ConvertToMaterial(),
                            IsThrowingShadow = true
                        };
                        boneGroup.Children.Add(model);
                    }
                    else if (obj3D.Geometry is MeshGeometry3D)
                    {
                        Models.Add(new MeshGeometryModel3D()
                        {
                            Geometry = obj3D.Geometry,
                            CullMode = CullMode.Back,
                            FrontCounterClockwise = false
                        });
                    }
                }
               
                var skeletonModel = new BoneSkinMeshGeometryModel3D()
                {
                    Geometry = BoneSkinnedMeshGeometry3D.CreateSkeletonMesh(group.Bones, 0.1f),
                    CullMode = CullMode.Back,
                    Material = BoneMaterial,
                    PostEffects = "xray"
                };
                skeletonGroup.Children.Add(skeletonModel);
                Models.Add(boneGroup);
                BoneModels.Add(skeletonGroup);
                BoneGroupsDictionary.Add(group.GUID, new List<BoneGroupModel3D>() { boneGroup, skeletonGroup });
            }
            
            var diffuse = new MemoryStream();
            using (var file = File.OpenRead(@"Sphere_Bot_Rusty_UVMap_color.png"))
            {
                
                file.CopyTo(diffuse);
            }

            var normal = new MemoryStream();
            using (var file = File.OpenRead(@"Sphere_Bot_Rusty_UVMap_nmap.png"))
            {
                file.CopyTo(normal);
            }
            foreach (var group in Models)
            {
                if(group is GroupElement3D g)
                {
                    foreach(var subModel in g.Children)
                    {
                        if(subModel is MaterialGeometryModel3D model)
                        {
                            var m = model.Material as PhongMaterial;
                            m.EmissiveColor = Colors.Black.ToColor4();                
                            m.DiffuseMap = diffuse;
                            m.NormalMap = normal;
                            m.RenderShadowMap = true;
                        }
                    }
                }
            }

            Animations = loader.UniqueAnimations.Keys.ToArray();

            ModelTransform = new Media3D.MatrixTransform3D((Matrix.CreateScale(10,10,10) * Matrix.CreateFromAxisAngle(Vector3.UnitX, -(float)Math.PI / 2)).ToMatrix3D());

            foreach(var group in loader.AnimationHierarchy)
            {
                foreach (var ani in group.Animations)
                {
                    if (!keyframeUpdaters.ContainsKey(ani.Key))
                    {
                        keyframeUpdaters.Add(ani.Key, new List<KeyValuePair<Guid, KeyFrameUpdater>>());
                    }
                    keyframeUpdaters[ani.Key].Add(new KeyValuePair<Guid, KeyFrameUpdater>(group.GUID, new KeyFrameUpdater(ani.Value, group.Bones)));
                }
            }
        }

        private void StartAnimation()
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
            var token = cts.Token;
            SelectedAnimation = loader.UniqueAnimations.Keys.First();
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (reset)
                    {
                        reset = false;
                        foreach(var updater in selectedUpdaters)
                        {
                            updater.Value.RepeatMode = selectedRepeatMode;
                            updater.Value.Reset();
                        }
                    }
                    else
                    {
                        Timer_Tick();
                    }
                    Task.Delay(15).Wait();
                }
            }, token);
        }

        private void Timer_Tick()
        {
            var curr = Stopwatch.GetTimestamp();
            var bones = selectedUpdaters.Select(x => { return new KeyValuePair<Guid, Matrix[]>(x.Key, x.Value.Update(curr, Stopwatch.Frequency)); }).ToArray();
            context.Send((o) =>
            {
                foreach(var b in bones)
                {
                    var groups = BoneGroupsDictionary[b.Key];
                    foreach (var g in groups)
                    {
                        g.BoneMatrices = b.Value;
                    }
                }

            }, null);
            //float maxEndTime = 0;

            //foreach(var group in loader.AnimationHierarchy)
            //{
            //    var ani = group.Animations[selectedAnimation];
            //    maxEndTime = Math.Max(ani.EndTime, maxEndTime);
            //    var bones = group.Bones;
            //    if(bones.Count > boneInternal.Length)
            //    {
            //        boneInternal = new Matrix[bones.Count];
            //    }
            //    if (bones != null)
            //    {
            //        // Retrieve each bone's local transform
            //        for (var i = 0; i < bones.Count; i++)
            //        {
            //            boneInternal[i] = bones[i].BoneLocalTransform;
            //        }

            //        // Load bone transforms from animation frames

            //        // Keep track of the last key-frame used for each bone
            //        Keyframe?[] lastKeyForBones = new Keyframe?[bones.Count];
            //        // Keep track of whether a bone has been interpolated
            //        bool[] lerpedBones = new bool[bones.Count];
            //        for(int i=currKeyFrame; i < ani.Keyframes.Count; i += bones.Count)
            //        {
            //            if(ani.Keyframes[i].Time > time)
            //            {
            //                currKeyFrame = i - bones.Count;
            //                break;
            //            }
            //        }
            //        for (int i = currKeyFrame; i < currKeyFrame + bones.Count; i++)
            //        {
            //            // Retrieve current key-frame
            //            var frame = ani.Keyframes[i];

            //            // If the current frame is not in the future
            //            if (frame.Time <= time)
            //            {
            //                // Keep track of last key-frame for bone
            //                lastKeyForBones[frame.BoneIndex] = frame;
            //                // Retrieve transform from current key-frame
            //                boneInternal[frame.BoneIndex] = frame.Transform;
            //            }
            //            // Frame is in the future, check if we should interpolate
            //            else
            //            {
            //                // Only interpolate a bone's key-frames ONCE
            //                if (!lerpedBones[frame.BoneIndex])
            //                {
            //                    // Retrieve the previous key-frame if exists
            //                    Keyframe prevFrame;
            //                    if (lastKeyForBones[frame.BoneIndex] != null)
            //                        prevFrame = lastKeyForBones[frame.BoneIndex].Value;
            //                    else
            //                        continue; // nothing to interpolate
            //                    // Make sure we only interpolate with 
            //                    // one future frame for this bone
            //                    lerpedBones[frame.BoneIndex] = true;

            //                    // Calculate time difference between frames
            //                    var frameLength = frame.Time - prevFrame.Time;
            //                    var timeDiff = time - prevFrame.Time;
            //                    var amount = timeDiff / frameLength;

            //                    // Interpolation using Lerp on scale and translation, and Slerp on Rotation (Quaternion)
            //                    Vector3 t1, t2;   // Translation
            //                    Quaternion q1, q2;// Rotation
            //                    float s1, s2;     // Scale
            //                    // Decompose the previous key-frame's transform
            //                    prevFrame.Transform.DecomposeUniformScale(out s1, out q1, out t1);
            //                    // Decompose the current key-frame's transform
            //                    frame.Transform.DecomposeUniformScale(out s2, out q2, out t2);

            //                    // Perform interpolation and reconstitute matrix
            //                    boneInternal[frame.BoneIndex] =
            //                        Matrix.Scaling(MathUtil.Lerp(s1, s2, amount)) *
            //                        Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, amount)) *
            //                        Matrix.Translation(Vector3.Lerp(t1, t2, amount));
            //                }
            //            }

            //        }
            //        currKeyFrame += bones.Count;
            //        // Apply parent bone transforms
            //        // We assume here that the first bone has no parent
            //        // and that each parent bone appears before children
            //        for (var i = 1; i < bones.Count; i++)
            //        {
            //            var bone = bones[i];
            //            if (bone.ParentIndex > -1)
            //            {
            //                var parentTransform = boneInternal[bone.ParentIndex];
            //                boneInternal[i] = (boneInternal[i] * parentTransform);
            //            }
            //        }

            //        // Change the bone transform from rest pose space into bone space (using the inverse of the bind/rest pose)
            //        var newBones = boneInternal.ToArray();
            //        for (var i = 0; i < bones.Count; i++)
            //        {
            //            newBones[i] = bones[i].InvBindPose * boneInternal[i];
            //        }

            //        context.Post((o) => 
            //        {
            //            var groups = BoneGroupsDictionary[group.GUID];
            //            foreach(var g in groups)
            //            {
            //                g.BoneMatrices = newBones;
            //            }
            //        }, null);
            //    }
            //}

            // Check need to loop animation
            //if (maxEndTime <= time)
            //{
            //    lastTime = curr;
            //    currKeyFrame = 0;
            //}
        }

        protected override void Dispose(bool disposing)
        {
            cts.Cancel(true);
            base.Dispose(disposing);
        }
    }
}
