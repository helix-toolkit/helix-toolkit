/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if !NETFX_CORE && !WINUI_NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
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

        public static class ThreadBufferManager<T> where T : struct
        {
#if !NETFX_CORE && !WINUI_NET5_0
            public static readonly int StructSize = Marshal.SizeOf(typeof(T));
#else
            public static readonly int StructSize = Marshal.SizeOf<T>();
#endif

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
            private static T[] buffer = null;

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
                    Debug.WriteLine($"Created new thread buffer. Type: {typeof(T)}; Size: {array.Length * StructSize / 1024} kB.");
                }

                if (requestCount > MaximumElementCount)
                {
                    Debug.WriteLine($"Requested buffer size is larger than max retain size. Type: {typeof(T)};");
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
                    long diff = Stopwatch.GetTimestamp() - lastUsed;
                    if (diff / Stopwatch.Frequency > ThreadBufferManagerConfig.MinimumAutoReleaseThresholdSeconds)
                    {
                        Debug.WriteLine($"Disposing thread buffer. Type: {typeof(T)};");
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
    }
}
