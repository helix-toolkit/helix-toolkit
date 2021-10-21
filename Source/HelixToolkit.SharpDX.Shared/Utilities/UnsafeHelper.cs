using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
        public static class UnsafeHelper
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int SizeOf<T>() where T : unmanaged
            {
                unsafe
                {
                    return sizeof(T);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int SizeOf<T>(T[] array) where T : unmanaged
            {
                return SizeOf<T>() * array.Length;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int SizeOf<T>(ref T _) where T : unmanaged
            {
                return SizeOf<T>();
            }
            /// <summary>
            /// Unsafe memory copy
            /// </summary>
            /// <param name="dst"></param>
            /// <param name="src"></param>
            /// <param name="sizeInBytes"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MemoryCopy(IntPtr dst, IntPtr src, int sizeInBytes)
            {
                if (dst == IntPtr.Zero || src == IntPtr.Zero)
                {
                    return;
                }
                unsafe
                {
#if NETSTANDARD2_1
                    Buffer.MemoryCopy((void*)src, (void*)dst, sizeInBytes, sizeInBytes);
#else
                    var pDest = (byte*)dst.ToPointer();
                    var pSrc = (byte*)src.ToPointer();
                    for (var i = 0; i < sizeInBytes; ++i)
                    {
                        *(pDest++) = *(pSrc++);
                    }
#endif
                }
            }
            /// <summary>
            /// Clears the memory.
            /// </summary>
            /// <param name="dest">The dest.</param>
            /// <param name="value">The value.</param>
            /// <param name="sizeInBytesToClear">The size in bytes to clear.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ClearMemory(IntPtr dest, byte value, int sizeInBytesToClear)
            {
                if (dest == IntPtr.Zero)
                {
                    return;
                }
                unsafe
                {
#if NETSTANDARD2_1
                    var span = new Span<byte>((void*)dest, (int)sizeInBytesToClear);
                    span.Fill(value);
#else
                    var pDest = (byte*)dest.ToPointer();
                    for (var i = 0; i < sizeInBytesToClear; ++i)
                    {
                        *(pDest++) = value;
                    }
#endif
                }
            }

            /// <summary>
            /// Reads the specified T data from a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to read.</typeparam>
            /// <param name="source">Memory location to read from.</param>
            /// <param name="data">The data write to.</param>
            /// <returns>source pointer + sizeof(T).</returns>
            public static IntPtr ReadAndPosition<T>(IntPtr source, ref T data) where T : unmanaged
            {
                if (source == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
                unsafe
                {
                    data = *(T*)source;
                    return source + (int)SizeOf<T>();
                }
            }
            /// <summary>
            /// Reads the specified T data from a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to read.</typeparam>
            /// <param name="source">Memory location to read from.</param>
            /// <returns>The data read from the memory location.</returns>
            public static T Read<T>(IntPtr source) where T : unmanaged
            {
                if (source == IntPtr.Zero)
                {
                    return default;
                }
                unsafe
                {
                    return *(T*)source;
                }
            }
            /// <summary>
            /// Reads the specified T data from a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to read.</typeparam>
            /// <param name="source">Memory location to read from.</param>
            /// <param name="data">The data write to.</param>
            /// <returns>source pointer + sizeof(T).</returns>
            public static void Read<T>(IntPtr source, ref T data) where T : unmanaged
            {
                if (source == IntPtr.Zero)
                {
                    return;
                }
                unsafe
                {
                    data = *(T*)source;
                }
            }

            /// <summary>
            /// Reads the specified T data from a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to read.</typeparam>
            /// <param name="source">Memory location to read from.</param>
            /// <param name="data">The data write to.</param>
            /// <returns>source pointer + sizeof(T).</returns>
            public static void ReadOut<T>(IntPtr source, out T data) where T : unmanaged
            {
                data = default;
                if (source == IntPtr.Zero)
                {
                    return;
                }
                unsafe
                {
                    data = *(T*)source;
                }
            }
            /// <summary>
            /// Reads the specified array T[] data from a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to read.</typeparam>
            /// <param name="source">Memory location to read from.</param>
            /// <param name="data">The data write to.</param>
            /// <param name="offset">The offset in the array to write to.</param>
            /// <param name="count">The number of T element to read from the memory location.</param>
            /// <returns>source pointer + sizeof(T) * count.</returns>
            public static IntPtr Read<T>(IntPtr source, T[] data, int offset, int count) where T : unmanaged
            {
                if (source == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
                unsafe
                {
                    var bytesToCopy = SizeOf<T>() * count;
                    fixed (T* pData = &data[offset])
                    {
                        MemoryCopy(new IntPtr((void*)pData), source, bytesToCopy);
                    }
                    return source + bytesToCopy;
                }
            }

            /// <summary>
            /// Writes the specified T data to a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to write.</typeparam>
            /// <param name="destination">Memory location to write to.</param>
            /// <param name="data">The data to write.</param>
            /// <returns>destination pointer + sizeof(T).</returns>
            public static IntPtr Write<T>(IntPtr destination, T data) where T : unmanaged
            {
                return Write(destination, ref data);
            }
            /// <summary>
            /// Writes the specified T data to a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to write.</typeparam>
            /// <param name="destination">Memory location to write to.</param>
            /// <param name="data">The data to write.</param>
            /// <returns>destination pointer + sizeof(T).</returns>
            public static IntPtr Write<T>(IntPtr destination, ref T data) where T : unmanaged
            {
                if (destination == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
                unsafe
                {
                    *(T*)destination = data;
                    return destination + SizeOf<T>();
                }
            }

            /// <summary>
            /// Writes the specified array T[] data to a memory location.
            /// </summary>
            /// <typeparam name="T">Type of a data to write.</typeparam>
            /// <param name="destination">Memory location to write to.</param>
            /// <param name="data">The array of T data to write.</param>
            /// <param name="offset">The offset in the array to read from.</param>
            /// <param name="count">The number of T element to write to the memory location.</param>
            /// <returns>destination pointer + sizeof(T) * count.</returns>
            public static IntPtr Write<T>(IntPtr destination, T[] data, int offset, int count) where T : unmanaged
            {
                if (destination == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                } 
                unsafe
                {
                    var bytesToWrite = count * SizeOf<T>();
                    fixed (T* pData = &data[offset])
                    {
                        MemoryCopy(destination, new IntPtr((byte*)pData), bytesToWrite);
                    }
                    return destination + bytesToWrite;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="destination"></param>
            /// <param name="data"></param>
            /// <param name="offset">By bytes</param>
            /// <param name="count">By bytes</param>
            /// <returns></returns>
            public static IntPtr Write(IntPtr destination, IntPtr data, int offset, int count)
            {
                if (destination == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
                unsafe
                {
                    MemoryCopy(destination, data + offset, count);
                    return destination + count;
                }
            }
        }
    }
}
