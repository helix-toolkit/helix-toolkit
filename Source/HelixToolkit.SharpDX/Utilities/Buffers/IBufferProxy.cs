using SDX11 = SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public interface IBufferProxy : IDisposable
{
    /// <summary>
    /// Raw Buffer
    /// </summary>
    SDX11.Buffer? Buffer
    {
        get;
    }
    /// <summary>
    /// Element Size
    /// </summary>
    int StructureSize
    {
        get;
    }
    /// <summary>
    /// Element count
    /// </summary>
    int ElementCount
    {
        get;
    }
    /// <summary>
    /// Buffer offset in bytes
    /// </summary>
    int Offset
    {
        set; get;
    }
    /// <summary>
    /// Buffer binding flag
    /// </summary>
    SDX11.BindFlags BindFlags
    {
        get;
    }
}
