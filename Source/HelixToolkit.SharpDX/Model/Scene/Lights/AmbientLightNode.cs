using HelixToolkit.SharpDX.Core;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public sealed class AmbientLightNode : LightNode
{
    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new AmbientLightCore();
    }
}
