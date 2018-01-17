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
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Utilities;
    using Render;
    /// <summary>
    /// Base class for all render core classes
    /// </summary>
    public abstract class RenderCoreBase<TModelStruct> : DisposeObject, IRenderCore where TModelStruct : struct
    {
        /// <summary>
        /// <see cref="IRenderCore.OnInvalidateRenderer"/>
        /// </summary>
        public event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// <see cref="IGUID.GUID"/>
        /// </summary>
        public Guid GUID { get; } = Guid.NewGuid();

        public bool IsEmpty { protected set; get; } = false;

        private bool isThrowingShadow = false;
        /// <summary>
        /// <see cref="IThrowingShadow.IsThrowingShadow"/>
        /// </summary>
        public bool IsThrowingShadow
        {
            set
            {
                if (Set(ref isThrowingShadow, value))
                {
                    InvalidateRenderer();
                }
            }
            get { return isThrowingShadow; }
        }


        /// <summary>
        /// Model matrix
        /// </summary>
        public Matrix ModelMatrix { set; get; } = Matrix.Identity;
        /// <summary>
        /// 
        /// </summary>
        public IRenderTechnique EffectTechnique { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Device Device { get { return EffectTechnique == null ? null : EffectTechnique.Device; } }
        /// <summary>
        /// Is render core has been attached
        /// </summary>
        public bool IsAttached { private set; get; } = false;


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
        protected IConstantBufferProxy modelCB { private set; get; }

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

        /// <summary>
        /// Gets the model constant buffer description.
        /// </summary>
        /// <returns></returns>
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
        /// <param name="deviceContext"></param>
        public void Render(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRender(context))
            {
                OnUpdatePerModelStruct(ref modelStruct, context);
                OnAttachBuffers(deviceContext);
                OnUploadPerModelConstantBuffers(deviceContext);
                OnBindRasterState(deviceContext);
                switch (context.IsShadowPass)
                {
                    case false:
                        OnRender(context, deviceContext);
                        break;
                    case true:                        
                        OnRenderShadow(context, deviceContext);
                        break;
                }
                PostRender(context);
            }
        }

        /// <summary>
        /// Called when [render shadow].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext"></param>
        protected virtual void OnRenderShadow(IRenderContext context, DeviceContextProxy deviceContext) { }

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
        protected abstract void OnRender(IRenderContext context, DeviceContextProxy deviceContext);

        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected abstract void OnUpdatePerModelStruct(ref TModelStruct model, IRenderContext context);

        /// <summary>
        /// Called when [upload per model constant buffers].
        /// </summary>
        /// <param name="context">The context.</param>
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

        /// <summary>
        /// Invalidates the renderer.
        /// </summary>
        protected void InvalidateRenderer()
        {
            OnInvalidateRenderer?.Invoke(this, true);
        }

        /// <summary>
        /// Resets the invalidate handler.
        /// </summary>
        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            OnInvalidateRenderer = null;
            base.Dispose(disposeManagedResources);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            InvalidateRenderer();
            return true;
        }
    }
}
