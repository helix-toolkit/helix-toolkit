using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class NodePostEffectXRay : SceneNode
{
    #region Properties
    /// <summary>
    /// Gets or sets the name of the effect.
    /// </summary>
    /// <value>
    /// The name of the effect.
    /// </value>
    public string EffectName
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                core.EffectName = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                return core.EffectName;
            }

            return string.Empty;
        }
    }

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
            if (RenderCore is IPostEffectMeshXRay core)

            {
                core.Color = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                return core.Color;
            }

            return Maths.Color.Zero;
        }
    }
    /// <summary>
    /// Gets or sets the outline fading factor.
    /// </summary>
    /// <value>
    /// The outline fading factor.
    /// </value>
    public float OutlineFadingFactor
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                core.OutlineFadingFactor = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                return core.OutlineFadingFactor;
            }

            return 1.0f;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable double pass].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable double pass]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableDoublePass
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                core.EnableDoublePass = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRay core)
            {
                return core.EnableDoublePass;
            }

            return false;
        }
    }
    #endregion

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PostEffectMeshXRayCore();
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
