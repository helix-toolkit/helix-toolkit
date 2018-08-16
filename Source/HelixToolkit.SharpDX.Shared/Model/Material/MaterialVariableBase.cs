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
    public abstract class MaterialVariable : ReferenceCountDisposeObject
    {
        public abstract string DefaultShaderPassName { set; get; }

        public static readonly ConstantBufferDescription DefaultMeshConstantBufferDesc
            = new ConstantBufferDescription(DefaultBufferNames.MeshPhongCB,
                      ModelStruct.SizeInBytes + PhongMaterialStruct.SizeInBytes);

        public event EventHandler OnUpdateNeeded;
        /// <summary>
        /// Gets or sets the identifier. Used for material sorting
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ushort ID { set; get; } = 0;

        protected IRenderTechnique Technique { private set; get; }
        protected bool NeedUpdate { set; get; } = true;
        protected ConstantBufferProxy ConstantBuffer { private set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialVariable"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="technique">The technique.</param>
        /// <param name="meshConstantBufferDesc">The Constant Buffer description</param>
        public MaterialVariable(IEffectsManager manager, IRenderTechnique technique, ConstantBufferDescription meshConstantBufferDesc)
        {
            Technique = technique;
            if (meshConstantBufferDesc != null)
            {
                ConstantBuffer = manager.ConstantBufferPool.Register(meshConstantBufferDesc);
            }
        }
        /// <summary>
        /// Binds the material.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="shaderPass">The shader pass.</param>
        /// <returns></returns>
        public bool BindMaterial(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if (CanUpdateMaterial())
            {
                return OnBindMaterialTextures(context, deviceContext, shaderPass);
            }
            else
            {
                return false;
            }
        }

        protected abstract bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass);

        protected abstract void UpdateInternalVariables(DeviceContextProxy deviceContext);

        protected abstract void WriteMaterialDataToConstantBuffer(global::SharpDX.DataStream cbStream);

        protected virtual bool CanUpdateMaterial() { return !IsDisposed; }

        public abstract ShaderPass GetPass(RenderType renderType, RenderContext context);

        public void UpdateMaterialStruct<T>(DeviceContextProxy context, ref T model) where T : struct
        {
            UpdateInternalVariables(context);
            using (var dataStream = ConstantBuffer.Map(context))
            {
                dataStream.Write(model);
                WriteMaterialDataToConstantBuffer(dataStream);
            }
            ConstantBuffer.Unmap(context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateModelStructOnly<T>(DeviceContextProxy context, ref T model) where T : struct
        {
            ConstantBuffer.UploadDataToBuffer(context, ref model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            OnUpdateNeeded = null;
            ConstantBuffer = null;
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
