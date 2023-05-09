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
using System.Windows.Input;
using System.Linq;

namespace MorphTargetAnimationDemo
{
    public class MainViewModel : BaseViewModel
    {
        public SceneNodeGroupModel3D ModelGroup { get; private set; }
        public string DebugLabel { get; set; }

        private HelixToolkitScene scn;
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();
        private List<IAnimationUpdater> animationUpdaters;

        private double endTime = 0;
        public double EndTime
        {
            set { SetValue(ref endTime, value); }
            get { return endTime; }
        }

        private double currTime = 0;
        public double CurrTime
        {
            set
            {
                if (SetValue(ref currTime, value))
                {
                    foreach (IAnimationUpdater updater in animationUpdaters)
                    { 
                        updater.Update((float)value, 1);
                    }
                }
            }
            get { return currTime; }
        }

        private bool isPlaying = false;
        public bool IsPlaying
        {
            private set { SetValue(ref isPlaying, value); }
            get { return isPlaying; }
        }

        public ICommand PlayCommand { get; }

        private long initTime = 0;

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            ModelGroup = new SceneNodeGroupModel3D();

            //Test importing
            Importer importer = new Importer();
            importer.Configuration.CreateSkeletonForBoneSkinningMesh = true;
            importer.Configuration.SkeletonSizeScale = 0.01f;
            importer.Configuration.GlobalScale = 0.1f;
            scn = importer.Load("Gunan_animated.fbx");

            //Add to model group for rendering
            ModelGroup.AddNode(scn.Root);

            //Setup each animation, this will actively play all (not always desired)
            animationUpdaters = new List<IAnimationUpdater>(scn.Animations.CreateAnimationUpdaters().Values);
            EndTime = scn.Animations.Max(x => x.EndTime);
            PlayCommand = new RelayCommand((o) =>
            {
                if (!IsPlaying)
                {
                    initTime = 0;
                    compositeHelper.Rendering += Render;
                }
                else
                {
                    compositeHelper.Rendering -= Render;
                }
                IsPlaying = !IsPlaying;
            });
        }

        private void Render(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            //Animation with perf testing
            long t = Stopwatch.GetTimestamp();
            if (initTime == 0)
            {
                initTime = t;
            }
            //Update animation. Ensures all animation times are in sync
            CurrTime = ((t - initTime) / (double)Stopwatch.Frequency) % EndTime;
            t = Stopwatch.GetTimestamp() - t;
            DebugLabel = t.ToString();
        }
    }
}