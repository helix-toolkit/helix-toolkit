/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Animations
#else
namespace HelixToolkit.UWP.Animations
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public enum AnimationRepeatMode
    {
        PlayOnce,
        Loop,
        PlayOnceHold,
    }
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
                            Matrix.CreateScale(MathUtil.Lerp(s1, s2, amount)) *
                            Matrix.CreateFromQuaternion(Quaternion.Slerp(q1, q2, amount)) *
                            Matrix.CreateTranslation(Vector3.Lerp(t1, t2, amount));
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
}
