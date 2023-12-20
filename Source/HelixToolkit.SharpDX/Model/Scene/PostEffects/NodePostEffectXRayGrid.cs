using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class NodePostEffectXRayGrid : SceneNode
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
            if (RenderCore is IPostEffect core)
            {
                core.EffectName = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffect core)
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
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                core.Color = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                return core.Color;
            }

            return Maths.Color.Zero;
        }
    }
    /// <summary>
    /// Gets or sets the grid density.
    /// </summary>
    /// <value>
    /// The grid density.
    /// </value>
    public int GridDensity
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                core.GridDensity = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                return core.GridDensity;
            }

            return 0;
        }
    }
    /// <summary>
    /// Gets or sets the dimming factor.
    /// </summary>
    /// <value>
    /// The dimming factor.
    /// </value>
    public float DimmingFactor
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                core.DimmingFactor = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                return core.DimmingFactor;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the blending factor for grid and original mesh color blending
    /// </summary>
    /// <value>
    /// The blending factor.
    /// </value>
    public float BlendingFactor
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                core.BlendingFactor = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                return core.BlendingFactor;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the name of the x ray drawing pass. This is the final pass to draw mesh and grid overlay onto render target
    /// </summary>
    /// <value>
    /// The name of the x ray drawing pass.
    /// </value>
    public string XRayDrawingPassName
    {
        set
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                core.XRayDrawingPassName = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectMeshXRayGrid core)
            {
                return core.XRayDrawingPassName;
            }

            return string.Empty;
        }
    }
    #endregion

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PostEffectMeshXRayGridCore();
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
