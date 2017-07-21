using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX.Helpers
{
    /// <summary>
    /// Use to skip event if event frequency is too high.
    /// </summary>
    public sealed class EventSkipper
    {
        /// <summary>
        /// Stopwatch
        /// </summary>
        private static readonly Stopwatch watch = new Stopwatch();
        public long lag { private set; get; } = 0;
        /// <summary>
        /// 
        /// </summary>
        static EventSkipper()
        {
            watch.Start();
        }
        /// <summary>
        /// The threshold used to skip if previous event happened less than the threshold.
        /// </summary>
        public long Threshold = 15;

        /// <summary>
        /// Sometimes invalidate renderer ran too fast, causes sence not reflect the latest update. 
        /// Set this to delay render one more time after last rendering. Default is 1 second.
        /// </summary>
        public long ForceRefreshInterval = 1000;

        /// <summary>
        /// Previous event happened.
        /// </summary>
        private long previous = 0;

        private bool forceRender = false;
        /// <summary>
        /// Determine if this event should be skipped.
        /// </summary>
        /// <returns>If skip, return true. Otherwise, return false.</returns>
        public bool IsSkip()
        {
            var curr = watch.ElapsedMilliseconds;
            var elpased = curr - previous;
            previous = curr;
            lag += elpased;

            if (lag < Threshold)
            {                
                return true;
            }
            else
            {
                lag = Math.Min(lag - Threshold, Threshold - 2);
                forceRender = true;
                return false;
            }
        }
        /// <summary>
        /// Delay certain time to render one more time after previous rendering, ensure scene is update to latest.
        /// (Used to solve latest scene is not reflected glitch)
        /// </summary>
        /// <returns></returns>
        public bool ForceRender()
        {
            if(forceRender && watch.ElapsedMilliseconds - previous > ForceRefreshInterval)
            {
                forceRender = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
