#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal { set; get; } = false;

        protected override void OnUpdateModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdateModelStruct(ref model, context);
            model.InvertNormal = InvertNormal ? 1 : 0;
        }

        protected override void OnRender(IRenderMatrices context)
        {                  
            UpdateModelConstantBuffer(context.DeviceContext);
            if (!UpdateMaterialConstantBuffer(context.DeviceContext))
            {
                return;
            }
            EffectTechnique[0].BindShader(context.DeviceContext);
            EffectTechnique[0].BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            if(!BindMaterialTextures(context.DeviceContext, EffectTechnique[0].GetShader(ShaderStage.Pixel)))
            {
                return;
            }
            context.DeviceContext.Rasterizer.State = RasterState;
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
