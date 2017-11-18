#if !NETFX_CORE
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal { set; get; } = false;

        private EffectScalarVariable bInvertNormalVar;
        public MeshRenderCore()
        {
            OnRender = (context) => 
            {               
                SetConstantVariables(context);
                SetMaterialVariables(Geometry as MeshGeometry3D);
                SetRasterState(context.DeviceContext);
                EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
                GeometryBuffer.AttachAndDraw(context.DeviceContext, InstanceBuffer);
            };
        }

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                bInvertNormalVar = Collect(Effect.GetVariableByName("bInvertNormal").AsScalar());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void SetConstantVariables(IRenderMatrices context)
        {
            base.SetConstantVariables(context);
            bInvertNormalVar.Set(InvertNormal);
        }
    }
}
