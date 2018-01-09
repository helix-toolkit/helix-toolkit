/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    public class LineRenderCore : GeometryRenderCore<PointLineModelStruct>, ILineRenderParams
    {
        public float Thickness { set; get; } = 0.5f;
        public float Smoothness { set; get; }
        /// <summary>
        /// Final Line Color = LineColor * PerVertexLineColor
        /// </summary>
        public Color4 LineColor { set; get; } = Color.Black;

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, IRenderContext context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            model.Color = LineColor;
            model.Params.X = Thickness;
            model.Params.Y = Smoothness;
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes);
        }

        protected override void OnRender(IRenderContext context)
        {
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }

        protected override void OnRenderShadow(IRenderContext context)
        {
            ShadowPass.BindShader(context.DeviceContext);
            ShadowPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
