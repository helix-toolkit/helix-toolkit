using System.ComponentModel;

namespace HelixToolkit.SharpDX.Utilities;

public interface IFrameStatistics : INotifyPropertyChanged
{
    event EventHandler<FrameStatisticsArg> OnValueChanged;
    /// <summary>
    /// Gets the average value.
    /// </summary>
    /// <value>
    /// The average value.
    /// </value>
    double AverageValue
    {
        get;
    }
    /// <summary>
    /// Gets the average frequency.
    /// </summary>
    /// <value>
    /// The average frequency.
    /// </value>
    double AverageFrequency
    {
        get;
    }
    /// <summary>
    /// Gets or sets the update frequency by number of samples
    /// </summary>
    /// <value>
    /// The update frequency.
    /// </value>
    uint UpdateFrequency
    {
        set; get;
    }
    /// <summary>
    /// Pushes the specified latency by milliseconds
    /// </summary>
    /// <param name="latency">The latency.</param>
    void Push(double latency);

    void Reset();
}
