using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace HelixToolkit
{
    public static class NativeHelper
    {
        public static unsafe string ToString(char* str)
        {
            return str == null ? String.Empty : new string(str);
        }

        public static unsafe string ToString(byte* str)
        {
            return str == null ? String.Empty : new string((sbyte*)str);
        }

        /// <summary>
        /// SizeOf an unmanaged struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SizeOf<T>() where T : unmanaged
        {
            return sizeof(T);
        }
         
        /// <summary>
        /// SizeOf array of unmanaged struct by bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>(T[] array) where T : unmanaged
        {
            return SizeOf<T>() * array.Length;
        }
        /// <summary>
        /// SizeOf an unmanaged struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <returns></returns>
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
        public static void MemoryCopy(IntPtr dst, IntPtr src, uint sizeInBytes)
        {
            if (dst == IntPtr.Zero || src == IntPtr.Zero)
            {
                return;
            }
            unsafe
            {
                Buffer.MemoryCopy((void*)src, (void*)dst, sizeInBytes, sizeInBytes);
            }
        }
        /// <summary>
        /// Clears the memory.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="value">The value.</param>
        /// <param name="sizeInBytesToClear">The size in bytes to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearMemory(IntPtr dest, byte value, uint sizeInBytesToClear)
        {
            if (dest == IntPtr.Zero)
            {
                return;
            }
            unsafe
            {
                var span = new Span<byte>((void*)dest, (int)sizeInBytesToClear);
                span.Fill(value);
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
        /// Swaps the value between two references.
        /// </summary>
        /// <typeparam name="T">Type of a data to swap.</typeparam>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        public static void Swap<T>(ref T left, ref T right)
        {
            var temp = left;
            left = right;
            right = temp;
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
        public static IntPtr Read<T>(IntPtr source, T[] data, uint offset, uint count) where T : unmanaged
        {
            if (source == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            unsafe
            {
                var bytesToCopy = SizeOf<T>() * count;
                var offsetBytes = offset * SizeOf<T>();
                fixed (T* pData = &data[0])
                {
                    Buffer.MemoryCopy((byte*)source, ((byte*)pData + offsetBytes), 
                        (data.Length - offset) * SizeOf<T>(), bytesToCopy);                                   
                }
                return source + (int)bytesToCopy;
            }
        }

        /// <summary>
        /// Read stream to a byte[] buffer.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>A byte[] buffer.</returns>
        public static byte[] ReadStream(Stream stream)
        {
            var readLength = 0;
            return ReadStream(stream, ref readLength);
        }

        /// <summary>
        /// Read stream to a byte[] buffer.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="readLength">Length to read.</param>
        /// <returns>A byte[] buffer.</returns>
        public static byte[] ReadStream(Stream stream, ref int readLength)
        {
            var num = readLength;
            Debug.Assert(num <= (stream.Length - stream.Position));
            if (num == 0)
            {
                readLength = (int)(stream.Length - stream.Position);
            }

            num = readLength;

            Debug.Assert(num >= 0);
            if (num == 0)
            {
                return Array.Empty<byte>();
            }

            var buffer = new byte[num];
            var bytesRead = 0;
            if (num > 0)
            {
                do
                {
                    bytesRead += stream.Read(buffer, bytesRead, readLength - bytesRead);
                } while (bytesRead < readLength);
            }
            return buffer;
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
            return Write(destination, 0, ref data);
        }
        /// <summary>
        /// Writes the specified T data to a memory location with offset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IntPtr Write<T>(IntPtr destination, int offset, ref T data) where T : unmanaged
        {
            if (destination == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            unsafe
            {
                fixed (T* ptr = &data)
                {
                    MemoryCopy(destination + offset, (IntPtr)ptr, (uint)SizeOf<T>());
                }
                return destination + offset + (int)SizeOf<T>();
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
        public static IntPtr Write<T>(IntPtr destination, T[] data, uint offset, uint count) where T : unmanaged
        {
            if (destination == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            unsafe
            {
                var bytesToWrite = count * SizeOf<T>();
                var offSetBytes = offset * SizeOf<T>();
                fixed (T* pData = &data[0])
                {
                    MemoryCopy(destination, (IntPtr)((byte*)pData + offSetBytes), (uint)bytesToWrite);
                }               
                return destination + (int)bytesToWrite;
            }
        }
        /// <summary>
        /// Compare two structs are the same.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool StructIsSame<T>(ref T s1, ref T s2) where T : unmanaged
        {
            unsafe
            {
                fixed (T* p1 = &s1)
                {
                    fixed (T* p2 = &s2)
                    {
                        var t1 = (byte*)p1;
                        var t2 = (byte*)p2;
                        var count = SizeOf<T>();
                        for (var i = 0; i < count; ++i)
                        {
                            if (t1[i] != t2[i])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Compare two structs are the same. To use this function, struct must be 4 bytes aligned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool StructIsSame4BytesAligned<T>(ref T s1, ref T s2) where T : unmanaged
        {
            unsafe
            {
                fixed (T* p1 = &s1)
                {
                    fixed (T* p2 = &s2)
                    {
                        var t1 = (uint*)p1;
                        var t2 = (uint*)p2;
                        var count = SizeOf<T>() / sizeof(uint);
                        for (var i = 0; i < count; ++i)
                        {
                            if (t1[i] != t2[i])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        public static unsafe bool CopyStruct<T1, T2>(ref T1 t1, ref T2 t2) where T1 : unmanaged where T2 : unmanaged
        {
            if (SizeOf<T1>() != SizeOf<T2>())
            {
                return false;
            }
            unsafe
            {
                fixed (T1* p1 = &t1)
                    fixed (T2* p2 = &t2)
                        Buffer.MemoryCopy((void*)p1, (void*)p2, SizeOf<T1>(), SizeOf<T2>());
            }
            return true;
        }

        public static unsafe bool CopyStruct<T1, T2>(T1 t1, ref T2 t2) where T1 : unmanaged where T2 : unmanaged
        {
            return CopyStruct(t1, ref t2);
        }

        public static unsafe T2 ToStruct<T1, T2>(ref T1 t1) where T1 : unmanaged where T2 : unmanaged
        {
            Debug.Assert(SizeOf<T1>() == SizeOf<T2>());
            T2 t2;
            unsafe
            {
                fixed (T1* p1 = &t1)
                {
                    T2* p2 = &t2;
                    Buffer.MemoryCopy((void*)p1, (void*)p2, SizeOf<T1>(), SizeOf<T2>());
                }
            }
            return t2;
        }

        public static unsafe T2 ToStruct<T1, T2>(this T1 t1) where T1 : unmanaged where T2 : unmanaged
        {
            return ToStruct<T1, T2>(ref t1);
        }
    }
}
