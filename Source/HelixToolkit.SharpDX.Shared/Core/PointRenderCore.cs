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
        private EffectVectorVariable pointParamsVar;
        private EffectVectorVariable colorParamsVar;


        public Vector4 PointParams = new Vector4();
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public Color PointColor = Color.Black;

        public PointRenderCore()
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
            if (base.OnAttach(host, technique))
            {
                pointParamsVar = Collect(Effect.GetVariableByName("vLineParams").AsVector());
                colorParamsVar = Collect(Effect.GetVariableByName("vPointColor").AsVector());
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
            pointParamsVar.Set(ref PointParams);
            colorParamsVar.Set(ref PointColor);
        }
    }
}
