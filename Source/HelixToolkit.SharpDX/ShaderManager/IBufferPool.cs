using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public interface IBufferPool
{
    /// <summary>
    /// Register a buffer with object as its key
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="description"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    Buffer Register<T>(System.Guid guid, BufferDescription description, IList<T> data) where T : unmanaged;
}
