using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.ShaderManager;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Default PhongMaterial Variables
/// </summary>
public class PhongMaterialVariables : MaterialVariable
{
    private const int NUMTEXTURES = 6;

    private const int DiffuseIdx = 0,
        AlphaIdx = 1,
        NormalIdx = 2,
        DisplaceIdx = 3,
        SpecularColorIdx = 4,
        EmissiveIdx = 5;

    private readonly ITextureResourceManager? textureManager;
    private readonly IStatePoolManager? statePoolManager;
    private readonly ShaderResourceViewProxy?[] textureResources = new ShaderResourceViewProxy?[NUMTEXTURES];
    private SamplerStateProxy? surfaceSampler, displacementSampler, shadowSampler;

    private int texDiffuseSlot, texAlphaSlot, texNormalSlot, texDisplaceSlot, texShadowSlot,
        texSpecularSlot, texEmissiveSlot, texSSAOSlot, texEnvironmentSlot;
    private int samplerDiffuseSlot, samplerDisplaceSlot, samplerShadowSlot;
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
    public ShaderPass TessellationPass
    {
        get;
    }
    public ShaderPass TessellationOITPass
    {
        get;
    }
    public ShaderPass TessellationOITDPPass
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
    public string ShaderAlphaTexName { get; } = DefaultBufferNames.AlphaMapTB;
    /// <summary>
    /// 
    /// </summary>
    public string ShaderDiffuseTexName { get; } = DefaultBufferNames.DiffuseMapTB;
    /// <summary>
    /// 
    /// </summary>
    public string ShaderNormalTexName { get; } = DefaultBufferNames.NormalMapTB;
    /// <summary>
    /// 
    /// </summary>
    public string ShaderDisplaceTexName { get; } = DefaultBufferNames.DisplacementMapTB;
    /// <summary>
    /// Gets or sets the name of the shader shadow tex.
    /// </summary>
    /// <value>
    /// The name of the shader shadow tex.
    /// </value>
    public string ShaderShadowTexName { get; } = DefaultBufferNames.ShadowMapTB;
    /// <summary>
    /// Gets the shader specular texture.
    /// </summary>
    /// <value>
    /// The shader specular texture.
    /// </value>
    public string ShaderSpecularTexName { get; } = DefaultBufferNames.SpecularTB;
    /// <summary>
    /// Gets the name of the shader emissive tex.
    /// </summary>
    /// <value>
    /// The name of the shader emissive tex.
    /// </value>
    public string ShaderEmissiveTexName { get; } = DefaultBufferNames.EmissiveTB;
    /// <summary>
    /// 
    /// </summary>
    public string ShaderSamplerDiffuseTexName { get; } = DefaultSamplerStateNames.SurfaceSampler;
    /// <summary>
    /// 
    /// </summary>
    public string ShaderSamplerDisplaceTexName { get; } = DefaultSamplerStateNames.DisplacementMapSampler;
    /// <summary>
    /// 
    /// </summary>
    public string ShaderSamplerShadowMapName { get; } = DefaultSamplerStateNames.ShadowMapSampler;

    private bool enableTessellation = false;
    public bool EnableTessellation
    {
        private set
        {
            if (Set(ref enableTessellation, value))
            {
                UpdateMappings(CurrentMaterialPass);
                InvalidateRenderer();
            }
        }
        get
        {
            return enableTessellation;
        }
    }

