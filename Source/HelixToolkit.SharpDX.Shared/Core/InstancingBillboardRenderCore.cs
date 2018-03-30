/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class InstancingBillboardRenderCore : BillboardRenderCore
    {
        private IElementsBufferModel parameterBufferModel;
        public IElementsBufferModel ParameterBuffer
        {
            set
            {
                if (parameterBufferModel == value)
                {
                    return;
                }
                if (parameterBufferModel != null)
                {
                    parameterBufferModel.OnElementChanged -= ParameterBufferModel_OnElementChanged;
                }
                parameterBufferModel = value;
                if (parameterBufferModel != null)
                {
                    parameterBufferModel.OnElementChanged += ParameterBufferModel_OnElementChanged;
                }
            }
            get { return parameterBufferModel; }
        }

        private void ParameterBufferModel_OnElementChanged(object sender, EventArgs e)
        {
            InvalidateRenderer();
        }
        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && InstanceBuffer != null && InstanceBuffer.HasElements;
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.HasInstanceParams = ParameterBuffer != null && ParameterBuffer.HasElements ? 1 : 0;
        }

        protected override void OnAttachBuffers(DeviceContext context, ref int vertStartSlot)
        {
            base.OnAttachBuffers(context, ref vertStartSlot);
            ParameterBuffer?.AttachBuffer(context, ref vertStartSlot);
        }
    }
}
