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
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            OnDraw(context.DeviceContext, InstanceBuffer);
        }

        protected override void OnDraw(DeviceContext context, IElementsBufferModel instanceModel)
        {
            var vertexCount = GeometryBuffer.Geometry.Positions.Count;
            var type = (GeometryBuffer as IBillboardBufferModel).Type;
            if (instanceModel == null || !instanceModel.HasElements)
            {
                switch (type)
                {
                    case BillboardType.MultipleText:
                        // Use foreground shader to draw text
                        // EffectTechnique.GetPassByIndex(0).Apply(context);

                        // --- draw text, foreground vertex is beginning from 0.
                        context.Draw(vertexCount, 0);
                        break;
                    case BillboardType.SingleText:
                        //if (vertexCount == 8)
                        //{
                        //    var half = vertexCount / 2;
                        //    // Use background shader to draw background first
                        //    EffectTechnique.GetPassByIndex(1).Apply(context);
                        //    // --- draw background, background vertex is beginning from middle. <see cref="BillboardSingleText3D"/>
                        //    context.Draw(half, half);

                        //    // Use foreground shader to draw text
                        //    EffectTechnique.GetPassByIndex(0).Apply(context);

                        //    // --- draw text, foreground vertex is beginning from 0.
                        //    context.Draw(half, 0);
                        //}
                        break;
                    case BillboardType.SingleImage:
                        //// Use foreground shader to draw text
                        //EffectTechnique.GetPassByIndex(2).Apply(context);
                        //// --- draw text, foreground vertex is beginning from 0.
                        //context.Draw(vertexCount, 0);
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case BillboardType.MultipleText:
                        //// Use foreground shader to draw text
                        //EffectTechnique.GetPassByIndex(0).Apply(context);

                        //// --- draw text, foreground vertex is beginning from 0.
                        //context.DrawInstanced(vertexCount, instanceModel.Buffer.Count, 0, 0);
                        break;
                    case BillboardType.SingleText:
                        //if (vertexCount == 8)
                        //{
                        //    var half = vertexCount / 2;
                        //    // Use background shader to draw background first
                        //    EffectTechnique.GetPassByIndex(1).Apply(context);
                        //    // --- draw background, background vertex is beginning from middle. <see cref="BillboardSingleText3D"/>
                        //    context.DrawInstanced(half, instanceModel.Buffer.Count, half, 0);

                        //    // Use foreground shader to draw text
                        //    EffectTechnique.GetPassByIndex(0).Apply(context);

                        //    // --- draw text, foreground vertex is beginning from 0.
                        //    context.DrawInstanced(half, instanceModel.Buffer.Count, 0, 0);
                        //}
                        break;
                    case BillboardType.SingleImage:
                        //// Use foreground shader to draw text
                        //EffectTechnique.GetPassByIndex(2).Apply(context);
                        //// --- draw text, foreground vertex is beginning from 0.
                        //context.DrawInstanced(vertexCount, instanceModel.Buffer.Count, 0, 0);
                        break;
                }
            }
        }
    }
}
