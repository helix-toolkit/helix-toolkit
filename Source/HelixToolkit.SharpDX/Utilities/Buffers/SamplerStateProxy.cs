using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class SamplerStateProxy : StateProxy<SamplerState>
{
    public static readonly SamplerStateProxy Empty = new(null);
    internal SamplerStateProxy(SamplerState? state) : base(state) { }
}
