/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public abstract class Renderable2DBase : DisposeObject, IRenderable2D, ITransform2D
    {
        public Matrix3x2 RenderTargetTransform { private set; get; }

        public bool IsRendering
        {
            set; get;
        } = true;

        private RectangleF rect = new RectangleF(0, 0, 100, 100);
        /// <summary>
        /// Absolute layout rectangle cooridnate for renderable
        /// </summary>
        public RectangleF Bound
        {
            set
            {
                if(Set(ref rect, value))
                {
                    LocalDrawingRect = new RectangleF(0, 0, Bound.Width, Bound.Height);
                    RenderTargetTransform = transform * new Matrix3x2(1, 0, 0, 1, (Bound.Left), (Bound.Top));
                }
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

        private IDevice2DProxy renderTarget;
        protected IDevice2DProxy RenderTarget
        {
            private set
            {
                Set(ref renderTarget, value);
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
                if(Set(ref transform, value))
                {
                    RenderTargetTransform = transform * new Matrix3x2(1, 0, 0, 1, (Bound.Left), (Bound.Top));
                }
            }
            get
            {
                return transform;
            }
        }
        /// <summary>
        /// For debugging only
        /// </summary>
        private D2D.Brush borderBrush = null;
        private D2D.StrokeStyle borderDotStyle;
        private D2D.StrokeStyle borderLineStyle;
#if DEBUG
        public bool ShowDrawingBorder { set; get; } = true;
#else
        public bool ShowDrawingBorder { set; get; } = false;
#endif

        public bool IsMouseOver { set; get; } = false;

        public bool IsAttached { private set; get; } = false;

        public void Attach(IRenderHost host)
        {
            if (IsAttached)
            { return; }
            RenderTarget = host.D2DTarget;
            IsAttached = OnAttach(RenderTarget);
        }

        protected virtual bool OnAttach(IDevice2DProxy target)
        {
            borderBrush = Collect(new D2D.SolidColorBrush(target.D2DTarget, Color.LightBlue));
            borderDotStyle = Collect(new D2D.StrokeStyle(RenderTarget.D2DTarget.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.DashDot }));
            borderLineStyle = Collect(new D2D.StrokeStyle(RenderTarget.D2DTarget.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.Solid }));
            return true;
        }

        public void Detach()
        {
            IsAttached = false;
            DisposeAndClear();
        }

        public void Render(IRenderContext2D context)
        {
            if (CanRender(context))
            {         
                if (ShowDrawingBorder)
                {
                    context.D2DTarget.Transform = Matrix3x2.Identity;
                    context.D2DTarget.DrawRectangle(Bound, borderBrush, 1f, IsMouseOver ? borderLineStyle : borderDotStyle);
                }
                context.D2DTarget.Transform = RenderTargetTransform;
                if (ShowDrawingBorder)
                {
                    context.D2DTarget.DrawRectangle(LocalDrawingRect, borderBrush, 0.5f, borderDotStyle);
                }
                OnRender(context);
            }
        }

        protected abstract void OnRender(IRenderContext2D matrices);

        protected virtual bool CanRender(IRenderContext2D context)
        {
            return IsAttached && IsRendering;
        }
    }
}
