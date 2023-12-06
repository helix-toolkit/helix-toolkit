using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class SpotLightNode : PointLightNode
{
    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    /// <value>
    /// The direction.
    /// </value>
    public Vector3 Direction
    {
        set
        {
            if (RenderCore is SpotLightCore core)
            {
                core.Direction = value;
            }
        }
        get
        {
            if (RenderCore is SpotLightCore core)
            {
                return core.Direction;
            }

            return Vector3.Zero;
        }
    }
    /// <summary>
    /// Gets or sets the fall off.
    /// </summary>
    /// <value>
    /// The fall off.
    /// </value>
    public float FallOff
    {
        set
        {
            if (RenderCore is SpotLightCore core)
            {
                core.FallOff = value;
            }
        }
        get
        {
            if (RenderCore is SpotLightCore core)
            {
                return core.FallOff;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the inner angle.
    /// </summary>
    /// <value>
    /// The inner angle.
    /// </value>
    public float InnerAngle
    {
        set
        {
            if (RenderCore is SpotLightCore core)
            {
                core.InnerAngle = value;
            }
        }
        get
        {
            if (RenderCore is SpotLightCore core)
            {
                return core.InnerAngle;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the outer angle.
    /// </summary>
    /// <value>
    /// The outer angle.
    /// </value>
    public float OuterAngle
    {
        set
        {
            if (RenderCore is SpotLightCore core)
            {
                core.OuterAngle = value;
            }
        }
        get
        {
            if (RenderCore is SpotLightCore core)
            {
                return core.OuterAngle;
            }

            return 0.0f;
        }
    }

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new SpotLightCore();
    }
}