    private readonly PhongMaterialCore material;
    private ShaderPass CurrentMaterialPass => EnableTessellation ? TessellationPass : MaterialPass;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhongMaterialVariables"/> class.
    /// </summary>
    /// <param name="manager">The manager.</param>
    /// <param name="technique">The technique.</param>
    /// <param name="materialCore">The material core.</param>
    /// <param name="defaultPassName">Default pass name</param>
    public PhongMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore materialCore,
        string defaultPassName = DefaultPassNames.Default)
        : base(manager, technique, DefaultMeshConstantBufferDesc, materialCore)
    {
        this.material = materialCore;
        texDiffuseSlot = texAlphaSlot = texDisplaceSlot = texNormalSlot = -1;
        samplerDiffuseSlot = samplerDisplaceSlot = samplerShadowSlot = -1;
        textureManager = manager.MaterialTextureManager;
        statePoolManager = manager.StateManager;

        MaterialPass = technique[defaultPassName];
        OITPass = technique[DefaultPassNames.OITPass];
        OITDepthPeelingInit = technique[DefaultPassNames.OITDepthPeelingInit];
        OITDepthPeeling = technique[DefaultPassNames.OITDepthPeeling];
        ShadowPass = technique[DefaultPassNames.ShadowPass];
        WireframePass = technique[DefaultPassNames.Wireframe];
        WireframeOITPass = technique[DefaultPassNames.WireframeOITPass];
        WireframeOITDPPass = technique[DefaultPassNames.WireframeOITDPPass];
        TessellationPass = technique[DefaultPassNames.MeshTriTessellation];
        TessellationOITPass = technique[DefaultPassNames.MeshTriTessellationOIT];
        TessellationOITDPPass = technique[DefaultPassNames.MeshPBRTriTessellationOITDP];
        DepthPass = technique[DefaultPassNames.DepthPrepass];
        UpdateMappings(MaterialPass);
        EnableTessellation = materialCore.EnableTessellation;
    }

    protected override void OnInitialPropertyBindings()
    {
        AddPropertyBinding(nameof(PhongMaterialCore.DiffuseColor), () => { WriteValue(PhongPBRMaterialStruct.DiffuseStr, material.DiffuseColor); });
        AddPropertyBinding(nameof(PhongMaterialCore.AmbientColor), () => { WriteValue(PhongPBRMaterialStruct.AmbientStr, material.AmbientColor); });
        AddPropertyBinding(nameof(PhongMaterialCore.EmissiveColor), () => { WriteValue(PhongPBRMaterialStruct.EmissiveStr, material.EmissiveColor); });
        AddPropertyBinding(nameof(PhongMaterialCore.ReflectiveColor), () => { WriteValue(PhongPBRMaterialStruct.ReflectStr, material.ReflectiveColor); });
        AddPropertyBinding(nameof(PhongMaterialCore.SpecularColor), () => { WriteValue(PhongPBRMaterialStruct.SpecularStr, material.SpecularColor); });
        AddPropertyBinding(nameof(PhongMaterialCore.SpecularShininess), () => { WriteValue(PhongPBRMaterialStruct.ShininessStr, material.SpecularShininess); });
        AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMapScaleMask), () => { WriteValue(PhongPBRMaterialStruct.DisplacementMapScaleMaskStr, material.DisplacementMapScaleMask); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderShadowMap), () => { WriteValue(PhongPBRMaterialStruct.RenderShadowMapStr, material.RenderShadowMap ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderEnvironmentMap), () => { WriteValue(PhongPBRMaterialStruct.HasCubeMapStr, material.RenderEnvironmentMap ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.UVTransform), () =>
        {
            Matrix m = material.UVTransform;
            WriteValue(PhongPBRMaterialStruct.UVTransformR1Str, m.Column1);
            WriteValue(PhongPBRMaterialStruct.UVTransformR2Str, m.Column2);
        });
        AddPropertyBinding(nameof(PhongMaterialCore.EnableAutoTangent), () => { WriteValue(PhongPBRMaterialStruct.EnableAutoTangent, material.EnableAutoTangent); });
        AddPropertyBinding(nameof(PhongMaterialCore.MaxTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance); });
        AddPropertyBinding(nameof(PhongMaterialCore.MaxDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor); });
        AddPropertyBinding(nameof(PhongMaterialCore.MinTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance); });
        AddPropertyBinding(nameof(PhongMaterialCore.MinDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderDiffuseMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && textureResources[DiffuseIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderDiffuseAlphaMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseAlphaMapStr, material.RenderDiffuseAlphaMap && textureResources[AlphaIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderNormalMap), () => { WriteValue(PhongPBRMaterialStruct.HasNormalMapStr, material.RenderNormalMap && textureResources[NormalIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderSpecularColorMap), () => { WriteValue(PhongPBRMaterialStruct.HasSpecularColorMap, material.RenderSpecularColorMap && textureResources[SpecularColorIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderDisplacementMap), () => { WriteValue(PhongPBRMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && textureResources[DisplaceIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.RenderEmissiveMap), () => { WriteValue(PhongPBRMaterialStruct.HasEmissiveMapStr, material.RenderEmissiveMap && textureResources[EmissiveIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PhongMaterialCore.EnableFlatShading), () => { WriteValue(PhongPBRMaterialStruct.RenderFlat, material.EnableFlatShading); });
        AddPropertyBinding(nameof(PhongMaterialCore.VertexColorBlendingFactor), () => { WriteValue(PhongPBRMaterialStruct.VertColorBlending, material.VertexColorBlendingFactor); });
        AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMap), () =>
        {
            CreateTextureView(material.DiffuseMap, DiffuseIdx);
            TriggerPropertyAction(nameof(PhongMaterialCore.RenderDiffuseMap));
        });
        AddPropertyBinding(nameof(PhongMaterialCore.DiffuseAlphaMap), () =>
        {
            CreateTextureView(material.DiffuseAlphaMap, AlphaIdx);
            TriggerPropertyAction(nameof(PhongMaterialCore.RenderDiffuseAlphaMap));
        });

        AddPropertyBinding(nameof(PhongMaterialCore.NormalMap), () =>
        {
            CreateTextureView(material.NormalMap, NormalIdx);
            TriggerPropertyAction(nameof(PhongMaterialCore.RenderNormalMap));
        });
        AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMap), () =>
        {
            CreateTextureView(material.DisplacementMap, DisplaceIdx);
            TriggerPropertyAction(nameof(PhongMaterialCore.RenderDisplacementMap));
        });
        AddPropertyBinding(nameof(PhongMaterialCore.SpecularColorMap), () =>
        {
            CreateTextureView(material.SpecularColorMap, SpecularColorIdx);
            TriggerPropertyAction(nameof(PhongMaterialCore.RenderSpecularColorMap));
        });
        AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMapSampler), () =>
        {
            var newSampler = statePoolManager?.Register(material.DiffuseMapSampler);
            RemoveAndDispose(ref surfaceSampler);
            surfaceSampler = newSampler;
        });

        AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMapSampler), () =>
        {
            var newDisplaceSampler = statePoolManager?.Register(material.DisplacementMapSampler);
            RemoveAndDispose(ref displacementSampler);
            displacementSampler = newDisplaceSampler;
        });
        AddPropertyBinding(nameof(PhongMaterialCore.EmissiveMap), () =>
        {
            CreateTextureView(material.EmissiveMap, EmissiveIdx);
            TriggerPropertyAction(nameof(PhongMaterialCore.RenderEmissiveMap));
        });
        AddPropertyBinding(nameof(PhongMaterialCore.EnableTessellation), () => { EnableTessellation = material.EnableTessellation; });

        shadowSampler = statePoolManager?.Register(DefaultSamplers.ShadowSampler);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateTextureView(TextureModel? textureModel, int index)
    {
        var newTexture = textureModel == null ? null : textureManager?.Register(textureModel);
        RemoveAndDispose(ref textureResources[index]);
        textureResources[index] = newTexture;
        if (textureResources[index] != null)
        {
            textureIndex |= 1u << index;
        }
        else
        {
            textureIndex &= ~(1u << index);
        }
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        if (HasTextures)
        {
            OnBindMaterialTextures(deviceContext, shaderPass.VertexShader);
            OnBindMaterialTextures(deviceContext, shaderPass.DomainShader);
            OnBindMaterialTextures(context, deviceContext, shaderPass.PixelShader);
        }
        if (material.RenderShadowMap && context.IsShadowMapEnabled)
        {
            shaderPass.PixelShader.BindTexture(deviceContext, texShadowSlot, context.SharedResource?.ShadowView);
            shaderPass.PixelShader.BindSampler(deviceContext, samplerShadowSlot, shadowSampler);
        }
        shaderPass.PixelShader.BindTexture(deviceContext, texSSAOSlot, context.SharedResource?.SSAOMap);
        shaderPass.PixelShader.BindTexture(deviceContext, texEnvironmentSlot, context.SharedResource?.EnvironementMap);
        return true;
    }

    /// <summary>
    /// Actual bindings
    /// </summary>
    /// <param name="context"></param>
    /// <param name="shader"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnBindMaterialTextures(DeviceContextProxy context, VertexShader shader)
    {
        if (shader.IsNULL)
        {
            return;
        }
        var idx = shader.ShaderStageIndex;
        shader.BindTexture(context, texDisplaceSlot, textureResources[DisplaceIdx]);
        shader.BindSampler(context, samplerDisplaceSlot, displacementSampler);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnBindMaterialTextures(DeviceContextProxy context, DomainShader shader)
    {
        if (shader.IsNULL)
        {
            return;
        }
        var idx = shader.ShaderStageIndex;
        shader.BindTexture(context, texDisplaceSlot, textureResources[DisplaceIdx]);
        shader.BindSampler(context, samplerDisplaceSlot, displacementSampler);
    }
    /// <summary>
    /// Actual bindings
    /// </summary>
    /// <param name="context"></param>
    /// <param name="deviceContext"></param>
    /// <param name="shader"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, PixelShader shader)
    {
        if (shader.IsNULL)
        {
            return;
        }
        var idx = shader.ShaderStageIndex;
        shader.BindTexture(deviceContext, texDiffuseSlot, textureResources[DiffuseIdx]);
        shader.BindTexture(deviceContext, texNormalSlot, textureResources[NormalIdx]);
        shader.BindTexture(deviceContext, texAlphaSlot, textureResources[AlphaIdx]);
        shader.BindTexture(deviceContext, texSpecularSlot, textureResources[SpecularColorIdx]);
        shader.BindTexture(deviceContext, texEmissiveSlot, textureResources[EmissiveIdx]);
        shader.BindSampler(deviceContext, samplerDiffuseSlot, surfaceSampler);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMappings(ShaderPass shaderPass)
    {
        texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
        texAlphaSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderAlphaTexName);
        texNormalSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderNormalTexName);
        texShadowSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowTexName);
        texSpecularSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderSpecularTexName);
        texEmissiveSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderEmissiveTexName);
        texSSAOSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAOMapTB);
        texEnvironmentSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.CubeMapTB);
        samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
        samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
        if (!shaderPass.DomainShader.IsNULL && material.EnableTessellation)
        {
            texDisplaceSlot = shaderPass.DomainShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DisplacementMapTB);
            samplerDisplaceSlot = shaderPass.DomainShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DisplacementMapSampler);
        }
        else
        {
            texDisplaceSlot = shaderPass.VertexShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DisplacementMapTB);
            samplerDisplaceSlot = shaderPass.VertexShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DisplacementMapSampler);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposeManagedResources"></param>
    protected override void OnDispose(bool disposeManagedResources)
    {
        for (var i = 0; i < textureResources.Length; ++i)
        {
            RemoveAndDispose(ref textureResources[i]);
        }
        RemoveAndDispose(ref surfaceSampler);
        RemoveAndDispose(ref displacementSampler);
        RemoveAndDispose(ref shadowSampler);
        base.OnDispose(disposeManagedResources);
    }

    public override ShaderPass GetPass(RenderType renderType, RenderContext context)
    {
        if (renderType == RenderType.Transparent)
        {
            switch (context.OITRenderStage)
            {
                case OITRenderStage.SinglePassWeighted:
                    return EnableTessellation ? TessellationOITPass : OITPass;
                case OITRenderStage.DepthPeelingInitMinMaxZ:
                    return OITDepthPeelingInit;
                case OITRenderStage.DepthPeeling:
                    return EnableTessellation ? TessellationOITDPPass : OITDepthPeeling;
                default:
                    break;
            }
        }
        return CurrentMaterialPass;
    }

    public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
    {
        return ShadowPass;
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
