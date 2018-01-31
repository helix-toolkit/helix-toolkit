using System;
using System.ComponentModel;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    public struct FrameStatisticsArg
    {
        public double AverageValue;
        public double AverageFrequency;
    }
    public interface IFrameStatistics : INotifyPropertyChanged
    {
        event EventHandler<FrameStatisticsArg> OnValueChanged;
        /// <summary>
        /// Gets the average value.
        /// </summary>
        /// <value>
        /// The average value.
        /// </value>
        double AverageValue { get; }
        /// <summary>
        /// Gets the average frequency.
        /// </summary>
        /// <value>
        /// The average frequency.
        /// </value>
        double AverageFrequency { get; }
        /// <summary>
        /// Gets or sets the update frequency by number of samples
        /// </summary>
        /// <value>
        /// The update frequency.
        /// </value>
        uint UpdateFrequency { set; get; }
        /// <summary>
        /// Pushes the specified latency by milliseconds
        /// </summary>
        /// <param name="latency">The latency.</param>
        void Push(double latency);

        void Reset();
    }

    public sealed class FrameStatistics : Model.ObservableObject, IFrameStatistics
    {
        public event EventHandler<FrameStatisticsArg> OnValueChanged;

        private double averageValue = 0;
        /// <summary>
        /// Average latency
        /// </summary>
        public double AverageValue
        {
            private set
            {
                if (Set(ref averageValue, value))
                {
                    AverageFrequency = value < 1 ? 1000 : 1000 / value;
                    OnValueChanged?.Invoke(this, new FrameStatisticsArg() { AverageValue = value, AverageFrequency = AverageFrequency });
                }
            }
            get
            {
                return averageValue;
            }
        }

        private double averageFrequency = 0;
        /// <summary>
        /// Gets or sets the average frequency.
        /// </summary>
        /// <value>
        /// The average frequency.
        /// </value>
        public double AverageFrequency
        {
            private set
            {
                Set(ref averageFrequency, value);
            }
            get { return averageFrequency; }
        }
        /// <summary>
        /// Gets or sets the update frequency by number of samples, Default is 60
        /// </summary>
        /// <value>
        /// The update frequency.
        /// </value>
        public uint UpdateFrequency { set; get; } = 60;

        private double movingAverage = 0;
        private uint counter = 0;
        private const int RingBufferSize = 20;
        private readonly SimpleRingBuffer<double> ringBuffer = new SimpleRingBuffer<double>(RingBufferSize);
        /// <summary>
        /// Pushes the specified latency by milliseconds.
        /// </summary>
        /// <param name="latency">The latency.</param>
        public void Push(double latency)
        {
            if (ringBuffer.IsFull())
            {
                movingAverage -= (ringBuffer.First - movingAverage) / ringBuffer.Count;
                ringBuffer.RemoveFirst();
            }
            ringBuffer.Add(latency);
            movingAverage += (latency - movingAverage) / ringBuffer.Count; // moving average        
            movingAverage = Math.Min(1000, Math.Max(0, movingAverage));
            counter = (++counter) % UpdateFrequency;
            if (counter == 0)
            {
                AverageValue = movingAverage;
            }
        }

        public void Reset()
        {
            AverageValue = 0;
            movingAverage = 0;
            counter = 0;
        }
    }

    public interface IRenderStatistics
    {
        /// <summary>
        /// Gets the FPS statistics.
        /// </summary>
        /// <value>
        /// The FPS statistics.
        /// </value>
        IFrameStatistics FPSStatistics { get; }
        /// <summary>
        /// Gets the render latency statistics.
        /// </summary>
        /// <value>
        /// The latency statistics.
        /// </value>
        IFrameStatistics LatencyStatistics { get; }
        /// <summary>
        /// Resets all statistics.
        /// </summary>
        void Reset();
    }

    public class RenderStatistics : IRenderStatistics
    {
        /// <summary>
        /// Gets the FPS statistics.
        /// </summary>
        /// <value>
        /// The FPS statistics.
        /// </value>
        public IFrameStatistics FPSStatistics { get; } = new FrameStatistics();
        /// <summary>
        /// Gets the render latency statistics.
        /// </summary>
        /// <value>
        /// The latency statistics.
        /// </value>
        public IFrameStatistics LatencyStatistics { get; } = new FrameStatistics();
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            FPSStatistics.Reset();
            LatencyStatistics.Reset();
        }
    }
}
