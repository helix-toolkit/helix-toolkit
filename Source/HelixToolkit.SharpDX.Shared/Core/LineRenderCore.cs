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
        private EffectVectorVariable lineParamsVar;
        private EffectVectorVariable lineColorVar;

        public LineRenderCore()
        {
            OnRender = (context) =>
            {
                SetShaderVariables(context);
                SetRasterState(context.DeviceContext);
                EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
                GeometryBuffer.AttachBuffersAndDraw(context.DeviceContext, this.VertexLayout, InstanceBuffer);
            };
        }

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                lineParamsVar = Collect(Effect.GetVariableByName("vLineParams").AsVector());
                lineColorVar = Collect(Effect.GetVariableByName("vLineColor").AsVector());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void SetShaderVariables(IRenderMatrices matrices)
        {
            base.SetShaderVariables(matrices);
            lineParamsVar.Set(ref LineParams);
            lineColorVar.Set(ref LineColor);
        }
    }
}
