using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class InstancingMeshRenderCore : PatchMeshRenderCore
    {
        public IElementsBufferModel ParameterBuffer { set; get; }
        protected override bool CanRender()
        {
            return base.CanRender() && InstanceBuffer != null && InstanceBuffer.HasElements;
        }

        protected override void OnUpdateModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdateModelStruct(ref model, context);
            model.HasInstanceParams = ParameterBuffer != null && ParameterBuffer.HasElements ? 1 : 0;
        }

        protected override void OnAttachBuffers(DeviceContext context)
        {
            base.OnAttachBuffers(context);
            ParameterBuffer?.AttachBuffer(context, 2);
        }
    }
}
