using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Text;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Shared.D2DControls
{
    public abstract class D2DRenderableBase : ID2DRenderable
    {
        public bool IsRendering
        {
            set; get;
        } = true;

        private D2D.RenderTarget renderTarget;
        protected D2D.RenderTarget RenderTarget
        {
            private set
            {
                if (renderTarget == value) { return; }
                renderTarget = value;
                OnTargetChanged(value);
            }
            get
            {
                return renderTarget;
            }
        }

        protected abstract void OnTargetChanged(D2D.RenderTarget target);

        public void Render(IRenderMatrices matrices, D2D.RenderTarget target)
        {
            if (CanRender(target))
            {
                RenderTarget = target;
                OnRender(matrices);
            }
        }

        protected abstract void OnRender(IRenderMatrices matrices);

        protected virtual bool CanRender(D2D.RenderTarget target)
        {
            return IsRendering && target != null && !target.IsDisposed;
        }

        public virtual void Dispose()
        {
            RenderTarget = null;
        }
    }
}
