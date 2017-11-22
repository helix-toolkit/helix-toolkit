using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class InstancingMeshRenderCore : MeshRenderCore
    {
        public IInstanceBufferModel ParameterBuffer { set; get; }
        protected override bool CanRender()
        {
            return base.CanRender() && InstanceBuffer != null && InstanceBuffer.HasInstance;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            SetShaderVariables(context);
            SetMaterialVariables(GeometryBuffer.Geometry as MeshGeometry3D);
            SetRasterState(context.DeviceContext);
            GeometryBuffer.AttachBuffers(context.DeviceContext, this.VertexLayout, 0);
            InstanceBuffer?.AttachBuffer(context.DeviceContext, 1);
            ParameterBuffer?.AttachBuffer(context.DeviceContext, 2);
            EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
