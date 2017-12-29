/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public enum PointFigure
    {
        Rect,
        Ellipse,
        Cross,
    }
    public interface IPointRenderParams
    {
        Color4 PointColor { set; get; }
        float Width { set; get; }
        float Height { set; get; }
        PointFigure Figure { set; get; }
        float FigureRatio { set; get; }
    }
    public class PointRenderCore : GeometryRenderCore, IPointRenderParams
    {
        public float Width { set; get; } = 0.5f;
        public float Height { set; get; } = 0.5f;
        public PointFigure Figure { set; get; } = PointFigure.Ellipse;
        public float FigureRatio { set; get; } = 0.25f;
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public Color4 PointColor { set; get; } = Color.Black;

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            modelStruct.Color = PointColor;
            modelStruct.Params = new Vector4(Width, Height, (int)Figure, FigureRatio);
        }

        protected override void OnRender(IRenderContext context)
        {
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
