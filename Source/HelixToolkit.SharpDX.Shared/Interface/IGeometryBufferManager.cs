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
    public interface IGeometryBufferManager : IDisposable
    {
        /// <summary>
        /// Registers the specified model unique identifier.
        /// </summary>
        /// <typeparam name="T">Geometry Buffer Type</typeparam>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="structSize">Size of the vertex structure.</param>
        /// <returns></returns>
        IGeometryBufferModel Register<T>(Guid modelGuid, Geometry3D geometry) where T : IGeometryBufferModel;
        /// <summary>
        /// Unregisters the specified model unique identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        bool Unregister<T>(Guid modelGuid, Geometry3D geometry) where T : IGeometryBufferModel;
    }
}
