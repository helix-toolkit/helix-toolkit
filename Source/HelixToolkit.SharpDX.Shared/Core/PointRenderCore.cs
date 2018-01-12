/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Render;
    public class PointRenderCore : GeometryRenderCore<PointLineModelStruct>, IPointRenderParams
    {
        public virtual float Width
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.X, value);
            }
            get { return modelStruct.Params.X; }
        }

        public virtual float Height
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get { return modelStruct.Params.Y; }
        }
        public virtual PointFigure Figure
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Z, (int)value);
            }
            get { return (PointFigure)modelStruct.Params.Z; }
        }
        public virtual float FigureRatio
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.W, value);
            }
            get { return modelStruct.Params.W; }
        }
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public virtual Color4 PointColor { set; get; } = Color.Black;

        public PointRenderCore()
        {
            Width = 0.5f;
            Height = 0.5f;
            Figure = PointFigure.Ellipse;
            FigureRatio = 0.25f;
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, IRenderContext context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            modelStruct.Color = PointColor;
            modelStruct.Params = new Vector4(Width, Height, (int)Figure, FigureRatio);
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes);
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(deviceContext, InstanceBuffer);
        }
    }
}
