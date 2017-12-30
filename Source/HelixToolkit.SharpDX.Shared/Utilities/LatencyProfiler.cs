using System;
using System.Collections.Generic;
using System.Text;

#if NETFX_CORE
namespace HelixToolkit.UWP.Utility
#else
namespace HelixToolkit.Wpf.SharpDX.Utility
#endif
{
    using Model;

    public sealed class LatencyProfiler : ObservableObject
    {
        private double latency = 0;
        /// <summary>
        /// Average latency
        /// </summary>
        public double Latency
        {
            private set
            {
                if(Set(ref latency, value))
                {
                    Console.WriteLine($"Latency: {value}");
                }
            }
            get
            {
                return latency;
            }
        }

        public int UpdateFrequency
        {
            set; get;
        } =20;

        private uint counter = 0;
        private double average = 0;

        public void Push(double latency)
        {
            average = average + (latency - average) / (++counter); // moving average
            if (counter > UpdateFrequency)
            {
                counter = 0;
                Latency = average;
                average = 0;
            }
        }

        public void Reset()
        { 
            counter = 0;
            Latency = 0;
            average = 0;
        }
    }
}
