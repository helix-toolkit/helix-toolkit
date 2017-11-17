using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public abstract class RenderCoreBase : DisposeObject, IRenderCore
    {
        public delegate void OnRenderEvent(IRenderMatrices context);
        public OnRenderEvent OnRender;
        private EffectTransformVariables effectTransformVar;   
        public Effect Effect { private set; get; }  

        public bool IsAttached { set; get; } = false;
        public void Attach(IRenderHost host, RenderTechnique technique)
        {
            if (IsAttached)
            {
                return;
            }
            IsAttached = OnAttach(host, technique);
        }

        protected virtual bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            Effect = host.EffectsManager.GetEffect(technique);
            Collect(effectTransformVar = new EffectTransformVariables(Effect));
            return true;
        }

        public void Detach()
        {
            IsAttached = false;
            OnDetach();
        }

        protected virtual void OnDetach()
        {
            DisposeAndClear();
        }

        public void Render(IRenderMatrices context)
        {
            if (CanRender())
            {
                OnRender?.Invoke(context);
            }
        }        

        protected virtual bool CanRender()
        {
            return IsAttached;
        }

        public void SetModelWorldMatrix(Matrix matrix)
        {
            effectTransformVar.World.SetMatrix(matrix);
        }

        public sealed class EffectTransformVariables : DisposeObject
        {
            public EffectTransformVariables(Effect effect)
            {
                // openGL: uniform variables            
                Collect(mWorld = effect.GetVariableByName("mWorld").AsMatrix());
            }

            private EffectMatrixVariable mWorld;
            public EffectMatrixVariable World { get { return mWorld; } }
        }
    }
}
