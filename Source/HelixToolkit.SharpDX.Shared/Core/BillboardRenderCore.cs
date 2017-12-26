/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;

    public class BillboardRenderCore : GeometryRenderCore
    {
        public bool FixedSize = true;
        private SamplerStateDescription samplerDescription = DefaultSamplers.LinearSamplerWrapAni8;
        public SamplerStateDescription SamplerDescription
        {
            set
            {
                samplerDescription = value;
                if (TextureSampler == null)
                {
                    return;
                }
                TextureSampler.Description = value;
            }
            get
            {
                return samplerDescription;
            }
        }

        public SamplerProxy TextureSampler { private set; get; }

        public virtual string ShaderTextureSamplerName { get { return DefaultSamplerStateNames.BillboardTextureSampler; } }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                TextureSampler = new SamplerProxy(technique.EffectsManager);
                TextureSampler.Description = SamplerDescription;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            TextureSampler = null;
            base.OnDetach();
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.BoolParams.X = FixedSize;
            var type = (GeometryBuffer as IBillboardBufferModel).Type;
            model.Params.X = (int)type;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            context.DeviceContext.Rasterizer.State = RasterState;
            BindBillboardTexture(context.DeviceContext, DefaultShaderPass.GetShader(ShaderStage.Pixel));
            OnDraw(context.DeviceContext, InstanceBuffer);
        }

        protected virtual void BindBillboardTexture(DeviceContext context, IShader shader)
        {
            var buffer = GeometryBuffer as IBillboardBufferModel;
            shader.BindTexture(context, buffer.ShaderTextureName, buffer.TextureView);
            shader.BindSampler(context, ShaderTextureSamplerName, TextureSampler.SamplerState);
        }

        protected override void OnDraw(DeviceContext context, IElementsBufferModel instanceModel)
        {
            var billboardGeometry = GeometryBuffer.Geometry as IBillboardText;
            var vertexCount = billboardGeometry.BillboardVertices.Count;
            if (instanceModel == null || !instanceModel.HasElements)
            {
                context.Draw(vertexCount, 0);
            }
            else
            {
                context.DrawInstanced(vertexCount, instanceModel.Buffer.Count, 0, 0);
            }
        }
    }
}
