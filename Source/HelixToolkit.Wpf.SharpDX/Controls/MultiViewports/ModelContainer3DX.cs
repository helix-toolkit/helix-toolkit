using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using Core2D;
    using HelixToolkit.Logger;
    using Render;
    using Model.Scene;
    using Utilities;

    /// <summary>
    /// Use to contain shared models for multiple viewports. 
    /// <para>Suggest to bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak</para>
    /// </summary>
    public class ModelContainer3DX : ItemsControl, IModelContainer
    {
        /// <summary>
        /// The EffectsManager property. Suggest to bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak
        /// </summary>
        public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
            "EffectsManager", typeof(IEffectsManager), typeof(ModelContainer3DX), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsRender,
                (s, e) => ((ModelContainer3DX)s).EffectsManagerPropertyChanged()));
        /// <summary>
        /// The Render Technique property
        /// </summary>
        public static readonly DependencyProperty RenderTechniqueProperty = DependencyProperty.Register(
            "RenderTechnique", typeof(IRenderTechnique), typeof(ModelContainer3DX), new PropertyMetadata(null,
                (s, e) => ((ModelContainer3DX)s).RenderTechniquePropertyChanged()));


        /// <summary>
        /// Gets or sets the <see cref="EffectsManagerProperty"/>.
        /// <para>The EffectsManager property. Suggest bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak.</para>
        /// </summary>
        public IEffectsManager EffectsManager
        {
            get { return (IEffectsManager)GetValue(EffectsManagerProperty); }
            set { SetValue(EffectsManagerProperty, value); }
        }

        /// <summary>
        /// Gets or sets value for the shading model shading is used
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        public IRenderTechnique RenderTechnique
        {
            get { return (IRenderTechnique)this.GetValue(RenderTechniqueProperty); }
            set { this.SetValue(RenderTechniqueProperty, value); }
        }
        private static readonly LogWrapper NullLogger = new LogWrapper(new NullLogger());
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public LogWrapper Logger { get { return CurrentRenderHost != null ? CurrentRenderHost.Logger : NullLogger; } }

        private readonly IList<IViewport3DX> viewports = new List<IViewport3DX>();
#pragma warning disable 0067        
        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        /// <summary>
        /// Occurs when [on new render target texture].
        /// </summary>
        public event EventHandler<Texture2DArgs> OnNewRenderTargetTexture;
        /// <summary>
        /// Occurs when [start render loop].
        /// </summary>
        public event EventHandler<EventArgs> StartRenderLoop;
        /// <summary>
        /// Occurs when [stop render loop].
        /// </summary>
        public event EventHandler<EventArgs> StopRenderLoop;
