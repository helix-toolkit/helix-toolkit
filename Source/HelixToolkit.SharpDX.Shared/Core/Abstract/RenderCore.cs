/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
#endif
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public abstract class RenderCore : DisposeObject, IGUID, IThrowingShadow
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidateRenderer;
        /// <summary>
        /// <see cref="IGUID.GUID"/>
        /// </summary>
        public Guid GUID { get; } = Guid.NewGuid();

        private RenderType renderType = RenderType.None;
        /// <summary>
        /// Gets or sets the type of the render.
        /// </summary>
        /// <value>
        /// The type of the render.
        /// </value>
        public RenderType RenderType
        {
            set
            {
                SetAffectsRender(ref renderType, value);
            }
            get { return renderType; }
        }

        /// <summary>
        /// Indicate whether render host should call <see cref="Update(RenderContext, DeviceContextProxy)"/> before <see cref="Render(RenderContext, DeviceContextProxy)"/>
        /// <para><see cref="Update(RenderContext, DeviceContextProxy)"/> is used to run such as compute shader before rendering. </para>
        /// <para>Compute shader can be run at the beginning of any other <see cref="Render(RenderContext, DeviceContextProxy)"/> routine to avoid waiting.</para>
        /// </summary>
        public bool NeedUpdate { protected set; get; } = false;

        private bool isThrowingShadow = false;
        /// <summary>
        /// <see cref="IThrowingShadow.IsThrowingShadow"/>
        /// </summary>
        public bool IsThrowingShadow
        {
            set
            {
                SetAffectsRender(ref isThrowingShadow, value);
            }
            get { return isThrowingShadow; }
        }

        /// <summary>
        /// Gets or sets the default state binding.
        /// </summary>
        /// <value>
        /// The default state binding.
        /// </value>
        public StateType DefaultStateBinding
        {
            set; get;
        } = StateType.BlendState | StateType.DepthStencilState;

        /// <summary>
        /// Gets or sets the default state binding.
        /// </summary>
        /// <value>
        /// The default state binding.
        /// </value>
        public StateType ShadowStateBinding
        {
            set; get;
        } = StateType.BlendState | StateType.DepthStencilState;

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
        public Device Device { get { return EffectTechnique?.Device; } }
        /// <summary>
        /// Is render core has been attached
        /// </summary>
        public bool IsAttached { private set; get; } = false;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCoreBase{TModelStruct}"/> class.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        public RenderCore(RenderType renderType)
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
        protected abstract bool OnAttach(IRenderTechnique technique);

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
        /// Render routine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        public abstract void Render(RenderContext context, DeviceContextProxy deviceContext);

        /// <summary>
        /// Update routine. Only used to run update computation such as compute shader in particle system. 
        /// <para>Compute shader can be run at the beginning of any other <see cref="Render(RenderContext, DeviceContextProxy)"/> routine to avoid waiting.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        public virtual void Update(RenderContext context, DeviceContextProxy deviceContext) { }

        /// <summary>
        /// Resets the invalidate handler.
        /// </summary>
        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }

        /// <summary>
        /// Invalidates the renderer.
        /// </summary>
        protected void InvalidateRenderer()
        {
            OnInvalidateRenderer?.Invoke(this, EventArgs.Empty);
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
