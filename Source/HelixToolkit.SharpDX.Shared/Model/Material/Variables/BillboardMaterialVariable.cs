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
    using Utilities;
    using Shaders;
    using System.IO;

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

        public ShaderPass MaterialPass { get; }
        public ShaderPass ShadowPass { get; }

        public ShaderPass MaterialOITPass { get; }

        #region Private Variables
        private readonly int textureSamplerSlot;
        private readonly int shaderTextureSlot;
        private SamplerStateProxy textureSampler;
        //private PointLineMaterialStruct materialStruct;
        private readonly BillboardMaterialCore materialCore;
        #endregion

        public BillboardMaterialVariable(IEffectsManager manager, IRenderTechnique technique, BillboardMaterialCore core)
            : base(manager, technique, DefaultPointLineConstantBufferDesc)
        {
            MaterialPass = technique[DefaultPassNames.Default];
            ShadowPass = technique[DefaultPassNames.ShadowPass];
            MaterialOITPass = technique[DefaultPassNames.OITPass];
            materialCore = core;
            core.PropertyChanged += Core_PropertyChanged;
            shaderTextureSlot = MaterialPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureName);
            textureSamplerSlot = MaterialPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
            textureSampler = Collect(EffectsManager.StateManager.Register(core.SamplerDescription));
        }

        protected override void OnInitializeParameters()
        {
            base.OnInitializeParameters();
            ConstantBuffer.WriteValue(PointLineMaterialStruct.BoolParamsStr, new Bool4(materialCore.FixedSize, false, false, false));
            ConstantBuffer.WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4((int)materialCore.Type, 0, 0, 0));
        }

        private void Core_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(BillboardMaterialCore.FixedSize)))
            {
                ConstantBuffer.WriteValue(PointLineMaterialStruct.BoolParamsStr, new Bool4(materialCore.FixedSize, false, false, false));
            }
            else if (e.PropertyName.Equals(nameof(BillboardMaterialCore.Type)))
            {
                ConstantBuffer.WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4((int)materialCore.Type, 0, 0, 0));
            }
            else if (e.PropertyName.Equals(nameof(BillboardMaterialCore.SamplerDescription)))
            {
                RemoveAndDispose(ref textureSampler);
                textureSampler = Collect(EffectsManager.StateManager.Register(materialCore.SamplerDescription));
            }
            InvalidateRenderer();
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {           
            shaderPass.PixelShader.BindSampler(deviceContext, textureSamplerSlot, textureSampler);
            return true;
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
            materialCore.PropertyChanged -= Core_PropertyChanged;
            base.OnDispose(disposeManagedResources);
        }

        protected override void UpdateInternalVariables(DeviceContextProxy deviceContext)
        {
        }
    }
}
