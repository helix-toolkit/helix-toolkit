#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public BufferModel MeshBuffer { set; get; } = null;
        public MeshRenderCore()
        {
            OnRender = (context) => 
            {
                SetModelWorldMatrix(ModelMatrix * context.WorldMatrix);
                SetMaterialVariables(Geometry);
                MeshBuffer.Attach(context.DeviceContext, InstanceBuffer);
            };
        }

        protected override bool CanRender()
        {
            return base.CanRender() && MeshBuffer != null;
        }
    }
}
