using System;
using System.Diagnostics;
using System.Threading;

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
    using Utilities;
    /// <summary>
    /// A array buffer defined by its struct size.
    /// <para>
    /// To use it, caller must first call <see cref="GetId"/> to obtain an unique id in order to modify the buffer. 
    /// Caller must restrictly use this unique id to access the buffer.
    /// </para>
    /// Caller must call <see cref="ReleaseId(int)"/> to release the id once 
    /// caller is no longer needed to use this buffer. The released id will be reused by an new caller.
    /// </summary>
    public sealed unsafe class ArrayStorage : DisposeObject
    {
        const string tag = nameof(ArrayStorage);
        private readonly FastList<byte> binaryArray = new FastList<byte>();
        private readonly int structSize;
        private readonly IdHelper idHelper = new IdHelper();
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        private readonly Logger.LogWrapper logger;

        public int StructSize => structSize;

        public ArrayStorage(int structSize, Logger.LogWrapper logger)
        {
            this.structSize = structSize;
            this.logger = logger;
        }

        public int GetId()
        {
            var id = idHelper.GetNextId();
            if (binaryArray.Count <= id * structSize)
            {
                rwLock.EnterWriteLock();
                binaryArray.Resize((id + 1) * structSize, false);
                rwLock.ExitWriteLock();
                logger.Log(Logger.LogLevel.Debug, $"Resize struct array to " +
                    $"{structSize} * {id + 1} = {binaryArray.Count}", tag);
            }
            logger.Log(Logger.LogLevel.Debug, $"Getting new id {id} on struct size {StructSize}.", tag);
            return id;
        }

        public void ReleaseId(int id)
        {
            logger.Log(Logger.LogLevel.Debug, $"Release id {id} on struct size {StructSize}.", tag);
            idHelper.ReleaseId(id);
            Clear(id);
        }

        public void Clear(int id)
        {
            if (id < 0)
            {
                logger.Log(Logger.LogLevel.Error, $"Invalid Id {id}");
                return;
            }
            var offsetInArray = GetOffSet(id);
            if (offsetInArray + structSize > binaryArray.Count)
            {
                Debug.Assert(false);
                return;
            }
            rwLock.EnterReadLock();
            var array = binaryArray.GetInternalArray();
            fixed (byte* pArray = &array[offsetInArray])
            {
                UnsafeHelper.ClearMemory(new IntPtr(pArray), 0, structSize);
            }
            rwLock.ExitReadLock();
        }

        public bool Write(int id, int offset, IntPtr data, int dataLength)
        {
            if (id < 0)
            {
                logger.Log(Logger.LogLevel.Error, $"Invalid Id {id}");
                return false;
            }
            var offsetInArray = GetOffSet(id) + offset;
            if (offsetInArray + dataLength > binaryArray.Count || offset + dataLength > structSize)
            {
                Debug.Assert(false);
                return false;
            }
            rwLock.EnterReadLock();
            var array = binaryArray.GetInternalArray();
            fixed (byte* pArray = &array[offsetInArray])
            {
                UnsafeHelper.Write(new IntPtr(pArray), data, 0, dataLength);
            }
            rwLock.ExitReadLock();
            return true;
        }

        public bool Write<T>(int id, int offset, ref T value) where T : unmanaged
        {
            var size = UnsafeHelper.SizeOf<T>();
            fixed (T* pValue = &value)
            {
                return Write(id, offset, new IntPtr(pValue), size);
            }
        }

        public bool Read(int id, IntPtr dest)
        {
            return Read(id, dest, structSize);
        }

        public bool Read(int id, IntPtr dest, int size)
        {
            if (id < 0)
            {
                return false;
            }
            var offsetInArray = GetOffSet(id);
            if (offsetInArray + size > binaryArray.Count)
            {
                Debug.Assert(false);
                return false;
            }
            var array = binaryArray.GetInternalArray();
            fixed (byte* pArray = &array[offsetInArray])
            {
                UnsafeHelper.MemoryCopy(dest, new IntPtr(pArray), size);
            }
            return true;
        }

        public bool Read<T>(int id, int offset, out T value) where T : unmanaged
        {
            if (id < 0)
            {
                value = default;
                return false;
            }
            var size = UnsafeHelper.SizeOf<T>();
            var offsetInArray = GetOffSet(id) + offset;
            if (offsetInArray + size > binaryArray.Count)
            {
                Debug.Assert(false);
                value = default;
                return false;
            }
            var array = binaryArray.GetInternalArray();
            fixed (byte* pArray = &array[offsetInArray])
            {
                value = *(T*)pArray;
            }
            return true;
        }

        public int GetOffSet(int id)
        {
            return id * structSize;
        }

        public byte[] GetArray()
        {
            return binaryArray.GetInternalArray();
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            rwLock.EnterWriteLock();
            binaryArray.Clear();
            rwLock.ExitWriteLock();
            base.OnDispose(disposeManagedResources);
        }
    }

    /// <summary>
    /// Interface for struct array
    /// </summary>
    public interface IStructArrayPool : IDisposable
    {
        ArrayStorage Register(int structSize);
    }


    /// <summary>
    /// A pool contains various of binary buffers defined by struct size.
    /// </summary>
    public sealed class StructArrayPool : DisposeObject, IStructArrayPool
    {
        private sealed class ArrayPoolStorage : ReferenceCountedDictionaryPool<int, ArrayStorage, int>
        {
            private readonly Logger.LogWrapper logger;
            public ArrayPoolStorage(Logger.LogWrapper logger) : base(true)
            {
                this.logger = logger;
            }

            protected override bool CanCreate(ref int key, ref int argument)
            {
                return argument > 0;
            }

            protected override ArrayStorage OnCreate(ref int key, ref int argument)
            {
                logger.Log(Logger.LogLevel.Information, $"Creating new struct array with size {argument}");
                return new ArrayStorage(argument, logger);
            }        
        }

        private ArrayPoolStorage storage;
        private readonly Logger.LogWrapper logger;

        public StructArrayPool(Logger.LogWrapper logger)
        {
            this.logger = logger;
            storage = new ArrayPoolStorage(logger);
        }

        public ArrayStorage Register(int structSize)
        {
            return storage.TryCreateOrGet(structSize, structSize, out var s) ? s : null;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                RemoveAndDispose(ref storage);
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}
