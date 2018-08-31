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
        private const int NUMSAMPLERS = 5;
        private const int DiffuseIdx = 0, AlphaIdx = 1, NormalIdx = 2, DisplaceIdx = 3, ShadowIdx = 4;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private readonly SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];

        private int texDiffuseSlot, texAlphaSlot, texNormalSlot, texDisplaceSlot, texShadowSlot;
        private int samplerDiffuseSlot, samplerAlphaSlot, samplerNormalSlot, samplerDisplaceSlot, samplerShadowSlot;
        private uint textureIndex = 0;
        //private PhongMaterialStruct materialStruct = new PhongMaterialStruct() { UVTransformR1 = new Vector4(1, 0, 0, 0), UVTransformR2 = new Vector4(0, 1, 0, 0) };

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
        /// <param name="material"></param>
        public PhongMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : base(manager, technique, DefaultMeshConstantBufferDesc)
        {
            this.material = material;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texDiffuseSlot = texAlphaSlot = texDisplaceSlot = texNormalSlot = -1;
            samplerDiffuseSlot = samplerAlphaSlot = samplerDisplaceSlot = samplerNormalSlot = samplerShadowSlot = -1;
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
            EnableTessellation = material.EnableTessellation;
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

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseColor)))
            {
                WriteValue(PhongMaterialStruct.DiffuseStr, material.DiffuseColor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.AmbientColor)))
            {
                WriteValue(PhongMaterialStruct.AmbientStr, material.AmbientColor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.EmissiveColor)))
            {
                WriteValue(PhongMaterialStruct.EmissiveStr, material.EmissiveColor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.ReflectiveColor)))
            {
                WriteValue(PhongMaterialStruct.ReflectStr, material.ReflectiveColor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.SpecularColor)))
            {
                WriteValue(PhongMaterialStruct.SpecularStr, material.SpecularColor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.SpecularShininess)))
            {
                WriteValue(PhongMaterialStruct.ShininessStr, material.SpecularShininess);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DisplacementMapScaleMask)))
            {
                WriteValue(PhongMaterialStruct.DisplacementMapScaleMaskStr, material.DisplacementMapScaleMask);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.RenderShadowMap)))
            {
                WriteValue(PhongMaterialStruct.RenderShadowMapStr, material.RenderShadowMap);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.RenderEnvironmentMap)))
            {
                WriteValue(PhongMaterialStruct.HasCubeMapStr, material.RenderEnvironmentMap);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.UVTransform)))
            {
                WriteValue(PhongMaterialStruct.UVTransformR1Str, material.UVTransform.Column1);
                WriteValue(PhongMaterialStruct.UVTransformR2Str, material.UVTransform.Column2);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.MaxTessellationDistance)))
            {
                WriteValue(PhongMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.MaxDistanceTessellationFactor)))
            {
                WriteValue(PhongMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.MinTessellationDistance)))
            {
                WriteValue(PhongMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.MinDistanceTessellationFactor)))
            {
                WriteValue(PhongMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.RenderDiffuseMap)))
            {
                WriteValue(PhongMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.RenderDiffuseAlphaMap)))
            {
                WriteValue(PhongMaterialStruct.HasDiffuseAlphaMapStr, material.RenderDiffuseAlphaMap && TextureResources[AlphaIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.RenderNormalMap)))
            {
                WriteValue(PhongMaterialStruct.HasNormalMapStr, material.RenderNormalMap && TextureResources[NormalIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.RenderDisplacementMap)))
            {
                WriteValue(PhongMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && TextureResources[DisplaceIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DiffuseMap, DiffuseIdx);
                WriteValue(PhongMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.NormalMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).NormalMap, NormalIdx);
                WriteValue(PhongMaterialStruct.HasNormalMapStr, material.RenderNormalMap && TextureResources[NormalIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DisplacementMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DisplacementMap, DisplaceIdx);
                WriteValue(PhongMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && TextureResources[DisplaceIdx] != null ? 1 : 0);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseAlphaMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DiffuseAlphaMap, AlphaIdx);
                WriteValue(PhongMaterialStruct.HasDiffuseAlphaMapStr, material.RenderDiffuseAlphaMap && TextureResources[AlphaIdx] != null ? 1 : 0);
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
            InvalidateRenderer();
        }

        protected override void OnInitialPropertyBindings()
        {
            base.OnInitialPropertyBindings();
            WriteValue(PhongMaterialStruct.AmbientStr, material.AmbientColor);
            WriteValue(PhongMaterialStruct.DiffuseStr, material.DiffuseColor);
            WriteValue(PhongMaterialStruct.EmissiveStr, material.EmissiveColor);
            WriteValue(PhongMaterialStruct.ReflectStr, material.ReflectiveColor);
            WriteValue(PhongMaterialStruct.SpecularStr, material.SpecularColor);
            WriteValue(PhongMaterialStruct.ShininessStr, material.SpecularShininess);
            WriteValue(PhongMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0);
            WriteValue(PhongMaterialStruct.HasDiffuseAlphaMapStr, material.RenderDiffuseAlphaMap && TextureResources[AlphaIdx] != null ? 1 : 0);
            WriteValue(PhongMaterialStruct.HasNormalMapStr, material.RenderNormalMap && TextureResources[NormalIdx] != null ? 1 : 0);
            WriteValue(PhongMaterialStruct.HasDisplacementMapStr, material.RenderDisplacementMap && TextureResources[DisplaceIdx] != null ? 1 : 0);
            WriteValue(PhongMaterialStruct.DisplacementMapScaleMaskStr, material.DisplacementMapScaleMask);
            WriteValue(PhongMaterialStruct.RenderShadowMapStr, material.RenderShadowMap ? 1 : 0);
            WriteValue(PhongMaterialStruct.HasCubeMapStr, material.RenderEnvironmentMap ? 1 : 0);
            WriteValue(PhongMaterialStruct.MaxTessDistanceStr, material.MaxTessellationDistance);
            WriteValue(PhongMaterialStruct.MinTessDistanceStr, material.MinTessellationDistance);
            WriteValue(PhongMaterialStruct.MaxDistTessFactorStr, material.MaxDistanceTessellationFactor);
            WriteValue(PhongMaterialStruct.MinDistTessFactorStr, material.MinDistanceTessellationFactor);
            WriteValue(PhongMaterialStruct.UVTransformR1Str, material.UVTransform.Column1);
            WriteValue(PhongMaterialStruct.UVTransformR2Str, material.UVTransform.Column2);
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
