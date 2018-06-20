/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
using HelixToolkit.UWP.Utilities;
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using ShaderManager;
    using Shaders;

    using Utilities;

    /// <summary>
    /// Default PhongMaterial Variables
    /// </summary>
    public sealed class TextureSharedPhongMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        private const int NUMTEXTURES = 4;
        private const int NUMSAMPLERS = 5;
        private const int DiffuseIdx = 0, AlphaIdx = 1, NormalIdx = 2, DisplaceIdx = 3, ShadowIdx = 4;
        /// <summary>
        /// <see cref="IEffectMaterialVariables.OnInvalidateRenderer"/> 
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidateRenderer;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private readonly SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];

        private int texDiffuseSlot, texAlphaSlot, texNormalSlot, texDisplaceSlot;
        private int samplerDiffuseSlot, samplerAlphaSlot, samplerNormalSlot, samplerDisplaceSlot, samplerShadowSlot;
        private uint textureIndex = 0;

        private bool HasTextures
        {
            get
            {
                return textureIndex != 0;
            }
        }

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;

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

        private bool renderDiffuseMap = true;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderDiffuseMap
        {
            set
            {
                if (Set(ref renderDiffuseMap, value))
                {
                    needUpdate = true;
                }
            }
            get
            {
                return renderDiffuseMap;
            }
        }
        private bool renderDiffuseAlphaMap = true;
        /// <summary>
        ///
        /// </summary>
        public bool RenderDiffuseAlphaMap
        {
            set
            {
                if (Set(ref renderDiffuseAlphaMap, value))
                {
                    needUpdate = true;
                }
            }
            get
            {
                return renderDiffuseAlphaMap;
            }
        }
        private bool renderNormalMap = true;
        /// <summary>
        ///
        /// </summary>
        public bool RenderNormalMap
        {
            set
            {
                if (Set(ref renderNormalMap, value))
                {
                    needUpdate = true;
                }
            }
            get
            {
                return renderNormalMap;
            }
        }
        private bool renderDisplacementMap = true;
        /// <summary>
        ///
        /// </summary>
        public bool RenderDisplacementMap
        {
            set
            {
                if (Set(ref renderDisplacementMap, value))
                {
                    needUpdate = true;
                }
            }
            get
            {
                return renderDisplacementMap;
            }
        }

        private bool renderShadowMap = false;

        /// <summary>
        /// 
        /// </summary>
        public bool RenderShadowMap
        {
            set
            {
                if (Set(ref renderShadowMap, value))
                {
                    needUpdate = true;
                }
            }
            get
            {
                return renderShadowMap;
            }
        }

        private bool renderEnvironmentMap = false;

        /// <summary>
        /// Gets or sets a value indicating whether [render environment map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render environment map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderEnvironmentMap
        {
            set
            {
                if (Set(ref renderEnvironmentMap, value))
                {
                    needUpdate = true;
                }
            }
            get
            {
                return renderEnvironmentMap;
            }
        }

        private string defaultShaderPassName = DefaultPassNames.Default;
        public string DefaultShaderPassName
        {
            set
            {
                if (!fixedPassName && Set(ref defaultShaderPassName, value) && isAttached)
                {
                    MaterialPass = technique[value];
                    UpdateMappings(MaterialPass);
                }
            }
            get
            {
                return defaultShaderPassName;
            }
        }

        private bool needUpdate = true;
        private readonly PhongMaterialCore material;
        private bool isAttached = false;
        private IRenderTechnique technique;
        private readonly bool fixedPassName = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="material"></param>
        public TextureSharedPhongMaterialVariables(IEffectsManager manager, PhongMaterialCore material)
        {
            this.material = material;
            needUpdate = true;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texDiffuseSlot = texAlphaSlot = texDisplaceSlot = texNormalSlot = -1;
            samplerDiffuseSlot = samplerAlphaSlot = samplerDisplaceSlot = samplerNormalSlot = samplerShadowSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            CreateTextureViews();
            CreateSamplers();
            this.PropertyChanged += (s, e) => { OnInvalidateRenderer?.Invoke(this, EventArgs.Empty); };
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureSharedPhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="material">The material.</param>
        public TextureSharedPhongMaterialVariables(string passName, IEffectsManager manager, PhongMaterialCore material)
            : this(manager, material)
        {
            DefaultShaderPassName = passName;
            fixedPassName = true;
        }

        public bool Attach(IRenderTechnique technique)
        {
            this.technique = technique;
            MaterialPass = technique[DefaultShaderPassName];
            UpdateMappings(MaterialPass);
            isAttached = true;
            return !MaterialPass.IsNULL;
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            needUpdate = true;
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
            OnInvalidateRenderer?.Invoke(this, EventArgs.Empty);
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

        private void AssignVariables(ref ModelStruct modelstruct)
        {
            modelstruct.Ambient = material.AmbientColor;
            modelstruct.Diffuse = material.DiffuseColor;
            modelstruct.Emissive = material.EmissiveColor;
            modelstruct.Reflect = material.ReflectiveColor;
            modelstruct.Specular = material.SpecularColor;
            modelstruct.Shininess = material.SpecularShininess;
            modelstruct.HasDiffuseMap = RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0;
            modelstruct.HasDiffuseAlphaMap = RenderDiffuseAlphaMap && TextureResources[AlphaIdx] != null ? 1 : 0;
            modelstruct.HasNormalMap = RenderNormalMap && TextureResources[NormalIdx] != null ? 1 : 0;
            modelstruct.HasDisplacementMap = RenderDisplacementMap && TextureResources[DisplaceIdx] != null ? 1 : 0;
            modelstruct.DisplacementMapScaleMask = material.DisplacementMapScaleMask;
            modelstruct.RenderShadowMap = RenderShadowMap ? 1 : 0;
            modelstruct.HasCubeMap = RenderEnvironmentMap ? 1 : 0;
        }

        /// <summary>
        /// Updates the material variables.
        /// </summary>
        /// <param name="modelstruct">The modelstruct.</param>
        /// <returns></returns>
        public bool UpdateMaterialVariables(ref ModelStruct modelstruct)
        {
            if (material == null)
            {
                return false;
            }
            if (needUpdate)
            {
                AssignVariables(ref modelstruct);
                needUpdate = false;
            }
            return true;
        }

        /// <summary>
        /// <see cref="IEffectMaterialVariables.BindMaterialTextures(DeviceContextProxy, ShaderPass)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shaderPass"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContextProxy context, ShaderPass shaderPass)
        {
            if (material == null)
            {
                return false;
            }
            if (HasTextures)
            {
                OnBindMaterialTextures(context, shaderPass.VertexShader);
                OnBindMaterialTextures(context, shaderPass.DomainShader);
                OnBindMaterialTextures(context, shaderPass.PixelShader);
            }
            if (RenderShadowMap)
            {
                shaderPass.PixelShader.BindSampler(context, samplerShadowSlot, SamplerResources[NUMSAMPLERS - 1]);
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
        /// <param name="shader"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnBindMaterialTextures(DeviceContextProxy context, PixelShader shader)
        {
            if (shader.IsNULL)
            {
                return;
            }
            int idx = shader.ShaderStageIndex;
            shader.BindTexture(context, texDiffuseSlot, TextureResources[DiffuseIdx]);
            shader.BindTexture(context, texNormalSlot, TextureResources[NormalIdx]);
            shader.BindTexture(context, texAlphaSlot, TextureResources[AlphaIdx]);

            shader.BindSampler(context, samplerDiffuseSlot, SamplerResources[DiffuseIdx]);
            shader.BindSampler(context, samplerNormalSlot, SamplerResources[NormalIdx]);
            shader.BindSampler(context, samplerAlphaSlot, SamplerResources[AlphaIdx]);
            shader.BindSampler(context, samplerShadowSlot, SamplerResources[ShadowIdx]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
            texAlphaSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderAlphaTexName);
            texNormalSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderNormalTexName);
            texDisplaceSlot = shaderPass.VertexShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDisplaceTexName);

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
                isAttached = false;
                technique = null;
                material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                for (int i = 0; i < NUMTEXTURES; ++i)
                {
                    TextureResources[i] = null;
                }
                for (int i = 0; i < NUMSAMPLERS; ++i)
                {
                    SamplerResources[i] = null;
                }

                OnInvalidateRenderer = null;
            }

            base.OnDispose(disposeManagedResources);
        }
    }
}
