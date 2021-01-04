// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Media3D = System.Windows.Media.Media3D;
using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Wpf.SharpDX.Assimp;
using SharpDX;

namespace MorphTargetAnimationDemo
{
    public class MainViewModel : BaseViewModel
    {
        public SceneNodeGroupModel3D ModelGroup { get; private set; }
        public ObservableCollection<float> weights { get; set; }
        public string debugLabel { get; set; }

        private HelixToolkitScene scn;
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();
        private long sum = 0;
        private long count = 0;
        private List<IAnimationUpdater> animationUpdaters;

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            ModelGroup = new SceneNodeGroupModel3D();

            compositeHelper.Rendering += Render;

            //Test importing
            Importer importer = new Importer();
            importer.Configuration.CreateSkeletonForBoneSkinningMesh = true;
            importer.Configuration.SkeletonSizeScale = 0.01f;
            importer.Configuration.GlobalScale = 0.1f;
            scn = importer.Load("../../zophrac/source/Gunan_animated.fbx");

            //Make it renderable
            ModelGroup.AddNode(scn.Root);

            //Setup animation
            animationUpdaters = new List<IAnimationUpdater>();
            foreach (Animation anim in scn.Animations)
            {
                if (anim.AnimationType == AnimationType.MorphTarget)
                {
                    animationUpdaters.Add(new MorphTargetKeyFrameUpdater(anim, (anim.RootNode as BoneSkinMeshNode).MorphTargetWeights));
                    animationUpdaters[animationUpdaters.Count - 1].RepeatMode = AnimationRepeatMode.Loop;
                }
                else if (anim.AnimationType == AnimationType.Node)
                {
                    animationUpdaters.Add(new NodeAnimationUpdater(anim));
                    animationUpdaters[animationUpdaters.Count - 1].RepeatMode = AnimationRepeatMode.Loop;
                }
            }
        }

        private void Render(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            //Animation with perf testing
            long t = Stopwatch.GetTimestamp();

            //Update animation. Ensures all animation times are in sync
            long ts = Stopwatch.GetTimestamp();
            long fq = Stopwatch.Frequency;
            foreach (IAnimationUpdater updater in animationUpdaters)
                updater.Update(ts, fq);

            t = Stopwatch.GetTimestamp() - t;
            sum += t;
            count++;
            //debugLabel = (sum / count).ToString();
            debugLabel = t.ToString();

            scn.Root.InvalidateRender();
        }
    }
}