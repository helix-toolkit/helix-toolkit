using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Model.Scene;

public class ScreenQuadNode : SceneNode
{
    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public TextureModel? Texture
    {
        set
        {
            if (RenderCore is DrawScreenQuadCore core)
            {
                core.Texture = value;
            }
        }
        get
        {
            if (RenderCore is DrawScreenQuadCore core)
            {
                return core.Texture;
            }

            return null;
        }
    }
    /// <summary>
    /// Gets or sets the sampler.
    /// </summary>
    /// <value>
    /// The sampler.
    /// </value>
    public SamplerStateDescription Sampler
    {
        set
        {
            if (RenderCore is DrawScreenQuadCore core)
            {
                core.SamplerDescription = value;
            }
        }
        get
        {
            if (RenderCore is DrawScreenQuadCore core)
            {
                return core.SamplerDescription;
            }

            return SamplerStateDescription.Default();
        }
    }

    private float depth = 1f;
    public float Depth
    {
        set
        {
            if (SetAffectsRender(ref depth, value))
            {
                var core = RenderCore as DrawScreenQuadCore;
                if (core is not null)
                {
                    core.ModelStruct.TopLeft.Z = core.ModelStruct.TopRight.Z = core.ModelStruct.BottomLeft.Z = core.ModelStruct.BottomRight.Z = value;
                }
            }
        }
        get
        {
            return depth;
        }
    }

    public ScreenQuadNode()
    {
        IsHitTestVisible = false;
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new DrawScreenQuadCore();
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.ScreenQuad];
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
