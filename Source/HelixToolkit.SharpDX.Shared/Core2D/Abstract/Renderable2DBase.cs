/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public abstract class Renderable2DBase : DisposeObject, IRenderable2D
    {
        public bool IsChanged { private set; get; } = true;

        public Matrix3x2 RenderTargetTransform { private set; get; }

        public bool IsRendering
        {
            set; get;
        } = true;

        private RectangleF rect = new RectangleF(0, 0, 100, 100);
        /// <summary>
        /// Absolute layout rectangle cooridnate for renderable
        /// </summary>
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
        /// <summary>
        /// Absolute visual rendering rectangle coordinate for renderable
        /// </summary>
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

        private D2D.StrokeStyle borderDotStyle;
        private D2D.StrokeStyle borderLineStyle;
#if DEBUG
        public bool ShowDrawingBorder { set; get; } = true;
#else
        public bool ShowDrawingBorder { set; get; } = false;
#endif

        public bool IsMouseOver { set; get; } = false;

        protected virtual void OnTargetChanged(D2D.RenderTarget target)
        {
            RemoveAndDispose(ref borderBrush);
            RemoveAndDispose(ref borderDotStyle);
            RemoveAndDispose(ref borderLineStyle);
            if (target == null || target.IsDisposed)
            {
                return;
            }
            borderBrush = new D2D.SolidColorBrush(target, Color.LightBlue);
            borderDotStyle =  new D2D.StrokeStyle(RenderTarget.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.DashDot });
            borderLineStyle = new D2D.StrokeStyle(RenderTarget.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.Solid });
            Collect(borderBrush);
            Collect(borderDotStyle);
            Collect(borderLineStyle);
        }

        public void Render(IRenderContext matrices, D2D.RenderTarget target)
        {
            if (CanRender(target))
            {
                RenderTarget = target;
                UpdateRenderVariables();                
                if (ShowDrawingBorder && BorderBrush != null)
                {
                    RenderTarget.Transform = Matrix3x2.Identity;
                    RenderTarget.DrawRectangle(Rect, BorderBrush, 1f, IsMouseOver ? borderLineStyle : borderDotStyle);
                }
                RenderTarget.Transform = RenderTargetTransform;
                if (ShowDrawingBorder && BorderBrush != null)
                {
                    RenderTarget.DrawRectangle(LocalDrawingRect, BorderBrush, 0.5f, borderDotStyle);
                }
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

        protected abstract void OnRender(IRenderContext matrices);

        protected virtual bool CanRender(D2D.RenderTarget target)
        {
            return IsRendering && target != null && !target.IsDisposed;
        }
    }
}
