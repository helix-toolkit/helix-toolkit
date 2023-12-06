using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.ShaderManager;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
using DomainShader = HelixToolkit.SharpDX.Shaders.DomainShader;
using PixelShader = HelixToolkit.SharpDX.Shaders.PixelShader;
using VertexShader = HelixToolkit.SharpDX.Shaders.VertexShader;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Physics based rendering material
/// </summary>
public class PBRMaterialVariable : MaterialVariable
{
    private const int NUMTEXTURES = 7;
    private const int NUMSAMPLERS = 4;
    private const int AlbedoMapIdx = 0, NormalMapIdx = 1, RMMapIdx = 2, EmissiveMapIdx = 3,
        IrradianceMapIdx = 4, DisplaceMapIdx = 5, AOMapIdx = 6;
    private const int SurfaceSamplerIdx = 0, IBLSamplerIdx = 1, ShadowSamplerIdx = 2, DisplaceSamplerIdx = 3;
    private readonly ITextureResourceManager? textureManager;
    private readonly IStatePoolManager? statePoolManager;
    private readonly ShaderResourceViewProxy?[] TextureResources = new ShaderResourceViewProxy?[NUMTEXTURES];
    private readonly SamplerStateProxy?[] SamplerResources = new SamplerStateProxy?[NUMSAMPLERS];

    private int texDiffuseSlot, texNormalSlot, texRMSlot, texEmissiveSlot, texIrradianceSlot,
        texDisplaceSlot, texShadowSlot, texAOSlot, texSSAOSlot, texEnvironmentSlot;
    private int samplerSurfaceSlot, samplerIBLSlot, samplerShadowSlot, samplerDisplaceSlot;
    private uint textureIndex = 0;

    private bool HasTextures
    {
        get
        {
            return textureIndex != 0;
        }
    }

    private readonly PBRMaterialCore material;

    public ShaderPass MaterialPass
    {
        private set; get;
    }
    public ShaderPass OITPass
    {
        private set; get;
    }
    public ShaderPass OITDepthPeelingInit
    {
        get;
    }

    public ShaderPass OITDepthPeeling
    {
        get;
    }
    public ShaderPass TessellationPass
    {
        private set; get;
    }
    public ShaderPass TessellationOITPass
    {
        private set; get;
    }
    public ShaderPass TessellationOITDPPass
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
    private ShaderPass CurrentMaterialPass => EnableTessellation ? TessellationPass : MaterialPass;

    public PBRMaterialVariable(IEffectsManager manager, IRenderTechnique technique, PBRMaterialCore core,
        string defaultPassName = DefaultPassNames.PBR)
        : base(manager, technique, DefaultMeshConstantBufferDesc, core)
    {
        textureManager = manager.MaterialTextureManager;
        statePoolManager = manager.StateManager;
        material = core;
        MaterialPass = technique[defaultPassName];
        OITPass = technique[DefaultPassNames.PBROITPass];
        OITDepthPeelingInit = technique[DefaultPassNames.OITDepthPeelingInit];
        OITDepthPeeling = technique[DefaultPassNames.PBROITDPPass];
        TessellationPass = technique[DefaultPassNames.MeshPBRTriTessellation];
        TessellationOITPass = technique[DefaultPassNames.MeshPBRTriTessellationOIT];
        TessellationOITDPPass = technique[DefaultPassNames.MeshPBRTriTessellationOITDP];
        WireframePass = technique[DefaultPassNames.Wireframe];
        WireframeOITPass = technique[DefaultPassNames.WireframeOITPass];
        WireframeOITDPPass = technique[DefaultPassNames.WireframeOITDPPass];
        ShadowPass = technique[DefaultPassNames.ShadowPass];
        DepthPass = technique[DefaultPassNames.DepthPrepass];
        UpdateMappings(MaterialPass);
        CreateTextureViews();
        CreateSamplers();
    }

