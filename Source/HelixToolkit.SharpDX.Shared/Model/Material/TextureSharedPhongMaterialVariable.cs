/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
using HelixToolkit.UWP.Utilities;
namespace HelixToolkit.UWP.Model
#endif
{
    using Shaders;
    using System;
    using Utilities;
    using ShaderManager;
    /// <summary>
    /// Default PhongMaterial Variables
    /// </summary>
    public sealed class TextureSharedPhongMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        /// <summary>
        /// <see cref="IEffectMaterialVariables.OnInvalidateRenderer"/> 
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidateRenderer;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private const int NUMTEXTURES = 4;
        private const int NUMSAMPLERS = 5;
        private const int DiffuseIdx = 0, AlphaIdx = 1, NormalIdx = 2, DisplaceIdx = 3, ShadowIdx = 4;

        private SharedTextureResourceProxy[] TextureResources = new SharedTextureResourceProxy[NUMTEXTURES];
        private bool HasTextures
        {
            get
            {
                for (int i = 0; i < TextureResources.Length; ++i)
                {
                    if (TextureResources[i] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private int[][] TextureBindingMap = new int[Constants.NumShaderStages][];

        private SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];
        private int[][] SamplerBindingMap = new int[Constants.NumShaderStages][];

        private IShaderPass currentPass;
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

        private bool needUpdate = true;

        private PhongMaterialCore material;
        /// <summary>
        ///
        /// </summary>
        public MaterialCore Material
        {
            set
            {
                if (material != value)
                {
                    if (material != null)
                    {
                        material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                    }
                    material = value as PhongMaterialCore;
                    needUpdate = true;
                    if (material != null)
                    {
                        material.PropertyChanged += Material_OnMaterialPropertyChanged;
                    }
                    CreateTextureViews();
                    CreateSamplers();
                    RaisePropertyChanged();
                }
            }
            get
            {
                return material;
            }
        }

        private readonly Guid ModelGuid;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="modelGuid"></param>
        public TextureSharedPhongMaterialVariables(IEffectsManager manager, Guid modelGuid)
        {
            ModelGuid = modelGuid;
            for (int i = 0; i < Constants.NumShaderStages; ++i)
            {
                TextureBindingMap[i] = new int[NUMTEXTURES];
                SamplerBindingMap[i] = new int[NUMSAMPLERS];
            }
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            CreateTextureViews();
            CreateSamplers();
            this.PropertyChanged += (s, e) => { OnInvalidateRenderer?.Invoke(this, EventArgs.Empty); };
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

        private void CreateTextureView(System.IO.Stream stream, int index)
        {
            TextureResources[index]?.Detach(ModelGuid);
            TextureResources[index] = stream == null ? null : textureManager.Register(ModelGuid, stream);
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
                    TextureResources[i]?.Detach(ModelGuid);
                    TextureResources[i] = null;
                }
            }
        }

        private void CreateSamplers()
        {
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
        /// <see cref="IEffectMaterialVariables.BindMaterialTextures(DeviceContext, IShaderPass)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shaderPass"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContext context, IShaderPass shaderPass)
        {
            if (material == null)
            {
                return false;
            }
            UpdateMappings(shaderPass);
            if (HasTextures)
            {                
                for (int i = 0; i < shaderPass.Shaders.Count; ++i)
                {
                    var shader = shaderPass.Shaders[i];
                    if (shader.IsNULL || !EnumHelper.HasFlag(Constants.CanBindTextureStages, shader.ShaderType))
                    {
                        continue;
                    }
                    OnBindMaterialTextures(context, shader);
                }
            }
            if (RenderShadowMap)
            {
                shaderPass.GetShader(ShaderStage.Pixel).BindSampler(context, SamplerBindingMap[ShaderStage.Pixel.ToIndex()][NUMSAMPLERS - 1], SamplerResources[NUMSAMPLERS - 1]);
            }
            return true;
        }

        private void UpdateMappings(IShaderPass shaderPass)
        {
            if (currentPass == shaderPass)
            {
                return;
            }
            currentPass = shaderPass;
            for(int i = 0; i < shaderPass.Shaders.Count; ++i)
            {
                var shader = shaderPass.Shaders[i];
                if (shader.IsNULL || !EnumHelper.HasFlag(Constants.CanBindTextureStages, shader.ShaderType))
                {
                    continue;
                }
                int idx = shaderPass.Shaders[i].ShaderType.ToIndex();
                TextureBindingMap[idx][DiffuseIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
                TextureBindingMap[idx][AlphaIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderAlphaTexName);
                TextureBindingMap[idx][NormalIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderNormalTexName);
                TextureBindingMap[idx][DisplaceIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDisplaceTexName);

                SamplerBindingMap[idx][DiffuseIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
                SamplerBindingMap[idx][AlphaIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerAlphaTexName);
                SamplerBindingMap[idx][NormalIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerNormalTexName);
                SamplerBindingMap[idx][DisplaceIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerDisplaceTexName);
                SamplerBindingMap[idx][ShadowIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
            }
        }


        /// <summary>
        /// Actual bindings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        private void OnBindMaterialTextures(DeviceContext context, ShaderBase shader)
        {
            int idx = shader.ShaderType.ToIndex();
            for (int i = 0; i < NUMTEXTURES; ++i)
            {
                if (TextureResources[i] == null) { continue; }
                shader.BindTexture(context, TextureBindingMap[idx][i], TextureResources[i]);
                shader.BindSampler(context, SamplerBindingMap[idx][i], SamplerResources[i]);
            }
            if (RenderShadowMap)
            {
                shader.BindSampler(context, SamplerBindingMap[idx][NUMSAMPLERS - 1], SamplerResources[NUMSAMPLERS - 1]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            this.Material = null;
            for(int i=0; i< NUMTEXTURES; ++i)
            {
                TextureResources[i]?.Detach(ModelGuid);
                TextureResources[i] = null;
            }
            SamplerResources = null;
            TextureBindingMap = null;
            SamplerBindingMap = null;
            OnInvalidateRenderer = null;
            base.OnDispose(disposeManagedResources);
        }
    }
}
