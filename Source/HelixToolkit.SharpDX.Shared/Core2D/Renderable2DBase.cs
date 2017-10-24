using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D
{
    public abstract class Renderable2DBase : IRenderable2D
    {
        public bool IsRendering
        {
            set; get;
        } = true;

        public global::SharpDX.RectangleF Rect { set; get; } = new RectangleF(0, 0, 100, 100);

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

        private Matrix3x3 transform = Matrix3x3.Identity;
        public Matrix3x3 Transform
        {
            set
            {
                if (transform != value)
                {
                    transform = value;
                }
            }
            get
            {
                return transform;
            }
        }

        protected abstract void OnTargetChanged(D2D.RenderTarget target);

        public void Render(IRenderMatrices matrices, D2D.RenderTarget target)
        {
            if (CanRender(target))
            {
                RenderTarget = target;
                var trans = transform * new Matrix3x3(1, 0, 0, 0, 1, 0, (Rect.Left + Rect.Width / 2), (Rect.Top + Rect.Height / 2), 1);
                RenderTarget.Transform = new global::SharpDX.Mathematics.Interop.RawMatrix3x2(trans.M11,trans.M12,trans.M21,trans.M22, trans.M31, trans.M32);
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
