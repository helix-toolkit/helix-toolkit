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

    public abstract class MaterialVariable : ReferenceCountDisposeObject
    {
        public abstract string DefaultShaderPassName { set; get; }
        public abstract bool RenderShadowMap { set; get; }
        public abstract bool RenderEnvironmentMap { set; get; }
        public abstract ShaderPass GetPass(MaterialGeometryRenderCore core, RenderContext context);
        public abstract bool BindMaterial(DeviceContextProxy deviceContext, ShaderPass shaderPass);
        public abstract bool Attach(IRenderTechnique technique);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="MaterialStruct">The type of the material structure.</typeparam>
    public abstract class MaterialVariableBase<MaterialStruct> : MaterialVariable where MaterialStruct : struct
    {
        protected IRenderTechnique Technique { private set; get; }
        protected bool IsAttached { private set; get; } = false;
        private readonly ConstantBufferProxy materialCB;
        protected bool needUpdate = true;
        protected MaterialStruct materialStruct = new MaterialStruct();

        public MaterialVariableBase(IEffectsManager manager, string constBufferName = "", int materialStructSize = 0)
        {
            if (!string.IsNullOrEmpty(constBufferName) && materialStructSize != 0)
            {
                materialCB = manager.ConstantBufferPool.Register(constBufferName, materialStructSize);
            }
            this.PropertyChanged += (s, e) => { InvalidateRenderer(); };
        }

        public override bool Attach(IRenderTechnique technique)
        {
            Technique = technique;
            IsAttached = true;
            return !technique.IsNull;
        }

        public sealed override bool BindMaterial(DeviceContextProxy deviceContext, ShaderPass shaderPass)
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
                    materialCB?.UploadDataToBuffer(deviceContext, ref materialStruct);
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
            Technique?.EffectsManager?.InvalidateRenderer();
        }
    }
}
