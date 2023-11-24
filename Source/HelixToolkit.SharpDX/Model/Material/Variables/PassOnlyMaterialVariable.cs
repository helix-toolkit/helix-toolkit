using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public sealed class PassOnlyMaterialVariable : MaterialVariable
{
    public ShaderPass MaterialPass
    {
        get;
    }

    public ShaderPass ShadowPass
    {
        get;
    }

    public ShaderPass WireframePass
    {
        get;
    }

    public ShaderPass DepthPass
    {
        get;
    }

    private readonly string passName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PassOnlyMaterialVariable"/> class.
    /// </summary>
    /// <param name="passName">Name of the pass.</param>
    /// <param name="technique">The technique.</param>
    /// <param name="shadowPassName">Name of the shadow pass.</param>
    /// <param name="wireframePassName">Name of the wireframe pass.</param>
    /// <param name="depthPassName">Name of the depth pass</param>
    public PassOnlyMaterialVariable(string passName, IRenderTechnique technique,
        string shadowPassName = DefaultPassNames.ShadowPass,
        string wireframePassName = DefaultPassNames.Wireframe,
        string depthPassName = DefaultPassNames.DepthPrepass)
        : base(technique.EffectsManager, technique, DefaultMeshConstantBufferDesc, null)
    {
        this.passName = passName;
        MaterialPass = technique[passName];
        ShadowPass = technique[shadowPassName];
        WireframePass = technique[wireframePassName];
        DepthPass = technique[depthPassName];
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        return true;
    }

    public override ShaderPass GetPass(RenderType renderType, RenderContext context)
    {
        return MaterialPass;
    }
    public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
    {
        return ShadowPass;
    }

    public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
    {
        return WireframePass;
    }

    public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
    {
        return DepthPass;
    }

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel.IndexBuffer is null)
        {
            return;
        }

        DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
    }
}
