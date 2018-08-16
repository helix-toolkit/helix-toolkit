/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private IList<Color4> colorStripeX = null;
        /// <summary>
        /// Gets or sets the color stripe x. Use texture coordinate X for sampling
        /// </summary>
        /// <value>
        /// The color stripe x.
        /// </value>
        public IList<Color4> ColorStripeX
        {
            set { Set(ref colorStripeX, value); }
            get { return colorStripeX; }
        }

        private IList<Color4> colorStripeY = null;
        /// <summary>
        /// Gets or sets the color stripe y. Use texture coordinate Y for sampling
        /// </summary>
        /// <value>
        /// The color stripe y.
        /// </value>
        public IList<Color4> ColorStripeY
        {
            set { Set(ref colorStripeY, value); }
            get { return colorStripeY; }
        }

        private bool colorStripeXEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether [color stripe x enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [color stripe x enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorStripeXEnabled
        {
            set { Set(ref colorStripeXEnabled, value); }
            get { return colorStripeXEnabled; }
        }

        private bool colorStripeYEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether [color stripe y enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [color stripe y enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool ColorStripeYEnabled
        {
            set { Set(ref colorStripeYEnabled, value); }
            get { return colorStripeYEnabled; }
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

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new ColorStripeMaterialVariables(manager, technique, this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ColorStripeMaterialVariables : MaterialVariable
    {
        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] textures = new ShaderResourceViewProxy[2];
        private SamplerStateProxy sampler;

        private int texStripeXSlot, texStripeYSlot;
        private int samplerDiffuseSlot;

        private uint textureIndex = 0;

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderStripeTexXName { set; get; } = DefaultBufferNames.ColorStripe1DXTB;
        public string ShaderStripeTexYName { set; get; } = DefaultBufferNames.ColorStripe1DYTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;

        private readonly string defaultShaderPassName = DefaultPassNames.ColorStripe1D;
        public override string DefaultShaderPassName
        {
            set; get;
        }

        private readonly ColorStripeMaterialCore material;
        private readonly IDevice3DResources deviceResources;
        private PhongMaterialStruct materialStruct = new PhongMaterialStruct() { UVTransformR1 = new Vector4(1, 0, 0, 0), UVTransformR2 = new Vector4(0, 1, 0, 0) };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="technique"></param>
        /// <param name="material"></param>
        public ColorStripeMaterialVariables(IEffectsManager manager, IRenderTechnique technique, ColorStripeMaterialCore material)
            : base(manager, technique, DefaultMeshConstantBufferDesc)
        {
            this.material = material;
            deviceResources = manager;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texStripeXSlot = texStripeYSlot = -1;
            samplerDiffuseSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            MaterialPass = technique[defaultShaderPassName];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }
            if (e.PropertyName.Equals(nameof(ColorStripeMaterialCore.ColorStripeX)))
            {
                CreateTextureView((sender as ColorStripeMaterialCore).ColorStripeX, 0);
            }
            else if (e.PropertyName.Equals(nameof(ColorStripeMaterialCore.ColorStripeY)))
            {
                CreateTextureView((sender as ColorStripeMaterialCore).ColorStripeY, 1);
            }
            else if (e.PropertyName.Equals(nameof(ColorStripeMaterialCore.ColorStripeSampler)))
            {
                RemoveAndDispose(ref sampler);
                sampler = Collect(statePoolManager.Register((sender as ColorStripeMaterialCore).ColorStripeSampler));
            }
            NotifyUpdateNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateTextureView(IList<Color4> colors, int which)
        {
            RemoveAndDispose(ref textures[which]);
            textures[which] = (colors == null || colors.Count == 0) ? null : Collect(new ShaderResourceViewProxy(deviceResources.Device));
            textures[which]?.CreateViewFromColorArray(colors.ToArray());
            if (textures[which] != null)
            {
                textureIndex |= 1u << which;
            }
            else
            {
                textureIndex &= ~(1u << which);
            }
        }

        private void CreateTextureViews()
        {
            if (material != null)
            {
                CreateTextureView(material.ColorStripeX, 0);
                CreateTextureView(material.ColorStripeY, 1);
            }
            else
            {
                for (int i = 0; i < textures.Length; ++i)
                {
                    RemoveAndDispose(ref textures[i]);
                }
                textureIndex = 0;
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

        protected override void UpdateInternalVariables(DeviceContextProxy context)
        {
            if (NeedUpdate)
            {
                materialStruct = new PhongMaterialStruct
                {
                    Diffuse = material.DiffuseColor,
                    HasDiffuseMap = material.ColorStripeXEnabled && (textureIndex & 1u) != 0 ? 1 : 0,
                    HasDiffuseAlphaMap = material.ColorStripeYEnabled && (textureIndex & 1u << 1) != 0 ? 1 : 0,
                    UVTransformR1 = new Vector4(1, 0, 0, 0),
                    UVTransformR2 = new Vector4(0, 1, 0, 0)
                };
                NeedUpdate = false;
            }
        }

        protected override void WriteMaterialDataToConstantBuffer(DataStream cbStream)
        {
            cbStream.Write(materialStruct);
        }

        protected override bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if (textureIndex != 0)
            {
                OnBindMaterialTextures(deviceContext, shaderPass.PixelShader);
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
            shader.BindTexture(context, texStripeXSlot, textures[0]);
            shader.BindTexture(context, texStripeYSlot, textures[1]);
            shader.BindSampler(context, samplerDiffuseSlot, sampler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texStripeXSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderStripeTexXName);
            texStripeYSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderStripeTexYName);
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
                material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                for (int i = 0; i < textures.Length; ++i)
                {
                    textures[i] = null;
                }
                sampler = null;
            }

            base.OnDispose(disposeManagedResources);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return MaterialPass;
        }
    }
}
