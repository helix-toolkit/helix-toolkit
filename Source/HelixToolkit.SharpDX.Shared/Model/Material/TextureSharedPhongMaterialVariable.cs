/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
using System.ComponentModel;
using HelixToolkit.Mathematics;
using System.Numerics;

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
    public sealed class TextureSharedPhongMaterialVariables : MaterialVariable
    {
        private const int NUMTEXTURES = 4;
        private const int NUMSAMPLERS = 5;
        private const int DiffuseIdx = 0, AlphaIdx = 1, NormalIdx = 2, DisplaceIdx = 3, ShadowIdx = 4;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private readonly SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];

        private int texDiffuseSlot, texAlphaSlot, texNormalSlot, texDisplaceSlot, texShadowSlot;
        private int samplerDiffuseSlot, samplerAlphaSlot, samplerNormalSlot, samplerDisplaceSlot, samplerShadowSlot;
        private uint textureIndex = 0;
        private PhongMaterialStruct materialStruct = new PhongMaterialStruct() { UVTransformR1 = new Vector4(1, 0, 0, 0), UVTransformR2 = new Vector4(0, 1, 0, 0) };

        private bool HasTextures
        {
            get
            {
                return textureIndex != 0;
            }
        }

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        public ShaderPass TransparentPass { private set; get; } = ShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderAlphaTexName { set; get; } = DefaultBufferNames.AlphaMapTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDiffuseTexName { set; get; } = DefaultBufferNames.DiffuseMapTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderNormalTexName { set; get; } = DefaultBufferNames.NormalMapTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDisplaceTexName { set; get; } = DefaultBufferNames.DisplacementMapTB;
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
        public string ShaderSamplerAlphaTexName { set; get; } = DefaultSamplerStateNames.AlphaMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerNormalTexName { set; get; } = DefaultSamplerStateNames.NormalMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDisplaceTexName { set; get; } = DefaultSamplerStateNames.DisplacementMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerShadowMapName { set; get; } = DefaultSamplerStateNames.ShadowMapSampler;

        private string defaultShaderPassName = DefaultPassNames.Default;
        public override string DefaultShaderPassName
        {
            set
            {
                if (!fixedPassName && Set(ref defaultShaderPassName, value))
                {
                    MaterialPass = Technique[value];
                    UpdateMappings(MaterialPass);
                }
            }
            get
            {
                return defaultShaderPassName;
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
                    TransparentPass = Technique[value];
                }
            }
            get
            {
                return transparentPassName;
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
                                DefaultShaderPassName = DefaultPassNames.MeshTriTessellation;
                                TransparentPassName = DefaultPassNames.MeshTriTessellationOIT;
                                break;
                            case MeshTopologyEnum.PNQuads:
                                DefaultShaderPassName = DefaultPassNames.MeshQuadTessellation;
                                break;
                        }
                    }
                    else
                    {
                        DefaultShaderPassName = DefaultPassNames.Default;
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
        /// <param name="material"></param>
        public TextureSharedPhongMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : base(manager, technique, DefaultMeshConstantBufferDesc)
        {
            this.material = material;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texDiffuseSlot = texAlphaSlot = texDisplaceSlot = texNormalSlot = -1;
            samplerDiffuseSlot = samplerAlphaSlot = samplerDisplaceSlot = samplerNormalSlot = samplerShadowSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            MaterialPass = technique[DefaultShaderPassName];
            TransparentPass = technique[TransparentPassName];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
            EnableTessellation = material.EnableTessellation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureSharedPhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="technique"></param>
        /// <param name="material">The material.</param>
        public TextureSharedPhongMaterialVariables(string passName, IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : this(manager, technique, material)
        {
            DefaultShaderPassName = passName;
            fixedPassName = true;
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }
            if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DiffuseMap, DiffuseIdx);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.NormalMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).NormalMap, NormalIdx);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DisplacementMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DisplacementMap, DisplaceIdx);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseAlphaMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DiffuseAlphaMap, AlphaIdx);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseMapSampler)))
            {
                RemoveAndDispose(ref SamplerResources[DiffuseIdx]);
                SamplerResources[DiffuseIdx] = Collect(statePoolManager.Register((sender as PhongMaterialCore).DiffuseMapSampler));
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseAlphaMapSampler)))
            {
                RemoveAndDispose(ref SamplerResources[AlphaIdx]);
                SamplerResources[AlphaIdx] = Collect(statePoolManager.Register((sender as PhongMaterialCore).DiffuseAlphaMapSampler));
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DisplacementMapSampler)))
            {
                RemoveAndDispose(ref SamplerResources[DisplaceIdx]);
                SamplerResources[DisplaceIdx] = Collect(statePoolManager.Register((sender as PhongMaterialCore).DisplacementMapSampler));
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.NormalMapSampler)))
            {
                RemoveAndDispose(ref SamplerResources[NormalIdx]);
                SamplerResources[NormalIdx] = Collect(statePoolManager.Register((sender as PhongMaterialCore).NormalMapSampler));
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.EnableTessellation)))
            {
                EnableTessellation = material.EnableTessellation;
            }
            NotifyUpdateNeeded();
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
            RemoveAndDispose(ref SamplerResources[DiffuseIdx]);
            RemoveAndDispose(ref SamplerResources[NormalIdx]);
            RemoveAndDispose(ref SamplerResources[AlphaIdx]);
            RemoveAndDispose(ref SamplerResources[DisplaceIdx]);
            RemoveAndDispose(ref SamplerResources[ShadowIdx]);
            if (material != null)
            {
                SamplerResources[DiffuseIdx] = Collect(statePoolManager.Register(material.DiffuseMapSampler));
                SamplerResources[NormalIdx] = Collect(statePoolManager.Register(material.NormalMapSampler));
                SamplerResources[AlphaIdx] = Collect(statePoolManager.Register(material.DiffuseAlphaMapSampler));
                SamplerResources[DisplaceIdx] = Collect(statePoolManager.Register(material.DisplacementMapSampler));
                SamplerResources[ShadowIdx] = Collect(statePoolManager.Register(DefaultSamplers.ShadowSampler));
            }
        }

        protected sealed override void UpdateInternalVariables(DeviceContextProxy context)
        {
            if (NeedUpdate)
            {
                materialStruct = new PhongMaterialStruct
                {
                    Ambient = material.AmbientColor,
                    Diffuse = material.DiffuseColor,
                    Emissive = material.EmissiveColor,
                    Reflect = material.ReflectiveColor,
                    Specular = material.SpecularColor,
                    Shininess = material.SpecularShininess,
                    HasDiffuseMap = material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0,
                    HasDiffuseAlphaMap = material.RenderDiffuseAlphaMap && TextureResources[AlphaIdx] != null ? 1 : 0,
                    HasNormalMap = material.RenderNormalMap && TextureResources[NormalIdx] != null ? 1 : 0,
                    HasDisplacementMap = material.RenderDisplacementMap && TextureResources[DisplaceIdx] != null ? 1 : 0,
                    DisplacementMapScaleMask = material.DisplacementMapScaleMask,
                    RenderShadowMap = material.RenderShadowMap ? 1 : 0,
                    HasCubeMap = material.RenderEnvironmentMap ? 1 : 0,
                    MaxTessDistance = material.MaxTessellationDistance,
                    MinTessDistance = material.MinTessellationDistance,
                    MaxDistTessFactor = material.MaxDistanceTessellationFactor,
                    MinDistTessFactor = material.MinDistanceTessellationFactor,
                    UVTransformR1 = material.UVTransform.Column1(),
                    UVTransformR2 = material.UVTransform.Column2()
                    //UVTransformRow1 = new Vector2(material.UVTransform.M11, material.UVTransform.M12),
                    //UVTransformRow2 = new Vector2(material.UVTransform.M21, material.UVTransform.M22),
                    //UVTransformRow3 = Vector2.Zero,
                    //UVTransformRow4 = new Vector2(material.UVTransform.M41, material.UVTransform.M42)
                };
                NeedUpdate = false;
            }
        }

        protected override void WriteMaterialDataToConstantBuffer(global::SharpDX.DataStream cbStream)
        {
            cbStream.Write(materialStruct);
        }

        protected override bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
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
                shaderPass.PixelShader.BindSampler(deviceContext, samplerShadowSlot, SamplerResources[ShadowIdx]);
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
            shader.BindSampler(context, samplerDisplaceSlot, SamplerResources[DisplaceIdx]);
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
            shader.BindSampler(context, samplerDisplaceSlot, SamplerResources[DisplaceIdx]);
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

            shader.BindSampler(deviceContext, samplerDiffuseSlot, SamplerResources[DiffuseIdx]);
            shader.BindSampler(deviceContext, samplerNormalSlot, SamplerResources[NormalIdx]);
            shader.BindSampler(deviceContext, samplerAlphaSlot, SamplerResources[AlphaIdx]);
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
            samplerAlphaSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerAlphaTexName);
            samplerNormalSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerNormalTexName);
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
                material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                for (int i = 0; i < NUMTEXTURES; ++i)
                {
                    TextureResources[i] = null;
                }
                for (int i = 0; i < NUMSAMPLERS; ++i)
                {
                    SamplerResources[i] = null;
                }
            }

            base.OnDispose(disposeManagedResources);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? TransparentPass : MaterialPass;
        }
    }
}
