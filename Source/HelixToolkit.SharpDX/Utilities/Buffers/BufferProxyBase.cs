using SDX11 = SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public abstract class BufferProxyBase : DisposeObject, IBufferProxy
{
    /// <summary>
    /// 
    /// </summary>
    protected SDX11.Buffer? buffer;
    /// <summary>
    /// <see cref="IBufferProxy.StructureSize"/> 
    /// </summary>
    public int StructureSize
    {
        get; private set;
    }
    /// <summary>
    ///  <see cref="IBufferProxy.ElementCount"/> 
    /// </summary>
    public int ElementCount { get; protected set; } = 0;
    /// <summary>
    /// Buffer data offset in bytes.
    /// <see cref="IBufferProxy.Offset"/> 
    /// </summary>
    public int Offset { get; set; } = 0;
    /// <summary>
    ///  <see cref="IBufferProxy.Buffer"/> 
    /// </summary>
    public SDX11.Buffer? Buffer
    {
        get
        {
            return buffer;
        }
    }
    /// <summary>
    ///  <see cref="IBufferProxy.BindFlags"/> 
    /// </summary>
    public SDX11.BindFlags BindFlags
    {
        private set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="structureSize"></param>
    /// <param name="bindFlags"></param>
    public BufferProxyBase(int structureSize, SDX11.BindFlags bindFlags)
    {
        StructureSize = structureSize;
        BindFlags = bindFlags;
    }

    public void DisposeAndClear()
    {
        RemoveAndDispose(ref buffer);
        ElementCount = 0;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        DisposeAndClear();
        base.OnDispose(disposeManagedResources);
    }
}
