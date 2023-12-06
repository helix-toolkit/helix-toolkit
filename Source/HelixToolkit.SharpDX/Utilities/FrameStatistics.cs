namespace HelixToolkit.SharpDX.Utilities;

public sealed class FrameStatistics : Model.ObservableObject, IFrameStatistics
{
    public event EventHandler<FrameStatisticsArg>? OnValueChanged;

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
        get
        {
            return averageFrequency;
        }
    }
    /// <summary>
    /// Gets or sets the update frequency by number of samples, Default is 60
    /// </summary>
    /// <value>
    /// The update frequency.
    /// </value>
    public uint UpdateFrequency { set; get; } = 30;
    private double total = 0;
    private double movingAverage = 0;
    private uint counter = 0;
    private const int RingBufferSize = 120;
    private readonly SimpleRingBuffer<double> ringBuffer = new(RingBufferSize);
    /// <summary>
    /// Pushes the specified latency by milliseconds.
    /// </summary>
    /// <param name="latency">The latency.</param>
    public void Push(double latency)
    {
        if (latency > 1000 || latency < 0)
        {
            Reset();
            return;
        }
        if (ringBuffer.IsFull())
        {
            total -= ringBuffer.First;
            ringBuffer.RemoveFirst();
        }
        ringBuffer.Add(latency);
        total += latency;
        movingAverage = total / ringBuffer.Count; // moving average        
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
        total = 0;
        ringBuffer.Clear();
    }
}
