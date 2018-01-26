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
    using global::SharpDX.DXGI;
    using Utilities;
    public abstract class RenderCore2DBase : DisposeObject, IRenderCore2D
    {
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        public event EventHandler<bool> OnInvalidateRenderer;
        public bool IsEmpty { get; } = false;

        public bool IsRendering
        {
            set; get;
        } = true;

        private RectangleF rect = new RectangleF();
        /// <summary>
        /// Absolute layout rectangle cooridnate for renderable
        /// </summary>
        public RectangleF LayoutBound
        {
            set
            {
                SetAffectsRender(ref rect, value);
            }
            get
            {
                return rect;
            }
        }

        private RectangleF clippingBound = new RectangleF();
        public RectangleF LayoutClippingBound
        {
            set
            {
                SetAffectsRender(ref clippingBound, value);
            }
            get { return clippingBound; }
        }

        private Matrix3x2 transform = Matrix3x2.Identity;
        /// <summary>
        /// Gets or sets the transform. This transform to absolute position on screen
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        public Matrix3x2 Transform
        {
            set
            {
                SetAffectsRender(ref transform, value);
            }
            get
            {
                return transform;
            }
        }

        private Matrix3x2 localTransform = Matrix3x2.Identity;
        /// <summary>
        /// Gets or sets the local transform. This only transform local position. Same as RenderTransform
        /// </summary>
        /// <value>
        /// The local transform.
        /// </value>
        public Matrix3x2 LocalTransform
        {
            set
            {
                SetAffectsRender(ref localTransform, value);
            }
            get
            {
                return localTransform;
            }
        }

        public bool UseBitmapCache { set; get; } = false;

#if DEBUG
        public bool ShowDrawingBorder { set; get; } = true;
#else
        public bool ShowDrawingBorder { set; get; } = false;
#endif

        private bool isMouseOver = false;
        public bool IsMouseOver
        {
            set
            {
                SetAffectsRender(ref isMouseOver, value);
            }
            get
            {
                return isMouseOver;
            }
        }

        public bool IsAttached { private set; get; } = false;

        public void Attach(IRenderHost host)
        {
            if (IsAttached)
            { return; }
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
                context.DeviceContext.Transform = UseBitmapCache ? LocalTransform : Transform;
                if (ShowDrawingBorder)
                {
                    using (var borderBrush = new D2D.SolidColorBrush(context.DeviceContext, Color.Blue))
                    {
                        using (var borderDotStyle = new D2D.StrokeStyle(context.DeviceContext.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.DashDot }))
                        {
                            using (var borderLineStyle = new D2D.StrokeStyle(context.DeviceContext.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.Solid }))
                            {
                                context.DeviceContext.DrawRectangle(LayoutBound, borderBrush, 1f, IsMouseOver ? borderLineStyle : borderDotStyle);
                                context.DeviceContext.DrawRectangle(LayoutClippingBound, borderBrush, 0.5f, borderDotStyle);
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
