using System;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal = false;

        private EffectScalarVariable bInvertNormalVar;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                bInvertNormalVar = Collect(Effect.GetVariableByName(ShaderVariableNames.InvertNormal).AsScalar());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void SetShaderVariables(IRenderMatrices context)
        {
            base.SetShaderVariables(context);
            bInvertNormalVar.Set(InvertNormal);
        }

        protected override void OnRender(IRenderMatrices context)
        {                      
            EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
            SetMaterialVariables(GeometryBuffer.Geometry as MeshGeometry3D, context);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
