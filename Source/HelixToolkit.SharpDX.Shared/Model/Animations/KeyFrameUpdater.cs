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
        public sealed class KeyFrameUpdater
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
            /// <returns></returns>
            public Matrix[] Update(long timeStamp, long frequency)
            {
                Matrix[] bones = null;
                Update(timeStamp, frequency, ref bones);
                return bones;
            }
            /// <summary>
            /// Updates the animation by specified time stamp (ticks) and frequency (ticks per second).
            /// </summary>
            /// <param name="timeStamp">The time stamp (ticks).</param>
            /// <param name="frequency">The frequency (ticks per second).</param>
            /// <param name="bones">The bones.</param>
            public void Update(long timeStamp, long frequency, ref Matrix[] bones)
            {
                if(currentTime == 0)
                {
                    currentTime = timeStamp;
                }
                var timeElpased = (float)Math.Max(0, timeStamp - currentTime) / frequency;

                if(timeElpased > Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            return;
                        case AnimationRepeatMode.PlayOnceHold:
                            OutputBones(ref bones);
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
                    tempBones[frame.BoneIndex] = frame.Transform;
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
                            // Interpolation using Lerp on scale and translation, and Slerp on Rotation (Quaternion)
                            // Decompose the previous key-frame's transform
                            currFrame.Value.Transform.DecomposeUniformScale(out float s1, out Quaternion q1, out Vector3 t1);
                            // Decompose the current key-frame's transform
                            nextFrame.Transform.DecomposeUniformScale(out float s2, out Quaternion q2, out Vector3 t2);

                            // Perform interpolation and reconstitute matrix
                            tempBones[nextFrame.BoneIndex] =
                                Matrix.Scaling(MathUtil.Lerp(s1, s2, amount)) *
                                Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, amount)) *
                                Matrix.Translation(Vector3.Lerp(t1, t2, amount));
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
                OutputBones(ref bones);

                if(RepeatMode == AnimationRepeatMode.Loop)
                {
                    if(timeElpased > EndTime)
                    {
                        CurrentRangeIndex = 0;
                        currentTime = 0;
                    }
                }
            }


            private void OutputBones(ref Matrix[] bones)
            {
                if (bones == null || bones.Length != BoneCount)
                {
                    bones = currentBones.ToArray();
                }
                else
                {
                    currentBones.CopyTo(bones, 0);
                }
            }

            public void Reset()
            {
                CurrentRangeIndex = 0;
                currentTime = 0;
            }
        }

        public sealed class NodeAnimationUpdater : IAnimationUpdater
        {
            private struct IndexTime
            {
                public int Index;
                public float AccumulatedTime;
            }

            public Animation Animation { get; }
            private long currentTime;
            private IndexTime[] keyframeIndices;

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
                    return;
                }
                var timeElpased = (float)Math.Max(0, timeStamp - currentTime) / frequency;

                if (timeElpased > Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            return;
                        case AnimationRepeatMode.PlayOnceHold:                           
                            return;
                    }
                }
                UpdateBoneSkinMesh();
                UpdateNodes(timeElpased);
                currentTime = timeStamp;
            }

            private void UpdateBoneSkinMesh()
            {
                var rootInv = Animation.RootNode.TotalModelMatrixInternal;
                rootInv.Invert();
                foreach(var m in Animation.BoneSkinMeshes)
                {
                    if (m.IsRenderable && m.Geometry is BoneSkinnedMeshGeometry3D mesh)
                    {
                        m.BoneMatrices = BoneSkinnedMeshGeometry3D.CreateNodeBasedBoneMatrices(mesh.Bones, ref rootInv); 
                    }
                }
            }

            private void UpdateNodes(float timeElapsed)
            {
                for (int i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    int count = n.KeyFrames.Count; // Make sure to use this count
                    var frames = n.KeyFrames.Items; 
                    ref var idxTime = ref keyframeIndices[i];
                    idxTime.AccumulatedTime += timeElapsed;
                    while(idxTime.Index < count - 1 && idxTime.AccumulatedTime >= frames[idxTime.Index+1].Time)//check if should move to next time frame
                    {
                        ++idxTime.Index;
                    }
                    if (idxTime.Index >= count - 1)//check if is at the end
                    {
                        idxTime.Index = 0;
                        idxTime.AccumulatedTime = 0;
                    }
                    ref var currFrame = ref frames[idxTime.Index];
                    ref var nextFrame = ref frames[idxTime.Index + 1];
                    float diff = idxTime.AccumulatedTime - currFrame.Time;
                    float length = nextFrame.Time - currFrame.Time;
                    float amount = diff / length;
                    var transform = Matrix.Scaling(Vector3.Lerp(currFrame.Scale, nextFrame.Scale, amount)) *
                                Matrix.RotationQuaternion(Quaternion.Slerp(currFrame.Rotation, nextFrame.Rotation, amount)) *
                                Matrix.Translation(Vector3.Lerp(currFrame.Position, nextFrame.Position, amount));
                    n.Node.ModelMatrix = transform;
                }
            }

            public void Reset()
            {
                Array.Clear(keyframeIndices, 0, keyframeIndices.Length);
                currentTime = 0;
            }
        }
    }

}
