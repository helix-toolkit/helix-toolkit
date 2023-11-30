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
        public class MorphTargetKeyFrameUpdater : IAnimationUpdater
        {
            public string Name
            {
                set; get;
            } = string.Empty;
            public Animation Animation
            {
                get;
            }
            public IList<float> Weights
            {
                get;
            }
            public float StartTime
            {
                get;
            }
            public float EndTime
            {
                get;
            }
            public AnimationRepeatMode RepeatMode { get; set; } = AnimationRepeatMode.PlayOnce;

            private readonly FastList<MorphTargetKeyframe>[] kfs;

            public MorphTargetKeyFrameUpdater(Animation animation, IList<float> weights)
            {
                this.Animation = animation;
                Name = animation.Name;
                this.Weights = weights;
                StartTime = animation.StartTime;
                EndTime = animation.EndTime;
                kfs = new FastList<MorphTargetKeyframe>[weights.Count];
                for (var i = 0; i < kfs.Length; ++i)
                {
                    kfs[i] = new FastList<MorphTargetKeyframe>(animation.MorphTargetKeyframes.Count / weights.Count);
                }
                foreach (var ani in animation.MorphTargetKeyframes.OrderBy(x => x.Time))
                {
                    kfs[ani.Index].Add(ani);
                }
                foreach (var ani in kfs)
                {
                    Debug.Assert(ani.First().Time < ani.Last().Time);
                }
            }

            public void Update(float timeStamp, long frequency)
            {
                if (StartTime == EndTime || kfs.Length == 0)
                {
                    return;
                }
                //Find time(t)
                var timeSec = timeStamp / frequency;
                if (timeSec < StartTime)
                {
                    return;
                }
                var elapsed = (float)(timeSec - StartTime);
                if (elapsed > EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.Loop:
                            {
                                elapsed = elapsed % (EndTime - StartTime) + StartTime;
                                break;
                            }
                        case AnimationRepeatMode.PlayOnce:
                            {
                                SetWeights(StartTime);
                                return;
                            }
                        case AnimationRepeatMode.PlayOnceHold:
                            {
                                elapsed = EndTime;
                                break;
                            }
                    }
                }

                SetWeights(elapsed);
            }

            private void SetWeights(float timeElapsed)
            {
                //Interpolate between each individual weight's keyframe pairs at current time
                for (var i = 0; i < kfs.Length; ++i)
                {
                    var frames = kfs[i];
                    var idx = AnimationUtils.FindKeyFrame(timeElapsed, frames);
                    if (idx < 0)
                    {
                        Weights[i] = 0;
                        continue;
                    }
                    ref var currFrame = ref frames.GetInternalArray()[idx];
                    if (currFrame.Time > timeElapsed && idx == 0)
                    {
                        continue;
                    }
                    Debug.Assert(currFrame.Time <= timeElapsed);
                    if (frames.Count == 1 || idx == frames.Count - 1)
                    {
                        Weights[i] = currFrame.Weight;
                        continue;
                    }
                    ref var nextFrame = ref frames.GetInternalArray()[idx + 1];
                    Debug.Assert(nextFrame.Time >= timeElapsed);
                    var diff = timeElapsed - currFrame.Time;
                    var length = nextFrame.Time - currFrame.Time;
                    var ratio = diff / length;
                    Weights[i] = currFrame.Weight + (nextFrame.Weight - currFrame.Weight) * ratio;
                }

                //Mark weights updated
                (Animation.RootNode as Model.Scene.BoneSkinMeshNode)?.WeightUpdated();
            }

            public void Reset()
            {
                Update(0, 1);
            }
        }
    }
}
