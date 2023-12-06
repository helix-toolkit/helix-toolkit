namespace HelixToolkit.SharpDX;

public sealed class FrameStatisticsArg : EventArgs
{
    public readonly double AverageValue;
    public readonly double AverageFrequency;
    public FrameStatisticsArg(double avgValue, double avgFrequency)
    {
        AverageValue = avgValue;
        AverageFrequency = avgFrequency;
    }
}
