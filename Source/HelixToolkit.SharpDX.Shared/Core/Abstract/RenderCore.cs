/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        /// Indicate whether render host should call <see cref="Update(IRenderContext, DeviceContextProxy)"/> before <see cref="Render(IRenderContext, DeviceContextProxy)"/>
        /// <para><see cref="Update(IRenderContext, DeviceContextProxy)"/> is used to run such as compute shader before rendering. </para>
        /// <para>Compute shader can be run at the beginning of any other <see cref="Render(IRenderContext, DeviceContextProxy)"/> routine to avoid waiting.</para>
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

        /// <summary>
        /// Gets or sets the post effects.
        /// </summary>
        /// <value>
        /// The post effects.
        /// </value>
        private readonly Dictionary<string, IEffectAttributes> postEffectNames = new Dictionary<string, IEffectAttributes>();

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
        public abstract void Render(IRenderContext context, DeviceContextProxy deviceContext);

        /// <summary>
        /// Update routine. Only used to run update computation such as compute shader in particle system. 
        /// <para>Compute shader can be run at the beginning of any other <see cref="Render(IRenderContext, DeviceContextProxy)"/> routine to avoid waiting.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        public virtual void Update(IRenderContext context, DeviceContextProxy deviceContext) { }

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
