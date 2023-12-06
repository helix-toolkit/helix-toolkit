using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class BlendStateProxy : StateProxy<BlendState>
{
    public static readonly BlendStateProxy Empty = new(null);
    internal BlendStateProxy(BlendState? state) : base(state) { }
}
