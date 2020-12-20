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

            //Setup cube mesh
            MeshBuilder mb = new MeshBuilder();
            mb.AddCube();
            mb.ComputeNormalsAndTangents(MeshFaces.Default, true);

            //Setup bone skinned geometry
            BoneSkinnedMeshGeometry3D skinnedGeometry = new BoneSkinnedMeshGeometry3D(mb.ToMesh());

            //Setup skinned mesh node
            skinnedMeshNode = new BoneSkinMeshNode();
            skinnedMeshNode.Geometry = skinnedGeometry;
            skinnedMeshNode.Material = new PhongMaterial();

            //Add skinned mesh node to model group
            ModelGroup.AddNode(skinnedMeshNode);

            //We dont have a skeleton for this cube, just use identity
            skinnedMeshNode.SetupIdentitySkeleton();

            //Create morph targets
            System.Random r = new System.Random();
            int pitch = mb.ToMesh().Positions.Count;
            MorphTargetVertex[] mtv = new MorphTargetVertex[pitch * 4];
            for (int i = 0; i < mtv.Length; i++)
            {
                mtv[i].deltaPosition = new Vector3(0, 0, 0);
                mtv[i].deltaNormal = new Vector3(0, 0, 0);
                mtv[i].deltaTangent = new Vector3(0, 0, 0);

                if (i < pitch)
                    mtv[i].deltaPosition = skinnedGeometry.Normals[i];
                else if (i < pitch * 2)
                    mtv[i].deltaPosition = skinnedGeometry.Tangents[i % pitch];
                else if (i < pitch * 3)
                {
                    Vector3 p = skinnedGeometry.Positions[i % pitch] - new Vector3(.5f, .5f, .5f);
                    Vector3 n = skinnedGeometry.Normals[i % pitch];
                    mtv[i].deltaPosition = p - (Vector3.Dot(p, n) / n.LengthSquared() * n);
                }
                else if (i < pitch * 4)
                {
                    mtv[i].deltaPosition = new Vector3(r.NextFloat(0, 1), r.NextFloat(0, 1), r.NextFloat(0, 1));
                }
            }

            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).MorphTargetWeights = new float[] { 0, 0, 0, 0 };
            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).InitializeMorphTargets(mtv, pitch);

            //Setup animation
            animation = new Animation(AnimationType.MorphTarget);
            animation.StartTime = 5;
            animation.EndTime = 10;
            animation.morphTargetKeyframes = new List<MorphTargetKeyframe>
            {
                new MorphTargetKeyframe() { Weight=.0f, Time=5, Index=0 },
                new MorphTargetKeyframe() { Weight=1.0f, Time=10, Index=0 },

                new MorphTargetKeyframe() { Weight=.0f, Time=5, Index=1 },
                new MorphTargetKeyframe() { Weight=1.0f, Time=6, Index=1 },
                new MorphTargetKeyframe() { Weight=.0f, Time=7, Index=1 },
                new MorphTargetKeyframe() { Weight=1.0f, Time=8, Index=1 },
                new MorphTargetKeyframe() { Weight=.0f, Time=9, Index=1 },
                new MorphTargetKeyframe() { Weight=1.0f, Time=10, Index=1 },

                new MorphTargetKeyframe() { Weight=.5f, Time=10, Index=2 },

                //Index=3 is added in loop below for extensive perf testing
            };

            //50k keyframes
            for (float t = 5; t < 10; t += .0001f)
                animation.morphTargetKeyframes.Add(new MorphTargetKeyframe() { Weight = (t % .25f) * 4, Time = t, Index = 3 });

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
            debugLabel = (sum / count).ToString();

            //Sloppy way to set weights to updated so that it will update buffer
            skinnedMeshNode.SetWeight(0, skinnedMeshNode.MorphTargetWeights[0]);

            //Update weights sliders
            for (int i = 0; i < weights.Count; i++)
                weights[i] = skinnedMeshNode.MorphTargetWeights[i];

            skinnedMeshNode.InvalidateRender();
        }
    }
}