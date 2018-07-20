/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Utilities;
    using System.ComponentModel;

    public sealed class DiffuseMaterialCore : PhongMaterialCore
    {
        public override IEffectMaterialVariables CreateMaterialVariables(IEffectsManager manager)
        {
            return new DiffuseMaterialVariables(DefaultPassNames.Diffuse, manager, this);
        }
    }

    public sealed class ViewCubeMaterialCore : PhongMaterialCore
    {
        public override IEffectMaterialVariables CreateMaterialVariables(IEffectsManager manager)
        {
            return new DiffuseMaterialVariables(DefaultPassNames.ViewCube, manager, this);
        }
    }

    public sealed class DiffuseMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        private const int NUMTEXTURES = 1;
        private const int NUMSAMPLERS = 1;
        private const int DiffuseIdx = 0;
        /// <summary>
        /// <see cref="IEffectMaterialVariables.OnInvalidateRenderer"/> 
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidateRenderer;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private readonly SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];

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

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        public ShaderPass TransparentPass { private set; get; } = ShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDiffuseTexName { set; get; } = DefaultBufferNames.DiffuseMapTB;

        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerShadowMapName { set; get; } = DefaultSamplerStateNames.ShadowMapSampler;

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

        private string transparentPassName = DefaultPassNames.DiffuseOIT;
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
                if (!fixedPassName && Set(ref transparentPassName, value) && isAttached)
                {
                    TransparentPass = technique[value];
                }
            }
            get
            {
                return transparentPassName;
            }
        }
        /// <summary>
        /// Reflect the environment cube map
        /// </summary>
        public bool RenderEnvironmentMap { set; get; }
        private bool needUpdate = true;
        private readonly PhongMaterialCore material;
        private bool isAttached = false;
        private IRenderTechnique technique;
        private readonly bool fixedPassName = false;

        private PhongMaterialStruct materialStruct = new PhongMaterialStruct();
        private readonly ConstantBufferProxy materialCB;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="material"></param>
        private DiffuseMaterialVariables(IEffectsManager manager, PhongMaterialCore material)
        {
            this.material = material;
            needUpdate = true;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texDiffuseSlot = -1;
            samplerDiffuseSlot = samplerShadowSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            CreateTextureViews();
            CreateSamplers();
            materialCB = manager.ConstantBufferPool.Register(DefaultBufferNames.MeshPhongCB, PhongMaterialStruct.SizeInBytes);
            this.PropertyChanged += (s, e) => { OnInvalidateRenderer?.Invoke(this, EventArgs.Empty); };
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureSharedPhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="material">The material.</param>
        public DiffuseMaterialVariables(string passName, IEffectsManager manager, PhongMaterialCore material)
            : this(manager, material)
        {
            DefaultShaderPassName = passName;
            fixedPassName = true;
        }

        public bool Attach(IRenderTechnique technique)
        {
            this.technique = technique;
            MaterialPass = technique[DefaultShaderPassName];
            TransparentPass = technique[TransparentPassName];
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
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseMapSampler)))
            {
                RemoveAndDispose(ref SamplerResources[DiffuseIdx]);
                SamplerResources[DiffuseIdx] = Collect(statePoolManager.Register((sender as PhongMaterialCore).DiffuseMapSampler));
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
            if (material != null)
            {
                SamplerResources[DiffuseIdx] = Collect(statePoolManager.Register(material.DiffuseMapSampler));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssignVariables()
        {
            materialStruct.Ambient = material.AmbientColor;
            materialStruct.Diffuse = material.DiffuseColor;
            materialStruct.Emissive = material.EmissiveColor;
            materialStruct.Reflect = material.ReflectiveColor;
            materialStruct.Specular = material.SpecularColor;
            materialStruct.Shininess = material.SpecularShininess;
            materialStruct.HasDiffuseMap = material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0;
            materialStruct.HasDiffuseAlphaMap = 0;
            materialStruct.HasNormalMap = 0;
            materialStruct.HasDisplacementMap = 0;
            materialStruct.DisplacementMapScaleMask = material.DisplacementMapScaleMask;
            materialStruct.RenderShadowMap = RenderShadowMap ? 1 : 0;
            materialStruct.HasCubeMap = 0;
        }

        /// <summary>
        /// Updates the material variables.
        /// </summary>
        /// <param name="deviceContext"></param>
        /// <returns></returns>
        public bool UpdateMaterialVariables(DeviceContextProxy deviceContext)
        {
            if (material == null)
            {
                return false;
            }
            bool cbUpdate = deviceContext.SetCurrentMaterial(this);
            if (needUpdate)
            {
                AssignVariables();
                needUpdate = false;
                cbUpdate = true;
            }
            if (cbUpdate)
            {
                materialCB.UploadDataToBuffer(deviceContext, ref materialStruct);
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
        private void OnBindMaterialTextures(DeviceContextProxy context, PixelShader shader)
        {
            if (shader.IsNULL)
            {
                return;
            }
            int idx = shader.ShaderStageIndex;
            shader.BindTexture(context, texDiffuseSlot, TextureResources[DiffuseIdx]);
            shader.BindSampler(context, samplerDiffuseSlot, SamplerResources[DiffuseIdx]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
            samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
            samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
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

        public ShaderPass GetPass(MaterialGeometryRenderCore core, RenderContext context)
        {
            return core.RenderType == RenderType.Transparent && context.IsOITPass ? TransparentPass : MaterialPass;
        }

        private bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            needUpdate = true;
            OnInvalidateRenderer?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
