using Microsoft.Extensions.Logging;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Use to manage geometry vertex/index buffers. 
/// Same geometry with same buffer type will share the same buffer across all models.
/// </summary>
public sealed class GeometryBufferManager : IDisposable, IGeometryBufferManager
{
    private static readonly ILogger logger = Logger.LogManager.Create<GeometryBufferManager>();
    public int Count
    {
        get
        {
            return bufferDictionary.Count();
        }
    }
    /// <summary>
    /// The buffer dictionary. Key1=<see cref="Geometry3D.GUID"/>, Key2=Typeof(Buffer)
    /// </summary>
    private readonly DoubleKeyDictionary<Type, Guid, DisposeObject> bufferDictionary = new();
    private readonly IEffectsManager manager;
    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryBufferManager"/> class.
    /// </summary>
    public GeometryBufferManager(IEffectsManager manager)
    {
        this.manager = manager;
    }

    /// <summary>
    /// Registers the specified model unique identifier.
    /// </summary>
    /// <typeparam name="T">Geometry Buffer Type</typeparam>
    /// <param name="modelGuid">The model unique identifier.</param>
    /// <param name="geometry">The geometry.</param>
    /// <returns></returns>
    public IGeometryBufferModel? Register<T>(Guid modelGuid, Geometry3D? geometry) where T : class, IGeometryBufferModel, new()
    {
        if (geometry == null || modelGuid == Guid.Empty)
        {
            return EmptyGeometryBufferModel.Empty;
        }
        lock (bufferDictionary)
        {
            IGeometryBufferModel? container;
            if (bufferDictionary.TryGetValue(typeof(T), geometry.GUID, out var obj))
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace("Existing buffer found, GeomoetryGUID = {0}", geometry.GUID);
                }
                container = obj as IGeometryBufferModel;
                obj.IncRef();
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace("Buffer not found, create new buffer. GeomoetryGUID = {0}", geometry.GUID);
                }
                container = new T();
                var id = geometry.GUID;
                obj = container as DisposeObject;
                if (obj is not null)
                {
                    obj.Disposed += (s, e) =>
                    {
                        if (logger.IsEnabled(LogLevel.Trace))
                        {
                            logger.LogTrace("Disposing Geometry Buffer. GeomoetryGUID = {0}", id);
                        }
                        lock (bufferDictionary)
                        {
                            bufferDictionary.Remove(typeof(T), id);
                        }
                    };
                    container.EffectsManager = manager;
                    container.Geometry = geometry;
                    bufferDictionary.Add(typeof(T), geometry.GUID, obj);
                }
            }
            return container;
        }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                lock (bufferDictionary)
                {
                    foreach (var buffer in bufferDictionary.Values.ToArray())
                    {
                        buffer.ForceDispose();
                    }
                    bufferDictionary.Clear();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~GeometryBufferManager() {
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
