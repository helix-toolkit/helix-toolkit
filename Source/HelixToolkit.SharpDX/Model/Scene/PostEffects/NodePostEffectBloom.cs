using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class NodePostEffectBloom : SceneNode
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
            if (RenderCore is IPostEffectBloom core)
            {
                core.EffectName = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.EffectName;
            }

            return string.Empty;
        }
    }
    /// <summary>
    /// Gets or sets the color of the threshold.
    /// </summary>
    /// <value>
    /// The color of the threshold.
    /// </value>
    public Color4 ThresholdColor
    {
        set
        {
            if (RenderCore is IPostEffectBloom core)
            {
                core.ThresholdColor = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.ThresholdColor;
            }
            return Color.Zero;
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
            if (RenderCore is IPostEffectBloom core)
            {
                core.NumberOfBlurPass = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.NumberOfBlurPass;
            }

            return 0;
        }
    }

    /// <summary>
    /// Gets or sets the bloom extract intensity.
    /// </summary>
    /// <value>
    /// The bloom extract intensity.
    /// </value>
    public float BloomExtractIntensity
    {
        set
        {
            if (RenderCore is IPostEffectBloom core)
            {
                core.BloomExtractIntensity = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.BloomExtractIntensity;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the bloom pass intensity.
    /// </summary>
    /// <value>
    /// The bloom pass intensity.
    /// </value>
    public float BloomPassIntensity
    {
        set
        {
            if (RenderCore is IPostEffectBloom core)
            {
                core.BloomPassIntensity = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.BloomPassIntensity;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the bloom combine intensity.
    /// </summary>
    /// <value>
    /// The bloom combine intensity.
    /// </value>
    public float BloomCombineIntensity
    {
        set
        {
            if (RenderCore is IPostEffectBloom core)
            {
                core.BloomCombineIntensity = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.BloomCombineIntensity;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the bloom combine saturation.
    /// </summary>
    /// <value>
    /// The bloom combine saturation.
    /// </value>
    public float BloomCombineSaturation
    {
        set
        {
            if (RenderCore is IPostEffectBloom core)
            {
                core.BloomCombineSaturation = value;
            }
        }
        get
        {
            if (RenderCore is IPostEffectBloom core)
            {
                return core.BloomCombineSaturation;
            }

            return 0.0f;
        }
    }
    #endregion

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PostEffectBloomCore();
    }

    /// <summary>
    /// Override this function to set render technique during Attach Host.
    /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="SceneNode.OnCreateRenderTechnique" /> function will be called.</para>
    /// </summary>
    /// <param name="effectsManager"></param>
    /// <returns>
    /// Return RenderTechnique
    /// </returns>
    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.PostEffectBloom];
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
