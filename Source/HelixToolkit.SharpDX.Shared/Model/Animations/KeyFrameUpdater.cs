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
        /// <summary>
        /// 
        /// </summary>
        public class KeyFrameUpdater : IAnimationUpdater
        {
            public Animation Animation { get; }

            public int CurrentRangeIndex { private set; get; }

            public IList<Bone> Bones { get; }

            public AnimationRepeatMode RepeatMode
            {
                set; get;
            } = AnimationRepeatMode.PlayOnce;

            public float StartTime { get; }

            public float EndTime { get; }

            private readonly Keyframe?[] tempKeyframes;

            private readonly Matrix[] tempBones;

            private readonly Matrix[] currentBones;

            private readonly List<int> timeRange = new List<int>();

            private readonly int BoneCount;

            private long currentTime;
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyFrameUpdater"/> class.
            /// </summary>
            /// <param name="animation">The animation.</param>
            /// <param name="bones">The bones.</param>
            public KeyFrameUpdater(Animation animation, IList<Bone> bones)
            {
                Animation = animation;
                BoneCount = bones.Count;
                tempKeyframes = new Keyframe?[BoneCount];
                tempBones = new Matrix[BoneCount];
                currentBones = new Matrix[BoneCount];
                Bones = bones;
                EndTime = animation.EndTime;
                StartTime = animation.StartTime;
                float time = animation.Keyframes[0].Time;
                timeRange.Add(0);
                for (int i = 1; i < animation.Keyframes.Count; ++i)
                {
                    if (time < animation.Keyframes[i].Time)
                    {
                        time = animation.Keyframes[i].Time;
                        timeRange.Add(i);
                    }
                }
                timeRange.Add(animation.Keyframes.Count);
            }

            /// <summary>
            /// Updates the animation by specified time stamp (ticks) and frequency (ticks per second).
            /// </summary>
            /// <param name="timeStamp">The time stamp (ticks).</param>
            /// <param name="frequency">The frequency (ticks per second).</param>
            public void Update(long timeStamp, long frequency)
            {
                if(Animation.BoneSkinMeshes ==null || Animation.BoneSkinMeshes.Count == 0)
                {
                    return;
                }
                if(currentTime == 0)
                {
                    currentTime = timeStamp;
                }
                var timeElpased = (float)Math.Max(0, timeStamp - currentTime) / frequency;
                var boneNode = Animation.BoneSkinMeshes[0];
                if(timeElpased > Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            return;
                        case AnimationRepeatMode.PlayOnceHold:
                            OutputBones(boneNode);
                            return;
                    }
                }
                //Search for time range
                for (int i = CurrentRangeIndex + 1; i < timeRange.Count - 1; ++i)
                {
                    if (Animation.Keyframes[timeRange[i]].Time > timeElpased)
                    {
                        CurrentRangeIndex = i - 1;
                        break;
                    }
                }
                //Console.WriteLine("CurrentRangeIndex: " + CurrentRangeIndex);
                var start = timeRange[CurrentRangeIndex];
                var end = timeRange[CurrentRangeIndex + 1];
                for (int i = start; i < end; ++i)
                {
                    var frame = Animation.Keyframes[i];
                    tempBones[frame.BoneIndex] = frame.ToTransformMatrix();
                    tempKeyframes[frame.BoneIndex] = frame;
                }
                if (timeElpased > Animation.Keyframes[start].Time)
                {
                    var startNext = end;
                    var endNext = timeRange[Math.Min(CurrentRangeIndex + 2, timeRange.Count - 1)];
                    // Calculate time difference between frames
                    var frameLength = Animation.Keyframes[startNext].Time - Animation.Keyframes[start].Time;
                    var timeDiff = timeElpased - Animation.Keyframes[start].Time;
                    var amount = timeDiff / frameLength;
                    for (int i = startNext; i < endNext; ++i)
                    {
                        var nextFrame = Animation.Keyframes[i];
                        var currFrame = tempKeyframes[nextFrame.BoneIndex];
                        if (currFrame.HasValue)
                        {
                            // Perform interpolation and reconstitute matrix
                            tempBones[nextFrame.BoneIndex] =
                                Matrix.Scaling(Vector3.Lerp(currFrame.Value.Scale, nextFrame.Scale, amount)) *
                                Matrix.RotationQuaternion(Quaternion.Slerp(currFrame.Value.Rotation, nextFrame.Rotation, amount)) *
                                Matrix.Translation(Vector3.Lerp(currFrame.Value.Translation, nextFrame.Translation, amount));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                // Apply parent bone transforms
                // We assume here that the first bone has no parent
                // and that each parent bone appears before children
                for (var i = 1; i < BoneCount; i++)
                {
                    var bone = Bones[i];
                    if (bone.ParentIndex > -1)
                    {
                        var parentTransform = tempBones[bone.ParentIndex];
                        tempBones[i] = tempBones[i] * parentTransform;
                    }
                }
                // Change the bone transform from rest pose space into bone space (using the inverse of the bind/rest pose)
                for (var i = 0; i < BoneCount; i++)
                {
                    currentBones[i] = Bones[i].InvBindPose * tempBones[i];
                }
                OutputBones(boneNode);

                if(RepeatMode == AnimationRepeatMode.Loop)
                {
                    if(timeElpased > EndTime)
                    {
                        CurrentRangeIndex = 0;
                        currentTime = 0;
                    }
                }
            }


            private void OutputBones(IBoneMatricesNode node)
            {
                if (node.BoneMatrices == null || node.BoneMatrices.Length != BoneCount)
                {
                    node.BoneMatrices = currentBones.ToArray();
                }
                else
                {
                    currentBones.CopyTo(node.BoneMatrices, 0);
                }
            }

            public void Reset()
            {
                CurrentRangeIndex = 0;
                currentTime = 0;
            }
        }

        public class NodeAnimationUpdater : IAnimationUpdater
        {
            private struct IndexTime
            {
                public int Index;
            }

            public Animation Animation { get; }
            private long currentTime;
            private IndexTime[] keyframeIndices;
            private float accumulatedTime;
            private bool isStartFrame = false;
            private bool changed = false;

            public IList<NodeAnimation> NodeCollection { get => Animation.NodeAnimationCollection; }

            public AnimationRepeatMode RepeatMode
            {
                set; get;
            } = AnimationRepeatMode.Loop;

            public NodeAnimationUpdater(Animation animation)
            {
                Animation = animation;
                keyframeIndices = new IndexTime[NodeCollection.Count];
            }

            public void Update(long timeStamp, long frequency)
            {
                if(currentTime == 0)
                {
                    currentTime = timeStamp;
                    accumulatedTime = 0;
                    SetToStart();
                    return;
                }

                var timeElpased = (float)Math.Max(0, timeStamp - currentTime) / frequency;

                if (accumulatedTime >= Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            UpdateBoneSkinMesh();
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
                    foreach(var m in Animation.BoneSkinMeshes)
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
                for (int i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    int count = n.KeyFrames.Count; // Make sure to use this count
                    var frames = n.KeyFrames.Items; 
                    ref var idxTime = ref keyframeIndices[i];
                    while(idxTime.Index < count - 1 && accumulatedTime > frames[idxTime.Index+1].Time)//check if should move to next time frame
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
                        float diff = accumulatedTime - currFrame.Time;
                        float length = nextFrame.Time - currFrame.Time;
                        float amount = diff / length;
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
                for (int i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    int count = n.KeyFrames.Count; // Make sure to use this count
                    if(count ==0)
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
