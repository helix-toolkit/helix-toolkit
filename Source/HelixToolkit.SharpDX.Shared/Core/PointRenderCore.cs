/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Render;
    using Components;

    /// <summary>
    /// 
    /// </summary>
    public class PointRenderCore : GeometryRenderCore<PointLineModelStruct>, IPointRenderParams
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.X, value);
            }
            get { return modelStruct.Params.X; }
        }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get { return modelStruct.Params.Y; }
        }
        /// <summary>
        /// Gets or sets the figure.
        /// </summary>
        /// <value>
        /// The figure.
        /// </value>
        public PointFigure Figure
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Z, (int)value);
            }
            get { return (PointFigure)modelStruct.Params.Z; }
        }
        /// <summary>
        /// Gets or sets the figure ratio.
        /// </summary>
        /// <value>
        /// The figure ratio.
        /// </value>
        public float FigureRatio
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
        public Color4 PointColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Color, value);
            }
            get
            {
                return modelStruct.Color.ToColor4();
            }
        }

        private readonly ConstantBufferComponent modelCB;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointRenderCore"/> class.
        /// </summary>
        public PointRenderCore()
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes)));
            Width = 0.5f;
            Height = 0.5f;
            Figure = PointFigure.Ellipse;
            FigureRatio = 0.25f;
            PointColor = Color.Black;
        }
        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            modelStruct.Color = PointColor;
        }

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            modelCB.Upload(deviceContext, ref modelStruct);
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, DefaultStateBinding);          
            DrawPoints(deviceContext, GeometryBuffer.VertexBuffer[0], InstanceBuffer);
        }

        protected sealed override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
            DrawPoints(deviceContext, GeometryBuffer.VertexBuffer[0], InstanceBuffer);
        }

        protected sealed override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (!IsThrowingShadow || ShadowPass.IsNULL)
            { return; }
            ShadowPass.BindShader(deviceContext);
            ShadowPass.BindStates(deviceContext, ShadowStateBinding);
            DrawPoints(deviceContext, GeometryBuffer.VertexBuffer[0], InstanceBuffer);
        }
    }
}
