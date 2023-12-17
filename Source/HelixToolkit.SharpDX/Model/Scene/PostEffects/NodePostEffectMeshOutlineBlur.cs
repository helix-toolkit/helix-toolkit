using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
///
/// </summary>
public class NodePostEffectMeshOutlineBlur : SceneNode
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
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                core.EffectName = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectOutlineBlur core)
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
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                core.Color = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                return core.Color;
            }

            return Maths.Color.Zero;
        }
    }

    /// <summary>
    /// Gets or sets the scale x.
    /// </summary>
    /// <value>
    /// The scale x.
    /// </value>
    public float ScaleX
    {
        set
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                core.ScaleX = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                return core.ScaleX;
            }

            return 1.0f;
        }
    }

    /// <summary>
    /// Gets or sets the scale y.
    /// </summary>
    /// <value>
    /// The scale y.
    /// </value>
    public float ScaleY
    {
        set
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                core.ScaleY = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                return core.ScaleY;
            }

            return 1.0f;
        }
    }

    /// <summary>
    /// Gets or sets the number of blur pass.
    /// </summary>
    /// <value>
    /// The number of blur pass.
    /// </value>
    public int NumberOfBlurPass
    {
        set
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                core.NumberOfBlurPass = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectOutlineBlur core)
            {
                return core.NumberOfBlurPass;
            }

            return 0;
        }
    }
    #endregion

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PostEffectMeshOutlineBlurCore();
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur];
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
