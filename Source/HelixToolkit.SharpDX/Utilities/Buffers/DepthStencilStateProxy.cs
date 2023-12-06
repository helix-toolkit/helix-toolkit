using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class DepthStencilStateProxy : StateProxy<DepthStencilState>
{
    public static readonly DepthStencilStateProxy Empty = new(null);
    internal DepthStencilStateProxy(DepthStencilState? state) : base(state) { }
}
