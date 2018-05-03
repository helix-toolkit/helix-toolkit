/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;


    /// <summary>
    /// Base class for all render core classes
    /// </summary>
    public abstract class RenderCoreBase<TModelStruct> : RenderCore where TModelStruct : struct
    {
        #region Properties
        /// <summary>
        /// The model structure
        /// </summary>
        protected TModelStruct modelStruct;
        /// <summary>
        /// Gets or sets the model cb.
        /// </summary>
        /// <value>
        /// The model cb.
        /// </value>
        protected ConstantBufferProxy ModelConstBuffer { private set; get; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCoreBase{TModelStruct}"/> class.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        public RenderCoreBase(RenderType renderType) : base(renderType)
        {
        }

        /// <summary>
        /// During attatching render core. Create all local resources. Use Collect(resource) to let object be released automatically during Detach().
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            ModelConstBuffer = technique.ConstantBufferPool.Register(GetModelConstantBufferDescription());
            return true;
        }        

        /// <summary>
        /// Gets the model constant buffer description.
        /// </summary>
        /// <returns></returns>
        protected abstract ConstantBufferDescription GetModelConstantBufferDescription();

        /// <summary>
        /// Trigger OnRender function delegate if CanRender()==true
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRender(context))
            {
                OnUpdatePerModelStruct(ref modelStruct, context);
                int vertStartSlot = 0;
                OnAttachBuffers(deviceContext, ref vertStartSlot);
                OnUploadPerModelConstantBuffers(deviceContext);
                OnBindRasterState(deviceContext, context.IsInvertCullMode);
                switch (context.IsShadowPass)
                {
                    case true:
                        switch (context.IsCustomPass)
                        {
                            case true:
                                OnRenderCustom(context, deviceContext, null);
                                break;
                            default:
                                OnRenderShadow(context, deviceContext);
                                break;
                        }
                        break;
                    default:
                        switch (context.IsCustomPass)
                        {
                            case true:
                                OnRenderCustom(context, deviceContext, null);
                                break;
                            default:
                                OnRender(context, deviceContext);
                                break;
                        }
                        break;
                }
                PostRender(context);
            }
        }

        public override void Update(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRender(context))
            {
                OnUpdate(context, deviceContext);
            }
        }

        /// <summary>
        /// Called when [render shadow].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext"></param>
        protected virtual void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        { }

        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vertStartSlot">Start slot for vertex buffer binding</param>
        protected virtual void OnAttachBuffers(DeviceContext context, ref int vertStartSlot)
        {            
        }

        /// <summary>
        /// Set model default raster state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isInvertCullMode"></param>
        protected virtual void OnBindRasterState(DeviceContextProxy context, bool isInvertCullMode)
        { }

        /// <summary>
        /// Actual render function. Used to attach different render states and call the draw call.
        /// </summary>
        protected abstract void OnRender(RenderContext context, DeviceContextProxy deviceContext);

        /// <summary>
        /// Only used for running compute shader such as in particle system.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        protected virtual void OnUpdate(RenderContext context, DeviceContextProxy deviceContext) { }

        /// <summary>
        /// Render function for custom shader pass. Used to do special effects
        /// </summary>
        protected virtual void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        { }

        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected abstract void OnUpdatePerModelStruct(ref TModelStruct model, RenderContext context);

        /// <summary>
        /// Called when [upload per model constant buffers].
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            ModelConstBuffer.UploadDataToBuffer(context, ref modelStruct);
        }

        /// <summary>
        /// After calling OnRender. Restore some variables, such as HasInstance etc.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void PostRender(RenderContext context)
        { }

        /// <summary>
        /// Check if can render
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanRender(RenderContext context)
        {
            return IsAttached;
        }
    }
}
