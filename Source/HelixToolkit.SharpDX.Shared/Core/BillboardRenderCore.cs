using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    public class BillboardRenderCore : GeometryRenderCore
    {
        public bool FixedSize = true;

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.Params.X = FixedSize ? 1 : 0;
            var type = (GeometryBuffer as IBillboardBufferModel).Type;
            model.Params.Y = (int)type;
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
            int slot = shader.TryGetTextureIndex(buffer.TextureName);
            context.AttachShaderResources(shader.ShaderType, slot, buffer.TextureView);
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
