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
    public class LineRenderCore : GeometryRenderCore<PointLineModelStruct>, ILineRenderParams
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public float Thickness
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.X, value);
            }
            get
            {
                return modelStruct.Params.X;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Smoothness
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get { return modelStruct.Params.Y; }
        }

        /// <summary>
        /// Final Line Color = LineColor * PerVertexLineColor
        /// </summary>
        public Color4 LineColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Color, value);
            }
            get { return modelStruct.Color.ToColor4(); }
        }

        #endregion

        private readonly ConstantBufferComponent modelCB;

        public LineRenderCore()
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes)));
            LineColor = Color.Black;
            Thickness = 0.5f;
            Smoothness = 0;
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            modelCB.Upload(deviceContext, ref modelStruct);
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, DefaultStateBinding);
            DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);
        }

        protected override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
            DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);
        }

        protected override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (!IsThrowingShadow || ShadowPass.IsNULL)
            { return; }
            ShadowPass.BindShader(deviceContext);
            ShadowPass.BindStates(deviceContext, ShadowStateBinding);
            DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);
        }
    }
}
