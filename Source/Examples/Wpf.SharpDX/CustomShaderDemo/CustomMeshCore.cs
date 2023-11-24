using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core;

namespace CustomShaderDemo;

public class CustomMeshCore : MeshRenderCore
{
    private float dataHeightScale = 5;
    public float DataHeightScale
    {
        set
        {
            SetAffectsRender(ref dataHeightScale, value);
        }
        get { return dataHeightScale; }
    }

    protected override void OnUpdatePerModelStruct(RenderContext context)
    {
        base.OnUpdatePerModelStruct(context);

        modelStruct.Params.Y = dataHeightScale;
    }
}
