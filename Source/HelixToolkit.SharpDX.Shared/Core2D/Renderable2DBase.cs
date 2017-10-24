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
        public bool IsChanged { private set; get; } = true;

        public Matrix3x2 RenderTargetTransform { private set; get; }

        public bool IsRendering
        {
            set; get;
        } = true;

        private RectangleF rect = new RectangleF(0, 0, 100, 100);
        public RectangleF Rect
        {
            set
            {
                if (rect == value) { return; }
                rect = value;
                IsChanged = true;
            }
            get
            {
                return rect;
            }
        }

        public RectangleF LocalDrawingRect { private set; get; }

        private D2D.RenderTarget renderTarget;
        protected D2D.RenderTarget RenderTarget
        {
            private set
            {
                if (renderTarget == value) { return; }
                renderTarget = value;
                IsChanged = true;
                OnTargetChanged(value);
            }
            get
            {
                return renderTarget;
            }
        }

        private Matrix3x2 transform = Matrix3x2.Identity;
        public Matrix3x2 Transform
        {
            set
            {
                if (transform == value) { return; }
                transform = value;
                IsChanged = true;
            }
            get
            {
                return transform;
            }
        }

        private D2D.Brush borderBrush = null;
        public D2D.Brush BorderBrush
        {
            set
            {
                borderBrush = value;
            }
            get
            {
                return borderBrush;
            }
        }

        private D2D.StrokeStyle borderStyle;
#if DEBUG
        public bool ShowDrawingBorder { set; get; } = true;
#else
        public bool ShowDrawingBorder { set; get; } = false;
#endif
        protected virtual void OnTargetChanged(D2D.RenderTarget target)
        {
            Disposer.RemoveAndDispose(ref borderBrush);
            Disposer.RemoveAndDispose(ref borderStyle);
            if (target == null || target.IsDisposed)
            {
                return;
            }
            borderBrush = new D2D.SolidColorBrush(target, Color.LightBlue);
            borderStyle =  new D2D.StrokeStyle(RenderTarget.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.DashDot });
        }

        public void Render(IRenderMatrices matrices, D2D.RenderTarget target)
        {
            if (CanRender(target))
            {
                RenderTarget = target;
                UpdateRenderVariables();
                RenderTarget.Transform = RenderTargetTransform;
                OnRender(matrices);
            }
        }

        protected virtual void UpdateRenderVariables()
        {
            if (IsChanged)
            {
                RenderTargetTransform = transform * new Matrix3x2(1, 0, 0, 1, (Rect.Left), (Rect.Top));
                LocalDrawingRect = new RectangleF(0, 0, Rect.Width, Rect.Height);
                IsChanged = false;
            }
        }

        protected virtual void OnRender(IRenderMatrices matrices)
        {
            if (ShowDrawingBorder && BorderBrush != null)
            {
                RenderTarget.DrawRectangle(LocalDrawingRect, BorderBrush, 0.5f, borderStyle);
            }
        }

        protected virtual bool CanRender(D2D.RenderTarget target)
        {
            return IsRendering && target != null && !target.IsDisposed;
        }

        public virtual void Dispose()
        {
            Disposer.RemoveAndDispose(ref borderBrush);
            RenderTarget = null;
        }
    }
}
