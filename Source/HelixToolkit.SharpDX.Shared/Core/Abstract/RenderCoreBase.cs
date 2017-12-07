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
    using Utilities;
    /// <summary>
    /// Base class for all render core classes
    /// </summary>
    public abstract class RenderCoreBase<TModelStruct> : DisposeObject, IRenderCore where TModelStruct : struct
    {
        public Guid GUID { get; } = Guid.NewGuid();

        //private EffectMatrixVariable mWorldVar;
        protected TModelStruct modelStruct;
        protected IBufferProxy modelCB { private set; get; }
        public event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Model matrix
        /// </summary>
        public Matrix ModelMatrix { set; get; } = Matrix.Identity;      
        //public Effect Effect { private set; get; }
        public IRenderTechnique EffectTechnique { private set; get; }
        public Device Device { get { return EffectTechnique == null ? null : EffectTechnique.Device; } }
        /// <summary>
        /// Is render core has been attached
        /// </summary>
        public bool IsAttached { private set; get; } = false;
        /// <summary>
        /// Call to attach the render core.
        /// </summary>
        /// <param name="host"></param>
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
        /// <param name="host"></param>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected virtual bool OnAttach(IRenderTechnique technique)
        {
            //mWorldVar = Collect(Effect.GetVariableByName(ShaderVariableNames.WorldMatrix).AsMatrix());
            modelCB = technique.ConstantBufferPool.Register(GetModelConstantBufferDescription());
            return true;
        }

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
        public void Render(IRenderMatrices context)
        {
            if (CanRender())
            {
                SetStates(context);
                OnAttachBuffers(context.DeviceContext);
                OnUpdateModelStruct(context);
                OnRender(context);
                PostRender(context);
            }
        }

        /// <summary>
        /// Before calling OnRender. Setup commonly used rendering states.
        /// <para>Default to call <see cref="SetShaderVariables"/> and <see cref="SetRasterStates"/></para>
        /// </summary>
        /// <param name="context"></param>
        protected virtual void SetStates(IRenderMatrices context)
        {
            SetRasterStates(context);
        }

        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnAttachBuffers(DeviceContext context)
        {
        }

        /// <summary>
        /// Actual render function. Used to attach different render states and call the draw call.
        /// </summary>
        protected abstract void OnRender(IRenderMatrices context);

        protected abstract void OnUpdateModelStruct(IRenderMatrices context);

        protected void UpdateModelConstantBuffer(DeviceContext context)
        {
            modelCB.UploadDataToBuffer(context, ref modelStruct);
        }

        /// <summary>
        /// After calling OnRender. Restore some variables, such as HasInstance etc.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void PostRender(IRenderMatrices context) { }

        /// <summary>
        /// Check if can render
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanRender()
        {
            return IsAttached;
        }

        protected virtual void SetRasterStates(IRenderMatrices matrices) { }

        protected void InvalidateRenderer(object sender, bool e)
        {
            OnInvalidateRenderer?.Invoke(sender, e);
        }

        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }
    }
}
