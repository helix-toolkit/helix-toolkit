﻿namespace HelixToolkit.SharpDX.ShaderManager;

///// <summary>
///// 
///// </summary>
//public sealed class ElementsBufferPool : DisposeObject, IBufferPool
//{
//    private readonly DoubleKeyDictionary<System.Guid, BufferDescription, Buffer> pool = new DoubleKeyDictionary<System.Guid, BufferDescription, Buffer>();
//    /// <summary>
//    /// 
//    /// </summary>
//    public Device Device { private set; get; }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="device"></param>
//    public ElementsBufferPool(Device device)
//    {
//        this.Device = device;
//    }
//    /// <summary>
//    /// <see cref="IBufferPool.Register{T}(System.Guid, BufferDescription, IList{T})"/>
//    /// </summary>
//    /// <param name="guid"></param>
//    /// <param name="description"></param>
//    /// <param name="data"></param>
//    /// <returns></returns>
//    public Buffer Register<T>(System.Guid guid, BufferDescription description, IList<T> data) where T : unmanaged
//    {
//        Buffer value;
//        if (pool.TryGetValue(guid, description, out value))
//        {
//            ((IUnknown)value).AddReference();
//            return value;
//        }
//        else
//        {
//            value = Collect(Buffer.Create<T>(Device, data.GetArrayByType(), description));
//            pool.Add(guid, description, value);
//            value.Disposed += (s, e) =>
//            {
//                pool.Remove(guid, description);
//            };
//            return value;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="disposeManagedResources"></param>
//    protected override void Dispose(bool disposeManagedResources)
//    {
//        pool.Clear();
//        base.Dispose(disposeManagedResources);
//    }
//}
