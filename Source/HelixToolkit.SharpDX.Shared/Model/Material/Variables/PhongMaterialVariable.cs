/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
using System.ComponentModel;
using global::SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Core;
    using Render;
    using ShaderManager;
    using Shaders;
    using Utilities;

    /// <summary>
    /// Default PhongMaterial Variables
    /// </summary>
    public sealed class PhongMaterialVariables : MaterialVariable
    {
        private const int NUMTEXTURES = 4;
        private const int DiffuseIdx = 0, AlphaIdx = 1, NormalIdx = 2, DisplaceIdx = 3, ShadowIdx = 4;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private SamplerStateProxy surfaceSampler, displacementSampler, shadowSampler;

        private int texDiffuseSlot, texAlphaSlot, texNormalSlot, texDisplaceSlot, texShadowSlot;
        private int samplerDiffuseSlot, samplerDisplaceSlot, samplerShadowSlot;
        private uint textureIndex = 0;

        private bool HasTextures
        {
            get
            {
                return textureIndex != 0;
            }
        }

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        public ShaderPass MaterialOITPass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass ShadowPass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass WireframePass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass WireframeOITPass { private set; get; } = ShaderPass.NullPass;
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
        public string ShaderShadowTexName { set; get; } = DefaultBufferNames.ShadowMapTB;

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

        private string materialShaderPassName = DefaultPassNames.Default;
        public string MaterialShaderPassName
        {
            set
            {
                if (!fixedPassName && Set(ref materialShaderPassName, value))
                {
                    MaterialPass = Technique[value];
                    UpdateMappings(MaterialPass);
                }
            }
            get
            {
                return materialShaderPassName;
            }
        }

        private string shadowPassName = DefaultPassNames.ShadowPass;
        public string ShadowPassName
        {
            set
            {
                if (Set(ref shadowPassName, value))
                {
                    ShadowPass = Technique[value];
                }
            }
            get
            {
                return shadowPassName;
            }
        }

        private string wireframePassName = DefaultPassNames.Wireframe;
        public string WireframePassName
        {
            set
            {
                if (Set(ref wireframePassName, value))
                {
                    WireframePass = Technique[value];
                }
            }
            get
            {
                return wireframePassName;
            }
        }

        private string transparentPassName = DefaultPassNames.OITPass;
        /// <summary>
        /// Gets or sets the name of the mesh transparent pass.
        /// </summary>
        /// <value>
        /// The name of the transparent pass.
        /// </value>
        public string TransparentPassName
        {
            set
            {
                if (!fixedPassName && Set(ref transparentPassName, value))
                {
                    MaterialOITPass = Technique[value];
                }
            }
            get
            {
                return transparentPassName;
            }
        }

        private string wireframeOITPassName = DefaultPassNames.WireframeOITPass;
        public string WireframeOITPassName
        {
            set
            {
                if (Set(ref wireframeOITPassName, value))
                {
                    WireframeOITPass = Technique[value];
                }
            }
            get
            {
                return wireframeOITPassName;
            }
        }

        private bool enableTessellation = false;
        public bool EnableTessellation
        {
            private set
            {
                if (SetAffectsRender(ref enableTessellation, value))
                {
                    if (enableTessellation)
                    {
                        switch (material.MeshType)
                        {
                            case MeshTopologyEnum.PNTriangles:
                                MaterialShaderPassName = DefaultPassNames.MeshTriTessellation;
                                TransparentPassName = DefaultPassNames.MeshTriTessellationOIT;
                                break;
                            case MeshTopologyEnum.PNQuads:
                                MaterialShaderPassName = DefaultPassNames.MeshQuadTessellation;
                                break;
                        }
                    }
                    else
                    {
                        MaterialShaderPassName = DefaultPassNames.Default;
                        TransparentPassName = DefaultPassNames.OITPass;
                    }
                }
            }
            get
            {
                return enableTessellation;
            }
        }

        private readonly bool fixedPassName = false;
        private readonly PhongMaterialCore material;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="technique"></param>
        /// <param name="materialCore"></param>
        public PhongMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore materialCore)
            : base(manager, technique, DefaultMeshConstantBufferDesc, materialCore)
        {
            this.material = materialCore;
            texDiffuseSlot = texAlphaSlot = texDisplaceSlot = texNormalSlot = -1;
            samplerDiffuseSlot = samplerDisplaceSlot = samplerShadowSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            MaterialPass = technique[MaterialShaderPassName];
            MaterialOITPass = technique[TransparentPassName];
            ShadowPass = technique[ShadowPassName];
            WireframePass = technique[WireframePassName];
            WireframeOITPass = technique[WireframeOITPassName];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
            EnableTessellation = materialCore.EnableTessellation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="technique"></param>
        /// <param name="material">The material.</param>
        public PhongMaterialVariables(string passName, IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : this(manager, technique, material)
        {
            MaterialShaderPassName = passName;
            fixedPassName = true;
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
            AddPropertyBinding(nameof(PhongMaterialCore.RenderShadowMap), ()=> { WriteValue(PhongPBRMaterialStruct.RenderShadowMapStr, material.RenderShadowMap ? 1 : 0); });
            AddPropertyBinding(nameof(PhongMaterialCore.RenderEnvironmentMap), () => { WriteValue(PhongPBRMaterialStruct.HasCubeMapStr, material.RenderEnvironmentMap ? 1 : 0); });
            AddPropertyBinding(nameof(PhongMaterialCore.UVTransform), () => 
            {
                WriteValue(PhongPBRMaterialStruct.UVTransformR1Str, material.UVTransform.Column1);
                WriteValue(PhongPBRMaterialStruct.UVTransformR2Str, material.UVTransform.Column2);
            });
            AddPropertyBinding(nameof(PhongMaterialCore.EnableAutoTangent), () => { WriteValue(PhongPBRMaterialStruct.EnableAutoTangent, material.EnableAutoTangent); });
            AddPropertyBinding(nameof(PhongMaterialCore.MaxTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance); });
            AddPropertyBinding(nameof(PhongMaterialCore.MaxDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor); });
            AddPropertyBinding(nameof(PhongMaterialCore.MinTessellationDistance), () => { WriteValue(PhongPBRMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance); });
            AddPropertyBinding(nameof(PhongMaterialCore.MinDistanceTessellationFactor), () => { WriteValue(PhongPBRMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor); });
            AddPropertyBinding(nameof(PhongMaterialCore.RenderDiffuseMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PhongMaterialCore.RenderDiffuseAlphaMap), () => { WriteValue(PhongPBRMaterialStruct.HasDiffuseAlphaMapStr, material.RenderDiffuseAlphaMap && TextureResources[AlphaIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PhongMaterialCore.RenderNormalMap), () => { WriteValue(PhongPBRMaterialStruct.HasNormalMapStr, material.RenderNormalMap && TextureResources[NormalIdx] != null ? 1 : 0); });
            AddPropertyBinding(nameof(PhongMaterialCore.RenderDisplacementMap), () => { WriteValue(PhongPBRMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && TextureResources[DisplaceIdx] != null ? 1 : 0); });
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

            AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMapSampler), () =>
            {
                RemoveAndDispose(ref surfaceSampler);
                surfaceSampler = Collect(statePoolManager.Register(material.DiffuseMapSampler));
            });

            AddPropertyBinding(nameof(PhongMaterialCore.DisplacementMapSampler), () =>
            {
                RemoveAndDispose(ref displacementSampler);
                displacementSampler = Collect(statePoolManager.Register(material.DisplacementMapSampler));
            });

            AddPropertyBinding(nameof(PhongMaterialCore.EnableTessellation), () => { EnableTessellation = material.EnableTessellation; });
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
                CreateTextureView(material.DiffuseMap, DiffuseIdx);
                CreateTextureView(material.NormalMap, NormalIdx);
                CreateTextureView(material.DisplacementMap, DisplaceIdx);
                CreateTextureView(material.DiffuseAlphaMap, AlphaIdx);
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
            RemoveAndDispose(ref surfaceSampler);
            RemoveAndDispose(ref displacementSampler);
            RemoveAndDispose(ref shadowSampler);
            if (material != null)
            {
                surfaceSampler = Collect(statePoolManager.Register(material.DiffuseMapSampler));
                displacementSampler = Collect(statePoolManager.Register(material.DisplacementMapSampler));
                shadowSampler = Collect(statePoolManager.Register(DefaultSamplers.ShadowSampler));
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
                shaderPass.PixelShader.BindSampler(deviceContext, samplerShadowSlot, shadowSampler);
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
            shader.BindTexture(context, texDisplaceSlot, TextureResources[DisplaceIdx]);
            shader.BindSampler(context, samplerDisplaceSlot, displacementSampler);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnBindMaterialTextures(DeviceContextProxy context, DomainShader shader)
        {
            if (shader.IsNULL)
            {
                return;
            }
            int idx = shader.ShaderStageIndex;
            shader.BindTexture(context, texDisplaceSlot, TextureResources[DisplaceIdx]);
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
            int idx = shader.ShaderStageIndex;
            shader.BindTexture(deviceContext, texDiffuseSlot, TextureResources[DiffuseIdx]);
            shader.BindTexture(deviceContext, texNormalSlot, TextureResources[NormalIdx]);
            shader.BindTexture(deviceContext, texAlphaSlot, TextureResources[AlphaIdx]);

            shader.BindSampler(deviceContext, samplerDiffuseSlot, surfaceSampler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
            texAlphaSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderAlphaTexName);
            texNormalSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderNormalTexName);
            texDisplaceSlot = shaderPass.VertexShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDisplaceTexName);
            texShadowSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowTexName);
            samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
            samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
            samplerDisplaceSlot = shaderPass.VertexShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDisplaceTexName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                for (int i = 0; i < NUMTEXTURES; ++i)
                {
                    TextureResources[i] = null;
                }
                surfaceSampler = displacementSampler = shadowSampler = null;
            }

            base.OnDispose(disposeManagedResources);
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

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }
    }
}
