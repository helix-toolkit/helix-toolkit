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

        private BoneSkinMeshNode skinnedMeshNode;
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();
        private Animation animation;
        private MorphTargetKeyFrameUpdater mtUpdater;
        private long sum = 0;
        private long count = 0;

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            ModelGroup = new SceneNodeGroupModel3D();

            compositeHelper.Rendering += Render;

            //Test importing
            Importer importer = new Importer();
            importer.Configuration.CreateSkeletonForBoneSkinningMesh = true;
            importer.Configuration.GlobalScale = .1f;
            HelixToolkitScene scn = importer.Load("../../MonkeyTwistMT.fbx");

            //Make it renderable
            ///scene->root->suzzanne SceneNode->suzzanne BoneSkinMeshNode
            skinnedMeshNode = scn.Root.Items[0].Items[0] as BoneSkinMeshNode;
            skinnedMeshNode.Material = new PhongMaterial();
            ModelGroup.AddNode(scn.Root);

            //Setup animation
            animation = new Animation(AnimationType.MorphTarget);
            animation.StartTime = 0;
            animation.EndTime = 1;
            animation.morphTargetKeyframes = new List<MorphTargetKeyframe>
            {
                new MorphTargetKeyframe() { Weight=.0f, Time=0, Index=0 },
                new MorphTargetKeyframe() { Weight=1.0f, Time=1, Index=0 },
            };

            mtUpdater = new MorphTargetKeyFrameUpdater(animation, skinnedMeshNode.MorphTargetWeights);
            mtUpdater.RepeatMode = AnimationRepeatMode.Loop;

            weights = new ObservableCollection<float>(skinnedMeshNode.MorphTargetWeights);
        }

        private void Render(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            //Animation with perf testing
            long t = Stopwatch.GetTimestamp();
            mtUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
            t = Stopwatch.GetTimestamp() - t;
            sum += t;
            count++;
            //debugLabel = (sum / count).ToString();
            debugLabel = t.ToString();

            //Sloppy way to set weights to updated so that it will update buffer
            skinnedMeshNode.SetWeight(0, skinnedMeshNode.MorphTargetWeights[0]);

            //Update weights sliders
            for (int i = 0; i < weights.Count; i++)
                weights[i] = skinnedMeshNode.MorphTargetWeights[i];

            skinnedMeshNode.InvalidateRender();
        }
    }
}