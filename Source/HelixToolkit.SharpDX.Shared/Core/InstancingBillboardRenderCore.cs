using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class InstancingBillboardRenderCore : BillboardRenderCore
    {
        public IInstanceBufferModel ParameterBuffer { set; get; }
        protected override bool CanRender()
        {
            return base.CanRender() && InstanceBuffer != null && InstanceBuffer.HasInstance;
        }

        protected override void PreRender(IRenderMatrices context)
        {
            base.PreRender(context);
            ParameterBuffer?.AttachBuffer(context.DeviceContext, 2);
        }

        protected override void PostRender(IRenderMatrices context)
        {
            ParameterBuffer?.ResetHasInstanceVariable();
            base.PostRender(context);
        }
    }
}
