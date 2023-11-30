/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        using Render;
        using Shaders;
        using System.Runtime.CompilerServices;
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public class LineMaterialVariable : MaterialVariable
        {
            private readonly LineMaterialCore material;

            public ShaderPass LinePass
            {
                get;
            }
            public ShaderPass ShadowPass
            {
                get;
            }
            public ShaderPass DepthPass
            {
                get;
            }
            /// <summary>
            /// Set texture variable name insider shader for binding
            /// </summary>
            public string ShaderTextureName { get; } = DefaultBufferNames.DiffuseMapTB;
            /// <summary>
            /// Set texture sampler variable name inside shader for binding
            /// </summary>
            public string ShaderTextureSamplerName { get; } = DefaultSamplerStateNames.BillboardTextureSampler;

            private readonly int textureSamplerSlot;
            private readonly int shaderTextureSlot;
            private SamplerStateProxy textureSampler;
            private ShaderResourceViewProxy textureResource;
            private readonly ITextureResourceManager textureManager;
            /// <summary>
            /// Initializes a new instance of the <see cref="LineMaterialVariable"/> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="technique">The technique.</param>
            /// <param name="materialCore">The material core.</param>
            /// <param name="defaultPassName">Default pass name</param>
            public LineMaterialVariable(IEffectsManager manager, IRenderTechnique technique, LineMaterialCore materialCore,
                string defaultPassName = DefaultPassNames.Default)
                : base(manager, technique, DefaultPointLineConstantBufferDesc, materialCore)
            {
                textureManager = manager.MaterialTextureManager;
                LinePass = technique[defaultPassName];
                ShadowPass = technique[DefaultPassNames.ShadowPass];
                DepthPass = technique[DefaultPassNames.DepthPrepass];
                this.material = materialCore;
                shaderTextureSlot = LinePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureName);
                textureSamplerSlot = LinePass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
                textureSampler = EffectsManager.StateManager.Register(materialCore.SamplerDescription);
            }

            protected override void OnInitialPropertyBindings()
            {
                AddPropertyBinding(nameof(LineMaterialCore.LineColor), () => { WriteValue(PointLineMaterialStruct.ColorStr, material.LineColor); });
                AddPropertyBinding(nameof(LineMaterialCore.Thickness), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector2(material.Thickness, material.Smoothness)); });
                AddPropertyBinding(nameof(LineMaterialCore.Smoothness), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector2(material.Thickness, material.Smoothness)); });
                AddPropertyBinding(nameof(LineMaterialCore.TextureScale), () => { WriteValue(PointLineMaterialStruct.TextureScaleStr, material.TextureScale); });
                AddPropertyBinding(nameof(LineMaterialCore.AlphaThreshold), () => { WriteValue(PointLineMaterialStruct.AlphaThresholdStr, material.AlphaThreshold); });
                AddPropertyBinding(nameof(LineMaterialCore.EnableDistanceFading), () => { WriteValue(PointLineMaterialStruct.EnableDistanceFading, material.EnableDistanceFading ? 1 : 0); });
                AddPropertyBinding(nameof(LineMaterialCore.FadingNearDistance), () => { WriteValue(PointLineMaterialStruct.FadeNearDistance, material.FadingNearDistance); });
                AddPropertyBinding(nameof(LineMaterialCore.FadingFarDistance), () => { WriteValue(PointLineMaterialStruct.FadeFarDistance, material.FadingFarDistance); });
                AddPropertyBinding(nameof(LineMaterialCore.FixedSize), () => { WriteValue(PointLineMaterialStruct.FixedSize, material.FixedSize); });
                AddPropertyBinding(nameof(LineMaterialCore.Texture), () =>
                {
                    CreateTextureView(material.Texture);
                    WriteValue(PointLineMaterialStruct.HasTextureStr, textureResource != null ? 1 : 0);
                });
                AddPropertyBinding(nameof(LineMaterialCore.SamplerDescription), () =>
                {
                    var newSampler = EffectsManager.StateManager.Register(material.SamplerDescription);
                    RemoveAndDispose(ref textureSampler);
                    textureSampler = newSampler;
                });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void CreateTextureView(TextureModel texture)
            {
                var newRes = texture == null ? null : textureManager.Register(texture);
                RemoveAndDispose(ref textureResource);
                textureResource = newRes;
            }

            public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
            {
                DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
            }

            public override ShaderPass GetPass(RenderType renderType, RenderContext context)
            {
                return LinePass;
            }

            public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
            {
                return ShadowPass;
            }

            public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
            {
                return ShaderPass.NullPass;
            }

            public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
            {
                return DepthPass;
            }

            public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
            {
                if (textureResource != null)
                {
                    shaderPass.PixelShader.BindTexture(deviceContext, shaderTextureSlot, textureResource);
                    shaderPass.PixelShader.BindSampler(deviceContext, textureSamplerSlot, textureSampler);
                }
                return true;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                RemoveAndDispose(ref textureResource);
                RemoveAndDispose(ref textureSampler);
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}
