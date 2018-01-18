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
    public abstract class LightCoreBase : DisposeObject, ILight3D, IRenderCore
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty { get; } = false;
        /// <summary>
        /// Gets or sets the type of the light.
        /// </summary>
        /// <value>
        /// The type of the light.
        /// </value>
        public LightType LightType { protected set; get; }

        private Color4 color = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            set { SetAffectsRender(ref color, value); }
            get { return color; }
        }
        /// <summary>
        /// Gets or sets the model matrix.
        /// </summary>
        /// <value>
        /// The model matrix.
        /// </value>
        public Matrix ModelMatrix { set; get; } = Matrix.Identity;
        /// <summary>
        /// Gets the effect technique.
        /// </summary>
        /// <value>
        /// The effect technique.
        /// </value>
        /// <exception cref="NotImplementedException"></exception>
        public IRenderTechnique EffectTechnique { get; }
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        /// <exception cref="NotImplementedException"></exception>
        public Device Device { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is attached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is attached; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttached { private set; get; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is throwing shadow.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is throwing shadow; otherwise, <c>false</c>.
        /// </value>
        public bool IsThrowingShadow { get; set; } = false;
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        public event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        public void Render(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRender(context.LightScene))
            {
                OnRender(context.LightScene, context.LightScene.LightModels.LightCount);
                switch (LightType)
                {
                    case LightType.Ambient:
                        break;
                    default:
                        context.LightScene.LightModels.IncrementLightCount();
                        break;
                }
            }
        }
        /// <summary>
        /// Determines whether this instance can render the specified light scene.
        /// </summary>
        /// <param name="lightScene">The light scene.</param>
        /// <returns>
        ///   <c>true</c> if this instance can render the specified light scene; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRender(Light3DSceneShared lightScene)
        {
            return IsAttached && lightScene.LightModels.LightCount < Constants.MaxLights;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="lightScene">The light scene.</param>
        /// <param name="idx">The index.</param>
        protected virtual void OnRender(Light3DSceneShared lightScene, int idx)
        {
            lightScene.LightModels.Lights[idx].LightColor = Color;
            lightScene.LightModels.Lights[idx].LightType = (int)LightType;
        }
        /// <summary>
        /// Invalidates the renderer.
        /// </summary>
        protected void InvalidateRenderer()
        {
            OnInvalidateRenderer?.Invoke(this, true);
        }
        /// <summary>
        /// Attaches the specified technique.
        /// </summary>
        /// <param name="technique">The technique.</param>
        public void Attach(IRenderTechnique technique)
        {
            IsAttached = true;
        }
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            IsAttached = false;
        }
        /// <summary>
        /// Resets the invalidate handler.
        /// </summary>
        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
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
    /// <summary>
    /// 
    /// </summary>
    public class AmbientLightCore : LightCoreBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientLightCore"/> class.
        /// </summary>
        public AmbientLightCore()
        {
            LightType = LightType.Ambient;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="lightScene">The light scene.</param>
        /// <param name="idx">The index.</param>
        protected override void OnRender(Light3DSceneShared lightScene, int idx)
        {
            lightScene.LightModels.AmbientLight = Color;
        }
    }
}
