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

    public class InstancingBillboardRenderCore : BillboardRenderCore
    {
        private IElementsBufferModel parameterBufferModel;
        public IElementsBufferModel ParameterBuffer
        {
            set
            {
                var old = parameterBufferModel;
                if(SetAffectsCanRenderFlag(ref parameterBufferModel, value))
                {
                    if (old != null)
                    {
                        old.OnElementChanged -= OnElementChanged;
                    }
                    if (parameterBufferModel != null)
                    {
                        parameterBufferModel.OnElementChanged += OnElementChanged;
                    }
                }                
            }
            get { return parameterBufferModel; }
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return base.OnUpdateCanRenderFlag() && InstanceBuffer != null && InstanceBuffer.HasElements;
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.HasInstanceParams = ParameterBuffer != null && ParameterBuffer.HasElements ? 1 : 0;
        }

        protected override bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
        {
            if (base.OnAttachBuffers(context, ref vertStartSlot))
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
