/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            public IList<Bone> Bones
            {
                get;
            }

            public AnimationRepeatMode RepeatMode
            {
                set; get;
            } = AnimationRepeatMode.PlayOnce;

            public float StartTime => Animation.StartTime;

            public float EndTime => Animation.EndTime;

            private readonly Keyframe?[] tempKeyframes;

            private readonly Matrix[] tempBones;

            private readonly Matrix[] currentBones;

            private readonly List<Keyframe>[] keyframes;

            private readonly int BoneCount;
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
                keyframes = new List<Keyframe>[BoneCount];
                for (int i = 0; i < BoneCount; ++i)
                {
                    keyframes[i] = new List<Keyframe>(animation.Keyframes.Count / BoneCount);
                }
                foreach(var frame in animation.Keyframes.OrderBy(x => x.Time))
                {
                    keyframes[frame.BoneIndex].Add(frame);
                }
            }

            /// <summary>
            /// Updates the animation by specified time stamp (ticks) and frequency (ticks per second).
            /// </summary>
            /// <param name="timeStamp">The time stamp (ticks).</param>
            /// <param name="frequency">The frequency (ticks per second).</param>
            public void Update(float timeStamp, long frequency)
            {
                if (Animation.BoneSkinMeshes == null || Animation.BoneSkinMeshes.Count == 0)
                {
                    return;
                }
                var timeSec = timeStamp / frequency;
                if (timeSec < StartTime)
                {
                    return;
                }
                if (StartTime == EndTime)
                {
                    return;
                }
                var timeElapsed = timeSec - StartTime;
                var boneNode = Animation.BoneSkinMeshes[0];
                if (timeElapsed > Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            return;
                        case AnimationRepeatMode.PlayOnceHold:
                            OutputBones(boneNode);
                            return;
                        case AnimationRepeatMode.Loop:
                            timeElapsed = timeElapsed % EndTime + StartTime;
                            return;
                    }
                }
                foreach(var frames in keyframes)
                {
                    var idx = AnimationUtils.FindKeyFrame(timeElapsed, frames);
                    ref var currFrame = ref frames.GetInternalArray()[idx]; 
                    if (currFrame.Time > timeElapsed && idx == 0)
                    {
                        continue;
                    }
                    Debug.Assert(currFrame.Time <= timeElapsed);
                    if (frames.Count == 1 || idx == frames.Count - 1)
                    {
                        tempBones[currFrame.BoneIndex] = currFrame.ToTransformMatrix();
                        continue;
                    }
                    ref var nextFrame = ref frames.GetInternalArray()[idx + 1];
                    Debug.Assert(nextFrame.Time >= timeElapsed);
                    var diff = timeElapsed - currFrame.Time;
                    var length = nextFrame.Time - currFrame.Time;
                    var amount = diff / length;
                    tempBones[currFrame.BoneIndex] = Matrix.Scaling(Vector3.Lerp(currFrame.Scale, nextFrame.Scale, amount)) *
                                Matrix.RotationQuaternion(Quaternion.Slerp(currFrame.Rotation, nextFrame.Rotation, amount)) *
                                Matrix.Translation(Vector3.Lerp(currFrame.Translation, nextFrame.Translation, amount));
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
                
            }
        }
    }
}
