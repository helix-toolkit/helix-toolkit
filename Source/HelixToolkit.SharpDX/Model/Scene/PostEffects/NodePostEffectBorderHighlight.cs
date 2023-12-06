using HelixToolkit.SharpDX.Core;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class NodePostEffectBorderHighlight : NodePostEffectMeshOutlineBlur
{
    /// <summary>
    /// Gets or sets the draw mode.
    /// </summary>
    /// <value>
    /// The draw mode.
    /// </value>
    public OutlineMode DrawMode
    {
        set
        {
            if (RenderCore is PostEffectMeshOutlineBlurCore core)
            {
                core.DrawMode = value;
            }
        }
        get
        {
            if (RenderCore is PostEffectMeshOutlineBlurCore core)
            {
                return core.DrawMode;
            }

            return OutlineMode.Merged;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NodePostEffectBorderHighlight"/> class.
    /// </summary>
    public NodePostEffectBorderHighlight()
    {
        EffectName = DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight;
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight];
    }

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new PostEffectMeshOutlineBlurCore(false);
    }
}
