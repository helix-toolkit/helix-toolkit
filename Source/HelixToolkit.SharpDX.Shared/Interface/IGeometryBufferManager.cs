using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGeometryBufferProxy : IDisposable
    {
        IGeometryBufferModel BufferModel { get; }
        Guid ModelGuid { get; }
        Guid GeometryGuid { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IGeometryBufferManager : IDisposable
    {
        /// <summary>
        /// Registers the specified model unique identifier.
        /// </summary>
        /// <typeparam name="T">Geometry Buffer Type</typeparam>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        IGeometryBufferProxy Register<T>(Guid modelGuid, Geometry3D geometry) where T : class, IGeometryBufferModel, new();
        /// <summary>
        /// Unregisters the specified model unique identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proxy">Buffer Proxy return from <see cref="Register{T}(Guid, Geometry3D)"/>.</param>
        /// <returns></returns>
        bool Unregister<T>(IGeometryBufferProxy proxy) where T : class, IGeometryBufferModel;
    }
}
