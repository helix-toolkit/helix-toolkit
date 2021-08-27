/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.DirectWrite;
using System.IO;
using System;
using System.Collections.Generic;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System.Linq;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    using Animations;
    public static class AnimationExtensions
    {
        public static Dictionary<string, IAnimationUpdater> CreateAnimationUpdaters(this IEnumerable<Animation> animations)
        {
            var dict = new Dictionary<string, IAnimationUpdater>();
            foreach (var ani in animations)
            {
                switch (ani.AnimationType)
                {
                    case AnimationType.Keyframe:
                        if (ani.RootNode is IBoneMatricesNode bNode)
                        {
                            AddUpdaterToDict(dict, new KeyFrameUpdater(ani, bNode.Bones));
                        }
                        else if (ani.BoneSkinMeshes != null)
                        {
                            foreach (var b in ani.BoneSkinMeshes)
                            {
                                AddUpdaterToDict(dict, new KeyFrameUpdater(ani, b.Bones));
                            }   
                        }
                        break;
                    case AnimationType.Node:
                        AddUpdaterToDict(dict, new NodeAnimationUpdater(ani));
                        break;
                    case AnimationType.MorphTarget:
                        if (ani.RootNode is IBoneMatricesNode mNode)
                        {
                            AddUpdaterToDict(dict, new MorphTargetKeyFrameUpdater(ani, mNode.MorphTargetWeights));
                        }
                        else if (ani.BoneSkinMeshes != null)
                        {
                            foreach (var b in ani.BoneSkinMeshes)
                            {
                                AddUpdaterToDict(dict, new MorphTargetKeyFrameUpdater(ani, b.MorphTargetWeights));
                            }
                        }
                        break;

                }
            }
            return dict;
        }

        private static void AddUpdaterToDict(Dictionary<string, IAnimationUpdater> dict, IAnimationUpdater updater)
        {
            if (dict.TryGetValue(updater.Name, out var existingUpdater))
            {
                if (existingUpdater is AnimationGroupUpdater group)
                {
                    group.Children.Add(updater);
                }
                else
                {
                    dict.Remove(updater.Name);
                    var newGroup = new AnimationGroupUpdater(updater.Name);
                    newGroup.Children.Add(existingUpdater);
                    newGroup.Children.Add(updater);
                    dict.Add(newGroup.Name, newGroup);
                }            
            }
            else
            {
                dict.Add(updater.Name, updater);
            }
        }
    }
}
