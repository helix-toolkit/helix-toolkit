using System.Diagnostics;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    /// Use to skip event if event frequency is too high.
    /// </summary>
    public sealed class FrameRateRegulator : Model.ObservableObject
    {
        /// <summary>
        /// Stopwatch
        /// </summary>
        private static readonly Stopwatch watch = new Stopwatch();
        public double lag { private set; get; } = 0;

        private double latency = 0;
        /// <summary>
        /// Average latency
        /// </summary>
        public double Latency
        {
            private set
            {
                if (Set(ref latency, value))
                {
                    //Console.WriteLine($"Latency: {value}");
                }
            }
            get
            {
                return latency;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        static FrameRateRegulator()
        {
            watch.Start();
        }
        /// <summary>
        /// The threshold used to skip if previous event happened less than the threshold.
        /// </summary>
        public double Threshold = 15;

        /// <summary>
        /// Sometimes invalidate renderer ran too fast, causes sence not reflect the latest update. 
        /// Set this to delay render one more time after last rendering. Default is 1 second.
        /// </summary>
        public long ForceRefreshInterval = 1000;

        /// <summary>
        /// Previous event happened.
        /// </summary>
        private double previous = 0;

        private bool forceRender = false;
        private uint counter = 0;
        private double average = 0;
        private const int RingBufferSize = 20;
        private readonly SimpleRingBuffer<double> ringBuffer = new SimpleRingBuffer<double>(RingBufferSize);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="latency"></param>
        public void Push(double latency)
        {
            if (ringBuffer.IsFull())
            {              
                average -= (ringBuffer.First - average) / ringBuffer.Count;
                ringBuffer.RemoveFirst();
            }
            ringBuffer.Add(latency);
            average += (latency - average) / ringBuffer.Count; // moving average
            counter = (++counter) % 60;
            if(counter == 0)
            {
                Latency = average;
            }
        }
        /// <summary>
        /// Determine if this event should be skipped.
        /// </summary>
        /// <returns>If skip, return true. Otherwise, return false.</returns>
        public bool IsSkip()
        {
            var curr = watch.Elapsed.TotalMilliseconds;
            var elapsed = curr - previous;
            previous = curr;
            lag += elapsed;

            if (lag < Threshold + average - elapsed)
            {
                return true;
            }
            else
            {
                lag = 0;
                forceRender = true;
                return false;
            }
        }
        /// <summary>
        /// Delay certain time to render one more time after last rendering, ensure scene is update to latest.
        /// (Used to solve latest scene is not reflected glitch)
        /// </summary>
        /// <returns></returns>
        public bool DelayTrigger()
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
