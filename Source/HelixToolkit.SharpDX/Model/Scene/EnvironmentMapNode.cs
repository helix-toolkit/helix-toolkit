using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class EnvironmentMapNode : SceneNode
{
    /// <summary>
    /// Gets or sets the environment texture. Must be 3D cube texture
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public TextureModel? Texture
    {
        set
        {
            if (RenderCore is ISkyboxRenderParams core)
            {
                core.CubeTexture = value;
            }
        }
        get
        {
            if (RenderCore is ISkyboxRenderParams core)
            {
                return core.CubeTexture;
            }

            return null;
        }
    }
    /// <summary>
    /// Skip environment map rendering, but still keep it available for other object to use.
    /// </summary>
    public bool SkipRendering
    {
        set
        {
            if (RenderCore is ISkyboxRenderParams core)
            {
                core.SkipRendering = value;
            }
        }
        get
        {
            if (RenderCore is ISkyboxRenderParams core)
            {
                return core.SkipRendering;
            }

            return false;
        }
    }

    private readonly bool UseSkyDome = false;
    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentMapNode"/> class. Default is using SkyBox. To use SkyDome, pass true into the constructor
    /// </summary>
    public EnvironmentMapNode()
    {
        RenderOrder = 1000;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentMapNode"/> class. Default is using SkyBox. To use SkyDome, pass true into the constructor
    /// </summary>
    /// <param name="useSkyDome">if set to <c>true</c> [use sky dome].</param>
    public EnvironmentMapNode(bool useSkyDome)
    {
        UseSkyDome = useSkyDome;
        RenderOrder = 1000;
    }
    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        if (UseSkyDome)
        {
            return new SkyDomeRenderCore();
        }
        else
        {
            return new SkyBoxRenderCore();
        }
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.Skybox];
    }

    public sealed override bool HitTest(HitTestContext? context, ref List<HitTestResult> hits)
    {
        return false;
    }

    protected sealed override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return false;
    }

    protected override bool CanRender(RenderContext context)
    {
        if (!base.CanRender(context))
        {
            if (context.SharedResource is not null)
            {
                context.SharedResource.EnvironementMap = null;
            }

            return false;
        }
        return true;
    }
}
