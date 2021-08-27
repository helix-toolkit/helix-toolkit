/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
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
    namespace Animations
    {
        public class MorphTargetKeyFrameUpdater : IAnimationUpdater
        {
            public string Name
            {
                set; get;
            } = "";
            public Animation animation { get; }
            public IList<float> weights { get; }
            public float startTime { get; }
            public float endTime { get; }
            public AnimationRepeatMode RepeatMode { get; set; } = AnimationRepeatMode.PlayOnce;

            public float Speed { set; get; } = 1.0f;

            private List<MorphTargetKeyframe> kfs;
            private int[][] targetKeyframeIds;
            private int[] prevKeyframes;
            private double timeOffset = 0;

            public MorphTargetKeyFrameUpdater(Animation animation, IList<float> weights)
            {
                this.animation = animation;
                Name = animation.Name;
                this.weights = weights;
                startTime = animation.StartTime;
                endTime = animation.EndTime;
                kfs = animation.MorphTargetKeyframes;

                //Setup in groups of weight id and sort by time. This is somewhat slow, look into better solutions
                targetKeyframeIds = new int[weights.Count][];
                for (int i = 0; i < targetKeyframeIds.Length; i++)
                {
                    FastList<int> ids = new FastList<int>(kfs.Count); //Quick initial cap
                    for (int j = 0; j < kfs.Count; j++)
                    {
                        if (kfs[j].Index == i)
                            ids.Add(j);
                    }
                    targetKeyframeIds[i] = ids.OrderBy(n => kfs[n].Time).ToArray();
                }

                //Used to cache previous keyframe id's per morph target
                prevKeyframes = new int[weights.Count];
            }

            public void Update(long timeStamp, long frequency)
            {
                //Find time(t)
                double globalTime = (double)timeStamp / frequency;
                if (timeOffset == 0)
                    timeOffset = globalTime;
                float t = (float)(globalTime - timeOffset) * Speed;

                //Handle repeat mode
                if (RepeatMode == AnimationRepeatMode.Loop)
                    t = t % (startTime - endTime) + startTime;
                else if (RepeatMode == AnimationRepeatMode.PlayOnce && t > endTime)
                    return;
                else if (RepeatMode == AnimationRepeatMode.PlayOnceHold)
                    t = Clamp(t, startTime, endTime);

                //Interpolate between each individual weight's keyframe pairs at current time
                for (int i = 0; i < targetKeyframeIds.Length; i++)
                {
                    //Skip if no keyframes for weight id
                    if (targetKeyframeIds[i].Length == 0)
                        continue;

                    //Locate keyframe where t is below it's set time. starting from recent kf and wrapping to search all
                    int id = -1;
                    int len = targetKeyframeIds[i].Length;
                    for (int j = 0; j < len; j++)
                    {
                        int kfId = (j + prevKeyframes[i]) % len;
                        if (kfs[targetKeyframeIds[i][kfId]].Time > t)
                        {
                            //Ensure this is only the first kf larger than t
                            if (kfId != 0 && kfs[targetKeyframeIds[i][kfId - 1]].Time > t)
                                continue;

                            prevKeyframes[i] = kfId;
                            id = kfId;
                            break;
                        }
                    }

                    //Set mt weight to end keyframes if id is on end, otherwise interpolate
                    if (id == 0)
                        weights[i] = kfs[targetKeyframeIds[i][0]].Weight;
                    else if (id == -1)
                        weights[i] = kfs[targetKeyframeIds[i].Last()].Weight;
                    else
                    {
                        //Interpolate between id-1 and 1 based on time between
                        MorphTargetKeyframe a = kfs[targetKeyframeIds[i][id - 1]];
                        MorphTargetKeyframe b = kfs[targetKeyframeIds[i][id]];

                        float k = (t - a.Time) / (b.Time - a.Time);
                        weights[i] = a.Weight + ((b.Weight - a.Weight) * k);
                    }
                }

                //Mark weights updated
                (animation.RootNode as Model.Scene.BoneSkinMeshNode)?.WeightUpdated();
            }

            public void Reset()
            {
                timeOffset = 0;
            }

            private float Clamp(float x, float a, float b)
            {
                if (x > a) return a;
                if (x < b) return b;
                return x;
            }
        }
    }
}
