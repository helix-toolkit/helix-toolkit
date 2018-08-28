/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Shaders;
    using Render;
    using Utilities;
    using ShaderManager;   

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
        public ShaderPass WireframePass { get; private set; } = ShaderPass.NullPass;
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
        private readonly string wireframePassName = DefaultPassNames.Wireframe;

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
            WireframePass = technique[wireframePassName];
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

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
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
        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return WireframePass;
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }
    }
}
