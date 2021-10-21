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
    public sealed unsafe class ArrayStorage : DisposeObject
    {
        private readonly FastList<byte> binaryArray = new FastList<byte>();
        private readonly int structSize;
        private readonly IdHelper idHelper = new IdHelper();
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public ArrayStorage(int structSize)
        {
            this.structSize = structSize;
        }

        public int GetId()
        {
            var id = idHelper.GetNextId();
            if (binaryArray.Count <= id)
            {
                rwLock.EnterWriteLock();
                binaryArray.Resize(id + 1, false);
                rwLock.ExitWriteLock();
            }
            return id;
        }

        public void ReleaseId(int id)
        {
            idHelper.ReleaseId(id);
        }

        public bool Write(int id, int offset, IntPtr data, int dataLength)
        {
            if (id >= binaryArray.Count || offset + dataLength > structSize)
            {
                Debug.Assert(false);
                return false;
            }
            var offsetInArray = GetOffSet(id) + offset;
            rwLock.EnterReadLock();
            var array = binaryArray.GetInternalArray();
            fixed (byte* pArray = &array[offsetInArray])
            {
                UnsafeHelper.Write((IntPtr)pArray, data, 0, dataLength);
            }
            rwLock.ExitReadLock();
            return true;
        }

        private int GetOffSet(int id)
        {
            return id * structSize;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            rwLock.EnterWriteLock();
            binaryArray.Clear();
            rwLock.ExitWriteLock();
            base.OnDispose(disposeManagedResources);
        }
    }

    public interface IStructArrayPool : IDisposable
    {
        ArrayStorage Register(int structSize);
    }

    public sealed class ArrayPool : DisposeObject, IStructArrayPool
    {
        private sealed class ArrayPoolStorage : ReferenceCountedDictionaryPool<int, ArrayStorage, int>
        {
            public ArrayPoolStorage() : base(true)
            {
            }

            protected override bool CanCreate(ref int key, ref int argument)
            {
                return argument > 0;
            }

            protected override ArrayStorage OnCreate(ref int key, ref int argument)
            {
                return new ArrayStorage(argument);
            }        
        }

        private ArrayPoolStorage storage = new ArrayPoolStorage();

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
