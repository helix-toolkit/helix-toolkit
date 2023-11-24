using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IBoneSkinMeshBufferModel : IGeometryBufferModel
{
    event EventHandler BoneIdBufferUpdated;

    IElementsBufferProxy? BoneIdBuffer
    {
        get;
    }
}
