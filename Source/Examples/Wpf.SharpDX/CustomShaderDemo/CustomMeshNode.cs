using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Model.Scene;

namespace CustomShaderDemo;

public class CustomMeshNode : MeshNode
{
    public float HeightScale
    {
        set
        {
            if (RenderCore is CustomMeshCore mesh)
            {
                mesh.DataHeightScale = value;
            }
        }

        get
        {
            if (RenderCore is CustomMeshCore mesh)
            {
                return mesh.DataHeightScale;
            }

            return 1.0f;
        }
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new CustomMeshCore();
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[CustomShaderNames.DataSampling];
    }
}
