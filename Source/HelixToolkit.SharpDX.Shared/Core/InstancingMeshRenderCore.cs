/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;

    public class InstancingMeshRenderCore : MeshRenderCore
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

        protected override bool OnAttach(IRenderTechnique technique)
        {
            return base.OnAttach(technique);
        }
        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && InstanceBuffer != null && InstanceBuffer.HasElements;
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, RenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.HasInstanceParams = ParameterBuffer != null && ParameterBuffer.HasElements ? 1 : 0;
        }

        protected override bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
        {
            if(base.OnAttachBuffers(context, ref vertStartSlot))
            {
                ParameterBuffer?.AttachBuffer(context, ref vertStartSlot);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
