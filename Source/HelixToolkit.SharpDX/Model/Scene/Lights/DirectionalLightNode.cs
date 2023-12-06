using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public sealed class DirectionalLightNode : LightNode
{
    public Vector3 Direction
    {
        set
        {
            if (RenderCore is DirectionalLightCore core)
            {
                core.Direction = value;
            }
        }
        get
        {
            if (RenderCore is DirectionalLightCore core)
            {
                return core.Direction;
            }

            return Vector3.Zero;
        }
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new DirectionalLightCore();
    }
}
