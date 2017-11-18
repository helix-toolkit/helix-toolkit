using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    /// <summary>
    /// Base class for all render core classes
    /// </summary>
    public abstract class RenderCoreBase : DisposeObject, IRenderCore
    {
        public Guid GUID { get; } = Guid.NewGuid();

        private EffectTransformVariables effectTransformVar;   
        public delegate void OnRenderEvent(IRenderMatrices context);
        /// <summary>
        /// Render function delegate. Used to attach different render routine
        /// </summary>
        public OnRenderEvent OnRender;
        public event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Model matrix
        /// </summary>
        public Matrix ModelMatrix { set; get; } = Matrix.Identity;      
        public Effect Effect { private set; get; }  
        public Device Device { get { return Effect == null ? null : Effect.Device; } }
        /// <summary>
        /// Is render core has been attached
        /// </summary>
        public bool IsAttached { private set; get; } = false;
        /// <summary>
        /// Call to attach the render core.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        public void Attach(IRenderHost host, RenderTechnique technique)
        {
            if (IsAttached)
            {
                return;
            }
            IsAttached = OnAttach(host, technique);
        }
        /// <summary>
        /// During attatching render core. Create all local resources. Use Collect(resource) to let object be released automatically during Detach().
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected virtual bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            Effect = host.EffectsManager.GetEffect(technique);
            effectTransformVar = Collect(new EffectTransformVariables(Effect));
            return true;
        }
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
                OnRender?.Invoke(context);
            }
        }        
        /// <summary>
        /// Check if can render
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanRender()
        {
            return IsAttached;
        }
        /// <summary>
        /// Set Model World transformation matrix
        /// </summary>
        /// <param name="matrix"></param>
        protected void SetModelWorldMatrix(Matrix matrix)
        {
            effectTransformVar.World.SetMatrix(matrix);
        }
        /// <summary>
        /// Set additional per model shader constant variables such as model->world matrix etc.
        /// </summary>
        protected virtual void SetConstantVariables(IRenderMatrices matrices)
        {
            SetModelWorldMatrix(ModelMatrix * matrices.WorldMatrix);
        }

        protected void InvalidateRenderer(object sender, bool e)
        {
            OnInvalidateRenderer?.Invoke(sender, e);
        }

        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }
        /// <summary>
        /// Used to handle world constant variable from shader
        /// </summary>
        public sealed class EffectTransformVariables : DisposeObject
        {
            public EffectTransformVariables(Effect effect)
            {         
                mWorld = Collect(effect.GetVariableByName("mWorld").AsMatrix());
            }

            private EffectMatrixVariable mWorld;
            public EffectMatrixVariable World { get { return mWorld; } }
        }
    }
}
