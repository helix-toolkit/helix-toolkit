/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using Shaders;
    using ShaderManager;
    using Utilities;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ColorStripeMaterialCore : MaterialCore
    {
        private Color4 diffuseColor = Color.White;
        /// <summary>
        /// Gets or sets the color of the diffuse.
        /// </summary>
        /// <value>
        /// The color of the diffuse.
        /// </value>
        public Color4 DiffuseColor
        {
            set { Set(ref diffuseColor, value); }
            get { return diffuseColor; }
        }

        private IList<Color4> colorStripe = null;
        public IList<Color4> ColorStripe
        {
            set { Set(ref colorStripe, value); }
            get { return colorStripe; }
        }

        private global::SharpDX.Direct3D11.SamplerStateDescription colorStripeSampler = DefaultSamplers.LinearSamplerClampAni1;
        /// <summary>
        /// Gets or sets the DiffuseMapSampler.
        /// </summary>
        /// <value>
        /// DiffuseMapSampler
        /// </value>
        public global::SharpDX.Direct3D11.SamplerStateDescription ColorStripeSampler
        {
            set { Set(ref colorStripeSampler, value); }
            get { return colorStripeSampler; }
        }

        public override IEffectMaterialVariables CreateMaterialVariables(IEffectsManager manager)
        {
            return new ColorStripeMaterialVariables(manager, this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ColorStripeMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        /// <summary>
        /// <see cref="IEffectMaterialVariables.OnInvalidateRenderer"/> 
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidateRenderer;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private ShaderResourceViewProxy texture;
        private SamplerStateProxy sampler;

        private int texDiffuseSlot;
        private int samplerDiffuseSlot;

        private bool hasTextures = false;

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDiffuseTexName { set; get; } = DefaultBufferNames.ColorStripe1DTB;

        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;

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

        /// <summary>
        ///
        /// </summary>
        public bool RenderDiffuseAlphaMap
        {
            set; get;
        }

        /// <summary>
        ///
        /// </summary>
        public bool RenderNormalMap
        {
            set; get;
        }

        /// <summary>
        ///
        /// </summary>
        public bool RenderDisplacementMap
        {
            set; get;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RenderShadowMap
        {
            set; get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render environment map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render environment map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderEnvironmentMap
        {
            set; get;
        }

        private readonly string defaultShaderPassName = DefaultPassNames.ColorStripe1D;
        public string DefaultShaderPassName
        {
            set;get;
        }

        private bool needUpdate = true;
        private readonly ColorStripeMaterialCore material;
        private IRenderTechnique technique;
        private readonly IDevice3DResources deviceResources;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="material"></param>
        public ColorStripeMaterialVariables(IEffectsManager manager, ColorStripeMaterialCore material)
        {
            this.material = material;
            deviceResources = manager;
            needUpdate = true;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texDiffuseSlot = -1;
            samplerDiffuseSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            CreateTextureViews();
            CreateSamplers();
            this.PropertyChanged += (s, e) => { OnInvalidateRenderer?.Invoke(this, EventArgs.Empty); };
        }

        public bool Attach(IRenderTechnique technique)
        {
            this.technique = technique;
            MaterialPass = technique[defaultShaderPassName];
            UpdateMappings(MaterialPass);
            return !MaterialPass.IsNULL;
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            needUpdate = true;
            if (IsDisposed)
            {
                return;
            }
            if (e.PropertyName.Equals(nameof(ColorStripeMaterialCore.ColorStripe)))
            {
                CreateTextureView((sender as ColorStripeMaterialCore).ColorStripe);
            }          
            else if (e.PropertyName.Equals(nameof(ColorStripeMaterialCore.ColorStripeSampler)))
            {
                RemoveAndDispose(ref sampler);
                sampler = Collect(statePoolManager.Register((sender as ColorStripeMaterialCore).ColorStripeSampler));
            }
            
            OnInvalidateRenderer?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateTextureView(IList<Color4> colors)
        {
            RemoveAndDispose(ref texture);
            texture = (colors == null || colors.Count == 0) ? null : Collect(new ShaderResourceViewProxy(deviceResources.Device));
            texture?.CreateViewFromColorArray(colors.ToArray());
            hasTextures = texture != null;
        }

        private void CreateTextureViews()
        {
            if (material != null)
            {
                CreateTextureView(material.ColorStripe);
            }
            else
            {
                RemoveAndDispose(ref texture);
            }
        }

        private void CreateSamplers()
        {
            RemoveAndDispose(ref sampler);
            if (material != null)
            {
                sampler = Collect(statePoolManager.Register(material.ColorStripeSampler));
            }
        }

        private void AssignVariables(ref ModelStruct modelstruct)
        {
            modelstruct.Diffuse = material.DiffuseColor;
            modelstruct.HasDiffuseMap = RenderDiffuseMap && texture != null ? 1 : 0;
            modelstruct.HasNormalMap = 0;
            modelstruct.HasDiffuseAlphaMap = 0;
            modelstruct.RenderShadowMap = 0;
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
            if (hasTextures)
            {
                OnBindMaterialTextures(context, shaderPass.PixelShader);
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
            shader.BindTexture(context, texDiffuseSlot, texture);
            shader.BindSampler(context, samplerDiffuseSlot, sampler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
            samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                technique = null;
                material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                texture = null;
                sampler = null;
                OnInvalidateRenderer = null;
            }

            base.OnDispose(disposeManagedResources);
        }
    }
}
