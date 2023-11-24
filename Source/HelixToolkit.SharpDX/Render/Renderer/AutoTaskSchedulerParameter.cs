namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public class AutoTaskSchedulerParameter
{
    /// <summary>
    /// Gets or sets the number processor.
    /// </summary>
    /// <value>
    /// The number processor.
    /// </value>
    public int NumProcessor
    {
        private set; get;
    }

    /// <summary>
    /// Gets or sets the minimum item to start multi-threading
    /// <para>https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/d3d11deferredcontextssample.htm</para>
    /// <para>Note: Only if draw calls > 3000 to be benefit according to the online performance test.</para>
    /// </summary>
    /// <value>
    /// The minimum item per task.
    /// </value>
    public int MinimumDrawCalls { set; get; } = 600;

    /// <summary>
    /// Gets or sets the maximum number of tasks.
    /// </summary>
    /// <value>
    /// The maximum number of tasks.
    /// </value>
    public int MaxNumberOfTasks { set; get; } = 4;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTaskSchedulerParameter"/> class.
    /// </summary>
    public AutoTaskSchedulerParameter()
    {
        NumProcessor = Environment.ProcessorCount;
    }
}
