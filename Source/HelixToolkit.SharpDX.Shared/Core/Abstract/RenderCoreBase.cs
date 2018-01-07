/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    /// <summary>
    /// Base class for all render core classes
    /// </summary>
    public abstract class RenderCoreBase<TModelStruct> : DisposeObject, IRenderCore where TModelStruct : struct
    {
        public Guid GUID { get; } = Guid.NewGuid();

        protected TModelStruct modelStruct;
        protected IConstantBufferProxy modelCB { private set; get; }
        public event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Model matrix
        /// </summary>
        public Matrix ModelMatrix { set; get; } = Matrix.Identity;      
        public IRenderTechnique EffectTechnique { private set; get; }
        public Device Device { get { return EffectTechnique == null ? null : EffectTechnique.Device; } }
        /// <summary>
        /// Is render core has been attached
        /// </summary>
        public bool IsAttached { private set; get; } = false;
        /// <summary>
        /// Call to attach the render core.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        public void Attach(IRenderTechnique technique)
        {
            if (IsAttached)
            {
                return;
            }
            EffectTechnique = technique;
            IsAttached = OnAttach(technique);
        }

        /// <summary>
        /// During attatching render core. Create all local resources. Use Collect(resource) to let object be released automatically during Detach().
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected virtual bool OnAttach(IRenderTechnique technique)
        {
            modelCB = technique.ConstantBufferPool.Register(GetModelConstantBufferDescription());
            return true;
        }

        protected abstract ConstantBufferDescription GetModelConstantBufferDescription();
        /// <summary>
        /// Detach render core. Release all resources
        /// </summary>
        public void Detach()
        {
            IsAttached = false;
            OnDetach();
        }
        /// <summary>
        /// On detaching, default is to release all resources
        /// </summary>
        protected virtual void OnDetach()
        {
            DisposeAndClear();
        }
        /// <summary>
        /// Trigger OnRender function delegate if CanRender()==true
        /// </summary>
        /// <param name="context"></param>
        public void Render(IRenderContext context)
        {
            if (CanRender(context))
            {
                OnUpdatePerModelStruct(ref modelStruct, context);
                OnAttachBuffers(context.DeviceContext);
                OnUploadPerModelConstantBuffers(context.DeviceContext);
                OnBindRasterState(context.DeviceContext);
                switch (context.IsShadowPass)
                {
                    case false:
                        OnRender(context);
                        break;
                    case true:                        
                        OnRenderShadow(context);
                        break;
                }
                PostRender(context);
            }
        }

        protected virtual void OnRenderShadow(IRenderContext context) { }

        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnAttachBuffers(DeviceContext context)
        {
            
        }

        /// <summary>
        /// Set model default raster state
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnBindRasterState(DeviceContext context) { }

        /// <summary>
        /// Actual render function. Used to attach different render states and call the draw call.
        /// </summary>
        protected abstract void OnRender(IRenderContext context);

        protected abstract void OnUpdatePerModelStruct(ref TModelStruct model, IRenderContext context);

        protected virtual void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            modelCB.UploadDataToBuffer(context, ref modelStruct);
        }

        /// <summary>
        /// After calling OnRender. Restore some variables, such as HasInstance etc.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void PostRender(IRenderContext context) { }

        /// <summary>
        /// Check if can render
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanRender(IRenderContext context)
        {
            return IsAttached;
        }

        protected void InvalidateRenderer(object sender, bool e)
        {
            OnInvalidateRenderer?.Invoke(sender, e);
        }

        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }
    }
}