    protected override void OnInitialPropertyBindings()
    {
        AddPropertyBinding(nameof(PBRMaterialCore.AlbedoColor), () => { WriteValue(PhongPBRMaterialStruct.DiffuseStr, material.AlbedoColor); });
        AddPropertyBinding(nameof(PBRMaterialCore.EmissiveColor), () => { WriteValue(PhongPBRMaterialStruct.EmissiveStr, material.EmissiveColor); });
        AddPropertyBinding(nameof(PBRMaterialCore.MetallicFactor), () => { WriteValue(PhongPBRMaterialStruct.ConstantMetallic, material.MetallicFactor); });
        AddPropertyBinding(nameof(PBRMaterialCore.RoughnessFactor), () => { WriteValue(PhongPBRMaterialStruct.RoughnessStr, material.RoughnessFactor); });
        AddPropertyBinding(nameof(PBRMaterialCore.AmbientOcclusionFactor), () => { WriteValue(PhongPBRMaterialStruct.AmbientOcclusionStr, material.AmbientOcclusionFactor); });
        AddPropertyBinding(nameof(PBRMaterialCore.ReflectanceFactor), () => { WriteValue(PhongPBRMaterialStruct.ReflectanceStr, material.ReflectanceFactor); });

        AddPropertyBinding(nameof(PBRMaterialCore.ClearCoatStrength), () => { WriteValue(PhongPBRMaterialStruct.ClearCoatStr, material.ClearCoatStrength); });

        AddPropertyBinding(nameof(PBRMaterialCore.ClearCoatRoughness), () => { WriteValue(PhongPBRMaterialStruct.ClearCoatRoughnessStr, material.ClearCoatRoughness); });

        AddPropertyBinding(nameof(PBRMaterialCore.RenderAlbedoMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseMapStr, material.RenderAlbedoMap && TextureResources[AlbedoMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderEmissiveMap), () => { WriteValue(PhongPBRMaterialStruct.HasEmissiveMapStr, material.RenderEmissiveMap && TextureResources[EmissiveMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderNormalMap), () => { WriteValue(PhongPBRMaterialStruct.HasNormalMapStr, material.RenderNormalMap && TextureResources[NormalMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderDisplacementMap), () => { WriteValue(PhongPBRMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && TextureResources[DisplaceMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderIrradianceMap), () => { WriteValue(PhongPBRMaterialStruct.HasIrradianceMapStr, material.RenderIrradianceMap && TextureResources[IrradianceMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderRoughnessMetallicMap), () => { WriteValue(PhongPBRMaterialStruct.HasRMMapStr, material.RenderRoughnessMetallicMap && TextureResources[RMMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderAmbientOcclusionMap), () => { WriteValue(PhongPBRMaterialStruct.HasAOMapStr, material.RenderAmbientOcclusionMap && TextureResources[AOMapIdx] != null ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.EnableAutoTangent), () => { WriteValue(PhongPBRMaterialStruct.EnableAutoTangent, material.EnableAutoTangent); });
        AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMapScaleMask), () => { WriteValue(PhongPBRMaterialStruct.DisplacementMapScaleMaskStr, material.DisplacementMapScaleMask); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderShadowMap), () => { WriteValue(PhongPBRMaterialStruct.RenderShadowMapStr, material.RenderShadowMap ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.RenderEnvironmentMap), () => { WriteValue(PhongPBRMaterialStruct.HasCubeMapStr, material.RenderEnvironmentMap ? 1 : 0); });
        AddPropertyBinding(nameof(PBRMaterialCore.MaxTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance); });
        AddPropertyBinding(nameof(PBRMaterialCore.MinTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance); });
        AddPropertyBinding(nameof(PBRMaterialCore.MaxDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor); });
        AddPropertyBinding(nameof(PBRMaterialCore.MinDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor); });
        AddPropertyBinding(nameof(PBRMaterialCore.UVTransform), () =>
        {
            Matrix m = material.UVTransform;
            WriteValue(PhongPBRMaterialStruct.UVTransformR1Str, m.Column1);
            WriteValue(PhongPBRMaterialStruct.UVTransformR2Str, m.Column2);
        });
        AddPropertyBinding(nameof(PBRMaterialCore.AlbedoMap), () => { CreateTextureView(material.AlbedoMap, AlbedoMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderAlbedoMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.EmissiveMap), () => { CreateTextureView(material.EmissiveMap, EmissiveMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderEmissiveMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.NormalMap), () => { CreateTextureView(material.NormalMap, NormalMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderNormalMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.IrradianceMap), () => { CreateTextureView(material.IrradianceMap, IrradianceMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderIrradianceMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMap), () => { CreateTextureView(material.DisplacementMap, DisplaceMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderDisplacementMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.RoughnessMetallicMap), () => { CreateTextureView(material.RoughnessMetallicMap, RMMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderRoughnessMetallicMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.AmbientOcculsionMap), () => { CreateTextureView(material.AmbientOcculsionMap, AOMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderAmbientOcclusionMap)); });
        AddPropertyBinding(nameof(PBRMaterialCore.SurfaceMapSampler), () => { CreateSampler(material.SurfaceMapSampler, SurfaceSamplerIdx); });
        AddPropertyBinding(nameof(PBRMaterialCore.IBLSampler), () => { CreateSampler(material.IBLSampler, IBLSamplerIdx); });
        AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMapSampler), () => { CreateSampler(material.DisplacementMapSampler, DisplaceSamplerIdx); });
        AddPropertyBinding(nameof(PBRMaterialCore.EnableTessellation), () =>
        {
            EnableTessellation = material.EnableTessellation;
        });

        WriteValue(PhongPBRMaterialStruct.RenderPBR, true); // Make sure to set this flag
        AddPropertyBinding(nameof(PBRMaterialCore.EnableFlatShading), () => { WriteValue(PhongPBRMaterialStruct.RenderFlat, material.EnableFlatShading); });
        AddPropertyBinding(nameof(PBRMaterialCore.VertexColorBlendingFactor), () => { WriteValue(PhongPBRMaterialStruct.VertColorBlending, material.VertexColorBlendingFactor); });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateTextureView(TextureModel? texture, int index)
    {
        var newTexture = texture == null ? null : textureManager?.Register(texture);
        RemoveAndDispose(ref TextureResources[index]);
        TextureResources[index] = newTexture;
        if (TextureResources[index] != null)
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
            CreateTextureView(material.AlbedoMap, AlbedoMapIdx);
            CreateTextureView(material.NormalMap, NormalMapIdx);
            CreateTextureView(material.DisplacementMap, DisplaceMapIdx);
            CreateTextureView(material.EmissiveMap, EmissiveMapIdx);
            CreateTextureView(material.IrradianceMap, IrradianceMapIdx);
            CreateTextureView(material.RoughnessMetallicMap, RMMapIdx);
            CreateTextureView(material.AmbientOcculsionMap, AOMapIdx);
        }
        else
        {
            for (var i = 0; i < NUMTEXTURES; ++i)
            {
                RemoveAndDispose(ref TextureResources[i]);
            }
            textureIndex = 0;
        }
    }

    private void CreateSamplers()
    {
        var newSurfaceSampler = statePoolManager?.Register(material.SurfaceMapSampler);
        var newIBLSampler = statePoolManager?.Register(material.IBLSampler);
        var newDisplaceSampler = statePoolManager?.Register(material.DisplacementMapSampler);
        var newShadowSampler = statePoolManager?.Register(DefaultSamplers.ShadowSampler);
        RemoveAndDispose(ref SamplerResources[SurfaceSamplerIdx]);
        RemoveAndDispose(ref SamplerResources[IBLSamplerIdx]);
        RemoveAndDispose(ref SamplerResources[DisplaceSamplerIdx]);
        RemoveAndDispose(ref SamplerResources[ShadowSamplerIdx]);
        if (material != null)
        {
            SamplerResources[SurfaceSamplerIdx] = newSurfaceSampler;
            SamplerResources[IBLSamplerIdx] = newIBLSampler;
            SamplerResources[DisplaceSamplerIdx] = newDisplaceSampler;
            SamplerResources[ShadowSamplerIdx] = newShadowSampler;
        }
    }

    private void CreateSampler(SamplerStateDescription desc, int index)
    {
        var newRes = statePoolManager?.Register(desc);
        RemoveAndDispose(ref SamplerResources[index]);
        SamplerResources[index] = newRes;
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
            shaderPass.PixelShader.BindSampler(deviceContext, samplerShadowSlot, SamplerResources[ShadowSamplerIdx]);
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
        shader.BindTexture(context, texDisplaceSlot, TextureResources[DisplaceMapIdx]);
        shader.BindSampler(context, samplerDisplaceSlot, SamplerResources[DisplaceSamplerIdx]);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnBindMaterialTextures(DeviceContextProxy context, DomainShader shader)
    {
        if (shader.IsNULL)
        {
            return;
        }
        var idx = shader.ShaderStageIndex;
        shader.BindTexture(context, texDisplaceSlot, TextureResources[DisplaceMapIdx]);
        shader.BindSampler(context, samplerDisplaceSlot, SamplerResources[DisplaceSamplerIdx]);
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
        shader.BindTexture(deviceContext, texDiffuseSlot, TextureResources[AlbedoMapIdx]);
        shader.BindTexture(deviceContext, texNormalSlot, TextureResources[NormalMapIdx]);
        shader.BindTexture(deviceContext, texRMSlot, TextureResources[RMMapIdx]);
        shader.BindTexture(deviceContext, texAOSlot, TextureResources[AOMapIdx]);
        shader.BindTexture(deviceContext, texEmissiveSlot, TextureResources[EmissiveMapIdx]);
        shader.BindTexture(deviceContext, texIrradianceSlot, TextureResources[IrradianceMapIdx]);

        shader.BindSampler(deviceContext, samplerSurfaceSlot, SamplerResources[SurfaceSamplerIdx]);
        shader.BindSampler(deviceContext, samplerIBLSlot, SamplerResources[IBLSamplerIdx]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMappings(ShaderPass shaderPass)
    {
        texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
        texEmissiveSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.EmissiveTB);
        texNormalSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.NormalMapTB);
        texRMSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.RMMapTB);
        texAOSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.AOMapTB);
        texShadowSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.ShadowMapTB);
        texIrradianceSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.IrradianceMap);
        texSSAOSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAOMapTB);
        texEnvironmentSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.CubeMapTB);
        samplerSurfaceSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler);
        samplerIBLSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.IBLSampler);
        samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.ShadowMapSampler);

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

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel.IndexBuffer is null)
        {
            return;
        }

        DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
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

    protected override void OnDispose(bool disposeManagedResources)
    {
        for (var i = 0; i < SamplerResources.Length; ++i)
        {
            RemoveAndDispose(ref SamplerResources[i]);
        }
        for (var i = 0; i < TextureResources.Length; ++i)
        {
            RemoveAndDispose(ref TextureResources[i]);
        }
        base.OnDispose(disposeManagedResources);
    }
}
