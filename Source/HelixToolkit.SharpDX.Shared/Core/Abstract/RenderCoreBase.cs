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
    using Model;
    /// <summary>
    /// Base class for all render core classes
    /// </summary>
    public abstract class RenderCoreBase<TModelStruct> : DisposeObject, IRenderCore where TModelStruct : struct
    {
        #region Properties
        /// <summary>
        /// <see cref="IRenderCore.OnInvalidateRenderer"/>
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidateRenderer;
        /// <summary>
        /// <see cref="IGUID.GUID"/>
        /// </summary>
        public Guid GUID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the type of the render.
        /// </summary>
        /// <value>
        /// The type of the render.
        /// </value>
        public RenderType RenderType { private set; get; }

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
        /// Gets or sets the post effects.
        /// </summary>
        /// <value>
        /// The post effects.
        /// </value>
        private readonly Dictionary<string, IEffectAttributes> postEffectNames = new Dictionary<string, IEffectAttributes>();

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
        /// Gets the post effect names.
        /// </summary>
        /// <value>
        /// The post effect names.
        /// </value>
        public IEnumerable<string> PostEffectNames
        {
            get { return postEffectNames.Keys; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has any post effect.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has any post effect; otherwise, <c>false</c>.
        /// </value>
        public bool HasAnyPostEffect { get { return postEffectNames.Count > 0; } }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCoreBase{TModelStruct}"/> class.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        public RenderCoreBase(RenderType renderType)
        {
            RenderType = renderType;
        }
        /// <summary>
        /// Call to attach the render core.
        /// </summary>
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
                int vertStartSlot = 0;
                OnAttachBuffers(deviceContext, ref vertStartSlot);
                OnUploadPerModelConstantBuffers(deviceContext);
                OnBindRasterState(deviceContext);
                switch (context.IsShadowPass)
                {
                    case true:
                        switch (context.IsCustomPass)
                        {
                            case true:
                                var pass = EffectTechnique[context.CustomPassName];
                                if (!pass.IsNULL)
                                {
                                    OnRenderCustom(context, deviceContext, pass);
                                }
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
                                var pass = EffectTechnique[context.CustomPassName];
                                if (!pass.IsNULL)
                                {
                                    OnRenderCustom(context, deviceContext, pass);
                                }
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
        /// <param name="vertStartSlot">Start slot for vertex buffer binding</param>
        protected virtual void OnAttachBuffers(DeviceContext context, ref int vertStartSlot)
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
        /// Render function for custom shader pass. Used to do special effects
        /// </summary>
        protected virtual void OnRenderCustom(IRenderContext context, DeviceContextProxy deviceContext, IShaderPass shaderPass) { }

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
            OnInvalidateRenderer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Resets the invalidate handler.
        /// </summary>
        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            OnInvalidateRenderer = null;
            base.OnDispose(disposeManagedResources);
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



        #region POST EFFECT        

        /// <summary>
        /// Adds the post effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        public void AddPostEffect(IEffectAttributes effect)
        {
            if (postEffectNames.ContainsKey(effect.EffectName))
            {
                return;
            }
            postEffectNames.Add(effect.EffectName, effect);
            InvalidateRenderer();
        }
        /// <summary>
        /// Removes the post effect.
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        public void RemovePostEffect(string effectName)
        {
            if (postEffectNames.Remove(effectName))
            {
                InvalidateRenderer();
            }
        }
        /// <summary>
        /// Determines whether [has post effect] [the specified effect name].
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        /// <returns>
        ///   <c>true</c> if [has post effect] [the specified effect name]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasPostEffect(string effectName)
        {
            return postEffectNames.ContainsKey(effectName);
        }
        /// <summary>
        /// Tries the get post effect.
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        /// <param name="effect">The effect.</param>
        /// <returns></returns>
        public bool TryGetPostEffect(string effectName, out IEffectAttributes effect)
        {
            return postEffectNames.TryGetValue(effectName, out effect);
        }
        /// <summary>
        /// Clears the post effect.
        /// </summary>
        public void ClearPostEffect()
        {
            postEffectNames.Clear();
            InvalidateRenderer();
        }
        #endregion
    }
}
