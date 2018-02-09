/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGBOUNDS
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

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty { get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
        /// </value>
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
        /// Gets or sets the transform. <see cref="IRenderCore2D.Transform"/>
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

#if DEBUGBOUNDS
        /// <summary>
        /// 
        /// </summary>
        public bool ShowDrawingBorder { set; get; } = true;
#else
        /// <summary>
        /// 
        /// </summary>
        public bool ShowDrawingBorder { set; get; } = false;
#endif

        private bool isMouseOver = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is attached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is attached; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttached { private set; get; } = false;
        /// <summary>
        /// Attaches the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (IsAttached)
            { return; }
            IsAttached = OnAttach(host);
        }
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="host">The target.</param>
        /// <returns></returns>
        protected virtual bool OnAttach(IRenderHost host)
        {
            return true;
        }
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            IsAttached = false;
            DisposeAndClear();
        }
        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Render(IRenderContext2D context)
        {
            if (CanRender(context))
            {
                context.DeviceContext.Transform = Transform;
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
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        protected abstract void OnRender(IRenderContext2D context);
        /// <summary>
        /// Determines whether this instance can render the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance can render the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRender(IRenderContext2D context)
        {
            return IsAttached && IsRendering;
        }
        /// <summary>
        /// Invalidates the renderer.
        /// </summary>
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
