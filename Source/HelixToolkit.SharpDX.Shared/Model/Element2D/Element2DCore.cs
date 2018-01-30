/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if DEBUG
//#define DEBUGDRAWING
//#define DISABLEBITMAPCACHE
#endif
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using SharpDX.Direct2D1;
using System.Diagnostics;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
#if NETFX_CORE
    public abstract partial class Element2DCore : IDisposable, IRenderable2D, INotifyPropertyChanged
#else
    public abstract partial class Element2DCore : FrameworkContentElement, IDisposable, IRenderable2D, INotifyPropertyChanged
#endif
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();

        private Visibility visibilityInternal = Visibility.Visible;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Element2DCore"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        internal Visibility VisibilityInternal
        {
            set
            {
                if (Set(ref visibilityInternal, value))
                { InvalidateRender(); }
            }
            get { return visibilityInternal; }
        }

        internal bool IsHitTestVisibleInternal { set; get; } = true;

        public bool IsAttached { private set; get; }

        public bool IsRenderable { private set; get; } = true;

        private IRenderCore2D renderCore;
        public IRenderCore2D RenderCore
        {
            private set
            {
                if (renderCore != value)
                {
                    if(renderCore != null)
                    {
                        renderCore.OnInvalidateRenderer -= RenderCore_OnInvalidateRenderer;
                    }
                    renderCore = value;
                    if(renderCore != null)
                    {
                        renderCore.OnInvalidateRenderer += RenderCore_OnInvalidateRenderer;
                    }
                }
            }
            get
            {
                if(renderCore == null)
                {
                    RenderCore = CreateRenderCore();
                }
                return renderCore;
            }
        }

        protected IRenderHost RenderHost { private set; get; }

        public virtual IEnumerable<IRenderable2D> Items { get { return Enumerable.Empty<IRenderable2D>(); } }

        private Matrix3x2 modelMatrix = Matrix3x2.Identity;
        public Matrix3x2 ModelMatrix
        {
            set
            {
                if(Set(ref modelMatrix, value))
                {
                    RenderCore.LocalTransform = value;
                    InvalidateVisual();
                }
            }
            get { return modelMatrix; }
        }

        private Matrix3x2 layoutTranslate = Matrix3x2.Identity;
        public Matrix3x2 LayoutTranslate
        {
            set
            {
                if(Set(ref layoutTranslate, value))
                {                    
                    InvalidateRender();
                }
            }
            get { return layoutTranslate; }
        }

        private Matrix3x2 parentMatrix = Matrix3x2.Identity;
        public Matrix3x2 ParentMatrix
        {
            set
            {
                if(Set(ref parentMatrix, value))
                {
                    InvalidateRender();
                }
            }
            get { return parentMatrix; }
        }

        private Matrix3x2 totalTransform = Matrix3x2.Identity;
        public Matrix3x2 TotalModelMatrix
        {
            private set
            {
                if(Set(ref totalTransform, value))
                {
                    IsTransformDirty = true;
                    LayoutBoundWithTransform = LayoutBound.Translate(value.TranslationVector);
                    foreach (var item in Items)
                    {
                        item.ParentMatrix = totalTransform;
                    }
                    TransformChanged(ref value);
                    OnTransformChanged?.Invoke(this, value);
                }
            }
            get
            {
                return totalTransform;
            }
        }
        /// <summary>
        /// Gets or sets the transform matrix relative to its parent
        /// </summary>
        /// <value>
        /// The relative matrix.
        /// </value>
        private Matrix3x2 RelativeMatrix { set; get; }

        public RectangleF LayoutBoundWithTransform
        {
            private set;get;
        }

        protected virtual IRenderCore2D CreateRenderCore() { return new EmptyRenderCore2D(); }
        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>To set different render technique instead of using technique from host, override <see cref="SetRenderTechnique"/></para>
        /// <para>Attach Flow: <see cref="SetRenderTechnique(IRenderHost)"/> -> Set RenderHost -> Get Effect -> <see cref="OnAttach(IRenderHost)"/> -> <see cref="OnAttached"/> -> <see cref="InvalidateRender"/></para>
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (IsAttached || host == null)
            {
                return;
            }
            RenderHost = host;
            IsAttached = OnAttach(host);
            InvalidateAll();
        }

        /// <summary>
        /// To override Attach routine, please override this.
        /// </summary>
        /// <param name="host"></param>       
        /// <returns>Return true if attached</returns>
        protected virtual bool OnAttach(IRenderHost host)
        {
            RenderCore.Attach(host);
            return true;
        }

        public void Detach()
        {
            if (IsAttached)
            {
                IsAttached = false;
                RenderCore.Detach();
                Disposer.RemoveAndDispose(ref bitmapCache);
                OnDetach();                
            }
        }

        protected virtual void OnDetach()
        {
            RenderHost = null;
        }

        public virtual void Update(IRenderContext2D context)
        {
            RelativeMatrix = Matrix3x2.Translation(-(RenderSize.Width * RenderTransformOriginInternal.X), -(RenderSize.Height * RenderTransformOriginInternal.Y))
                * ModelMatrix * Matrix3x2.Translation((RenderSize.Width * RenderTransformOriginInternal.X), (RenderSize.Height * RenderTransformOriginInternal.Y))
                * LayoutTranslate; 
            TotalModelMatrix = RelativeMatrix * ParentMatrix;
            IsTransformDirty = false;
            IsRenderable = CanRender(context);
        }

        #region Handling Transforms        
        /// <summary>
        /// Transforms the changed.
        /// </summary>
        /// <param name="totalTransform">The total transform.</param>
        protected virtual void TransformChanged(ref Matrix3x2 totalTransform)
        {

        }
        /// <summary>
        /// Occurs when [on transform changed].
        /// </summary>
        public event EventHandler<Matrix3x2> OnTransformChanged;
        #endregion
        #region Rendering

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanRender(IRenderContext2D context)
        {
            return VisibilityInternal == Visibility.Visible && IsAttached;
        }
        /// <summary>
        /// <para>Renders the element in the specified context. To override Render, please override <see cref="OnRender"/></para>
        /// <para>Uses <see cref="CanRender"/>  to call OnRender or not. </para>
        /// </summary>
        /// <param name="context">The context.</param>
        public void Render(IRenderContext2D context)
        {
            Update(context);
            if (IsRenderable)
            {
#if DISABLEBITMAPCACHE
                IsBitmapCacheValid = false;
#else
                EnsureBitmapCache(context, new Size2((int)Math.Ceiling(ClipBound.Width), (int)Math.Ceiling(ClipBound.Height)), context.DeviceContext.MaximumBitmapSize);
#endif
                if (EnableBitmapCacheInternal && IsBitmapCacheValid)
                {
                    if (IsVisualDirty)
                    {                        
#if DEBUGDRAWING
                        Debug.WriteLine("Redraw bitmap cache");
#endif
                        context.PushRenderTarget(bitmapCache, true);
                        context.DeviceContext.Transform = Matrix3x2.Identity;
                        context.PushRelativeTransform(Matrix3x2.Identity);
                        RenderCore.Transform = context.RelativeTransform;
                        OnRender(context);
                        context.PopRelativeTransform();
                        context.PopRenderTarget();
                        IsVisualDirty = false;
                    }
                    if (context.HasTarget)
                    {
                        context.DeviceContext.Transform = context.RelativeTransform * RelativeMatrix;                                     
                        context.DeviceContext.DrawImage(bitmapCache, new Vector2(0, 0), ClipBound, 
                            InterpolationMode.Linear, CompositeMode.SourceOver);
                    }
                    
                }
                else if(context.HasTarget)
                {
                    context.PushRelativeTransform(context.RelativeTransform * RelativeMatrix);
                    RenderCore.Transform = context.RelativeTransform;
                    OnRender(context);
                    context.PopRelativeTransform();
                }
            }
        }

        /// <summary>
        /// Renders the bitmap cache to a render target only.
        /// </summary>
        /// <param name="context">The context.</param>
        public void RenderBitmapCache(IRenderContext2D context)
        {
            if(IsRenderable && EnableBitmapCacheInternal && IsBitmapCacheValid && !IsVisualDirty && context.HasTarget)
            {
                context.DeviceContext.Transform = RelativeMatrix;
                context.DeviceContext.DrawImage(bitmapCache, new Vector2(0, 0), new RectangleF(0, 0, RenderSize.Width, RenderSize.Height),
                    InterpolationMode.Linear, CompositeMode.SourceOver);
            }
        }

        protected virtual void OnRender(IRenderContext2D context)
        {
            RenderCore.Render(context);
            foreach (var c in this.Items)
            {
                c.Render(context);
            }
        }
