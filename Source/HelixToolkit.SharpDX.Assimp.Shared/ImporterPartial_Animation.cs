/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Animation = Assimp.Animation;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using HxAnimations = Animations;
    using HxScene = Model.Scene;
    namespace Assimp
    {
        /// <summary>
        /// 
        /// </summary>
        public partial class Importer
        {
            /// <summary>
            /// Processes the node animation.
            /// </summary>
            /// <param name="channel">The channel.</param>
            /// <param name="ticksPerSecond">The ticks per second.</param>
            /// <param name="list">The list.</param>
            /// <returns></returns>
            protected virtual ErrorCode ProcessNodeAnimation(NodeAnimationChannel channel, double ticksPerSecond, out FastList<HxAnimations.Keyframe> list)
            {
                var posCount = channel.PositionKeyCount;
                var rotCount = channel.RotationKeyCount;
                var scaleCount = channel.ScalingKeyCount;
                if (posCount != rotCount || rotCount != scaleCount)
                {
                    list = null;
                    ErrorCode |= ErrorCode.NonUniformAnimationKeyDoesNotSupported;
                    return ErrorCode.NonUniformAnimationKeyDoesNotSupported;
                }

                var ret = new FastList<HxAnimations.Keyframe>(posCount);
                for (var i = 0; i < posCount; ++i)
                {
                    ret.Add(new HxAnimations.Keyframe
                    {
                        Time = (float)(channel.PositionKeys[i].Time / ticksPerSecond),
                        Translation = channel.PositionKeys[i].Value.ToSharpDXVector3(),
                        Rotation = channel.RotationKeys[i].Value.ToSharpDXQuaternion(),
                        Scale = channel.ScalingKeys[i].Value.ToSharpDXVector3()
                    });
                }
                list = ret;
                return ErrorCode.Succeed;
            }

            private ErrorCode LoadAnimations(HelixInternalScene scene)
            {
                var dict = new Dictionary<string, HxScene.SceneNode>(SceneNodes.Count);
                foreach (var node in SceneNodes)
                {
                    if (node is HxScene.GroupNode && !dict.ContainsKey(node.Name))
                    {
                        dict.Add(node.Name, node);
                    }
                }

                foreach (var node in SceneNodes.Where(x => x is Animations.IBoneMatricesNode)
                    .Select(x => x as Animations.IBoneMatricesNode))
                {
                    if (node.Bones != null)
                    {
                        for (var i = 0; i < node.Bones.Length; ++i)
                        {
                            if (dict.TryGetValue(node.Bones[i].Name, out var s))
                            {
                                ref var b = ref node.Bones[i];
                                b.ParentNode = s.Parent;
                                b.Node = s;
                                s.IsAnimationNode = true; // Make sure to set this to true
                            }
                        }
                    }
                }

                if (scene.AssimpScene.HasAnimations)
                {
                    bool hasBoneSkinnedMesh = scene.Meshes.Where(x => x.Mesh is BoneSkinnedMeshGeometry3D).Count() > 0 ? true : false;
                    var animationList = new List<HxAnimations.Animation>(scene.AssimpScene.AnimationCount);
                    if (Configuration.EnableParallelProcessing)
                    {
                        Parallel.ForEach(scene.AssimpScene.Animations, ani =>
                       {
                           if (LoadAnimation(ani, dict, hasBoneSkinnedMesh, out var hxAni) == ErrorCode.Succeed)
                           {
                               lock (animationList)
                               {
                                   animationList.Add(hxAni);
                               }
                           }
                       });
                    }
                    else
                    {
                        foreach (var ani in scene.AssimpScene.Animations)
                        {
                            if (LoadAnimation(ani, dict, hasBoneSkinnedMesh, out var hxAni) == ErrorCode.Succeed)
                                animationList.Add(hxAni);
                        }
                    }
                    scene.Animations = animationList;
                    Animations.AddRange(animationList);
                }
                return ErrorCode.Succeed;
            }

            private ErrorCode LoadAnimation(Animation ani, Dictionary<string, HxScene.SceneNode> dict, bool searchBoneSkinMeshNode,
                out HxAnimations.Animation hxAni)
            {
                hxAni = new HxAnimations.Animation(HxAnimations.AnimationType.Node)
                {
                    StartTime = 0,
                    EndTime = (float)(ani.DurationInTicks / ani.TicksPerSecond),
                    Name = ani.Name,
                    NodeAnimationCollection = new List<HxAnimations.NodeAnimation>(ani.NodeAnimationChannelCount)
                };

                if (ani.HasNodeAnimations)
                {
                    var code = ErrorCode.None;
                    foreach (var key in ani.NodeAnimationChannels)
                    {
                        if (dict.TryGetValue(key.NodeName, out var node))
                        {
                            var nAni = new HxAnimations.NodeAnimation
                            {
                                Node = node
                            };
                            node.IsAnimationNode = true;// Make sure to set this to true
                            code = ProcessNodeAnimation(key, ani.TicksPerSecond, out var keyframes);
                            if (code == ErrorCode.Succeed)
                            {
                                nAni.KeyFrames = keyframes;
                                hxAni.NodeAnimationCollection.Add(nAni);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (searchBoneSkinMeshNode)
                    {
                        FindBoneSkinMeshes(hxAni);
                    }
                    return code;
                }

                return ErrorCode.Failed;
            }

            private void FindBoneSkinMeshes(HxAnimations.Animation animation)
            {
                if(animation.NodeAnimationCollection != null && animation.NodeAnimationCollection.Count > 0)
                {
                    // Search all the bone skinned meshes from the common animation node root
                    var node = animation.NodeAnimationCollection[0].Node;
                    while(node != null && !node.IsAnimationNodeRoot)
                    {
                        node = node.Parent;
                    }

                    if (node == null)
                    {
                        return;
                    }
                    
                    if(node.Parent != null)
                    node = node.Parent;
                    animation.BoneSkinMeshes = new List<Animations.IBoneMatricesNode>();
                    animation.RootNode = node;
                    foreach (var n in SceneNodes[0].Items.PreorderDFT((m) => { return true; }))
                    {
                        if(n is Animations.IBoneMatricesNode boneNode)
                        {
                            animation.BoneSkinMeshes.Add(boneNode);
                        }
                    }
                }              
            }
        }
    }
}