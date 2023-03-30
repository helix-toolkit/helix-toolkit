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
        public static class AnimationUtils
        {
            public static int FindKeyFrame<T>(float timeElapsed, IList<T> frames) where T : struct, IKeyFrame
            {
                if (frames.Count == 0)
                {
                    return -1;
                }
                if (frames.Count <= 2)
                {
                    return 0;
                }
                timeElapsed = Math.Min(Math.Max(timeElapsed, frames.First().Time), frames.Last().Time);
                var diff = frames.Last().Time - frames.First().Time;
                var inc = diff / (frames.Count - 1);
                var est = (int)Math.Floor((timeElapsed - frames.First().Time) / inc);
                int start, end;
                if (frames[est].Time >= timeElapsed)
                {
                    start = 0;
                    end = est;
                }
                else
                {
                    start = est;
                    end = frames.Count - 1;
                }
                while (start < end)
                {
                    if (frames[start].Time >= timeElapsed)
                    {
                        return Math.Max(0, start - 1);
                    }
                    if (frames[end].Time <= timeElapsed)
                    {
                        return end;
                    }
                    ++start;
                    --end;
                }
                return frames[start].Time >= timeElapsed ? Math.Max(0, start - 1) : start;
            }
        }
    }
}