/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if DEBUG
//#define DEBUGDRAWING
//#define DISABLEBITMAPCACHE
#endif

#if !NETFX_CORE
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using SharpDX.Direct2D1;
using System.Diagnostics;

using System.Windows;
namespace HelixToolkit.Wpf.SharpDX.Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class Element2DCore : FrameworkContentElement, IDisposable, IRenderable2D, INotifyPropertyChanged
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
        /// <summary>
        /// Gets or sets a value indicating whether this instance is attached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is attached; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttached { private set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is renderable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
        /// </value>
        public bool IsRenderable { private set; get; } = true;

        private IRenderCore2D renderCore;
        /// <summary>
        /// Gets or sets the render core.
        /// </summary>
        /// <value>
        /// The render core.
        /// </value>
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
        /// <summary>
        /// Gets or sets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        protected IRenderHost RenderHost { private set; get; }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public virtual IList<IRenderable2D> Items { get; } = Constants.EmptyRenderable2D;

        private Matrix3x2 modelMatrix = Matrix3x2.Identity;
        /// <summary>
        /// Gets or sets the model matrix.
        /// </summary>
        /// <value>
        /// The model matrix.
        /// </value>
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
        /// <summary>
        /// Gets or sets the layout translate.
        /// </summary>
        /// <value>
        /// The layout translate.
        /// </value>
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
        /// <summary>
        /// Gets or sets the parent matrix.
        /// </summary>
        /// <value>
        /// The parent matrix.
        /// </value>
        public Matrix3x2 ParentMatrix
        {
            set
            {
                if(Set(ref parentMatrix, value))
                {
                    IsTransformDirty = true;
                }
            }
            get { return parentMatrix; }
        }

        private Matrix3x2 totalTransform = Matrix3x2.Identity;
        /// <summary>
        /// Gets or sets the total model matrix.
        /// </summary>
        /// <value>
        /// The total model matrix.
        /// </value>
        public Matrix3x2 TotalModelMatrix
        {
            private set
            {
                if(Set(ref totalTransform, value))
                {
                    for (int i = 0; i < Items.Count; ++i)
                    {
                        Items[i].ParentMatrix = totalTransform;
                    }
                    TransformChanged(ref value);
                    OnTransformChanged?.Invoke(this, new Transform2DArgs(ref value));
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
        private Matrix3x2 RelativeMatrix
        { set; get; }
        /// <summary>
        /// Gets or sets the layout bound with transform.
        /// </summary>
        /// <value>
        /// The layout bound with transform.
        /// </value>
        public RectangleF LayoutBoundWithTransform
        {
            private set;get;
        }
        /// <summary>
        /// Creates the render core.
        /// </summary>
        /// <returns></returns>
        protected virtual IRenderCore2D CreateRenderCore() { return new EmptyRenderCore2D(); }
        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>Attach Flow: Set RenderHost -> Get Effect ->
        /// <see cref="OnAttach(IRenderHost)"/> -> <see cref="OnAttach"/> -> <see cref="InvalidateRender"/></para>
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
        /// <summary>
        /// Detaches this instance.
        /// </summary>
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
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected virtual void OnDetach()
        {
            RenderHost = null;
        }
        /// <summary>
        /// Updates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Update(IRenderContext2D context)
        {         
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
        public event EventHandler<Transform2DArgs> OnTransformChanged;
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
            if (!IsRenderable)
            { return; }
            if (IsTransformDirty)
            {
                RelativeMatrix = Matrix3x2.Translation(-RenderSize * RenderTransformOriginInternal)
                    * ModelMatrix * Matrix3x2.Translation(RenderSize * RenderTransformOriginInternal)
                    * LayoutTranslate;
                TotalModelMatrix = RelativeMatrix * ParentMatrix;            
                IsTransformDirty = false;
                InvalidateVisual();
            }

            LayoutBoundWithTransform = LayoutBound.Translate(TotalModelMatrix.TranslationVector);

#if DISABLEBITMAPCACHE
            IsBitmapCacheValid = false;
#else
            EnsureBitmapCache(context, new Size2((int)Math.Ceiling(LayoutClipBound.Width), (int)Math.Ceiling(LayoutClipBound.Height)), context.DeviceContext.MaximumBitmapSize);
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
                    context.DeviceContext.DrawImage(bitmapCache, new Vector2(0, 0), LayoutClipBound, 
                        InterpolationMode.Linear, global::SharpDX.Direct2D1.CompositeMode.SourceOver);
                }
                    
            }
            else if(context.HasTarget)
            {
                context.PushRelativeTransform(context.RelativeTransform * RelativeMatrix);
                RenderCore.Transform = context.RelativeTransform;
                OnRender(context);
                context.PopRelativeTransform();
                IsVisualDirty = false;
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
                context.DeviceContext.DrawImage(bitmapCache, new Vector2(0, 0), new RectangleF(0, 0, RenderSize.X, RenderSize.Y),
                    InterpolationMode.Linear, global::SharpDX.Direct2D1.CompositeMode.SourceOver);
            }
            else
            {
                Render(context);
            }
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void OnRender(IRenderContext2D context)
        {
            RenderCore.Render(context);
            for (int i = 0; i < this.Items.Count; ++i)
            {
                Items[i].Render(context);
            }
        }
        #endregion
        /// <summary>
        /// Determines whether this instance [can hit test].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanHitTest()
        {
            return IsAttached && IsHitTestVisibleInternal;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="hitResult">The hit result.</param>
        /// <returns></returns>
        protected abstract bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult);
        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="hitResult">The hit result.</param>
        /// <returns></returns>
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

        private void RenderCore_OnInvalidateRenderer(object sender, EventArgs e)
        {
            InvalidateRender();
        }
        /// <summary>
        /// Invalidates the render.
        /// </summary>
        public void InvalidateRender()
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
    
#endif