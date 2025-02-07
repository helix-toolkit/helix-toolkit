﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Model;

public class BillboardMaterialVariable : MaterialVariable
{
    /// <summary>
    /// Set texture variable name insider shader for binding
    /// </summary>
    public string ShaderTextureName { get; } = DefaultBufferNames.BillboardTB;
    /// <summary>
    /// Set texture sampler variable name inside shader for binding
    /// </summary>
    public string ShaderTextureSamplerName { get; } = DefaultSamplerStateNames.BillboardTextureSampler;

    public ShaderPass BillboardPass
    {
        get;
    }

    public ShaderPass OITPass
    {
        get;
    }

    public ShaderPass OITDepthPeelingInit
    {
        get;
    }

    public ShaderPass OITDepthPeeling
    {
        get;
    }

    #region Private Variables
    private readonly int textureSamplerSlot;
    private readonly int shaderTextureSlot;
    private SamplerStateProxy? textureSampler;
    private readonly BillboardMaterialCore materialCore;
    #endregion
    /// <summary>
    /// Initializes a new instance of the <see cref="BillboardMaterialVariable"/> class.
    /// </summary>
    /// <param name="manager">The manager.</param>
    /// <param name="technique">The technique.</param>
    /// <param name="materialCore">The core.</param>
    /// <param name="defaultPassName">Default pass name</param>
    public BillboardMaterialVariable(IEffectsManager manager, IRenderTechnique technique, BillboardMaterialCore materialCore,
        string defaultPassName = DefaultPassNames.Default)
        : base(manager, technique, DefaultPointLineConstantBufferDesc, materialCore)
    {
        BillboardPass = technique[defaultPassName];
        OITPass = technique[DefaultPassNames.OITPass];
        OITDepthPeelingInit = technique[DefaultPassNames.OITDepthPeelingInit];
        OITDepthPeeling = technique[DefaultPassNames.OITDepthPeeling];
        this.materialCore = materialCore;
        shaderTextureSlot = BillboardPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureName);
        textureSamplerSlot = BillboardPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
        textureSampler = EffectsManager?.StateManager?.Register(materialCore.SamplerDescription);
    }

    protected override void OnInitialPropertyBindings()
    {
        base.OnInitialPropertyBindings();
        AddPropertyBinding(nameof(BillboardMaterialCore.FixedSize), () => { WriteValue(PointLineMaterialStruct.FixedSize, materialCore.FixedSize); });
        AddPropertyBinding(nameof(BillboardMaterialCore.Type), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4((int)materialCore.Type, 0, 0, 0)); });
        AddPropertyBinding(nameof(BillboardMaterialCore.SamplerDescription), () =>
        {
            var newSampler = EffectsManager?.StateManager?.Register(materialCore.SamplerDescription);
            RemoveAndDispose(ref textureSampler);
            textureSampler = newSampler;
        });
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        shaderPass.PixelShader.BindSampler(deviceContext, textureSamplerSlot, textureSampler);
        return true;
    }

    public override ShaderPass GetPass(RenderType renderType, RenderContext context)
    {
        if (renderType == RenderType.Transparent)
        {
            switch (context.OITRenderStage)
            {
                case OITRenderStage.SinglePassWeighted:
                    return OITPass;
                case OITRenderStage.DepthPeelingInitMinMaxZ:
                    return OITDepthPeelingInit;
                case OITRenderStage.DepthPeeling:
                    return OITDepthPeeling;
                default:
                    break;
            }
        }
        return BillboardPass;
    }

    public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
    {
        return ShaderPass.NullPass;
    }

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel is IBillboardBufferModel billboardModel && bufferModel.VertexBuffer[0] is not null)
        {
            deviceContext.SetShaderResource(PixelShader.Type, shaderTextureSlot, billboardModel.TextureView);
            DrawPoints(deviceContext, bufferModel.VertexBuffer[0]!.ElementCount, instanceCount);
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref textureSampler);
        base.OnDispose(disposeManagedResources);
    }
}
