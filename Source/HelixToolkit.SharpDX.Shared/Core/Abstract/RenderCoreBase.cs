/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
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
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCoreBase{TModelStruct}"/> class.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        public RenderCoreBase(RenderType renderType) : base(renderType)
        {
        }

        /// <summary>
        /// Trigger OnRender function delegate if CanRender()==true
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        public sealed override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (PreRender(context, deviceContext))
            {
                OnRender(context, deviceContext);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool PreRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRenderFlag)
            {
                OnUpdatePerModelStruct(ref modelStruct, context);
                int vertStartSlot = 0;
                if (!OnAttachBuffers(deviceContext, ref vertStartSlot))
                {
                    return false;
                }
                OnBindRasterState(deviceContext, context.IsInvertCullMode);
            }
            return CanRenderFlag;
        }

        public sealed override void Update(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRenderFlag)
            {
                OnUpdate(context, deviceContext);
            }
        }

        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vertStartSlot">Start slot for vertex buffer binding</param>
        protected virtual bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
        {
            return true;
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
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected abstract void OnUpdatePerModelStruct(ref TModelStruct model, RenderContext context);
    }
}
