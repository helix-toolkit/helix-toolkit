using System;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    /// A simple curcular ring buffer implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SimpleRingBuffer<T>
    {
        private readonly T[] buffer;
        private int next = 0;
        private int last = -1;
        private int first = 0;
        private readonly int bufferSize;
        private int count;
        /// <summary>
        /// 
        /// </summary>
        public int Count { get { return count; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        public SimpleRingBuffer(int size)
        {
            buffer = new T[size];
            bufferSize = size;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns>If buffer full, return false</returns>
        public void Add(T item)
        {
            if (IsFull())
            {
                RemoveFirst();
            }
            buffer[next] = item;
            last = next;
            next = IncLast();
            ++count;
        }
        /// <summary>
        /// Remove the last element added into the buffer
        /// </summary>
        /// <returns></returns>
        public bool RemoveLast()
        {
            if (IsEmpty())
            {
                return false;
            }
            else
            {         
                next = DecLast();
                buffer[next] = default(T);
                last = next == 0? bufferSize - 1 : next - 1;
                --count;
                return true;
            }
        }
        /// <summary>
        /// Remove the first element added into the buffer
        /// </summary>
        /// <returns></returns>
        public bool RemoveFirst()
        {
            if (IsEmpty())
            {
                return false;
            }
            else
            {
                buffer[first] = default(T);
                first = IncFirst();
                --count;
                return true;
            }
        }
        /// <summary>
        /// Get last added element
        /// </summary>
        public T Last
        {
            get
            {
                return IsEmpty() ? default(T) : buffer[last];
            }
        }
        /// <summary>
        /// Get first added element
        /// </summary>
        public T First
        {
            get
            {
                return IsEmpty() ? default(T) : buffer[first];
            }
        }

        /// <summary>
        /// If buffer is full
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return count == bufferSize;
        }

        /// <summary>
        /// If buffer is empty
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return count == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IncLast()
        {
            return (next + 1) % bufferSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int DecLast()
        {
            var prev = next - 1;
            return prev >= 0 ? prev : bufferSize - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IncFirst()
        {
            return (first + 1) % bufferSize;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public void Clear()
        {
            Array.Clear(buffer, 0, bufferSize);
            first = next = 0;
            last = -1;
            count = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get
            {
                return buffer[(first + i) % bufferSize];
            }
        }
    }
}
