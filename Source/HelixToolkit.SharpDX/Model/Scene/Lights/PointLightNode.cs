using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class PointLightNode : LightNode
{
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public Vector3 Position
    {
        set
        {
            if (RenderCore is PointLightCore core)
            {
                core.Position = value;
            }
        }
        get
        {
            if (RenderCore is PointLightCore core)
            {
                return core.Position;
            }

            return Vector3.Zero;
        }
    }
    /// <summary>
    /// Gets or sets the attenuation.
    /// </summary>
    /// <value>
    /// The attenuation.
    /// </value>
    public Vector3 Attenuation
    {
        set
        {
            if (RenderCore is PointLightCore core)
            {
                core.Attenuation = value;
            }
        }
        get
        {
            if (RenderCore is PointLightCore core)
            {
                return core.Attenuation;
            }

            return Vector3.Zero;
        }
    }
    /// <summary>
    /// Gets or sets the range.
    /// </summary>
    /// <value>
    /// The range.
    /// </value>
    public float Range
    {
        set
        {
            if (RenderCore is PointLightCore core)
            {
                core.Range = value;
            }
        }
        get
        {
            if (RenderCore is PointLightCore core)
            {
                return core.Range;
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
        return new PointLightCore();
    }
}
