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
        public static readonly ConstantBufferDescription DefaultMeshConstantBufferDesc
            = new ConstantBufferDescription(DefaultBufferNames.MeshPhongCB,
                      ModelStruct.SizeInBytes + PhongMaterialStruct.SizeInBytes);

        public static readonly ConstantBufferDescription DefaultPointLineConstantBufferDesc
            = new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB,
              PointLineModelStruct.SizeInBytes + PointLineMaterialStruct.SizeInBytes);

        public event EventHandler OnUpdateNeeded;
        /// <summary>
        /// Gets or sets the identifier. Used for material sorting
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ushort ID { set; get; } = 0;

        protected IRenderTechnique Technique { get; }
        protected IEffectsManager EffectsManager { get; }
        protected bool NeedUpdate { set; get; } = true;
        protected ConstantBufferProxy ConstantBuffer { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialVariable"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="technique">The technique.</param>
        /// <param name="meshConstantBufferDesc">The Constant Buffer description</param>
        public MaterialVariable(IEffectsManager manager, IRenderTechnique technique, ConstantBufferDescription meshConstantBufferDesc)
        {
            Technique = technique;
            EffectsManager = manager;
            if (meshConstantBufferDesc != null)
            {
                ConstantBuffer = manager.ConstantBufferPool.Register(meshConstantBufferDesc);
            }
        }
        /// <summary>
        /// Binds the material textures, samplers, etc,.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="shaderPass">The shader pass.</param>
        /// <returns></returns>
        public abstract bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass);

        protected abstract void UpdateInternalVariables(DeviceContextProxy deviceContext);

        protected abstract void WriteMaterialDataToConstantBuffer(global::SharpDX.DataStream cbStream);
        /// <summary>
        /// Gets the pass.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public abstract ShaderPass GetPass(RenderType renderType, RenderContext context);
        /// <summary>
        /// Gets the shadow pass.
        /// </summary>
        /// <param name="renderType"></param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public abstract ShaderPass GetShadowPass(RenderType renderType, RenderContext context);
        /// <summary>
        /// Gets the wireframe pass.
        /// </summary>
        /// <param name="renderType"></param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public abstract ShaderPass GetWireframePass(RenderType renderType, RenderContext context);
        /// <summary>
        /// Gets the name of the pass by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ShaderPass GetPassByName(string name)
        {
            return Technique[name];
        }
        /// <summary>
        /// Updates the material structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
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

        /// <summary>
        /// Draws the specified device context.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="bufferModel">Geometry buffer model.</param>
        /// <param name="instanceCount">The instance count.</param>
        public abstract void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawIndexed(DeviceContextProxy context, int indexCount, int instanceCount)
        {
            if (instanceCount <= 0)
            {
                context.DrawIndexed(indexCount, 0, 0);
            }
            else
            {
                context.DrawIndexedInstanced(indexCount, instanceCount, 0, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawPoints(DeviceContextProxy context, int vertexCount, int instanceCount)
        {
            if (instanceCount <= 0)
            {
                context.Draw(vertexCount, 0);
            }
            else
            {
                context.DrawInstanced(vertexCount, instanceCount, 0, 0);
            }
        }
    }
}
