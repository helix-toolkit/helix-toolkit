/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Core;
    using Render;
    using Shaders;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    /// <summary>
    /// 
    /// </summary>
    public abstract class MaterialVariable : ReferenceCountDisposeObject
    {
        public abstract string DefaultShaderPassName { set; get; }

        public event EventHandler OnUpdateNeeded;

        protected IRenderTechnique Technique { private set; get; }
        protected bool NeedUpdate { set; get; } = true;
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialVariable"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="technique">The technique.</param>
        public MaterialVariable(IEffectsManager manager, IRenderTechnique technique)
        {
            Technique = technique;
        }
        /// <summary>
        /// Binds the material.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="shaderPass">The shader pass.</param>
        /// <returns></returns>
        public bool BindMaterial(DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if (CanUpdateMaterial())
            {
                if (deviceContext.SetCurrentMaterial(this))
                {
                    return OnBindMaterialTextures(deviceContext, shaderPass);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract bool OnBindMaterialTextures(DeviceContextProxy context, ShaderPass shaderPass);

        protected abstract void AssignVariables(ref ModelStruct model);

        protected virtual bool CanUpdateMaterial() { return !IsDisposed; }

        public abstract ShaderPass GetPass(MaterialGeometryRenderCore core, RenderContext context);

        public void UpdateMaterialStruct(ref ModelStruct model)
        {
            AssignVariables(ref model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            OnUpdateNeeded = null;
            base.OnDispose(disposeManagedResources);
        }

        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            NotifyUpdateNeeded();
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InvalidateRenderer()
        {
            Technique?.EffectsManager?.InvalidateRenderer();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void NotifyUpdateNeeded()
        {
            NeedUpdate = true;
            OnUpdateNeeded?.Invoke(this, EventArgs.Empty);
            InvalidateRenderer();
        }
    }
}
