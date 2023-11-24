using HelixToolkit.SharpDX.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HelixToolkit.SharpDX;

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
    private static readonly ILogger logger = Logger.LogManager.Create<ArrayStorage>();
    public static int MinArraySize = 1024 * 4;
    public static int MaxArraySizeExpoentialIncrement = 1024 * 1024;
    private readonly FastList<byte> binaryArray = new(MinArraySize);
    private readonly int structSize;
    private readonly IdHelper idHelper = new();
    private readonly ReaderWriterLockSlim rwLock = new();

    public int StructSize => structSize;

    public ArrayStorage(int structSize)
    {
        this.structSize = structSize;
    }

    public int GetId()
    {
        var id = idHelper.GetNextId();
        if (binaryArray.Count <= id * structSize)
        {
            var newSize = (id * 2) * structSize;
            if (newSize > MaxArraySizeExpoentialIncrement)
            {
                newSize = (id + 1) * structSize;
            }
            rwLock.EnterWriteLock();
            binaryArray.Resize(newSize, false);
            rwLock.ExitWriteLock();
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Resize struct array to {0} * {1} = {2}", structSize, id + 1, binaryArray.Count);
            }
        }
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Getting new id [{0}] on struct size [{1}].", id, StructSize);
        }
        return id;
    }

    public void ReleaseId(int id)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Release id [{0}] on struct size [{1}].", id, StructSize);
        }
        idHelper.ReleaseId(id);
        Clear(id);
    }

    public void Clear(int id)
    {
        if (id < 0)
        {
            logger.LogError("Invalid Id {0}", id);
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
        Array.Clear(array, offsetInArray, structSize);
        rwLock.ExitReadLock();
    }

    public bool Write(int id, int offset, IntPtr data, int dataLength)
    {
        if (id < 0)
        {
            logger.LogError("Invalid Id {0}", id);
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
        return Read(id, 0, dest, structSize);
    }

    public bool Read(int id, int offset, IntPtr dest, int size)
    {
        if (id < 0)
        {
            return false;
        }
        var offsetInArray = GetOffSet(id) + offset;
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
