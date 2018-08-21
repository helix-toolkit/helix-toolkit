/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Animations
#else
namespace HelixToolkit.UWP.Animations
#endif
{
    public enum AnimationRepeatMode
    {
        PlayOnce,
        Loop,
        PlayOnceHold,
    }

    public sealed class KeyFrameUpdater
    {
        public Animation Animation { get; }

        public int CurrentRangeIndex { private set; get; }

        public IList<Bone> Bones { get; }

        public AnimationRepeatMode RepeatMode
        {
            set; get;
        } = AnimationRepeatMode.Loop;

        public float StartTime { get; }

        public float EndTime { get; }

        private readonly Keyframe?[] tempKeyframes;

        private readonly Matrix[] tempBones;

        private readonly List<int> timeRange = new List<int>();

        private readonly int BoneCount;

        private float currentTime;

        public KeyFrameUpdater(Animation animation, IList<Bone> bones)
        {
            Animation = animation;
            BoneCount = bones.Count;
            tempKeyframes = new Keyframe?[BoneCount];
            tempBones = new Matrix[BoneCount];
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

        public Matrix[] Update(float timeStamp)
        {
            Matrix[] bones = null;
            Update(timeStamp, ref bones);
            return bones;
        }

        public void Update(float timeStamp, ref Matrix[] bones)
        {
            if(currentTime == 0)
            {
                currentTime = timeStamp;
            }
            var timeElpased = Math.Max(0, timeStamp - currentTime);
            if(timeElpased > Animation.EndTime)
            {
                if(RepeatMode == AnimationRepeatMode.PlayOnceHold || RepeatMode == AnimationRepeatMode.PlayOnce)
                {
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

            var start = timeRange[CurrentRangeIndex];
            var end = timeRange[CurrentRangeIndex + 1];
            for (int i = start; i < end; ++i)
            {
                var frame = Animation.Keyframes[i];
                tempBones[frame.BoneIndex] = frame.Transform;
                tempKeyframes[frame.BoneIndex] = frame;
            }
            if (timeElpased != Animation.Keyframes[start].Time)
            {
                start = end;
                end = timeRange[Math.Min(CurrentRangeIndex + 2, timeRange.Count - 1)];
                for (int i = start; i < end; ++i)
                {
                    var nextFrame = Animation.Keyframes[i];
                    var currFrame = tempKeyframes[nextFrame.BoneIndex];
                    if (currFrame.HasValue)
                    {
                        // Calculate time difference between frames
                        var frameLength = nextFrame.Time - currFrame.Value.Time;
                        var timeDiff = timeElpased - currFrame.Value.Time;
                        var amount = timeDiff / frameLength;
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
                    tempBones[i] = (tempBones[i] * parentTransform);
                }
            }

            if (bones == null || bones.Length != BoneCount)
            {
                bones = new Matrix[BoneCount];
            }
            // Change the bone transform from rest pose space into bone space (using the inverse of the bind/rest pose)
            for (var i = 0; i < BoneCount; i++)
            {
                bones[i] = Bones[i].InvBindPose * tempBones[i];
            }

            if(RepeatMode == AnimationRepeatMode.Loop)
            {
                if(timeElpased > EndTime)
                {
                    CurrentRangeIndex = 0;
                    currentTime = 0;
                }
            }
        }

        public void Reset()
        {
            CurrentRangeIndex = 0;
            currentTime = 0;
        }
    }
}
