using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class InstancingMeshRenderCore : MeshRenderCore
    {
        public IElementsBufferModel ParameterBuffer { set; get; }
        protected override bool CanRender()
        {
            return base.CanRender() && InstanceBuffer != null && InstanceBuffer.HasElements;
        }

        protected override void OnAttachBuffers(DeviceContext context)
        {
            base.OnAttachBuffers(context);
            ParameterBuffer?.AttachBuffer(context, 2);
        }
    }
}
