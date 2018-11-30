using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    namespace Utilities
    {
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
                        AverageFrequency = 1000 / value;
                        OnValueChanged?.Invoke(this, new FrameStatisticsArg(value, AverageFrequency));
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
            public uint UpdateFrequency { set; get; } = 30;

            private double movingAverage = 0;
            private uint counter = 0;
            private const int RingBufferSize = 60;
            private readonly SimpleRingBuffer<double> ringBuffer = new SimpleRingBuffer<double>(RingBufferSize);
            /// <summary>
            /// Pushes the specified latency by milliseconds.
            /// </summary>
            /// <param name="latency">The latency.</param>
            public void Push(double latency)
            {
                if(latency > 1000 || latency < 0)
                {
                    Reset();
                    return;
                }
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
                ringBuffer.Clear();
            }
        }

        public interface IRenderStatistics
        {
            IFrameStatistics FPSStatistics { get; }
            IFrameStatistics LatencyStatistics { get; }
            int NumModel3D { get; }
            int NumCore3D { get; }
            int NumTriangles { get; }
            int NumDrawCalls { get; }
            RenderDetail FrameDetail { set; get; }
            ICamera Camera { set; get; }
            string GetDetailString();
            void Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class RenderStatistics : IRenderStatistics
        {
            const string LineBreak = "\n---------\n";
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
            /// Gets or sets the number of model3d per frame.
            /// </summary>
            /// <value>
            /// The number model3d.
            /// </value>
            public int NumModel3D { internal set; get; } = 0;
            /// <summary>
            /// Gets or sets the number of render core3d per frame.
            /// </summary>
            /// <value>
            /// The number core3 d.
            /// </value>
            public int NumCore3D { internal set; get; } = 0;
            /// <summary>
            /// Gets or sets the number triangles rendered in geometry model
            /// </summary>
            /// <value>
            /// The number triangles.
            /// </value>
            public int NumTriangles { internal set; get; } = 0;
            /// <summary>
            /// Gets or sets the number draw calls per frame
            /// </summary>
            /// <value>
            /// The number draw calls.
            /// </value>
            public int NumDrawCalls { internal set; get; } = 0;
            /// <summary>
            /// Gets or sets the camera.
            /// </summary>
            /// <value>
            /// The camera.
            /// </value>
            public ICamera Camera { set; get; }
            /// <summary>
            /// Gets or sets the frame detail.
            /// </summary>
            /// <value>
            /// The frame detail.
            /// </value>
            public RenderDetail FrameDetail { set; get; } = RenderDetail.None;

            public string GetDetailString()
            {
                return GetDetailString(FrameDetail);
            }

            public string GetDetailString(RenderDetail detail)
            {
                if(detail == RenderDetail.None)
                {
                    return "";
                }
                string s = "";
                if((detail & RenderDetail.FPS) == RenderDetail.FPS)
                {
                    s += GetFPS();
                }
                if((detail & RenderDetail.Statistics) == RenderDetail.Statistics)
                {
                    s += GetStatistics();
                }
                if((detail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
                {
                    s += GetTriangleCount();
                }
                if((detail & RenderDetail.Camera) == RenderDetail.Camera)
                {
                    s += GetCamera();
                }
                return s;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private string GetFPS()
            {
                return $"FPS:{Math.Round(FPSStatistics.AverageFrequency, 2)}" + LineBreak;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private string GetStatistics()
            {
                return $"Render(ms): {Math.Round(LatencyStatistics.AverageValue, 4)}\n" +
                    $"NumModel3D: {NumModel3D}\n" +
                    $"NumCore3D: {NumCore3D}\n" +
                    $"NumDrawCalls: {NumDrawCalls}"
                    + LineBreak;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private string GetTriangleCount()
            {
                return $"NumTriangle: {NumTriangles}" + LineBreak;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private string GetCamera()
            {
                return Camera == null ? "" : "Camera:\n" + Camera.ToString() + LineBreak;
            }
            /// <summary>
            /// Resets this instance.
            /// </summary>
            public void Reset()
            {
                FPSStatistics.Reset();
                LatencyStatistics.Reset();
                NumTriangles = NumCore3D = NumModel3D = NumDrawCalls = 0;
            }
            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return GetDetailString(RenderDetail.FPS | RenderDetail.Statistics);
            }
        }
    }

}
