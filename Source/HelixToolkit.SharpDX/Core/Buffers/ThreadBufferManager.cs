using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX.Core;

public static class ThreadBufferManager<T> where T : unmanaged
{
    static readonly ILogger logger = Logger.LogManager.Create(nameof(ThreadBufferManager<T>));
    public static readonly int StructSize = Marshal.SizeOf<T>();

    private const int MByteToByte = 1024 * 1024;
    public static int MaximumElementCount
    {
        get => ThreadBufferManagerConfig.MaximumSizeToRetainMb * MByteToByte / StructSize;
    }

    public static int MinimumElementCount
    {
        get => ThreadBufferManagerConfig.MinimumSizeToRetainMb * MByteToByte / StructSize;
    }

    [ThreadStatic]
    private static T[]? buffer = null;

    private static long lastUsed = 0;

    public static T[] GetBuffer(int requestCount)
    {
        var array = buffer;
        if (array == null || array.Length < requestCount)
        {
            float scale = 1;
            if (requestCount < MinimumElementCount)
            {
                scale = 2;
            }
            else if (requestCount < MaximumElementCount)
            {
                scale = 1.5f;
            }
            array = new T[(int)(requestCount * scale)];
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Created new thread buffer. Type: {0}; Size: {1} kB.", typeof(T), array.Length * StructSize / 1024);
            }
        }

        if (requestCount > MaximumElementCount)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Requested buffer size is larger than max retain size. Type: {0}.", typeof(T));
            }
            return array;
        }
        if (lastUsed == 0)
        {
            lastUsed = Stopwatch.GetTimestamp();
            buffer = array;
            return array;
        }

        if (array.Length > MinimumElementCount
            && array.Length > ThreadBufferManagerConfig.SizeReductionDividend * requestCount)
        {
            var diff = Stopwatch.GetTimestamp() - lastUsed;
            if (diff / Stopwatch.Frequency > ThreadBufferManagerConfig.MinimumAutoReleaseThresholdSeconds)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Disposing thread buffer. Type: {0}.", typeof(T));
                }
                buffer = null;
                lastUsed = 0;
                return array;
            }
        }

        buffer = array;
        lastUsed = Stopwatch.GetTimestamp();
        return array;
    }
}
