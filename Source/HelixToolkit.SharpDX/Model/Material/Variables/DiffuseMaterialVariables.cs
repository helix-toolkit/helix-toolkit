using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.ShaderManager;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public class DiffuseMaterialVariables : MaterialVariable
{
    private const int NUMTEXTURES = 1;
    private const int NUMSAMPLERS = 1;
    private const int DiffuseIdx = 0;

    private readonly ITextureResourceManager? textureManager;
    private readonly IStatePoolManager? statePoolManager;
    private ShaderResourceViewProxy? TextureResource;
    private SamplerStateProxy? SamplerResource;

    private int texDiffuseSlot;
    private int samplerDiffuseSlot, samplerShadowSlot;
    private uint textureIndex = 0;

    private bool HasTextures
    {
        get
        {
            return textureIndex != 0;
        }
    }

    public ShaderPass MaterialPass
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
    public ShaderPass ShadowPass
    {
        get;
    }
    public ShaderPass WireframePass
    {
        get;
    }
    public ShaderPass WireframeOITPass
    {
        get;
    }
    public ShaderPass WireframeOITDPPass
    {
        get;
    }
    public ShaderPass DepthPass
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    public string ShaderDiffuseTexName { get; } = DefaultBufferNames.DiffuseMapTB;

    /// <summary>
    /// 
    /// </summary>
    public string SamplerDiffuseTexName { get; } = DefaultSamplerStateNames.SurfaceSampler;
    /// <summary>
    /// 
    /// </summary>
    public string SamplerShadowMapName { get; } = DefaultSamplerStateNames.ShadowMapSampler;

    private readonly DiffuseMaterialCore material;
    /// <summary>
    /// Initializes a new instance of the <see cref="DiffuseMaterialVariables"/> class.
    /// </summary>
    /// <param name="manager">The manager.</param>
    /// <param name="technique">The technique.</param>
    /// <param name="materialCore">The material core.</param>
    /// <param name="defaultPassName"></param>
    private DiffuseMaterialVariables(IEffectsManager manager, IRenderTechnique technique, DiffuseMaterialCore materialCore,
        string defaultPassName = DefaultPassNames.Default)
        : base(manager, technique, DefaultMeshConstantBufferDesc, materialCore)
    {
        this.material = materialCore;
        texDiffuseSlot = -1;
        samplerDiffuseSlot = samplerShadowSlot = -1;
        textureManager = manager.MaterialTextureManager;
        statePoolManager = manager.StateManager;
        MaterialPass = technique[defaultPassName];
        OITPass = technique[DefaultPassNames.DiffuseOIT];
        OITDepthPeelingInit = technique[DefaultPassNames.OITDepthPeelingInit];
        OITDepthPeeling = technique[DefaultPassNames.DiffuseOITDP];
        ShadowPass = technique[DefaultPassNames.ShadowPass];
        WireframePass = technique[DefaultPassNames.Wireframe];
        WireframeOITPass = technique[DefaultPassNames.WireframeOITPass];
        WireframeOITDPPass = technique[DefaultPassNames.WireframeOITDPPass];
        DepthPass = technique[DefaultPassNames.DepthPrepass];
        UpdateMappings(MaterialPass);
        CreateTextureViews();
        CreateSamplers();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="DiffuseMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
    /// </summary>
    /// <param name="passName">Name of the pass.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="technique"></param>
    /// <param name="material">The material.</param>
    public DiffuseMaterialVariables(string passName, IEffectsManager manager, IRenderTechnique technique,
        DiffuseMaterialCore material)
        : this(manager, technique, material)
    {
        MaterialPass = technique[passName];
    }

    protected override void OnInitialPropertyBindings()
    {
        base.OnInitialPropertyBindings();
        AddPropertyBinding(nameof(DiffuseMaterialCore.DiffuseColor), () =>
        {
            WriteValue(PhongPBRMaterialStruct.DiffuseStr, material.DiffuseColor);
        });
        AddPropertyBinding(nameof(DiffuseMaterialCore.UVTransform), () =>
        {
            Matrix m = material.UVTransform;
            WriteValue(PhongPBRMaterialStruct.UVTransformR1Str, m.Column1);
            WriteValue(PhongPBRMaterialStruct.UVTransformR2Str, m.Column2);
        });
        AddPropertyBinding(nameof(DiffuseMaterialCore.DiffuseMap), () =>
        {
            CreateTextureView(material.DiffuseMap, DiffuseIdx);
            WriteValue(PhongPBRMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && TextureResource != null ? 1 : 0);
        });
        AddPropertyBinding(nameof(DiffuseMaterialCore.DiffuseMapSampler), () =>
        {
            var newSampler = statePoolManager?.Register(material.DiffuseMapSampler);
            RemoveAndDispose(ref SamplerResource);
            SamplerResource = newSampler;
        });
        AddPropertyBinding(nameof(DiffuseMaterialCore.EnableUnLit), () =>
        {
            WriteValue(PhongPBRMaterialStruct.HasNormalMapStr, material.EnableUnLit);
        });
        AddPropertyBinding(nameof(DiffuseMaterialCore.EnableFlatShading), () => { WriteValue(PhongPBRMaterialStruct.RenderFlat, material.EnableFlatShading); });
        AddPropertyBinding(nameof(DiffuseMaterialCore.VertexColorBlendingFactor), () => { WriteValue(PhongPBRMaterialStruct.VertColorBlending, material.VertexColorBlendingFactor); });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateTextureView(TextureModel? texture, int index)
    {
        var newTexture = texture == null ? null : textureManager?.Register(texture);
        RemoveAndDispose(ref TextureResource);
        TextureResource = newTexture;
        if (TextureResource != null)
        {
            textureIndex |= 1u << index;
        }
        else
        {
            textureIndex &= ~(1u << index);
        }
    }

    private void CreateTextureViews()
    {
        if (material != null)
        {
            CreateTextureView(material.DiffuseMap, DiffuseIdx);
        }
        else
        {
            RemoveAndDispose(ref TextureResource);
            textureIndex = 0;
        }
    }

    private void CreateSamplers()
    {
        var newSampler = material == null ? null : statePoolManager?.Register(material.DiffuseMapSampler);
        RemoveAndDispose(ref SamplerResource);
        SamplerResource = newSampler;
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        if (HasTextures)
        {
            OnBindMaterialTextures(deviceContext, shaderPass.PixelShader);
        }
        return true;
    }

    /// <summary>
    /// Actual bindings
    /// </summary>
    /// <param name="context"></param>
    /// <param name="shader"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnBindMaterialTextures(DeviceContextProxy context, PixelShader shader)
    {
        if (shader.IsNULL)
        {
            return;
        }
        var idx = shader.ShaderStageIndex;
        shader.BindTexture(context, texDiffuseSlot, TextureResource);
        shader.BindSampler(context, samplerDiffuseSlot, SamplerResource);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMappings(ShaderPass shaderPass)
    {
        texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
        samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(SamplerDiffuseTexName);
        samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(SamplerShadowMapName);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposeManagedResources"></param>
    protected override void OnDispose(bool disposeManagedResources)
    {
        if (disposeManagedResources)
        {
            RemoveAndDispose(ref SamplerResource);
            RemoveAndDispose(ref TextureResource);
        }

        base.OnDispose(disposeManagedResources);
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
        return MaterialPass;
    }

    public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
    {
        return ShadowPass;
    }

    public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
    {
        return DepthPass;
    }

    public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
    {
        if (renderType == RenderType.Transparent)
        {
            switch (context.OITRenderStage)
            {
                case OITRenderStage.SinglePassWeighted:
                    return WireframeOITPass;
                case OITRenderStage.DepthPeelingInitMinMaxZ:
                    return OITDepthPeelingInit;
                case OITRenderStage.DepthPeeling:
                    return WireframeOITDPPass;
                default:
                    break;
            }
        }
        return WireframePass;
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
