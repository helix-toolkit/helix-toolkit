/*
Model: Sphere Bot Rusty Version. Author: 3DHaupt. Source : https://sketchfab.com/models/d18753fe3e494ddbbc52a8a2e58be7a4
Model: Character. Source : https://github.com/spazzarama/Direct3D-Rendering-Cookbook
*/
using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Wpf.SharpDX.Assimp;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using Vector3 = global::SharpDX.Vector3;

namespace BoneSkinDemo
{
    public class MainViewModel : BaseViewModel
    {
        public SceneNodeGroupModel3D ModelGroup { get; } = new SceneNodeGroupModel3D();

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                if(SetValue(ref showWireframe, value))
                {
                    foreach(var m in boneSkinNodes)
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

        private bool showSkeleton = false;
        public bool ShowSkeleton
        {
            set
            {
                if(SetValue(ref showSkeleton, value))
                {
                    foreach(var m in skeletonNodes)
                    {
                        m.Visible = value;
                    }
                }
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
                    compositeHelper.Rendering += CompositeHelper_Rendering;
                }
                else
                {
                    compositeHelper.Rendering -= CompositeHelper_Rendering;
                }
            }
            get
            {
                return enableAnimation;
            }
        }

        private string selectedAnimation;
        public string SelectedAnimation
        {
            set
            {
                if(SetValue(ref selectedAnimation, value))
                {
                    reset = true;
                    var curr = scene.Animations.Where(x => x.Name == value).FirstOrDefault();
                    animationUpdater = new NodeAnimationUpdater(curr);
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

        public LineGeometry3D HitLineGeometry { get; } = new LineGeometry3D() { IsDynamic = true };

        public string[] Animations { set; get; }

        public GridPattern[] GridTypes { get; } = new GridPattern[] { GridPattern.Tile, GridPattern.Grid };

        public AnimationRepeatMode[] RepeatModes { get; } = new AnimationRepeatMode[] { AnimationRepeatMode.Loop, AnimationRepeatMode.PlayOnce, AnimationRepeatMode.PlayOnceHold };

        private const int NumSegments = 100;
        private const int Theta = 24;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SynchronizationContext context = SynchronizationContext.Current;

        private bool reset = true;
        private HelixToolkitScene scene;
        private NodeAnimationUpdater animationUpdater;
        private List<BoneSkinMeshNode> boneSkinNodes = new List<BoneSkinMeshNode>();
        private List<BoneSkinMeshNode> skeletonNodes = new List<BoneSkinMeshNode>();
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();

        public MainViewModel()
        {
            this.Title = "BoneSkin Demo";
            this.SubTitle = "WPF & SharpDX";
            EffectsManager = new DefaultEffectsManager();

            this.Camera = new PerspectiveCamera
            {
                Position = new Media3D.Point3D(50, 50, 50),
                LookDirection = new Media3D.Vector3D(-50, -50, -50),
                UpDirection = new Media3D.Vector3D(0, 1, 0),
                NearPlaneDistance = 1,
                FarPlaneDistance = 2000
            };
            HitLineGeometry.Positions = new Vector3Collection(2);
            HitLineGeometry.Positions.Add(Vector3.Zero);
            HitLineGeometry.Positions.Add(Vector3.Zero);
            HitLineGeometry.Indices = new IntCollection(2);
            HitLineGeometry.Indices.Add(0);
            HitLineGeometry.Indices.Add(1);
            LoadFile();
            compositeHelper.Rendering += CompositeHelper_Rendering;           
        }

        private void LoadFile()
        {
            var importer = new Importer();
            importer.Configuration.CreateSkeletonForBoneSkinningMesh = true;
            importer.Configuration.SkeletonSizeScale = 0.04f;
            importer.Configuration.GlobalScale = 0.1f;
            scene = importer.Load("Solus_The_Knight.fbx");
            ModelGroup.AddNode(scene.Root);
            Animations = scene.Animations.Select(x => x.Name).ToArray();
            foreach(var node in scene.Root.Items.Traverse(false))
            {
                if(node is BoneSkinMeshNode m)
                {                     
                    if (!m.IsSkeletonNode)
                    {
                        m.IsThrowingShadow = true;
                        m.WireframeColor = new SharpDX.Color4(0, 0, 1, 1);
                        boneSkinNodes.Add(m);
                        m.MouseDown += M_MouseDown;
                    }
                    else
                    {
                        skeletonNodes.Add(m);
                        m.Visible = false;
                    }
                }
            }
        }

        private void M_MouseDown(object sender, SceneNodeMouseDownArgs e)
        {
            var result = e.HitResult;
            HitLineGeometry.Positions[0] = result.PointHit - result.NormalAtHit * 0.5f;
            HitLineGeometry.Positions[1] = result.PointHit + result.NormalAtHit * 0.5f;
            HitLineGeometry.UpdateVertices();
        }

        private void CompositeHelper_Rendering(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            if (animationUpdater != null)
            {
                if (reset)
                {
                    animationUpdater.Reset();
                    animationUpdater.RepeatMode = SelectedRepeatMode;
                    reset = false;
                }
                else
                {
                    animationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            cts.Cancel(true);
            compositeHelper.Dispose();
            base.Dispose(disposing);
        }
    }
}