#endregion

        protected virtual bool CanHitTest()
        {
            return IsAttached && IsHitTestVisibleInternal;
        }

        protected abstract bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult);

        public bool HitTest(Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            if (CanHitTest())
            {
                return OnHitTest(ref mousePoint, out hitResult);
            }
            else
            {
                hitResult = null;
                return false;
            }
        }

        private void RenderCore_OnInvalidateRenderer(object sender, bool e)
        {
            InvalidateRender();
        }

        protected void InvalidateRender()
        {
            RenderHost?.InvalidateRender();
        }

#region INotifyPropertyChanged
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private bool disablePropertyChangedEvent = false;
        /// <summary>
        /// Gets or sets a value indicating whether [disable property changed event].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable property changed event]; otherwise, <c>false</c>.
        /// </value>
        public bool DisablePropertyChangedEvent
        {
            set
            {
                if (disablePropertyChangedEvent == value)
                {
                    return;
                }
                disablePropertyChangedEvent = value;
                RaisePropertyChanged();
            }
            get
            {
                return disablePropertyChangedEvent;
            }
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!DisablePropertyChangedEvent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Sets the specified backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Sets the specified backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="raisePropertyChanged">if set to <c>true</c> [raise property changed].</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            if (raisePropertyChanged)
            { this.RaisePropertyChanged(propertyName); }
            return true;
        }
#endregion

#region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Element2DCore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
#endregion
    }
}
