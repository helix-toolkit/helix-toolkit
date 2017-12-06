using System;
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class LineRenderCore : GeometryRenderCore
    {
        /// <summary>
        /// Line parameters, X is Thickness, Y is Smoothness, ZW are unused
        /// </summary>
        public Vector4 LineParams = new Vector4();
        /// <summary>
        /// Final Line Color = LineColor * PerVertexLineColor
        /// </summary>
        public Color4 LineColor = Color.Black;
        //private EffectVectorVariable lineParamsVar;
        //private EffectVectorVariable lineColorVar;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                //lineParamsVar = Collect(Effect.GetVariableByName(ShaderVariableNames.LineParams).AsVector());
                //lineColorVar = Collect(Effect.GetVariableByName(ShaderVariableNames.LineColor).AsVector());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderMatrices context)
        {
            //EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
            //OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
