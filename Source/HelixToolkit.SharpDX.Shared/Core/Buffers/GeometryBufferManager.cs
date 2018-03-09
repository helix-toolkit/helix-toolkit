/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#define DEBUGDETAIL
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class GeometryBufferProxy<T> : IGeometryBufferProxy where T : class, IGeometryBufferModel
    {
        public static readonly IGeometryBufferProxy Empty = new GeometryBufferProxy<T>();
        /// <summary>
        /// Gets the model unique identifier.
        /// </summary>
        /// <value>
        /// The model unique identifier.
        /// </value>
        public Guid ModelGuid { get; private set; } = Guid.Empty;
        /// <summary>
        /// Gets the geometry unique identifier.
        /// </summary>
        /// <value>
        /// The geometry unique identifier.
        /// </value>
        public Guid GeometryGuid { get; private set; } = Guid.Empty;
        /// <summary>
        /// Gets or sets the buffer model.
        /// </summary>
        /// <value>
        /// The buffer model.
        /// </value>
        public IGeometryBufferModel BufferModel { private set; get; }
        
        private readonly IGeometryBufferManager manager;
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryBufferProxy{T}"/> class.
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometryGuid">The geometry unique identifier.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="manager">The manager.</param>
        public GeometryBufferProxy(Guid modelGuid, Guid geometryGuid, IGeometryBufferModel buffer, IGeometryBufferManager manager)
        {
            this.ModelGuid = modelGuid;
            this.GeometryGuid = geometryGuid;
            this.manager = manager;
            this.BufferModel = buffer;
        }

        private GeometryBufferProxy()
        {
            this.BufferModel = EmptyGeometryBufferModel.Empty;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    manager?.Unregister<T>(this);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GeometryBufferProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// Use to manage geometry vertex/index buffers. 
    /// Same geometry with same buffer type will share the same buffer across all models.
    /// </summary>
    public sealed class GeometryBufferManager : DisposeObject, IGeometryBufferManager
    {
        /// <summary>
        /// The buffer dictionary. Key1=<see cref="Geometry3D.GUID"/>, Key2=Typeof(Buffer)
        /// </summary>
        private readonly DoubleKeyDictionary<Type, Guid, GeometryBufferContainer> bufferDictionary
            = new DoubleKeyDictionary<Type, Guid, GeometryBufferContainer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryBufferManager"/> class.
        /// </summary>
        public GeometryBufferManager()
        {

        }
        /// <summary>
        /// Registers the specified model unique identifier.
        /// </summary>
        /// <typeparam name="T">Geometry Buffer Type</typeparam>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        public IGeometryBufferProxy Register<T>(Guid modelGuid, Geometry3D geometry) where T : class, IGeometryBufferModel, new()
        {
            if (geometry == null || modelGuid == Guid.Empty)
            {
                return GeometryBufferProxy<T>.Empty;
            }
            lock (bufferDictionary)
            {
                GeometryBufferContainer container = null;
                if(bufferDictionary.TryGetValue(typeof(T), geometry.GUID, out container))
                {
#if DEBUGDETAIL
                    Debug.WriteLine("Existing buffer found, GeomoetryGUID = " + geometry.GUID);
#endif
                    container.Attach(modelGuid);
                }
                else
                {
#if DEBUGDETAIL
                    Debug.WriteLine("Buffer not found, create new buffer. GeomoetryGUID = " + geometry.GUID);
#endif
                    container = GeometryBufferContainer.Create<T>();
                    var id = geometry.GUID;
                    container.Disposed += (s, e) => 
                    {
                        lock (bufferDictionary)
                        {
                            bufferDictionary.Remove(typeof(T), id);
                        }
                    };
                    container.Buffer.Geometry = geometry;
                    container.Attach(modelGuid);
                    bufferDictionary.Add(typeof(T), geometry.GUID, container);
                
                }
                return new GeometryBufferProxy<T>(modelGuid, geometry.GUID, container.Buffer, this);
            }
        }

        /// <summary>
        /// Unregisters the specified model unique identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proxy">Buffer Proxy</param>
        /// <returns></returns>
        public bool Unregister<T>(IGeometryBufferProxy proxy) where T : class, IGeometryBufferModel
        {
            if (proxy.GeometryGuid == Guid.Empty || proxy.ModelGuid == Guid.Empty)
            {
                return false;
            }
            lock (bufferDictionary)
            {
                GeometryBufferContainer container = null;
                if(bufferDictionary.TryGetValue(typeof(T), proxy.GeometryGuid, out container))
                {
#if DEBUGDETAIL
                    Debug.WriteLine("Existing buffer found, Detach model from buffer. ModelGUID = " + proxy.ModelGuid);
#endif
                    container.Detach(proxy.ModelGuid);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Unregister geometry buffer, buffer is not found.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (bufferDictionary)
                {
                    foreach (var buffer in bufferDictionary.Values.ToArray())
                    {
                        buffer.Dispose();
                    }
                    bufferDictionary.Clear();
                }
            }
            base.OnDispose(disposeManagedResources);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class GeometryBufferContainer : ResourceSharedObject
        {
            private readonly IGeometryBufferModel buffer;
            /// <summary>
            /// Gets the buffer.
            /// </summary>
            /// <value>
            /// The buffer.
            /// </value>
            public IGeometryBufferModel Buffer { get { return buffer; } }
            /// <summary>
            /// Initializes a new instance of the <see cref="GeometryBufferContainer"/> class.
            /// </summary>
            /// <param name="model">The model.</param>
            private GeometryBufferContainer(IGeometryBufferModel model)
            {
                buffer = Collect(model);
            }

            /// <summary>
            /// Creates the specified structure size.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static GeometryBufferContainer Create<T>() where T : class, IGeometryBufferModel, new()
            {
                return new GeometryBufferContainer(new T());
            }
        }
    }
}
