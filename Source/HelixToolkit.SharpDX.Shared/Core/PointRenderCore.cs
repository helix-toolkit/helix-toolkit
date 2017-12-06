using System;
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class PointRenderCore : GeometryRenderCore
    {
        //private EffectVectorVariable pointParamsVar;
        //private EffectVectorVariable colorParamsVar;


        public Vector4 PointParams = new Vector4();
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public Color4 PointColor = Color.Black;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                //pointParamsVar = Collect(Effect.GetVariableByName(ShaderVariableNames.PointParams).AsVector());
                //colorParamsVar = Collect(Effect.GetVariableByName(ShaderVariableNames.PointColor).AsVector());
                return true;
            }
            else
            {
                return false;
            }
        }

        //protected override void SetShaderVariables(IRenderMatrices matrices)
        //{
        //    base.SetShaderVariables(matrices);
        //    pointParamsVar.Set(ref PointParams);
        //    colorParamsVar.Set(ref PointColor);
        //}

        protected override void OnRender(IRenderMatrices context)
        {
            //EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
            //OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
