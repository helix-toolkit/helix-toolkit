/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
using global::SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using global::SharpDX;
    using Render;
    using ShaderManager;
    using Shaders;
    using Utilities;
    /// <summary>
    /// Physics based rendering material
    /// </summary>
    public sealed class PBRMaterialVariable : MaterialVariable
    {
        private const int NUMTEXTURES = 6;
        private const int NUMSAMPLERS = 4;
        private const int AlbedoMapIdx = 0, NormalMapIdx = 1, RMAMapIdx = 2, EmissiveMapIdx = 3, IrradianceMapIdx = 4, DisplaceMapIdx = 5;
        private const int SurfaceSamplerIdx = 0, IBLSamplerIdx = 1, ShadowSamplerIdx = 2, DisplaceSamplerIdx = 3;
        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private readonly SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];

        private int texDiffuseSlot, texNormalSlot, texRMASlot, texEmissiveSlot, texIrradianceSlot, texDisplaceSlot, texShadowSlot;
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

        public ShaderPass MaterialPass { private set; get; }
        public ShaderPass MaterialOITPass { private set; get; }
        public ShaderPass ShadowPass { get; }
        public ShaderPass WireframePass { get; } 
        public ShaderPass WireframeOITPass { get; }

        private int numRadianceMipLevels = 0;

        public PBRMaterialVariable(IEffectsManager manager, IRenderTechnique technique, PBRMaterialCore core)
            : base(manager, technique, DefaultMeshConstantBufferDesc, core)
        {
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            material = core;
            MaterialPass = technique[material.EnableTessellation ? DefaultPassNames.MeshPBRTriTessellation : DefaultPassNames.PBR];
            MaterialOITPass = technique[material.EnableTessellation ? DefaultPassNames.MeshPBRTriTessellationOIT : DefaultPassNames.PBROITPass];
            WireframePass = technique[DefaultPassNames.Wireframe];
            WireframeOITPass = technique[DefaultPassNames.WireframeOITPass];
            ShadowPass = technique[DefaultPassNames.ShadowPass];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
            //EnableTessellation = material.EnableTessellation;
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
            AddPropertyBinding(nameof(PBRMaterialCore.RenderRMAMap), () => { WriteValue(PhongPBRMaterialStruct.HasRMAMapStr, material.RenderRMAMap && TextureResources[RMAMapIdx] != null ? 1 : 0); });
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
                WriteValue(PhongPBRMaterialStruct.UVTransformR1Str, material.UVTransform.Column1);
                WriteValue(PhongPBRMaterialStruct.UVTransformR2Str, material.UVTransform.Column2);
            });
            AddPropertyBinding(nameof(PBRMaterialCore.AlbedoMap), () => { CreateTextureView(material.AlbedoMap, AlbedoMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderAlbedoMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.EmissiveMap), () => { CreateTextureView(material.EmissiveMap, EmissiveMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderEmissiveMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.NormalMap), () => { CreateTextureView(material.NormalMap, NormalMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderNormalMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.IrradianceMap), () => { CreateTextureView(material.IrradianceMap, IrradianceMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderIrradianceMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMap), () => { CreateTextureView(material.DisplacementMap, DisplaceMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderDisplacementMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.RMAMap), () => { CreateTextureView(material.RMAMap, RMAMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderRMAMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.SurfaceMapSampler), () => { CreateSampler(material.SurfaceMapSampler, SurfaceSamplerIdx); });
            AddPropertyBinding(nameof(PBRMaterialCore.IBLSampler), () => { CreateSampler(material.IBLSampler, IBLSamplerIdx); });
            AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMapSampler), () => { CreateSampler(material.DisplacementMapSampler, DisplaceSamplerIdx); });
            AddPropertyBinding(nameof(PBRMaterialCore.EnableTessellation), () =>
            {
                MaterialPass = Technique[material.EnableTessellation ? DefaultPassNames.MeshPBRTriTessellation : DefaultPassNames.PBR];
                MaterialOITPass = Technique[material.EnableTessellation ? DefaultPassNames.MeshPBRTriTessellationOIT : DefaultPassNames.PBROITPass];
            });

            WriteValue(PhongPBRMaterialStruct.RenderPBR, true); // Make sure to set this flag
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateTextureView(System.IO.Stream stream, int index)
        {
            RemoveAndDispose(ref TextureResources[index]);
            TextureResources[index] = stream == null ? null : Collect(textureManager.Register(stream));
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
                CreateTextureView(material.RMAMap, RMAMapIdx);
            }
            else
            {
                for (int i = 0; i < NUMTEXTURES; ++i)
                {
                    RemoveAndDispose(ref TextureResources[i]);
                }
                textureIndex = 0;
            }
        }

        private void CreateSamplers()
        {
            RemoveAndDispose(ref SamplerResources[SurfaceSamplerIdx]);
            RemoveAndDispose(ref SamplerResources[IBLSamplerIdx]);
            RemoveAndDispose(ref SamplerResources[ShadowSamplerIdx]);
            if (material != null)
            {
                SamplerResources[SurfaceSamplerIdx] = Collect(statePoolManager.Register(material.SurfaceMapSampler));
                SamplerResources[IBLSamplerIdx] = Collect(statePoolManager.Register(material.IBLSampler));
                SamplerResources[DisplaceSamplerIdx] = Collect(statePoolManager.Register(material.DisplacementMapSampler));
                SamplerResources[ShadowSamplerIdx] = Collect(statePoolManager.Register(DefaultSamplers.ShadowSampler));
            }
        }

        private void CreateSampler(SamplerStateDescription desc, int index)
        {
            RemoveAndDispose(ref SamplerResources[index]);
            if (material != null)
            {
                SamplerResources[index] = Collect(statePoolManager.Register(desc));
            }
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if(numRadianceMipLevels != context.SharedResource.EnvironmentMapMipLevels)
            {
                numRadianceMipLevels = context.SharedResource.EnvironmentMapMipLevels;
                WriteValue(PhongPBRMaterialStruct.NumRadianceMipLevels, numRadianceMipLevels);
                InvalidateRenderer();
            }
            if (HasTextures)
            {
                OnBindMaterialTextures(deviceContext, shaderPass.VertexShader);
                OnBindMaterialTextures(deviceContext, shaderPass.DomainShader);
                OnBindMaterialTextures(context, deviceContext, shaderPass.PixelShader);
            }
            if (material.RenderShadowMap && context.IsShadowMapEnabled)
            {
                shaderPass.PixelShader.BindTexture(deviceContext, texShadowSlot, context.SharedResource.ShadowView);
                shaderPass.PixelShader.BindSampler(deviceContext, samplerShadowSlot, SamplerResources[ShadowSamplerIdx]);
            }
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
            int idx = shader.ShaderStageIndex;
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
            int idx = shader.ShaderStageIndex;
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
            int idx = shader.ShaderStageIndex;
            shader.BindTexture(deviceContext, texDiffuseSlot, TextureResources[AlbedoMapIdx]);
            shader.BindTexture(deviceContext, texNormalSlot, TextureResources[NormalMapIdx]);
            shader.BindTexture(deviceContext, texRMASlot, TextureResources[RMAMapIdx]);
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
            texRMASlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.RMAMapTB);
            texDisplaceSlot = shaderPass.VertexShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DisplacementMapTB);
            texShadowSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.ShadowMapTB);
            texIrradianceSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.IrradianceMap);
            samplerSurfaceSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler);
            samplerIBLSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.IBLSampler);
            samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.ShadowMapSampler);
            samplerDisplaceSlot = shaderPass.VertexShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DisplacementMapSampler);
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? MaterialOITPass : MaterialPass;
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShadowPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? WireframeOITPass : WireframePass;
        }
    }
}
