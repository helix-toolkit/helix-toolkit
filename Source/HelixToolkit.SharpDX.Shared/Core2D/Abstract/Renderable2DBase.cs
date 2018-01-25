/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public abstract class RenderCore2DBase : DisposeObject, IRenderCore2D
    {
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        public event EventHandler<bool> OnInvalidateRenderer;
        public bool IsEmpty { get; } = false;
        //public Matrix3x2 RenderTargetTransform { private set; get; }

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
                    //RenderTargetTransform = transform * new Matrix3x2(1, 0, 0, 1, (Bound.Left), (Bound.Top));
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

        private ID2DTargetProxy renderTarget;
        protected ID2DTargetProxy RenderTarget
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
                    //RenderTargetTransform = transform * new Matrix3x2(1, 0, 0, 1, (Bound.Left), (Bound.Top));
                }
            }
            get
            {
                return transform;
            }
        }

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
            IsAttached = OnAttach(host);
        }

        protected virtual bool OnAttach(IRenderHost target)
        {
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
                context.DeviceContext.Transform = Transform;
                if (ShowDrawingBorder)
                {
                    using (var borderBrush = new D2D.SolidColorBrush(context.DeviceContext, Color.LightBlue))
                    {
                        using (var borderDotStyle = new D2D.StrokeStyle(context.DeviceContext.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.DashDot }))
                        {
                            using (var borderLineStyle = new D2D.StrokeStyle(context.DeviceContext.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.Solid }))
                            {
                                context.DeviceContext.DrawRectangle(Bound, borderBrush, 1f, IsMouseOver ? borderLineStyle : borderDotStyle);                             
                                context.DeviceContext.DrawRectangle(LocalDrawingRect, borderBrush, 0.5f, borderDotStyle);
                            }
                        }
                    }
                }
                OnRender(context);
            }
        }

        protected abstract void OnRender(IRenderContext2D matrices);

        protected virtual bool CanRender(IRenderContext2D context)
        {
            return IsAttached && IsRendering;
        }

        protected void InvalidateRenderer()
        {
            OnInvalidateRenderer?.Invoke(this, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            InvalidateRenderer();
            return true;
        }
    }
}
