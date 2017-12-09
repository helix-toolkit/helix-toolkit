using SharpDX.Direct3D;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class BillboardRenderCore : GeometryRenderCore
    {
        public bool FixedSize = true;

        protected override void OnUpdateModelStruct(IRenderMatrices context)
        {
            base.OnUpdateModelStruct(context);
            modelStruct.Params.X = FixedSize ? 1 : 0;
            var type = (GeometryBuffer as IBillboardBufferModel).Type;
            modelStruct.Params.Y = (int)type;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            UpdateModelConstantBuffer(context.DeviceContext);
            EffectTechnique.BindShader(context.DeviceContext);
            EffectTechnique.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            context.DeviceContext.Rasterizer.State = RasterState;
            var buffer = GeometryBuffer as IBillboardBufferModel;
            context.DeviceContext.PixelShader.SetShaderResource(0, buffer.TextureView);
            OnDraw(context.DeviceContext, InstanceBuffer);
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
