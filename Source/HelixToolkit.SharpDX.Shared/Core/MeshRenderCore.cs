using System;
using SharpDX.Direct3D11;
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal { set; get; } = false;

        protected override void OnUpdateModelStruct(IRenderMatrices context)
        {
            base.OnUpdateModelStruct(context);
            modelStruct.InvertNormal = InvertNormal ? 1 : 0;
        }

        protected override void OnRender(IRenderMatrices context)
        {                  
            SetMaterialVariables(GeometryBuffer.Geometry as MeshGeometry3D, context);
            UpdateModelConstantBuffer(context.DeviceContext);
            EffectTechnique.BindShader(context.DeviceContext);
            EffectTechnique.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            context.DeviceContext.Rasterizer.State = RasterState;
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
