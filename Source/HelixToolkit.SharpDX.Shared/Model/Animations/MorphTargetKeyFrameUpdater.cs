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
        public class MorphTargetKeyFrameUpdater : IAnimationUpdater
        {
            //TODO: setup a more useful organization of weight keyframe data (List per id)
            public Animation animation { get; }
            public IList<float> weights { get; }
            public float startTime { get; }
            public float endTime { get; }
            public AnimationRepeatMode RepeatMode { get; set; } = AnimationRepeatMode.PlayOnce;

            private List<MorphTargetKeyframe> kfs;
            private int[][] targetKeyframeIds;

            public MorphTargetKeyFrameUpdater(Animation animation, IList<float> weights)
            {
                this.animation = animation;
                this.weights = weights;
                startTime = animation.StartTime;
                endTime = animation.EndTime;
                kfs = animation.morphTargetKeyframes;

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
            }

            public void Update(long timeStamp, long frequency)
            {
                //NOTE
                //This does not take advantage of caching previous id's and time, do that later...
                //This does not handle repeat modes

                //Interpolate between each individual weight's keyframe pairs at current time(t)
                float t = (float)timeStamp / frequency;

                //ONLY FOR TESTING, make animation loop, assumes start time 0
                t %= endTime;

                for (int i = 0; i < targetKeyframeIds.Length; i++)
                {
                    //Skip if no keyframes for weight id
                    if (targetKeyframeIds[i].Length == 0)
                        continue;

                    //Locate keyframe where t is below it's set time
                    int id = -1;
                    for (int j = 0; j < targetKeyframeIds[i].Length; j++)
                    {
                        if (kfs[targetKeyframeIds[i][j]].Time > t)
                        {
                            id = j;
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
                        weights[i] = a.Weight + (b.Weight - a.Weight) * k;
                    }
                }
            }

            //TODO
            public void Reset()
            {

            }
        }
    }
}
