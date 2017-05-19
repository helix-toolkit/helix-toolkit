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
        /// Previous event happened.
        /// </summary>
        private long previous = 0;

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
                return false;
            }
        }
    }
}
