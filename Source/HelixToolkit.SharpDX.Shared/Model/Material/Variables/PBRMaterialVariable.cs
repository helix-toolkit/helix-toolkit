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

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        public ShaderPass MaterialOITPass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass ShadowPass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass WireframePass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass WireframeOITPass { private set; get; } = ShaderPass.NullPass;


        public PBRMaterialVariable(IEffectsManager manager, IRenderTechnique technique, PBRMaterialCore core)
            : base(manager, technique, DefaultMeshPBRConstantBufferDesc)
        {
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            material = core;
            material.PropertyChanged += MaterialCore_PropertyChanged;
            MaterialPass = technique[DefaultPassNames.PBR];
            WireframePass = technique[DefaultPassNames.Wireframe];
            WireframeOITPass = technique[DefaultPassNames.WireframeOITPass];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
            //EnableTessellation = material.EnableTessellation;
        }

        protected override void OnInitialPropertyBindings()
        {
            AddPropertyBinding(nameof(PBRMaterialCore.AlbedoColor), () => { WriteValue(PBRMaterialStruct.ConstantAlbedoStr, material.AlbedoColor); });
            AddPropertyBinding(nameof(PBRMaterialCore.MetallicFactor), () => { WriteValue(PBRMaterialStruct.ConstantMetallicStr, material.MetallicFactor); });
            AddPropertyBinding(nameof(PBRMaterialCore.RoughnessFactor), () => { WriteValue(PBRMaterialStruct.ConstantRoughnessStr, material.RoughnessFactor); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderAlbedoMap), () => { WriteValue(PBRMaterialStruct.HasAlbedoMapStr, material.RenderAlbedoMap && TextureResources[AlbedoMapIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderEmissiveMap), () => { WriteValue(PBRMaterialStruct.HasEmissiveMapStr, material.RenderEnvironmentMap && TextureResources[EmissiveMapIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderNormalMap), () => { WriteValue(PBRMaterialStruct.HasNormalMapStr, material.RenderNormalMap && TextureResources[NormalMapIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderDisplacementMap), () => { WriteValue(PBRMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && TextureResources[DisplaceMapIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderIrradianceMap), () => { WriteValue(PBRMaterialStruct.HasIrradianceMapStr, material.RenderIrradianceMap && TextureResources[IrradianceMapIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderRMAMap), () => { WriteValue(PBRMaterialStruct.HasRMAMapStr, material.RenderRMAMap && TextureResources[RMAMapIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMapScaleMask), () => { WriteValue(PBRMaterialStruct.DisplacementMapScaleMaskStr, material.DisplacementMapScaleMask); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderShadowMap), () => { WriteValue(PBRMaterialStruct.RenderShadowMapStr, material.RenderShadowMap ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.RenderEnvironmentMap), () => { WriteValue(PBRMaterialStruct.HasRadianceMapStr, material.RenderEnvironmentMap ? 1 : 0); });
            AddPropertyBinding(nameof(PBRMaterialCore.NumRadianceMipLevels), () => { WriteValue(PBRMaterialStruct.NumRadianceMipLevelsStr, material.NumRadianceMipLevels); });
            AddPropertyBinding(nameof(PBRMaterialCore.MaxTessellationDistance), () => { WriteValue(PBRMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance); });
            AddPropertyBinding(nameof(PBRMaterialCore.MinTessellationDistance), () => { WriteValue(PBRMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance); });
            AddPropertyBinding(nameof(PBRMaterialCore.MaxDistanceTessellationFactor), () => { WriteValue(PBRMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor); });
            AddPropertyBinding(nameof(PBRMaterialCore.MinDistanceTessellationFactor), () => { WriteValue(PBRMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor); });
            AddPropertyBinding(nameof(PBRMaterialCore.UVTransform), () => 
            {
                WriteValue(PBRMaterialStruct.UVTransformR1Str, material.UVTransform.Column1);
                WriteValue(PBRMaterialStruct.UVTransformR2Str, material.UVTransform.Column2);
            });
            AddPropertyBinding(nameof(PBRMaterialCore.AlbedoMap), () => { CreateTextureView(material.AlbedoMap, AlbedoMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderAlbedoMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.EmissiveMap), () => { CreateTextureView(material.AlbedoMap, EmissiveMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderEmissiveMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.NormalMap), () => { CreateTextureView(material.AlbedoMap, NormalMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderNormalMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.IrradianceMap), () => { CreateTextureView(material.AlbedoMap, IrradianceMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderIrradianceMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMap), () => { CreateTextureView(material.AlbedoMap, DisplaceMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderDisplacementMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.RMAMap), () => { CreateTextureView(material.AlbedoMap, RMAMapIdx); TriggerPropertyAction(nameof(PBRMaterialCore.RenderRMAMap)); });
            AddPropertyBinding(nameof(PBRMaterialCore.SurfaceMapSampler), () => { CreateSampler(material.SurfaceMapSampler, SurfaceSamplerIdx); });
            AddPropertyBinding(nameof(PBRMaterialCore.IBLSampler), () => { CreateSampler(material.IBLSampler, IBLSamplerIdx); });
            AddPropertyBinding(nameof(PBRMaterialCore.DisplacementMapSampler), () => { CreateSampler(material.DisplacementMapSampler, DisplaceSamplerIdx); });
            AddPropertyBinding(nameof(PBRMaterialCore.EnableTessellation), () => { });
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
            shader.BindSampler(context, samplerDisplaceSlot, SamplerResources[DisplaceMapIdx]);
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
            shader.BindSampler(context, samplerDisplaceSlot, SamplerResources[DisplaceMapIdx]);
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
            return MaterialPass;
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShadowPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? WireframeOITPass : WireframePass;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            material.PropertyChanged -= MaterialCore_PropertyChanged;
            base.OnDispose(disposeManagedResources);
        }
    }
}
