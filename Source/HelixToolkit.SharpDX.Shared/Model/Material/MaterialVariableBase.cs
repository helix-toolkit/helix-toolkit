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
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="MaterialStruct">The type of the material structure.</typeparam>
    public abstract class MaterialVariableBase<MaterialStruct> : DisposeObject, IEffectMaterialVariables where MaterialStruct : struct
    {
        public event EventHandler<EventArgs> OnInvalidateRenderer;

        public abstract string DefaultShaderPassName { set; get; }
        public abstract bool RenderShadowMap { set; get; }
        public abstract bool RenderEnvironmentMap { set; get; }
        protected IRenderTechnique Technique { private set; get; }
        protected bool IsAttached { private set; get; } = false;

        private readonly ConstantBufferProxy materialCB;
        protected bool needUpdate = true;
        protected MaterialStruct materialStruct = new MaterialStruct();

        public MaterialVariableBase(IEffectsManager manager, string constBufferName, int materialStructSize)
        {
            materialCB = manager.ConstantBufferPool.Register(constBufferName, materialStructSize);
            this.PropertyChanged += (s, e) => { OnInvalidateRenderer?.Invoke(this, EventArgs.Empty); };
        }

        public MaterialVariableBase(IEffectsManager manager)
        {
            this.PropertyChanged += (s, e) => { OnInvalidateRenderer?.Invoke(this, EventArgs.Empty); };
        }

        public virtual bool Attach(IRenderTechnique technique)
        {
            Technique = technique;
            IsAttached = true;
            return !technique.IsNull;
        }

        public bool BindMaterial(DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if (CanUpdateMaterial())
            {
                bool cbUpdate = deviceContext.SetCurrentMaterial(this);
                if (needUpdate)
                {
                    AssignVariables();
                    needUpdate = false;
                    cbUpdate = true;
                }
                if (cbUpdate)
                {
                    materialCB.UploadDataToBuffer(deviceContext, ref materialStruct);
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

        public abstract ShaderPass GetPass(MaterialGeometryRenderCore core, RenderContext context);

        protected abstract void AssignVariables();

        protected virtual bool CanUpdateMaterial() { return !IsDisposed; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                IsAttached = false;
                Technique = null;
                OnInvalidateRenderer = null;
            }

            base.OnDispose(disposeManagedResources);
        }

        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            needUpdate = true;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InvalidateRenderer()
        {
            OnInvalidateRenderer?.Invoke(this, EventArgs.Empty);
        }
    }
}
