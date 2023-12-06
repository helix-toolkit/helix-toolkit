using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class RasterizerStateProxy : StateProxy<RasterizerState>
{
    public static readonly RasterizerStateProxy Empty = new(null);
    internal RasterizerStateProxy(RasterizerState? state) : base(state) { }
}
