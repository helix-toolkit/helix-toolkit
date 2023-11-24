using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public abstract class LightNode : SceneNode, ILight3D
{
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Color4 Color
    {
        set
        {
            if (RenderCore is LightCoreBase light)
            {
                light.Color = value;
            }
        }
        get
        {
            if (RenderCore is LightCoreBase light)
            {
                return light.Color;
            }

            return Color4.Black;
        }
    }
    /// <summary>
    /// Gets the type of the light.
    /// </summary>
    /// <value>
    /// The type of the light.
    /// </value>
    public LightType LightType
    {
        get
        {
            if (RenderCore is LightCoreBase light)
            {
                return light.LightType;
            }

            return LightType.None;
        }
    }

    public sealed override bool HitTest(HitTestContext? context, ref List<HitTestResult> hits)
    {
        return false;
    }

    protected sealed override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return false;
    }
}
