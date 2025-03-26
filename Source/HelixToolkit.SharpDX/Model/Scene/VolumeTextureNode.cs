using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

public class VolumeTextureNode : SceneNode
{
    private MaterialCore? material;
    /// <summary>
    ///
    /// </summary>
    public MaterialCore? Material
    {
        get
        {
            return material;
        }
        set
        {
            if (Set(ref material, value))
            {
                if (EffectsManager != null)
                {
                    if (IsAttached)
                    {
                        AttachMaterial();
                        InvalidateRender();
                    }
                    else
                    {
                        Detach();
                        Attach(EffectsManager);
                    }
                }
            }
        }
    }

    private MaterialVariable? materialVariable;

    protected override bool OnAttach(IEffectsManager effectsManager)
    {
        if (base.OnAttach(effectsManager))
        {
            AttachMaterial();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref materialVariable);
        base.OnDetach();
    }

    protected virtual void AttachMaterial()
    {
        var newVar = material != null && RenderCore is VolumeRenderCore ?
            EffectsManager?.MaterialVariableManager?.Register(material, EffectTechnique) : null;
        RemoveAndDispose(ref materialVariable);
        if (RenderCore is VolumeRenderCore core)
        {
            materialVariable = core.MaterialVariables = newVar;
        }
    }


    protected override OrderKey OnUpdateRenderOrderKey()
    {
        return OrderKey.Create(RenderOrder, materialVariable == null ? (ushort)0 : materialVariable.ID);
    }

    protected override bool CanRender(RenderContext context)
    {
        return base.CanRender(context) && materialVariable != null;
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new VolumeRenderCore() { DefaultStateBinding = StateType.All };
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.Volume3D];
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return false;
    }
}
