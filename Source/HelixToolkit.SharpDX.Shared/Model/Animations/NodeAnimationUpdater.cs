/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

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
    namespace Animations
    {
        public class NodeAnimationUpdater : IAnimationUpdater
        {
            private struct IndexTime
            {
                public int Index;
            }

            public string Name
            {
                set; get;
            } = string.Empty;

            public Animation Animation
            {
                get;
            }

            public float Speed { set; get; } = 1.0f;

            private long currentTime;
            private IndexTime[] keyframeIndices;
            private float accumulatedTime;
            private bool isStartFrame = false;
            private bool changed = false;

            public IList<NodeAnimation> NodeCollection
            {
                get => Animation.NodeAnimationCollection;
            }

            public AnimationRepeatMode RepeatMode
            {
                set; get;
            } = AnimationRepeatMode.Loop;

            public NodeAnimationUpdater(Animation animation)
            {
                Animation = animation;
                Name = animation.Name;
                keyframeIndices = new IndexTime[NodeCollection.Count];
            }

            public void Update(long timeStamp, long frequency)
            {
                if (currentTime == 0)
                {
                    currentTime = timeStamp;
                    accumulatedTime = 0;
                    SetToStart();
                    return;
                }

                var timeElpased = (float)Math.Max(0, timeStamp - currentTime) / frequency * Speed;

                if (accumulatedTime >= Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            UpdateBoneSkinMesh();
                            isStartFrame = false;
                            SetToStart();
                            return;
                        case AnimationRepeatMode.PlayOnceHold:
                            return;
                    }
                }
                if (accumulatedTime >= Animation.EndTime)
                {
                    Reset();
                }
                UpdateBoneSkinMesh();
                UpdateNodes(timeElpased);
                currentTime = timeStamp;
            }

            private void UpdateBoneSkinMesh()
            {
                if (Animation.HasBoneSkinMeshes && changed)
                {
                    foreach (var m in Animation.BoneSkinMeshes)
                    {
                        if (m.IsRenderable && !m.HasBoneGroup)// Do not update if has a bone group. Update the group only
                        {
                            var inv = m.TotalModelMatrix.Inverted();
                            var matrices = OnGetNewBoneMatrices(m.Bones.Length);
                            BoneSkinnedMeshGeometry3D.CreateNodeBasedBoneMatrices(m.Bones, ref inv, ref matrices);
                            var old = m.BoneMatrices;
                            m.BoneMatrices = matrices;
                            OnReturnOldBoneMatrices(old);
                        }
                    }
                    changed = false;
                }
            }

            /// <summary>
            /// Called when [get new bone matrices]. Override this to provide your own matices pool to avoid small object creation
            /// </summary>
            /// <param name="count">The count.</param>
            /// <returns></returns>
            protected virtual Matrix[] OnGetNewBoneMatrices(int count)
            {
                return new Matrix[count];
            }
            /// <summary>
            /// Called when [return old bone matrices]. Override this to return the old matrix array back to your own matices pool. <see cref="OnGetNewBoneMatrices(int)"/>
            /// </summary>
            /// <param name="m">The m.</param>
            protected virtual void OnReturnOldBoneMatrices(Matrix[] m)
            {

            }

            private void UpdateNodes(float timeElapsed)
            {
                accumulatedTime += timeElapsed;
                for (var i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    var count = n.KeyFrames.Count; // Make sure to use this count
                    var frames = n.KeyFrames.Items;
                    ref var idxTime = ref keyframeIndices[i];
                    while (idxTime.Index < count - 1 && accumulatedTime > frames[idxTime.Index + 1].Time)//check if should move to next time frame
                    {
                        ++idxTime.Index;
                    }
                    if (idxTime.Index >= count - 1)//check if is at the end, if at the end, stays there
                    {
                        continue;
                    }
                    ref var currFrame = ref frames[idxTime.Index];
                    if (count == 1)
                    {
                        n.Node.ModelMatrix = Matrix.Scaling(currFrame.Scale) *
                                Matrix.RotationQuaternion(currFrame.Rotation) *
                                Matrix.Translation(currFrame.Translation);
                    }
                    else
                    {
                        ref var nextFrame = ref frames[idxTime.Index + 1];
                        var diff = accumulatedTime - currFrame.Time;
                        var length = nextFrame.Time - currFrame.Time;
                        var amount = diff / length;
                        var transform = Matrix.Scaling(Vector3.Lerp(currFrame.Scale, nextFrame.Scale, amount)) *
                                    Matrix.RotationQuaternion(Quaternion.Slerp(currFrame.Rotation, nextFrame.Rotation, amount)) *
                                    Matrix.Translation(Vector3.Lerp(currFrame.Translation, nextFrame.Translation, amount));
                        n.Node.ModelMatrix = transform;
                    }
                }
                changed = true;
            }

            public void Reset()
            {
                Array.Clear(keyframeIndices, 0, keyframeIndices.Length);
                currentTime = 0;
                accumulatedTime = 0;
                changed = false;
                isStartFrame = false;
            }

            private void SetToStart()
            {
                if (isStartFrame)
                {
                    return;
                }
                for (var i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    var count = n.KeyFrames.Count; // Make sure to use this count
                    if (count == 0)
                    {
                        //n.Node.ModelMatrix = Matrix.Identity;
                        continue;
                    }
                    var frames = n.KeyFrames.Items;
                    ref var currFrame = ref frames[0];
                    n.Node.ModelMatrix = Matrix.Scaling(currFrame.Scale) *
                            Matrix.RotationQuaternion(currFrame.Rotation) *
                            Matrix.Translation(currFrame.Translation);
                }
                isStartFrame = true;
                changed = true;
            }
        }
    }
}
