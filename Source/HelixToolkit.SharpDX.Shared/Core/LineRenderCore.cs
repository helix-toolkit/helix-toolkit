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
    using Render;
    using Shaders;
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

        public LineRenderCore()
        {
            LineColor = Color.Black;
            Thickness = 0.5f;
            Smoothness = 0;
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes);
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, DefaultStateBinding);
            OnDraw(deviceContext, InstanceBuffer);
        }
    }
}
