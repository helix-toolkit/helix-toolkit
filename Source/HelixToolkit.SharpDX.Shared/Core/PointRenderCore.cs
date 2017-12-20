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
        public Vector4 PointParams = new Vector4();
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public Color4 PointColor = Color.Black;

        protected override void OnUpdateModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdateModelStruct(ref model, context);
            modelStruct.Color = PointColor;
            modelStruct.Params = PointParams;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            UpdateModelConstantBuffer(context.DeviceContext);
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            context.DeviceContext.Rasterizer.State = RasterState;
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