#pragma warning restore 0067        
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets a value indicating whether this instance is rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
        /// </value>
        public bool IsRendering { set; get; } = true;

        private int d3dCounter = 0;

        private IRenderHost currentRenderHost = null;
        /// <summary>
        /// </summary>
        public IRenderHost CurrentRenderHost
        {
            set
            {
                if (currentRenderHost != value)
                {
                    currentRenderHost = value;
                    currentRenderHost.SetDefaultRenderTargets(false);
                }
            }
            get
            {
                return currentRenderHost;
            }
        }

        

        /// <summary>
        /// Gets the current frame renderables for rendering.
        /// </summary>
        /// <value>
        /// The per frame renderable.
        /// </value>
        public List<SceneNode> PerFrameRenderables { get { return CurrentRenderHost != null ? CurrentRenderHost.PerFrameRenderables : Constants.EmptyRenderable; } }
        /// <summary>
        /// Gets the current frame Lights for rendering.
        /// </summary>
        /// <value>
        /// The per frame renderable.
        /// </value>
        public IEnumerable<LightCoreBase> PerFrameLights { get { return CurrentRenderHost != null ? CurrentRenderHost.PerFrameLights : Enumerable.Empty<LightCoreBase>(); } }
        /// <summary>
        /// Gets the per frame post effect cores.
        /// </summary>
        /// <value>
        /// The per frame post effect cores.
        /// </value>
        public List<RenderCore> PerFrameGeneralCoresWithPostEffect { get { return CurrentRenderHost != null ? CurrentRenderHost.PerFrameGeneralCoresWithPostEffect : Constants.EmptyCore; } }
        /// <summary>
        /// Gets the per frame general render cores.
        /// </summary>
        /// <value>
        /// The per frame general render cores.
        /// </value>
        public List<RenderCore> PerFrameGeneralRenderCores { get { return CurrentRenderHost != null ? CurrentRenderHost.PerFrameGeneralRenderCores : Constants.EmptyCore; } }

        /// <summary>
        /// Handles the change of the effects manager.
        /// </summary>
        private void EffectsManagerPropertyChanged()
        {
            foreach(var viewport in viewports)
            {
                viewport.EffectsManager = this.EffectsManager;
            }
        }

        /// <summary>
        /// Handles the change of the render technique        
        /// </summary>
        private void RenderTechniquePropertyChanged()
        {
            foreach (var viewport in viewports)
            {
                viewport.RenderTechnique = this.RenderTechnique;
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="viewport"></param>
        public void AttachViewport3DX(IViewport3DX viewport)
        {
            viewports.Add(viewport);
            viewport.RenderTechnique = this.RenderTechnique;
            viewport.EffectsManager = this.EffectsManager;
        }
        /// <summary>
        /// </summary>
        /// <param name="viewport"></param>
        public void DettachViewport3DX(IViewport3DX viewport)
        {
            viewports.Remove(viewport);
        }
        /// <summary>
        /// Invalidates the render.
        /// </summary>
        public void InvalidateRender()
        {
            foreach(var v in viewports)
            {
                v.InvalidateRender();
            }
        }
        /// <summary>
        /// Sets the default render targets.
        /// </summary>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public void SetDefaultRenderTargets(bool clear = true)
        {
            CurrentRenderHost.SetDefaultRenderTargets(clear);
        }
        /// <summary>
        /// </summary>
        public IEnumerable<SceneNode> Renderables
        {
            get
            {
                foreach(Element3D item in Items)
                {
                    yield return item.SceneNode;
                }
            }
        }
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device
        {
            get
            {
                return this.EffectsManager != null ? this.EffectsManager.Device : null;
            }
        }
        /// <summary>
        /// Gets the device2d.
        /// </summary>
        /// <value>
        /// The device2d.
        /// </value>
        public global::SharpDX.Direct2D1.Device Device2D
        {
            get
            {
                return this.EffectsManager != null ? this.EffectsManager.Device2D : null;
            }
        }

        /// <summary>
        /// Gets or sets the color of the clear.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        /// <exception cref="NotImplementedException"></exception>
        public Color4 ClearColor
        {
            get
            {
                return currentRenderHost != null ? currentRenderHost.ClearColor : Color.White;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is shadow map enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shadow map enabled; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsShadowMapEnabled
        {
            get
            {
                return currentRenderHost != null ? currentRenderHost.IsShadowMapEnabled : false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Gets or sets the msaa.
        /// </summary>
        /// <value>
        /// The msaa.
        /// </value>
        public MSAALevel MSAA
        {
            set;get;
        }
        /// <summary>
        /// Gets or sets the viewport.
        /// </summary>
        /// <value>
        /// The viewport.
        /// </value>
        public IViewport3DX Viewport
        {
            set;get;
        }
        /// <summary>
        /// Gets the render context.
        /// </summary>
        /// <value>
        /// The render context.
        /// </value>
        public IRenderContext RenderContext
        {
            get
            {
                return CurrentRenderHost.RenderContext;
            }
        }
        /// <summary>
        /// Indicates if DPFCanvas busy on rendering.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return CurrentRenderHost.IsBusy;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable render frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render frustum]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderFrustum
        {
            set;get;
        }
        /// <summary>
        /// Gets or sets the maximum FPS.
        /// </summary>
        /// <value>
        /// The maximum FPS.
        /// </value>
        public uint MaxFPS
        {
            set;get;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is deferred lighting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deferred lighting; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeferredLighting
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.IsDeferredLighting : false;
            }
        }
        /// <summary>
        /// Gets or sets the shared model container.
        /// </summary>
        /// <value>
        /// The shared model container.
        /// </value>
        public IModelContainer SharedModelContainer
        {
            set { } get { return this; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable sharing model mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable sharing model mode]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSharingModelMode
        {
            set { }
            get { return true; }
        }
        /// <summary>
        /// Gets the color buffer view.
        /// </summary>
        /// <value>
        /// The color buffer view.
        /// </value>
        public RenderTargetView RenderTargetBufferView
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.RenderTargetBufferView : null;
            }
        }
        /// <summary>
        /// Gets the depth stencil buffer view.
        /// </summary>
        /// <value>
        /// The depth stencil buffer view.
        /// </value>
        public DepthStencilView DepthStencilBufferView
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.DepthStencilBufferView : null;
            }
        }
        /// <summary>
        /// Gets the d2d target.
        /// </summary>
        /// <value>
        /// The d2d target.
        /// </value>
        public ID2DTargetProxy D2DTarget
        {
            get
            {
                return CurrentRenderHost != null ? CurrentRenderHost.D2DTarget : null;
            }
        }
        /// <summary>
        /// Gets the render statistics.
        /// </summary>
        /// <value>
        /// The render statistics.
        /// </value>
        public IRenderStatistics RenderStatistics { get { return CurrentRenderHost != null ? CurrentRenderHost.RenderStatistics : null; } }
        /// <summary>
        /// Gets or sets a value indicating whether [show statistics].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show statistics]; otherwise, <c>false</c>.
        /// </value>
        public RenderDetail ShowRenderDetail
        {
            set;get;
        }
        /// <summary>
        /// Gets or sets the render configuration.
        /// </summary>
        /// <value>
        /// The render configuration.
        /// </value>
        public DX11RenderHostConfiguration RenderConfiguration { set; get; }

        public IDX11RenderBufferProxy RenderBuffer { get { return CurrentRenderHost != null ? CurrentRenderHost.RenderBuffer : null; } }

        public ModelContainer3DX()
        {
            this.Visibility = Visibility.Collapsed;
            this.Focusable = false;
            this.IsHitTestVisible = false;
        }

        /// <summary>
        /// Attaches the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (Interlocked.Increment(ref d3dCounter) == 1 && host.EffectsManager != null)
            {
                foreach (var renderable in Renderables)
                {
                    renderable.Attach(host);
                }
            }
        }
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">D3DCounter is negative.</exception>
        public void Detach()
        {
            if (Interlocked.Decrement(ref d3dCounter) == 0)
            {
                foreach (var renderable in Renderables)
                {
                    renderable.Detach();
                }
            }
            else if (d3dCounter < 0)
            {
                throw new IndexOutOfRangeException("D3DCounter is negative.");
            }
        }
        /// <summary>
        /// Starts the d3 d.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void StartD3D(double width, double height)
        {
            
        }
        /// <summary>
        /// Ends the d3 d.
        /// </summary>
        public void EndD3D()
        {

        }
        /// <summary>
        /// Updates the and render.
        /// </summary>
        public void UpdateAndRender()
        {
            
        }
        /// <summary>
        /// Resizes
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Resize(double width, double height)
        {
            
        }

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clearBackBuffer">if set to <c>true</c> [clear back buffer].</param>
        /// <param name="clearDepthStencilBuffer">if set to <c>true</c> [clear depth stencil buffer].</param>
        public void ClearRenderTarget(DeviceContext context, bool clearBackBuffer, bool clearDepthStencilBuffer)
        {
            if (CurrentRenderHost != null)
            {
                CurrentRenderHost.ClearRenderTarget(context, clearBackBuffer, clearDepthStencilBuffer);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Detach();
                    // TODO: dispose managed state (managed objects).
                    viewports.Clear();
                    currentRenderHost = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ModelContainer3DX() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
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
