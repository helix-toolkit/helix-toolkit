/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using Shaders;
    using Utilities;

    public sealed class BillboardMaterialVariable : MaterialVariable
    {
        /// <summary>
        /// Set texture variable name insider shader for binding
        /// </summary>
        public string ShaderTextureName { get; } = DefaultBufferNames.BillboardTB;
        /// <summary>
        /// Set texture sampler variable name inside shader for binding
        /// </summary>
        public string ShaderTextureSamplerName { get; } = DefaultSamplerStateNames.BillboardTextureSampler;

        public ShaderPass BillboardPass { get; }

        public ShaderPass BillboardOITPass { get; }

        #region Private Variables
        private readonly int textureSamplerSlot;
        private readonly int shaderTextureSlot;
        private SamplerStateProxy textureSampler;
        private readonly BillboardMaterialCore materialCore;
        #endregion        
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardMaterialVariable"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="technique">The technique.</param>
        /// <param name="materialCore">The core.</param>
        /// <param name="billboardPassName">Name of the billboard pass.</param>
        /// <param name="billboardOITPassName">Name of the billboard oit pass.</param>
        public BillboardMaterialVariable(IEffectsManager manager, IRenderTechnique technique, BillboardMaterialCore materialCore,
            string billboardPassName = DefaultPassNames.Default, string billboardOITPassName = DefaultPassNames.OITPass)
            : base(manager, technique, DefaultPointLineConstantBufferDesc, materialCore)
        {
            BillboardPass = technique[billboardPassName];
            BillboardOITPass = technique[billboardOITPassName];
            this.materialCore = materialCore;
            shaderTextureSlot = BillboardPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureName);
            textureSamplerSlot = BillboardPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
            textureSampler = Collect(EffectsManager.StateManager.Register(materialCore.SamplerDescription));
        }

        protected override void OnInitialPropertyBindings()
        {
            base.OnInitialPropertyBindings();
            AddPropertyBinding(nameof(BillboardMaterialCore.FixedSize), () => { WriteValue(PointLineMaterialStruct.BoolParamsStr, new Bool4(materialCore.FixedSize, false, false, false)); });
            AddPropertyBinding(nameof(BillboardMaterialCore.Type), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4((int)materialCore.Type, 0, 0, 0)); });
            AddPropertyBinding(nameof(BillboardMaterialCore.SamplerDescription), () => {
                RemoveAndDispose(ref textureSampler);
                textureSampler = Collect(EffectsManager.StateManager.Register(materialCore.SamplerDescription));
            });
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {           
            shaderPass.PixelShader.BindSampler(deviceContext, textureSamplerSlot, textureSampler);
            return true;
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? BillboardOITPass : BillboardPass;
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            if (bufferModel is IBillboardBufferModel billboardModel)
            {
                deviceContext.SetShaderResource(PixelShader.Type, shaderTextureSlot, billboardModel.TextureView);
                DrawPoints(deviceContext, bufferModel.VertexBuffer[0].ElementCount, instanceCount);
            }
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            textureSampler = null;
            base.OnDispose(disposeManagedResources);
        }
    }
}
