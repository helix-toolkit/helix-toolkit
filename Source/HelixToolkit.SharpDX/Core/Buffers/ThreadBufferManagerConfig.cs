namespace HelixToolkit.SharpDX.Core;

public static class ThreadBufferManagerConfig
{
    /// <summary>
    /// Gets or sets the maximum size to retain the buffer in memory.
    /// If requested size is larger than this value, buffer will be temporary instead of being retained for reuse.
    /// 
    /// </summary>
    /// <value>
    /// The maximum size to retain mb.
    /// </value>
    public static int MaximumSizeToRetainMb
    {
        set;
        get;
    } = 64;
    /// <summary>
    /// Gets or sets the minimum size to retain the buffer in memory.
    /// If requested size is smaller than this value, buffer will always be retained for reuse.
    /// </summary>
    /// <value>
    /// The minimum size to retain mb.
    /// </value>
    public static int MinimumSizeToRetainMb
    {
        set;
        get;
    } = 4;
    /// <summary>
    /// Gets or sets the minimum buffer release threshold by seconds.
    /// Buffer will not be released automatically if it has been used within last N seconds.
    /// </summary>
    /// <value>
    /// The minimum buffer release threshold by seconds.
    /// </value>
    public static int MinimumAutoReleaseThresholdSeconds
    {
        set; get;
    } = 60;

    /// <summary>
    /// Gets or sets the size reduction multiplier.
    /// If buffer is not being used more than <see cref="MinimumAutoReleaseThresholdSeconds"/> seconds,
    /// and new request size is smaller than buffer size / <see cref="SizeReductionDividend"/> but larger than <see cref="MinimumSizeToRetainMb"/>,
    /// buffer will be released after usage.
    /// </summary>
    /// <value>
    /// The size reduction multiplier.
    /// </value>
    public static float SizeReductionDividend
    {
        set; get;
    } = 2;
}
