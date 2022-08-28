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
            public string Name
            {
                set; get;
            } = string.Empty;

            public Animation Animation
            {
                get;
            }

            public int CurrentRangeIndex
            {
                private set; get;
            }

            public IList<Bone> Bones
            {
                get;
            }

            public AnimationRepeatMode RepeatMode
            {
                set; get;
            } = AnimationRepeatMode.PlayOnce;

            public float StartTime
            {
                get;
            }

            public float EndTime
            {
                get;
            }

            public float Speed { set; get; } = 1.0f;

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
                Name = animation.Name;
                BoneCount = bones.Count;
                tempKeyframes = new Keyframe?[BoneCount];
                tempBones = new Matrix[BoneCount];
                currentBones = new Matrix[BoneCount];
                Bones = bones;
                EndTime = animation.EndTime;
                StartTime = animation.StartTime;
                var time = animation.Keyframes[0].Time;
                timeRange.Add(0);
                for (var i = 1; i < animation.Keyframes.Count; ++i)
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
                if (Animation.BoneSkinMeshes == null || Animation.BoneSkinMeshes.Count == 0)
                {
                    return;
                }
                if (currentTime == 0)
                {
                    currentTime = timeStamp;
                }
                var timeElpased = (float)Math.Max(0, timeStamp - currentTime) / frequency * Speed;
                var boneNode = Animation.BoneSkinMeshes[0];
                if (timeElpased > Animation.EndTime)
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
                for (var i = CurrentRangeIndex + 1; i < timeRange.Count - 1; ++i)
                {
                    if (Animation.Keyframes[timeRange[i]].Time > timeElpased)
                    {
                        CurrentRangeIndex = i - 1;
                        break;
                    }
                }

                var start = timeRange[CurrentRangeIndex];
                var end = timeRange[CurrentRangeIndex + 1];
                for (var i = start; i < end; ++i)
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
                    for (var i = startNext; i < endNext; ++i)
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

                if (RepeatMode == AnimationRepeatMode.Loop)
                {
                    if (timeElpased > EndTime)
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
    }
}
