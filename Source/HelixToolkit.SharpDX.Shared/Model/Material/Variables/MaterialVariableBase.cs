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
    using Render;
    using Shaders;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Core.Components;
    using System.ComponentModel;

    /// <summary>
    /// 
    /// </summary>
    public abstract class MaterialVariable : ReferenceCountDisposeObject
    {
        public static readonly ConstantBufferDescription DefaultMeshConstantBufferDesc
            = new ConstantBufferDescription(DefaultBufferNames.ModelCB,
                      PhongPBRMaterialStruct.SizeInBytes);

        public static readonly ConstantBufferDescription DefaultPointLineConstantBufferDesc
            = new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB,
                        PointLineMaterialStruct.SizeInBytes);

        public event EventHandler UpdateNeeded;
        /// <summary>
        /// Gets or sets the identifier. Used for material sorting
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ushort ID { set; get; } = 0;

        protected IRenderTechnique Technique { get; }
        protected IEffectsManager EffectsManager { get; }
        protected bool NeedUpdate { private set; get; } = true;
        protected ConstantBufferComponent ConstantBuffer { get; }
        private readonly object updateLock = new object();
        private readonly MaterialCore material;
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialVariable"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="technique">The technique.</param>
        /// <param name="meshConstantBufferDesc">The Constant Buffer description</param>
        /// <param name="materialCore"></param>
        public MaterialVariable(IEffectsManager manager, IRenderTechnique technique, ConstantBufferDescription meshConstantBufferDesc, MaterialCore materialCore)
        {
            Technique = technique;
            EffectsManager = manager;
            if(materialCore != null)
            {
                material = materialCore;
                material.PropertyChanged += MaterialCore_PropertyChanged;
            }
            ConstantBuffer = new ConstantBufferComponent(meshConstantBufferDesc);
        }

        internal void Initialize()
        {
            ConstantBuffer.Attach(Technique);
            OnInitialPropertyBindings();
            foreach(var v in propertyBindings.Values)
            {
                v.Invoke();
            }
        }

        protected virtual void OnInitialPropertyBindings() { }
        /// <summary>
        /// Binds the material textures, samplers, etc,.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="shaderPass">The shader pass.</param>
        /// <returns></returns>
        public abstract bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass);

        protected virtual void UpdateInternalVariables(DeviceContextProxy deviceContext) { }

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
        /// Updates the material structure. And upload data to constant buffer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        /// <param name="structSize"></param>
        public bool UpdateMaterialStruct<T>(DeviceContextProxy context, ref T model, int structSize) where T : struct
        {
            if (NeedUpdate)
            {
                lock (updateLock)
                {
                    if (NeedUpdate)
                    {
                        UpdateInternalVariables(context);
                        NeedUpdate = false;
                    }
                }
            }
            return ConstantBuffer.Upload(context, ref model, structSize);
        }

        /// <summary>
        /// Draws the specified device context.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="bufferModel">Geometry buffer model.</param>
        /// <param name="instanceCount">The instance count.</param>
        public abstract void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount);

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
            Technique?.EffectsManager?.RaiseInvalidateRender();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void NotifyUpdateNeeded()
        {
            NeedUpdate = true;
            UpdateNeeded?.Invoke(this, EventArgs.Empty);
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
        /// <summary>
        /// Writes the value to internal buffer array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValue<T>(string name, ref T value) where T : struct
        {
            ConstantBuffer.WriteValueByName(name, value);
        }
        /// <summary>
        /// Writes the value to internal buffer array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValue<T>(string name, T value) where T : struct
        {
            ConstantBuffer.WriteValueByName(name, value);
        }
        /// <summary>
        /// Writes the value to internal buffer array with offset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="offset">The offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValue<T>(ref T value, int offset) where T : struct
        {
            ConstantBuffer.WriteValue(value, offset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                UpdateNeeded = null;
                if(material != null)
                {
                    material.PropertyChanged -= MaterialCore_PropertyChanged;
                }
                ConstantBuffer.Detach();
                propertyBindings.Clear();
            }
            base.OnDispose(disposeManagedResources);
        }

        #region Material Property Bindings
        private readonly Dictionary<string, Action> propertyBindings = new Dictionary<string, Action>();

        protected void AddPropertyBinding(string propertyName, Action action)
        {
            propertyBindings.Add(propertyName, action);
        }

        protected void TriggerPropertyAction(string propertyName)
        {
            if (propertyBindings.TryGetValue(propertyName, out Action act))
            {
                act.Invoke();
            }
        }

        private void MaterialCore_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TriggerPropertyAction(e.PropertyName);
            InvalidateRenderer();
        }
        #endregion
    }
}
